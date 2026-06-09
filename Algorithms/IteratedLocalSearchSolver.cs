using PermutationOptimiser.Model;

namespace PermutationOptimiser.Algorithms;

public sealed class IteratedLocalSearchSolver : ISolver
{
    public string Name => "Iterated Local Search (LK + double-bridge)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    public int MaxIterations { get; init; } = 20; public int RandomSeed { get; init; } = 42; public int[]? Seed { get; init; }
    public SolverResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var permutation = Seed ?? new NearestNeighborSolver().Solve(m).Order.ToArray(); var n = permutation.Length;
            if (n < 4) return permutation; var rng = new Random(RandomSeed); permutation = RunLK(m, permutation);
            var bestPerm = (int[])permutation.Clone(); var bestCost = m.TourLength(bestPerm);
            for (var iter = 0; iter < MaxIterations; iter++) { var perturbed = DoubleBridge(permutation, rng); var improved = RunLK(m, perturbed); var ic = m.TourLength(improved); if (ic < bestCost) { bestCost = ic; bestPerm = (int[])improved.Clone(); } permutation = improved; }
            return bestPerm;
        });
    private static int[] DoubleBridge(int[] p, Random rng) { var n = p.Length; var a = rng.Next(1, n/3); var b = rng.Next(n/3, 2*n/3); var c = rng.Next(2*n/3, n); var r = new int[n]; r[0] = p[0]; var pos = 1; for (var i = 1; i <= a; i++) r[pos++] = p[i]; for (var i = b+1; i <= c; i++) r[pos++] = p[i]; for (var i = a+1; i <= b; i++) r[pos++] = p[i]; for (var i = c+1; i < n; i++) r[pos++] = p[i]; return r; }
    private static int[] RunLK(DistanceMatrix m, int[] seed) => new LinKernighanSolver { Seed = seed }.Solve(m).Order.ToArray();
}
