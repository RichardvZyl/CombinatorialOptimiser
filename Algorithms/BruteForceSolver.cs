using PermutationOptimiser.Model;

namespace PermutationOptimiser.Algorithms;

public sealed class BruteForceSolver : ISolver
{
    public string Name => "Brute Force (exact)";
    public SolverParadigm Paradigm => SolverParadigm.Exact;

    public SolverResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var n = m.Count;
            if (n <= 1) return new[] { 0 };
            var order = new int[n];
            order[0] = 0;
            var remaining = Enumerable.Range(1, n - 1).ToArray();
            var permuteInput = (int[])remaining.Clone();
            int[] best = null!;
            var bestCost = double.PositiveInfinity;

            foreach (var perm in Permute(permuteInput, 0))
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
        // Heap's non-recursive algorithm to generate all permutations.
        var n = a.Length;
        var c = new int[n];
        yield return (int[])a.Clone();

        var i = 0;
        while (i < n)
        {
            if (c[i] < i)
            {
                var swapIndex = i % 2 == 0 ? 0 : c[i];
                (a[swapIndex], a[i]) = (a[i], a[swapIndex]);
                yield return (int[])a.Clone();
                c[i]++;
                i = 0;
            }
            else
            {
                c[i] = 0;
                i++;
            }
        }
    }
}
