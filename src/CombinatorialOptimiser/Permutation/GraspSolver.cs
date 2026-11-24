using System;
using System.Linq;
using System.Collections.Generic;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Greedy Randomized Adaptive Search Procedure (GRASP) simple implementation.
/// Builds randomized greedy solutions (RCL) and improves via local 2-opt.
/// </summary>
public sealed class GraspSolver : ISolver<DistanceMatrix, PermutationResult>
{
    /// <inheritdoc/>
    public string Name => "GRASP";
    /// <inheritdoc/>
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    /// <summary>Number of GRASP iterations.</summary>
    public int Iterations { get; init; } = 50;
    /// <summary>RCL cutoff factor (0–1).</summary>
    public double RclAlpha { get; init; } = 0.2;

    /// <inheritdoc/>
    public PermutationResult Solve(DistanceMatrix matrix) =>
        SolverRunner.Timed(Name, Paradigm, matrix, () =>
        {
            var n = matrix.Count;
            var rng = new Random(99);
            var best = Enumerable.Range(0, n).ToArray();
            var bestCost = matrix.TourLength(best);

            for (int it = 0; it < Iterations; it++)
            {
                var tour = BuildGreedyRandomized(matrix, rng);
                tour = TwoOptLocalImprove(matrix, tour);
                var cost = matrix.TourLength(tour);
                if (cost < bestCost)
                {
                    bestCost = cost;
                    best = tour.ToArray();
                }
            }

            return best;
        });

    private int[] BuildGreedyRandomized(DistanceMatrix matrix, Random rng)
    {
        var n = matrix.Count;
        var start = rng.Next(n);
        var tour = new List<int> { start };
        while (tour.Count < n)
        {
            var last = tour[^1];
            var candidates = Enumerable.Range(0, n).Where(i => !tour.Contains(i))
                .Select(i => (i, cost: matrix[last, i])).OrderBy(x => x.cost).ToArray();
            var cutoff = Math.Max(1, (int)(candidates.Length * RclAlpha));
            var rcl = candidates.Take(cutoff).Select(x => x.i).ToArray();
            var pick = rcl[rng.Next(rcl.Length)];
            tour.Add(pick);
        }

        return tour.ToArray();
    }

    private static int[] TwoOptLocalImprove(DistanceMatrix matrix, int[] tour)
    {
        var improved = true;
        var n = tour.Length;
        var current = tour.ToArray();
        while (improved)
        {
            improved = false;
            for (int i = 0; i < n - 1; i++)
            {
                // Accumulated cost change of reversing the interior edges t[i..j] (handles asymmetric matrices).
                var interiorDelta = 0.0;
                for (int j = i + 1; j < n; j++)
                {
                    interiorDelta += matrix[current[j], current[j - 1]] - matrix[current[j - 1], current[j]];

                    // Reversing the whole tour leaves no boundary edges; skip it.
                    if (i == 0 && j == n - 1) continue;

                    var prev = current[(i - 1 + n) % n];
                    var next = current[(j + 1) % n];
                    var boundaryDelta = matrix[prev, current[j]] + matrix[current[i], next]
                        - matrix[prev, current[i]] - matrix[current[j], next];

                    if (boundaryDelta + interiorDelta < -1e-10)
                    {
                        Array.Reverse(current, i, j - i + 1);
                        improved = true;
                        // Segment changed; restart accumulation from the new state for this i.
                        interiorDelta = 0.0;
                        for (int k = i + 1; k <= j; k++)
                            interiorDelta += matrix[current[k], current[k - 1]] - matrix[current[k - 1], current[k]];
                    }
                }
            }
        }

        return current;
    }
}
