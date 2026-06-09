using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

public sealed class TwoOptSolver : ISolver
{
    public string Name => "2-opt (local search)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    public int[]? Seed { get; init; }
    public SolverResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var permutation = Seed ?? new NearestNeighborSolver().Solve(m).Order.ToArray();
            var n = permutation.Length; if (n < 4) return permutation;
            var improved = true;
            while (improved)
            {
                improved = false;
                for (var i = 0; i < n - 1; i++)
                    for (var k = i + 1; k < n; k++)
                    {
                        var a = permutation[i == 0 ? n - 1 : i - 1]; var b = permutation[i]; var c = permutation[k]; var d = permutation[(k + 1) % n];
                        if (a == c || b == d) continue;
                        if ((m[a, c] + m[b, d]) - (m[a, b] + m[c, d]) < -1e-10) { Reverse(permutation, i, k); improved = true; }
                    }
            }
            return permutation;
        });
    private static void Reverse(int[] order, int i, int k) { while (i < k) { (order[i], order[k]) = (order[k], order[i]); i++; k--; } }
}
