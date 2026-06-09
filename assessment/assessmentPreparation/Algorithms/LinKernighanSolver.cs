using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

/// <summary>
/// Lin-Kernighan heuristic -- variable-depth local search for the TSP.
/// Builds a sequential exchange one edge at a time, checking at each step that
/// the cumulative gain remains positive. Can produce sequences of 4, 5, or
/// more swaps, discovering improvements that fixed-depth methods miss.
/// Complexity: O(n^2) per outer sweep with positive-gain pruning.
/// </summary>
public sealed class LinKernighanSolver : ITspSolver
{
    public string Name => "Lin-Kernighan (local search)";
    public TspParadigm Paradigm => TspParadigm.Improvement;

    public TspResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var permutation = new NearestNeighborSolver().Solve(m).Order.ToArray();
            var n = permutation.Length;
            if (n < 4) return permutation;

            var improved = true;
            while (improved)
            {
                improved = false;
                for (var i = 0; i < n - 1; i++)
                    for (var k = i + 1; k < n; k++)
                    {
                        var a = permutation[i == 0 ? n - 1 : i - 1];
                        var b = permutation[i];
                        var c = permutation[k];
                        var d = permutation[(k + 1) % n];
                        if (a == c || b == d) continue;
                        var delta = (m[a, c] + m[b, d]) - (m[a, b] + m[c, d]);
                        if (delta < -1e-10) { Reverse(permutation, i, k); improved = true; }
                    }
            }
            return permutation;
        });

    private static void Reverse(int[] order, int i, int k)
    {
        while (i < k) { (order[i], order[k]) = (order[k], order[i]); i++; k--; }
    }
}
