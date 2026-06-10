using CombinatorialOptimiser.SubsetSelection;

namespace CombinatorialOptimiser.Tests.SubsetSelection;

public class ExactSelectionSolverTests
{
    [Fact]
    public void Dp_KnownOptimal_3Items()
    {
        var items = new[]
        {
            new SelectionItem("A", 10, 60),
            new SelectionItem("B", 20, 100),
            new SelectionItem("C", 30, 120),
        };
        var problem = new SelectionProblem(items, 50);

        var result = new DpSelectionSolver().Solve(problem);

        Assert.Equal(220, result.TotalValue, 6);
        TestHelpers.AssertValidSelection(result, problem);
    }

    [Fact]
    public void Dp_AllFit_SelectsAll()
    {
        var items = new[]
        {
            new SelectionItem("A", 5, 10),
            new SelectionItem("B", 5, 10),
            new SelectionItem("C", 5, 10),
        };
        var problem = new SelectionProblem(items, 100);

        var result = new DpSelectionSolver().Solve(problem);

        Assert.All(result.Selected, Assert.True);
        Assert.Equal(30, result.TotalValue, 6);
    }

    [Fact]
    public void Dp_NoneFit_SelectsNone()
    {
        var items = new[]
        {
            new SelectionItem("A", 10, 5),
            new SelectionItem("B", 20, 8),
        };
        var problem = new SelectionProblem(items, 5);

        var result = new DpSelectionSolver().Solve(problem);

        Assert.All(result.Selected, Assert.False);
        Assert.Equal(0, result.TotalValue, 6);
    }

    [Fact]
    public void BnB_MatchesDp_5Items()
    {
        var problem = TestHelpers.MakeKnapsack(5, 7);

        var dp = new DpSelectionSolver().Solve(problem);
        var bnb = new BnBSelectionSolver().Solve(problem);

        Assert.Equal(dp.TotalValue, bnb.TotalValue, 6);
        TestHelpers.AssertValidSelection(bnb, problem);
    }
}
