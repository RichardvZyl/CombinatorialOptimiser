using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

/// <summary>
/// 3-opt local search. An extension of 2-opt that considers removing three edges
/// and reconnecting the three resulting segments in every profitable way.
/// Checks four possible reconnections: three 2-opt-style moves and one 3-opt move.
/// Complexity: O(n³) per sweep.
/// </summary>
public sealed class ThreeOptSolver : ITspSolver
{
    public string Name => "3-opt (local search)";

    public TspResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, m, () =>
        {
            var permutation = new NearestNeighborSolver().Solve(m).Order.ToArray();
            var n = permutation.Length;
            if (n < 4) return permutation;

            var improved = true;
            while (improved)
            {
                improved = false;
                for (var a = 0; a < n - 1; a++)
                {
                    for (var b = a + 1; b < n; b++)
                    {
                        for (var c = b + 1; c < n; c++)
                        {
                            var i1 = a;
                            var i2 = (a + 1) % n;
                            var i3 = b;
                            var i4 = (b + 1) % n;
                            var i5 = c;
                            var i6 = (c + 1) % n;

                            var p1 = permutation[i1];
                            var p2 = permutation[i2];
                            var p3 = permutation[i3];
                            var p4 = permutation[i4];
                            var p5 = permutation[i5];
                            var p6 = permutation[i6];

                            // 2-opt between E1 and E2
                            var gain12 = (m[p1, p3] + m[p2, p4]) - (m[p1, p2] + m[p3, p4]);
                            if (gain12 < -1e-10)
                            {
                                Reverse(permutation, i2, i3);
                                improved = true;
                                goto nextA;
                            }

                            // 2-opt between E1 and E3
                            var gain13 = (m[p1, p5] + m[p2, p6]) - (m[p1, p2] + m[p5, p6]);
                            if (gain13 < -1e-10)
                            {
                                Reverse(permutation, i2, i5);
                                improved = true;
                                goto nextA;
                            }

                            // 2-opt between E2 and E3
                            var gain23 = (m[p3, p5] + m[p4, p6]) - (m[p3, p4] + m[p5, p6]);
                            if (gain23 < -1e-10)
                            {
                                Reverse(permutation, i4, i5);
                                improved = true;
                                goto nextA;
                            }

                            // 3-opt
                            var gain3opt = (m[p1, p4] + m[p3, p6] + m[p5, p2])
                                         - (m[p1, p2] + m[p3, p4] + m[p5, p6]);
                            if (gain3opt < -1e-10)
                            {
                                Reverse(permutation, i4, i5);
                                Reverse(permutation, i2, i5);
                                improved = true;
                                goto nextA;
                            }
                        }
                    }
                nextA:;
                }
            }
            return permutation;
        });

    private static void Reverse(int[] order, int i, int k)
    {
        while (i < k)
        {
            (order[i], order[k]) = (order[k], order[i]);
            i++;
            k--;
        }
    }
}
