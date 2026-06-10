using CombinatorialOptimiser.ConstraintAssignment;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.SubsetSelection;

namespace CombinatorialOptimiser.Tests;

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

    internal static void AssertValidTour(PermutationResult result, int nodeCount)
    {
        Assert.Equal(nodeCount, result.Order.Count);
        Assert.Equal(nodeCount, new HashSet<int>(result.Order).Count);
        Assert.All(result.Order, i => Assert.InRange(i, 0, nodeCount - 1));
        Assert.True(result.Distance >= 0);
    }

    internal static SelectionProblem MakeKnapsack(int n, int seed)
    {
        var rng = new Random(seed);
        var items = Enumerable.Range(0, n)
            .Select(i => new SelectionItem($"Item{i}", rng.Next(1, 20), rng.Next(1, 50)))
            .ToArray();
        var capacity = items.Sum(item => item.Cost) * 0.5;
        return new SelectionProblem(items, capacity);
    }

    internal static void AssertValidSelection(SelectionResult result, SelectionProblem problem)
    {
        Assert.Equal(problem.Items.Count, result.Selected.Length);
        Assert.True(result.TotalCost <= problem.Capacity + 1e-9);

        double expectedValue = 0, expectedCost = 0;
        for (var i = 0; i < result.Selected.Length; i++)
            if (result.Selected[i]) { expectedValue += problem.Items[i].Value; expectedCost += problem.Items[i].Cost; }

        Assert.Equal(expectedValue, result.TotalValue, 6);
        Assert.Equal(expectedCost, result.TotalCost, 6);
    }

    /// <summary>Complete bipartite graph K(n,n): chromatic number 2.</summary>
    internal static AssignmentProblem MakeBipartiteGraph(int n)
    {
        var entities = Enumerable.Range(0, 2 * n).Select(i => $"V{i}").ToArray();
        var edges = new List<(int, int)>();
        for (var a = 0; a < n; a++)
            for (var b = n; b < 2 * n; b++)
                edges.Add((a, b));
        return AssignmentProblem.FromEdges(entities, edges.ToArray());
    }

    internal static void AssertValidAssignment(AssignmentResult result, AssignmentProblem problem)
    {
        Assert.Equal(problem.Count, result.Labels.Length);
        Assert.True(result.IsValid(problem));
        Assert.Equal(result.Labels.Distinct().Count(), result.LabelCount);
        Assert.True(result.LabelCount >= 1);
    }
}
