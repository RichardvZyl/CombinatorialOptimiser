using CombinatorialOptimiser.ConstraintAssignment;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.SubsetSelection;

namespace CombinatorialOptimiser.Tests;

public class SolverRegistryTests
{
    [Theory]
    [InlineData(4, typeof(BruteForceSolver))]
    [InlineData(10, typeof(BruteForceSolver))]
    [InlineData(11, typeof(HeldKarpSolver))]
    [InlineData(16, typeof(HeldKarpSolver))]
    [InlineData(17, typeof(ChristofidesSolver))]
    [InlineData(40, typeof(ChristofidesSolver))]
    [InlineData(41, typeof(LinKernighanSolver))]
    public void RecommendPermutation_PicksExpectedSolverBySize(int nodeCount, Type expected)
    {
        var solver = SolverRegistry.RecommendPermutation(nodeCount);
        Assert.IsType(expected, solver);
    }

    [Theory]
    [InlineData(10, typeof(DpSelectionSolver))]
    [InlineData(30, typeof(DpSelectionSolver))]
    [InlineData(31, typeof(GaSelectionSolver))]
    public void RecommendSelection_PicksExpectedSolverBySize(int itemCount, Type expected)
    {
        var solver = SolverRegistry.RecommendSelection(itemCount);
        Assert.IsType(expected, solver);
    }

    [Theory]
    [InlineData(20, typeof(BacktrackingSolver))]
    [InlineData(21, typeof(GaAssignmentSolver))]
    public void RecommendAssignment_PicksExpectedSolverBySize(int entityCount, Type expected)
    {
        var solver = SolverRegistry.RecommendAssignment(entityCount);
        Assert.IsType(expected, solver);
    }

    [Fact]
    public void AllPermutationSolvers_SmallProblem_IncludesExactSolvers()
    {
        var solvers = SolverRegistry.AllPermutationSolvers(8);
        Assert.Contains(solvers, s => s is BruteForceSolver);
        Assert.Contains(solvers, s => s is RecursiveBruteForceSolver);
        Assert.Contains(solvers, s => s is HeldKarpSolver);
    }

    [Theory]
    [InlineData(5, typeof(BruteForceSolver))]
    [InlineData(11, typeof(HeldKarpSolver))]
    [InlineData(17, typeof(NearestNeighborSolver))]
    [InlineData(50, typeof(IteratedLocalSearchSolver))]
    public void AllPermutationSolvers_ReturnsRepresentativeSolverTypes(int nodeCount, Type expectedRepresentative)
    {
        var solvers = SolverRegistry.AllPermutationSolvers(nodeCount, matrix: null);
        Assert.Contains(solvers, s => expectedRepresentative.IsInstanceOfType(s));
    }

    [Fact]
    public void AllPermutationSolvers_LargeProblem_ExcludesExpensiveExactSolvers()
    {
        var solvers = SolverRegistry.AllPermutationSolvers(25);
        Assert.DoesNotContain(solvers, s => s is BruteForceSolver);
        Assert.DoesNotContain(solvers, s => s is RecursiveBruteForceSolver);
        Assert.DoesNotContain(solvers, s => s is BranchAndBoundSolver);
        Assert.DoesNotContain(solvers, s => s is HeldKarpSolver);
        Assert.DoesNotContain(solvers, s => s is ChristofidesSolver { UseExactMatching: true });
    }

    [Fact]
    public void AllPermutationSolvers_WithMatrix_IncludesChristofidesSeededVariants()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(8, seed: 1);
        var matrix = new DistanceMatrix(costs, nodes);

        var solvers = SolverRegistry.AllPermutationSolvers(8, matrix);

        Assert.Contains(solvers, s => s is IteratedLocalSearchSolver { Seed: not null });
    }
}
