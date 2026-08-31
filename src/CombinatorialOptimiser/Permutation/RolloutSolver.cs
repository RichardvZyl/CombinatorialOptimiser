using System;
using System.Linq;
using System.Collections.Generic;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Rollout policy: given a base policy (e.g., nearest neighbour or GNN policy), perform
/// one-step lookahead rollouts replacing the next action with the best observed rollout.
/// This is a simple deterministic rollout engine for testing.
/// </summary>
public sealed class RolloutSolver : ISolver<DistanceMatrix, PermutationResult>
{
    public ISolver<DistanceMatrix, PermutationResult> BasePolicy { get; init; } = new NearestNeighborSolver();

    public PermutationResult Solve(DistanceMatrix matrix)
    {
        var n = matrix.Count;
        var order = new List<int>();
        var used = new HashSet<int>();

        for (int step = 0; step < n; step++)
        {
            var candidates = Enumerable.Range(0, n).Where(i => !used.Contains(i)).ToArray();
            int best = candidates[0];
            double bestCost = double.PositiveInfinity;
            foreach (var c in candidates)
            {
                var prefix = order.Concat(new[] { c }).ToArray();
                // build a matrix with prefix fixed by setting huge costs for prefix transitions to force construction
                var simulated = SimulateWithPrefix(matrix, prefix);
                var res = BasePolicy.Solve(simulated);
                var cost = res.Cost + matrix.TourCost(order.Concat(new[] { c }).ToArray());
                if (cost < bestCost)
                {
                    bestCost = cost;
                    best = c;
                }
            }

            order.Add(best);
            used.Add(best);
        }

        return new PermutationResult(order.ToArray(), matrix.TourCost(order.ToArray()));
    }

    private DistanceMatrix SimulateWithPrefix(DistanceMatrix original, int[] prefix)
    {
        // For tests we just return the original; a real implementation would create a modified view
        // where the prefix is fixed and the remainder is the subproblem.
        return original;
    }
}
