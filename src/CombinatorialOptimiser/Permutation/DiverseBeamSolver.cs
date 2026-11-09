using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Diverse beam search variant that encourages diversity across beam members by
/// penalising duplicate actions or tours using a simple Jaccard-style penalty.
/// Falls back to BeamSearch behaviour when diversity parameter is zero.
/// </summary>
public sealed class DiverseBeamSolver : ISolver<DistanceMatrix, PermutationResult>
{
    /// <inheritdoc/>
    public string Name => "Diverse Beam Search";

    /// <inheritdoc/>
    public SolverParadigm Paradigm => SolverParadigm.Construction;

    /// <summary>Beam width.</summary>
    public int BeamWidth { get; init; } = 6;

    /// <summary>Temperature for transition log-probabilities.</summary>
    public double Temperature { get; init; } = 1.0;

    /// <summary>Strength of the diversity penalty applied during expansion.</summary>
    public double DiversityStrength { get; init; } = 0.5;

    /// <inheritdoc/>
    public PermutationResult Solve(DistanceMatrix matrix) =>
        SolverRunner.Timed(Name, Paradigm, matrix, () => SolveInternal(matrix));

    private int[] SolveInternal(DistanceMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        var beam = new List<int[]> { new[] { 0 } };
        var beamScores = new List<double> { 0.0 };

        var logProbs = matrix.TransitionLogProbabilities(Temperature);
        var n = matrix.Count;

        for (var step = 1; step < n; step++)
        {
            var candidates = new List<(int[] Tour, double Score)>();
            for (var i = 0; i < beam.Count; i++)
            {
                var prefix = beam[i];
                var used = new HashSet<int>(prefix);
                var last = prefix[^1];
                for (var next = 0; next < n; next++)
                {
                    if (used.Contains(next))
                        continue;

                    var newTour = prefix.Append(next).ToArray();
                    var stepScore = beamScores[i] + logProbs[last, next];
                    if (DiversityStrength > 0)
                    {
                        var penalty = 0.0;
                        foreach (var (other, _) in candidates)
                            penalty += JaccardSimilarity(newTour, other);

                        penalty = penalty / Math.Max(1, candidates.Count) * DiversityStrength;
                        stepScore -= penalty;
                    }

                    candidates.Add((newTour, stepScore));
                }
            }

            var top = candidates.OrderByDescending(c => c.Score).Take(BeamWidth).ToList();
            beam = top.Select(c => c.Tour).ToList();
            beamScores = top.Select(c => c.Score).ToList();
        }

        return beam.OrderBy(t => matrix.TourLength(t)).First();
    }

    private static double JaccardSimilarity(int[] a, int[] b)
    {
        var sa = new HashSet<int>(a);
        var sb = new HashSet<int>(b);
        var inter = sa.Intersect(sb).Count();
        var uni = sa.Union(sb).Count();
        return uni == 0 ? 0.0 : (double)inter / uni;
    }
}
