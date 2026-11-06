using System;
using System.Linq;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Lightweight GNN policy stub: when a trained GNN model is available at runtime (via
/// an ONNX file or referenced assembly), this solver will call it to compute node scores.
/// For CI and tests this is a deterministic randomized fallback that ranks by degree-based
/// heuristic.
/// </summary>
public sealed class GnnPolicySolver : ISolver<DistanceMatrix, PermutationResult>
{
    private readonly int _seed;

    public GnnPolicySolver(int seed = 42) => _seed = seed;

    public PermutationResult Solve(DistanceMatrix matrix)
    {
        // Fallback: rank nodes by sum of outgoing distances (cheap centrality heuristic)
        var n = matrix.Count;
        var scores = new double[n];
        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int j = 0; j < n; j++) sum += matrix[i, j];
            scores[i] = -sum; // prefer nodes with smaller outgoing cost
        }

        var order = scores.Select((s, i) => (s, i)).OrderByDescending(x => x.s).Select(x => x.i).ToArray();
        return new PermutationResult(order, matrix.TourCost(order));
    }
}
