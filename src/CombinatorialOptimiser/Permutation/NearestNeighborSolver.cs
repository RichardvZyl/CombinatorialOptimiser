using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

// Greedy construction heuristic: starting at node 0, always extend the partial tour to
// the nearest unvisited node. Runs in O(n²) and produces a fast, reasonable starting
// tour, though early greedy choices can foreclose shorter global paths.
internal sealed class NearestNeighborSolver : ISolver<DistanceMatrix, PermutationResult>
{
    public string Name => "Nearest Neighbor (greedy)";
    public SolverParadigm Paradigm => SolverParadigm.Construction;
    public PermutationResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var n = m.Count; var used = new bool[n]; var permutation = new int[n];
            permutation[0] = 0; used[0] = true;
            for (var step = 1; step < n; step++)
            {
                var current = permutation[step - 1]; var bestIndex = -1; var bestCost = double.PositiveInfinity;
                for (var next = 0; next < n; next++)
                    if (!used[next] && m[current, next] < bestCost) { bestCost = m[current, next]; bestIndex = next; }
                permutation[step] = bestIndex; used[bestIndex] = true;
            }
            return permutation;
        });
}
