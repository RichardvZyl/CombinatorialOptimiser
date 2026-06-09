using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

/// <summary>
/// Local-search improvement. Starts from a Nearest-Neighbor tour (or a
/// provided seed), then repeatedly reverses a segment whenever doing so
/// shortens the sequence. Runs until no improving move remains.
/// Complexity: O(n²) per sweep.
/// </summary>
public sealed class TwoOptSolver : ITspSolver
{
    public string Name => "2-opt (local search)";
    public TspParadigm Paradigm => TspParadigm.Improvement;

    /// <summary>Optional seed permutation. If null, uses Nearest Neighbor.</summary>
    public int[]? Seed { get; init; }

    public TspResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var permutation = Seed ?? new NearestNeighborSolver().Solve(m).Order.ToArray();
            var n = permutation.Length;
            if (n < 4) return permutation;

            var improved = true;
            while (improved)
            {
                improved = false;
                for (var i = 0; i < n - 1; i++)
                {
                    for (var k = i + 1; k < n; k++)
                    {
                        var a = permutation[i == 0 ? n - 1 : i - 1];
                        var b = permutation[i];
                        var c = permutation[k];
                        var d = permutation[(k + 1) % n];
                        if (a == c || b == d) continue;

                        var delta = (m[a, c] + m[b, d]) - (m[a, b] + m[c, d]);
                        if (delta < -1e-10)
                        {
                            Reverse(permutation, i, k);
                            improved = true;
                        }
                    }
                }
            }
            return permutation;
        });

    private static void Reverse(int[] order, int i, int k)
    {
        while (i < k) { (order[i], order[k]) = (order[k], order[i]); i++; k--; }
    }
}
