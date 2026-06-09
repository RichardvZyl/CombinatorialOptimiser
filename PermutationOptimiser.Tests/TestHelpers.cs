using PermutationOptimiser.Model;

namespace PermutationOptimiser.Tests;

internal static class TestHelpers
{
    internal static Node[] MakeSeededNodes(int n, int seed)
    {
        var rng = new Random(seed);
        return Enumerable.Range(0, n).Select(i => new Node($"N{i}", rng.Next(0, 100), rng.Next(0, 100))).ToArray();
    }

    internal static (double[,] Costs, Node[] Nodes) MakeRawMatrix(int n, int seed)
    {
        var rng = new Random(seed);
        var costs = new double[n, n];
        for (var i = 0; i < n; i++)
            for (var j = i + 1; j < n; j++) {
                var d = rng.Next(1, 100); costs[i, j] = d; costs[j, i] = d; }
        var nodes = Enumerable.Range(0, n).Select(i => new Node($"N{i}")).ToArray();
        return (costs, nodes);
    }

    internal static void AssertValidTour(SolverResult result, int nodeCount)
    {
        Assert.Equal(nodeCount, result.Order.Count);
        Assert.Equal(nodeCount, new HashSet<int>(result.Order).Count);
        Assert.All(result.Order, i => Assert.InRange(i, 0, nodeCount - 1));
        Assert.True(result.Distance >= 0);
    }
}
