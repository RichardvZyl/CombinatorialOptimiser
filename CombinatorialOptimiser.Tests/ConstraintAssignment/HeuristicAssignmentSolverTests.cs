using CombinatorialOptimiser.ConstraintAssignment;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.ConstraintAssignment;

public class HeuristicAssignmentSolverTests
{
    private static readonly ISolver<AssignmentProblem, AssignmentResult>[] Heuristics =
    [
        new DsaturSolver(),
        new SaAssignmentSolver(),
        new GaAssignmentSolver(),
    ];

    public static IEnumerable<object[]> HeuristicData => Heuristics.Select(s => new object[] { s });

    [Theory]
    [MemberData(nameof(HeuristicData))]
#pragma warning disable CA1062 // Solver parameters are injected by xUnit [MemberData] and will never be null.
    public void ProducesValidColouring(ISolver<AssignmentProblem, AssignmentResult> solver)
    {
        var problem = TestHelpers.MakeBipartiteGraph(4);

        var result = solver.Solve(problem);

        TestHelpers.AssertValidAssignment(result, problem);
    }

    [Fact]
    public void Dsatur_BipartiteIsOptimal()
    {
        var problem = TestHelpers.MakeBipartiteGraph(4);

        var result = new DsaturSolver().Solve(problem);

        Assert.Equal(2, result.LabelCount);
    }

    [Theory]
    [MemberData(nameof(HeuristicData))]
    public void AllHeuristics_WithinTwoTimesChromatic(ISolver<AssignmentProblem, AssignmentResult> solver)
    {
        var problem = TestHelpers.MakeBipartiteGraph(4);
        var chromatic = new BacktrackingSolver().Solve(problem).LabelCount;

        var result = solver.Solve(problem);

        Assert.True(result.LabelCount <= 2 * chromatic, $"Expected <= {2 * chromatic}, got {result.LabelCount}");
    }
#pragma warning restore CA1062
}
