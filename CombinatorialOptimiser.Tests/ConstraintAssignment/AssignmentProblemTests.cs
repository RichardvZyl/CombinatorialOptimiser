using CombinatorialOptimiser.ConstraintAssignment;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.ConstraintAssignment;

public class AssignmentProblemTests
{
    [Fact]
    public void NullEntities_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new AssignmentProblem(null!, new bool[0, 0]));
    }

    [Fact]
    public void MismatchedConflictSize_Throws()
    {
        var entities = new[] { "A", "B", "C" };
        var conflicts = new bool[2, 2];
        Assert.Throws<ArgumentException>(() => new AssignmentProblem(entities, conflicts));
    }

    [Fact]
    public void FromEdges_SetsConflicts()
    {
        var entities = new[] { "A", "B", "C" };
        var problem = AssignmentProblem.FromEdges(entities, [(0, 1), (1, 2)]);

        Assert.True(problem.HasConflict(0, 1));
        Assert.True(problem.HasConflict(1, 0));
        Assert.True(problem.HasConflict(1, 2));
        Assert.False(problem.HasConflict(0, 2));
    }

    [Fact]
    public void IsValid_RejectsConflict()
    {
        var problem = AssignmentProblem.FromEdges(["A", "B"], [(0, 1)]);
        var result = new AssignmentResult("Test", SolverParadigm.Construction, [0, 0], 1, TimeSpan.Zero);

        Assert.False(result.IsValid(problem));
    }
}
