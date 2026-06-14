using CombinatorialOptimiser.ConstraintAssignment;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.SubsetSelection;

namespace CombinatorialOptimiser.Tests;

public class CancellationTests
{
    private static readonly CancellationToken PreCancelled = new(canceled: true);

    [Fact]
    public void SimulatedAnnealing_Permutation_PreCancelled_Throws()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(10, seed: 1);
        var matrix = new DistanceMatrix(costs, nodes);
        var solver = new SimulatedAnnealingSolver();

        Assert.Throws<OperationCanceledException>(() => solver.Solve(matrix, PreCancelled));
    }

    [Fact]
    public void GeneticAlgorithm_Permutation_PreCancelled_Throws()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(10, seed: 2);
        var matrix = new DistanceMatrix(costs, nodes);
        var solver = new GeneticAlgorithmSolver();

        Assert.Throws<OperationCanceledException>(() => solver.Solve(matrix, PreCancelled));
    }

    [Fact]
    public void IteratedLocalSearch_Permutation_PreCancelled_Throws()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(10, seed: 3);
        var matrix = new DistanceMatrix(costs, nodes);
        var solver = new IteratedLocalSearchSolver();

        Assert.Throws<OperationCanceledException>(() => solver.Solve(matrix, PreCancelled));
    }

    [Fact]
    public void SaSelectionSolver_PreCancelled_Throws()
    {
        var problem = TestHelpers.MakeKnapsack(12, seed: 4);
        var solver = new SaSelectionSolver();

        Assert.Throws<OperationCanceledException>(() => solver.Solve(problem, PreCancelled));
    }

    [Fact]
    public void GaSelectionSolver_PreCancelled_Throws()
    {
        var problem = TestHelpers.MakeKnapsack(12, seed: 5);
        var solver = new GaSelectionSolver();

        Assert.Throws<OperationCanceledException>(() => solver.Solve(problem, PreCancelled));
    }

    [Fact]
    public void SaAssignmentSolver_PreCancelled_Throws()
    {
        var problem = TestHelpers.MakeBipartiteGraph(8);
        var solver = new SaAssignmentSolver();

        Assert.Throws<OperationCanceledException>(() => solver.Solve(problem, PreCancelled));
    }

    [Fact]
    public void GaAssignmentSolver_PreCancelled_Throws()
    {
        var problem = TestHelpers.MakeBipartiteGraph(8);
        var solver = new GaAssignmentSolver();

        Assert.Throws<OperationCanceledException>(() => solver.Solve(problem, PreCancelled));
    }

    [Fact]
    public async Task SolveAsync_PreCancelled_ThrowsOperationCanceled()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(10, seed: 6);
        var matrix = new DistanceMatrix(costs, nodes);
        var solver = new SimulatedAnnealingSolver();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            ((ISolver<DistanceMatrix, PermutationResult>)solver).SolveAsync(matrix, PreCancelled));
    }
}
