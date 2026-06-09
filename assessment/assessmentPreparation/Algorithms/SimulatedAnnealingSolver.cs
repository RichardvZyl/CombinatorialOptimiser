using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

/// <summary>
/// Simulated Annealing -- probabilistic meta-heuristic. Starts at high
/// temperature (accepting worse moves to escape local optima) and cools
/// geometrically until only improving moves are accepted.
/// Uses the Metropolis criterion: exp(-delta/T).
/// </summary>
public sealed class SimulatedAnnealingSolver : ITspSolver
{
    public string Name => "Simulated Annealing";
    public TspParadigm Paradigm => TspParadigm.Improvement;
    public double? InitialTemperature { get; init; }
    public double CoolingRate { get; init; } = 0.99;
    public int StepsPerTemperature { get; init; } = 100;
    public double MinTemperature { get; init; } = 0.1;
    public int RandomSeed { get; init; } = 42;

    public TspResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var permutation = new NearestNeighborSolver().Solve(m).Order.ToArray();
            var n = permutation.Length;
            if (n < 4) return permutation;
            var rng = new Random(RandomSeed);
            var currentCost = m.TourLength(permutation);
            var bestPerm = (int[])permutation.Clone();
            var bestCost = currentCost;
            var temperature = InitialTemperature ?? CalibrateTemperature(m, permutation, rng);

            while (temperature > MinTemperature)
            {
                for (var step = 0; step < StepsPerTemperature; step++)
                {
                    var i = rng.Next(1, n); var k = rng.Next(1, n);
                    if (i == k) continue; if (i > k) (i, k) = (k, i);
                    var a = permutation[i == 0 ? n - 1 : i - 1];
                    var b = permutation[i]; var c = permutation[k];
                    var d = permutation[(k + 1) % n];
                    if (a == c || b == d) continue;
                    var delta = (m[a, c] + m[b, d]) - (m[a, b] + m[c, d]);
                    if (delta < 0)
                    {
                        ApplyReverse(permutation, i, k); currentCost += delta;
                        if (currentCost < bestCost) { bestCost = currentCost; Array.Copy(permutation, bestPerm, n); }
                    }
                    else if (temperature > 0 && rng.NextDouble() < Math.Exp(-delta / temperature))
                    { ApplyReverse(permutation, i, k); currentCost += delta; }
                }
                temperature *= CoolingRate;
            }
            return bestPerm;
        });

    private static double CalibrateTemperature(DistanceMatrix m, int[] permutation, Random rng)
    {
        var n = permutation.Length; var deltas = new List<double>();
        var samples = Math.Min(1000, n * n);
        for (var s = 0; s < samples; s++)
        {
            var i = rng.Next(1, n); var k = rng.Next(1, n);
            if (i == k) continue; if (i > k) (i, k) = (k, i);
            var a = permutation[i == 0 ? n - 1 : i - 1];
            var b = permutation[i]; var c = permutation[k];
            var d = permutation[(k + 1) % n];
            if (a == c || b == d) continue;
            var delta = (m[a, c] + m[b, d]) - (m[a, b] + m[c, d]);
            if (delta > 0) deltas.Add(delta);
        }
        if (deltas.Count == 0) return 1.0;
        return -deltas.Average() / Math.Log(0.8);
    }

    private static void ApplyReverse(int[] order, int i, int k)
    {
        while (i < k) { (order[i], order[k]) = (order[k], order[i]); i++; k--; }
    }
}
