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
        /// <summary>Number of iterations.</summary>
    public int Iterations { get; init; } = 50;
        /// <summary>RCL Alpha param.</summary>
    public double RclAlpha { get; init; } = 0.2;

        /// <inheritdoc/>
    public PermutationResult Solve(DistanceMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
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

        return SolverRunner.Timed(Name, Paradigm, matrix, () => best);
    }

    private int[] BuildGreedyRandomized(DistanceMatrix matrix, Random rng)
    {
        ArgumentNullException.ThrowIfNull(matrix);
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
            for (int j = i + 1; j < n; j++)
            {
                var candidate = current.ToArray();
                Array.Reverse(candidate, i, j - i + 1);
                if (matrix.TourLength(candidate) < matrix.TourLength(current))
                {
                    current = candidate;
                    improved = true;
                }
            }
        }

        return current;
    }
}
