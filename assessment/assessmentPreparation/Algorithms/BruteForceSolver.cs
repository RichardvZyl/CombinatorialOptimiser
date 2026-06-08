using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

/// <summary>
/// Exact solver. Fixes city 0 as the first city and enumerates all (n-1)!
/// orderings of the remaining cities, keeping the shortest.
/// Complexity: O(n!) time, O(n) space.
/// </summary>
public sealed class BruteForceSolver : ITspSolver
{
    public string Name => "Brute Force (exact)";
    public TspParadigm Paradigm => TspParadigm.Exact;

    public TspResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var remaining = Enumerable.Range(1, m.Count - 1).ToArray();
            int[]? best = null;
            var bestCost = double.PositiveInfinity;

            foreach (var perm in Permute(remaining))
            {
                var order = new int[m.Count];
                order[0] = 0;
                Array.Copy(perm, 0, order, 1, perm.Length);
                var cost = m.TourLength(order);
                if (cost < bestCost) { bestCost = cost; best = order; }
            }
            return best ?? new[] { 0 };
        });

    private static IEnumerable<int[]> Permute(int[] a, int start = 0)
    {
        if (start >= a.Length) { yield return a.ToArray(); yield break; }
        for (var i = start; i < a.Length; i++)
        {
            (a[start], a[i]) = (a[i], a[start]);
            foreach (var perm in Permute(a, start + 1)) yield return perm;
            (a[start], a[i]) = (a[i], a[start]);
        }
    }
}
