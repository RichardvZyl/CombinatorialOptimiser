using System;
using System.Linq;
using System.Collections.Generic;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Simple Ant Colony Optimization solver: pheromone matrix with evaporation and basic probabilistic selection.
/// This is intentionally small and deterministic-seeded for testing.
/// </summary>
public sealed class AntColonySolver : ISolver<DistanceMatrix, PermutationResult>
{
    public int Ants { get; init; } = 10;
    public int Iterations { get; init; } = 50;
    public double Evaporation { get; init; } = 0.1;
    public double Alpha { get; init; } = 1.0;
    public double Beta { get; init; } = 2.0;

    public PermutationResult Solve(DistanceMatrix matrix)
    {
        var n = matrix.Count;
        var rng = new Random(123);
        var pher = Enumerable.Range(0, n).Select(_ => Enumerable.Range(0, n).Select(__ => 1.0).ToArray()).ToArray();
        var bestTour = Enumerable.Range(0, n).ToArray();
        var bestCost = matrix.TourCost(bestTour);

        for (int iter = 0; iter < Iterations; iter++)
        {
            var ants = new List<int[]>();
            for (int a = 0; a < Ants; a++)
            {
                var tour = BuildTour(matrix, pher, rng);
                var cost = matrix.TourCost(tour);
                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestTour = tour.ToArray();
                }
                ants.Add(tour);
            }

            // evaporate
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    pher[i][j] *= (1.0 - Evaporation);

            // deposit
            foreach (var tour in ants)
            {
                var cost = matrix.TourCost(tour);
                for (int i = 0; i < n; i++)
                {
                    var a = tour[i];
                    var b = tour[(i + 1) % n];
                    pher[a][b] += 1.0 / Math.Max(1e-9, cost);
                }
            }
        }

        return new PermutationResult(bestTour, bestCost);
    }

    private int[] BuildTour(DistanceMatrix matrix, double[][] pher, Random rng)
    {
        var n = matrix.Count;
        var tour = new List<int>();
        var start = rng.Next(n);
        tour.Add(start);
        while (tour.Count < n)
        {
            var last = tour[^1];
            var unvisited = Enumerable.Range(0, n).Where(i => !tour.Contains(i)).ToArray();
            var probs = new double[unvisited.Length];
            double sum = 0;
            for (int k = 0; k < unvisited.Length; k++)
            {
                var j = unvisited[k];
                var tau = Math.Pow(pher[last][j], Alpha);
                var eta = Math.Pow(1.0 / Math.Max(1e-9, matrix[last, j]), Beta);
                probs[k] = tau * eta;
                sum += probs[k];
            }
            var pick = rng.NextDouble() * sum;
            double acc = 0;
            for (int k = 0; k < unvisited.Length; k++)
            {
                acc += probs[k];
                if (pick <= acc)
                {
                    tour.Add(unvisited[k]);
                    break;
                }
            }
        }

        return tour.ToArray();
    }
}
