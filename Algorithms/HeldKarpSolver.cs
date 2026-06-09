using PermutationOptimiser.Model;

namespace PermutationOptimiser.Algorithms;

public sealed class HeldKarpSolver : ISolver
{
    public string Name => "Held-Karp DP (exact)";
    public SolverParadigm Paradigm => SolverParadigm.Exact;
    public SolverResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var n = m.Count; if (n <= 1) return Enumerable.Range(0, n).ToArray();
            if (n >= 31) throw new InvalidOperationException("Held-Karp requires n < 31 (got " + n + ").");
            var totalMasks = 1 << n; var dp = new double[totalMasks, n]; var parent = new int[totalMasks, n];
            for (var mask = 0; mask < totalMasks; mask++) for (var j = 0; j < n; j++) dp[mask, j] = double.PositiveInfinity;
            dp[1, 0] = 0;
            for (var mask = 1; mask < totalMasks; mask++)
            {
                if ((mask & 1) == 0) continue;
                for (var last = 0; last < n; last++)
                {
                    if ((mask & (1 << last)) == 0) continue;
                    var costToLast = dp[mask, last]; if (double.IsPositiveInfinity(costToLast)) continue;
                    for (var next = 0; next < n; next++)
                    {
                        if ((mask & (1 << next)) != 0) continue;
                        var nextMask = mask | (1 << next); var candidate = costToLast + m[last, next];
                        if (candidate < dp[nextMask, next]) { dp[nextMask, next] = candidate; parent[nextMask, next] = last; }
                    }
                }
            }
            var fullSet = totalMasks - 1; var bestEnd = 0; var bestCost = double.PositiveInfinity;
            for (var last = 1; last < n; last++) { var c = dp[fullSet, last] + m[last, 0]; if (c < bestCost) { bestCost = c; bestEnd = last; } }
            var permutation = new int[n]; var maskNow = fullSet; var node = bestEnd;
            for (var i = n - 1; i >= 0; i--) { permutation[i] = node; var prev = parent[maskNow, node]; maskNow ^= (1 << node); node = prev; }
            return permutation;
        });
}
