using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

// Exact depth-first branch-and-bound. Seeds an upper bound from Nearest Neighbor then
// prunes branches whose partial cost plus a lower bound (half the sum of two cheapest
// edges per unvisited node) cannot beat the incumbent. Requires n < 31; often faster
// than Held-Karp in practice due to aggressive pruning on structured instances.
internal sealed class BranchAndBoundSolver : ISolver<DistanceMatrix, PermutationResult>
{
    public string Name => "Branch and Bound (exact)";
    public SolverParadigm Paradigm => SolverParadigm.Exact;
    public PermutationResult Solve(DistanceMatrix m) => SolverRunner.Timed(Name, Paradigm, m, () => SolveImpl(m));
    private static int[] SolveImpl(DistanceMatrix m)
    {
        var n = m.Count; if (n >= 31) throw new InvalidOperationException("Branch and Bound requires n < 31 (got " + n + ").");
        if (n <= 1) return Enumerable.Range(0, n).ToArray(); if (n == 2) return new[] { 0, 1 };
        var min1 = new double[n]; var min2 = new double[n];
        for (var i = 0; i < n; i++) { var best = double.PositiveInfinity; var second = double.PositiveInfinity; for (var j = 0; j < n; j++) { if (i == j) continue; var d = m[i, j]; if (d < best) { second = best; best = d; } else if (d < second) { second = d; } } min1[i] = best; min2[i] = second; }
        var permutation = new int[n]; permutation[0] = 0;
        var nnResult = new NearestNeighborSolver().Solve(m);
        var bestLen = nnResult.Distance;
        var bestPermutation = nnResult.Order.ToArray();
        Search(1, 0, 0.0, 1); return bestPermutation;
        void Search(int depth, int last, double currentCost, int mask)
        {
            if (currentCost + LowerBound(last, mask) >= bestLen) return;
            if (depth == n) { var c = currentCost + m[last, 0]; if (c < bestLen) { bestLen = c; Array.Copy(permutation, 0, bestPermutation, 0, n); } return; }
            var remaining = new List<int>(n - depth); for (var next = 1; next < n; next++) if ((mask & (1 << next)) == 0) remaining.Add(next);
            remaining.Sort((a, b) => m[last, a].CompareTo(m[last, b]));
            foreach (var next in remaining) { var nextCost = currentCost + m[last, next]; if (nextCost >= bestLen) continue; permutation[depth] = next; Search(depth + 1, next, nextCost, mask | (1 << next)); }
        }
        double LowerBound(int current, int visitedMask) { var sum = min1[current] + min1[0]; for (var i = 1; i < n; i++) if ((visitedMask & (1 << i)) == 0) sum += min1[i] + min2[i]; return sum * 0.5; }
    }
}
