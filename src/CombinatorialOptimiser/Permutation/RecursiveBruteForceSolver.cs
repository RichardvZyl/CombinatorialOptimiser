using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

// Exact solver: enumerates all (n-1)! permutations via swap-based recursive backtracking
// and returns the shortest tour. O(n!) — only viable for n ≤ 12. The swap/recurse/unswap
// structure is easier to verify by inspection than Heap's algorithm but allocates one
// iterator state machine per recursion depth.
internal sealed class RecursiveBruteForceSolver : ISolver<DistanceMatrix, PermutationResult>
{
    private static readonly int[] _singleZero = [0];

    public string Name => "Brute Force – Recursive (exact)";
    public SolverParadigm Paradigm => SolverParadigm.Exact;

    public PermutationResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var n = m.Count;
            if (n <= 1) return _singleZero;
            if (n > 12) throw new InvalidOperationException("Brute force requires n ≤ 12 (got " + n + "). Use HeldKarp or BranchAndBound for larger inputs.");
            var order = new int[n];
            order[0] = 0;
            var remaining = Enumerable.Range(1, n - 1).ToArray();
            int[] best = null!;
            var bestCost = double.PositiveInfinity;

            foreach (var perm in Permute(remaining, 0))
            {
                Array.Copy(perm, 0, order, 1, perm.Length);
                var cost = m.TourLength(order);
                if (cost < bestCost)
                {
                    bestCost = cost;
                    best = (int[])order.Clone();
                }
            }
            return best;
        });

    private static IEnumerable<int[]> Permute(int[] a, int start)
    {
        if (start >= a.Length) { yield return (int[])a.Clone(); yield break; }
        for (var i = start; i < a.Length; i++)
        {
            (a[start], a[i]) = (a[i], a[start]);
            foreach (var p in Permute(a, start + 1)) yield return p;
            (a[start], a[i]) = (a[i], a[start]);
        }
    }
}
