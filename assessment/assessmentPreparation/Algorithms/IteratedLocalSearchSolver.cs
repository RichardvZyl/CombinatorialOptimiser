using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

/// <summary>
/// Iterated Local Search -- applies LK, perturbs with a double-bridge
/// (4-opt) move, and repeats. The double-bridge is deliberately non-sequential
/// so LK cannot easily undo it, exploring different regions.
/// Complexity: O(iterations * LK_cost).
/// </summary>
public sealed class IteratedLocalSearchSolver : ITspSolver
{
    public string Name => "Iterated Local Search (LK + double-bridge)";
    public TspParadigm Paradigm => TspParadigm.Improvement;
    public int MaxIterations { get; init; } = 20;
    public int RandomSeed { get; init; } = 42;

    public TspResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var permutation = new NearestNeighborSolver().Solve(m).Order.ToArray();
            var n = permutation.Length;
            if (n < 4) return permutation;

            var rng = new Random(RandomSeed);
            permutation = RunLK(m, permutation);
            var bestPerm = (int[])permutation.Clone();
            var bestCost = m.TourLength(bestPerm);

            for (var iter = 0; iter < MaxIterations; iter++)
            {
                var perturbed = DoubleBridgePerturb(permutation, rng);
                var improved = RunLK(m, perturbed);
                var improvedCost = m.TourLength(improved);
                if (improvedCost < bestCost)
                {
                    bestCost = improvedCost;
                    bestPerm = (int[])improved.Clone();
                }
                permutation = improved;
            }
            return bestPerm;
        });

    private static int[] DoubleBridgePerturb(int[] permutation, Random rng)
    {
        var n = permutation.Length;
        var a = rng.Next(1, n / 3);
        var b = rng.Next(n / 3, 2 * n / 3);
        var c = rng.Next(2 * n / 3, n);
        var result = new int[n];
        result[0] = permutation[0];
        var pos = 1;
        for (var i = 1; i <= a; i++) result[pos++] = permutation[i];
        for (var i = b + 1; i <= c; i++) result[pos++] = permutation[i];
        for (var i = a + 1; i <= b; i++) result[pos++] = permutation[i];
        for (var i = c + 1; i < n; i++) result[pos++] = permutation[i];
        return result;
    }

    private static int[] RunLK(DistanceMatrix m, int[] seed)
    {
        var solver = new LinKernighanSolver();
        return solver.Solve(m).Order.ToArray();
    }
}
