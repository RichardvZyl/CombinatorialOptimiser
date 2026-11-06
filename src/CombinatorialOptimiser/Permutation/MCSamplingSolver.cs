using System;
using System.Linq;
using System.Collections.Generic;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Monte Carlo sampling solver: samples random tours and keeps the best found within budget.
/// Deterministic seed ensures test reproducibility.
/// </summary>
public sealed class MCSamplingSolver : ISolver<DistanceMatrix, PermutationResult>
{
    public int Samples { get; init; } = 1000;

    public PermutationResult Solve(DistanceMatrix matrix)
    {
        var n = matrix.Count;
        var rng = new Random(1234);
        var best = Enumerable.Range(0, n).ToArray();
        var bestCost = matrix.TourCost(best);

        var arr = Enumerable.Range(0, n).ToArray();
        for (int s = 0; s < Samples; s++)
        {
            // Fisher-Yates shuffle
            for (int i = n - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }

            var cost = matrix.TourCost(arr);
            if (cost < bestCost)
            {
                bestCost = cost;
                best = arr.ToArray();
            }
        }

        return new PermutationResult(best, bestCost);
    }
}
