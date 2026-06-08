using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.SubsetSelection;

namespace CombinatorialOptimiser.Tests.SubsetSelection;

public class HeuristicSelectionSolverTests
{
    private static readonly ISolver<SelectionProblem, SelectionResult>[] Heuristics =
    [
        new GreedySelectionSolver(),
        new SaSelectionSolver(),
        new GaSelectionSolver(),
    ];

    public static IEnumerable<object[]> HeuristicData => Heuristics.Select(s => new object[] { s });

    [Theory]
    [MemberData(nameof(HeuristicData))]
#pragma warning disable CA1062 // Solver parameters are injected by xUnit [MemberData] and will never be null.
    public void ProducesValidSolution(ISolver<SelectionProblem, SelectionResult> solver)
    {
        var problem = TestHelpers.MakeKnapsack(10, 42);

        var result = solver.Solve(problem);

        TestHelpers.AssertValidSelection(result, problem);
    }

    [Fact]
    public void Greedy_MatchesOptimal_SimpleCase()
    {
        var items = new[]
        {
            new SelectionItem("A", 10, 100),
            new SelectionItem("B", 20, 50),
            new SelectionItem("C", 5, 80),
        };
        var problem = new SelectionProblem(items, 100); // capacity exceeds total cost: everything fits

        var greedy = new GreedySelectionSolver().Solve(problem);
        var dp = new DpSelectionSolver().Solve(problem);

        Assert.Equal(dp.TotalValue, greedy.TotalValue, 6);
        Assert.All(greedy.Selected, Assert.True);
    }

    [Theory]
    [MemberData(nameof(HeuristicData))]
    public void AllHeuristics_AtLeast50PctOfDpOptimal(ISolver<SelectionProblem, SelectionResult> solver)
    {
        var problem = TestHelpers.MakeKnapsack(10, 42);
        var optimal = new DpSelectionSolver().Solve(problem).TotalValue;

        var result = solver.Solve(problem).TotalValue;

        Assert.True(result >= optimal * 0.5, $"Expected >= {optimal * 0.5}, got {result}");
    }
#pragma warning restore CA1062
}
