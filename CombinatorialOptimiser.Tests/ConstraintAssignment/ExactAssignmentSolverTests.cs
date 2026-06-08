using CombinatorialOptimiser.ConstraintAssignment;

namespace CombinatorialOptimiser.Tests.ConstraintAssignment;

public class ExactAssignmentSolverTests
{
    [Fact]
    public void Bipartite_TwoColours()
    {
        var problem = TestHelpers.MakeBipartiteGraph(4);

        var result = new BacktrackingSolver().Solve(problem);

        Assert.Equal(2, result.LabelCount);
        TestHelpers.AssertValidAssignment(result, problem);
    }

    [Fact]
    public void Triangle_ThreeColours()
    {
        var problem = AssignmentProblem.FromEdges(["A", "B", "C"], [(0, 1), (1, 2), (0, 2)]);

        var result = new BacktrackingSolver().Solve(problem);

        Assert.Equal(3, result.LabelCount);
        TestHelpers.AssertValidAssignment(result, problem);
    }

    [Fact]
    public void Complete4_FourColours()
    {
        var entities = new[] { "A", "B", "C", "D" };
        (int, int)[] edges = [(0, 1), (0, 2), (0, 3), (1, 2), (1, 3), (2, 3)];
        var problem = AssignmentProblem.FromEdges(entities, edges);

        var result = new BacktrackingSolver().Solve(problem);

        Assert.Equal(4, result.LabelCount);
        TestHelpers.AssertValidAssignment(result, problem);
    }

    [Fact]
    public void Backtracking_MatchesDsatur_Bipartite()
    {
        var problem = TestHelpers.MakeBipartiteGraph(4);

        var dsatur = new DsaturSolver().Solve(problem);
        var backtracking = new BacktrackingSolver().Solve(problem);

        Assert.Equal(dsatur.LabelCount, backtracking.LabelCount);
        TestHelpers.AssertValidAssignment(backtracking, problem);
    }
}
