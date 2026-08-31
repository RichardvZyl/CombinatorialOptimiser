using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

// Heuristic beam-search solver that treats -cost as a transition score (softmax via temperature)
// and expands the top-k partial sequences until full tours are formed. The solver returns
// the best tour found (by actual tour length) but uses log-probabilities to guide expansion.
/// <summary>
/// Beam-search heuristic for permutation problems. Guides expansion using transition
/// log-probabilities (computed from costs) and returns a completed tour.
/// </summary>
public sealed class BeamSearchSolver : ISolver<DistanceMatrix, PermutationResult>
{
    /// <summary>A short human-readable name for the solver.</summary>
    public string Name => "Beam Search (heuristic)";

    /// <summary>The algorithmic paradigm this solver belongs to.</summary>
    public SolverParadigm Paradigm => SolverParadigm.Construction;

    private readonly int _beamWidth;
    private readonly double _temperature;
    private readonly bool _useLogProbForFinalRanking;

    /// <summary>
    /// Creates a new <see cref="BeamSearchSolver"/>.
    /// </summary>
    /// <param name="beamWidth">Number of partial sequences to retain at each expansion step.</param>
    /// <param name="temperature">Softmax temperature applied to -cost when computing transition probabilities.</param>
    /// <param name="useLogProbForFinalRanking">If true, choose the final tour by accumulated log-prob; otherwise rank by tour length.</param>
    public BeamSearchSolver(int beamWidth = 4, double temperature = 1.0, bool useLogProbForFinalRanking = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(beamWidth, 1, nameof(beamWidth));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(temperature, nameof(temperature));
        _beamWidth = beamWidth;
        _temperature = temperature;
        _useLogProbForFinalRanking = useLogProbForFinalRanking;
    }

    /// <summary>Solves the given distance matrix returning a <see cref="PermutationResult"/>.</summary>
    public PermutationResult Solve(DistanceMatrix m) => SolverRunner.Timed(Name, Paradigm, m, () => SolveInternal(m));

    private int[] SolveInternal(DistanceMatrix m)
    {
        var n = m.Count;
        if (n <= 1) return new[] { 0 };

        var logProbs = m.TransitionLogProbabilities(_temperature, disallowSelf: true);

        // Beam entries: order (list of visited indices), last index, logProb so far
        var beam = new List<BeamEntry> { new(new() { 0 }, 0.0) };

        while (beam.Count > 0 && beam[0].Order.Count < n)
        {
            var candidates = new List<BeamEntry>();
            foreach (var entry in beam)
            {
                var visited = new bool[n];
                foreach (var v in entry.Order) visited[v] = true;
                for (var j = 0; j < n; j++)
                {
                    if (visited[j]) continue;
                    var lp = logProbs[entry.Last, j];
                    // If lp is -inf (disallowed), skip
                    if (double.IsNegativeInfinity(lp)) continue;
                    var newOrder = new List<int>(entry.Order) { j };
                    candidates.Add(new BeamEntry(newOrder, entry.LogProb + lp));
                }
            }

            if (candidates.Count == 0) break;

            // Keep top-k by logProb (higher is better)
            candidates.Sort((a, b) => b.LogProb.CompareTo(a.LogProb));
            beam = candidates.Count <= _beamWidth ? candidates : candidates.GetRange(0, _beamWidth);
        }

        // Evaluate beam completions; either rank by tour length (default) or by accumulated log-prob
        var bestOrder = (int[]?)null;
        if (_useLogProbForFinalRanking)
        {
            double bestLog = double.NegativeInfinity;
            foreach (var entry in beam)
            {
                if (entry.Order.Count != n) continue;
                if (entry.LogProb > bestLog)
                {
                    bestLog = entry.LogProb;
                    bestOrder = entry.Order.ToArray();
                }
            }
        }
        else
        {
            var bestDistance = double.PositiveInfinity;
            foreach (var entry in beam)
            {
                if (entry.Order.Count != n) continue;
                var arr = entry.Order.ToArray();
                var dist = m.TourLength(arr);
                if (dist < bestDistance)
                {
                    bestDistance = dist; bestOrder = arr;
                }
            }
        }

        // If beam failed to produce full permutations (shouldn't for small n), fall back to greedy
        if (bestOrder == null)
        {
            // simple greedy expansion from 0
            var order = new int[n]; order[0] = 0;
            var used = new bool[n]; used[0] = true;
            for (var i = 1; i < n; i++)
            {
                var last = order[i - 1]; double bestLp = double.NegativeInfinity; int best = -1;
                for (var j = 0; j < n; j++) if (!used[j]) { var lp = logProbs[last, j]; if (lp > bestLp) { bestLp = lp; best = j; } }
                if (best == -1) throw new InvalidOperationException("Beam/greeedy expansion failed to find a next node.");
                order[i] = best; used[best] = true;
            }
            return order;
        }

        return bestOrder!;
    }

    private sealed class BeamEntry
    {
        public List<int> Order { get; }
        public int Last => Order[^1];
        public double LogProb { get; }
        public BeamEntry(List<int> order, double logProb) { Order = order; LogProb = logProb; }
    }
}
