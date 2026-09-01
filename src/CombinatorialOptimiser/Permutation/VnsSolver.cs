using System;
using System.Linq;
using System.Collections.Generic;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Variable Neighborhood Search (VNS) simple implementation alternating 2-opt and 3-opt neighborhoods.
/// </summary>
public sealed class VnsSolver : ISolver<DistanceMatrix, PermutationResult>
{
    public int Iterations { get; init; } = 50;

    public PermutationResult Solve(DistanceMatrix matrix)
    {
        var n = matrix.Count;
        var rng = new Random(42);
        var current = Enumerable.Range(0, n).OrderBy(i => rng.Next()).ToArray();
        var bestCost = matrix.TourCost(current);

        for (int it = 0; it < Iterations; it++)
        {
            // shaking: random k-opt
            var k = (it % 2) + 2; // 2 or 3
            var shaken = Shake(current, k, rng);
            var improved = LocalSearch(matrix, shaken);
            var cost = matrix.TourCost(improved);
            if (cost < bestCost)
            {
                current = improved;
                bestCost = cost;
            }
        }

        return new PermutationResult(current, bestCost);
    }

    private int[] Shake(int[] tour, int k, Random rng)
    {
        var n = tour.Length;
        var t = tour.ToArray();
        var i = rng.Next(0, n - k + 1);
        Array.Reverse(t, i, k);
        return t;
    }

    private int[] LocalSearch(DistanceMatrix matrix, int[] tour)
    {
        // simple 2-opt local search
        var improved = true;
        var current = tour.ToArray();
        var n = tour.Length;
        while (improved)
        {
            improved = false;
            for (int i = 0; i < n - 1; i++)
            for (int j = i + 1; j < n; j++)
            {
                var candidate = current.ToArray();
                Array.Reverse(candidate, i, j - i + 1);
                if (matrix.TourCost(candidate) < matrix.TourCost(current))
                {
                    current = candidate;
                    improved = true;
                }
            }
        }

        return current;
    }
}
