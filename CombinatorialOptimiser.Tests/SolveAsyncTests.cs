using CombinatorialOptimiser.ConstraintAssignment;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.SubsetSelection;

namespace CombinatorialOptimiser.Tests;

public class SolveAsyncTests
{
    [Fact]
    public async Task SimulatedAnnealing_SolveAsync_MatchesSolve()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(10, seed: 1);
        var matrix = new DistanceMatrix(costs, nodes);
        var solver = new SimulatedAnnealingSolver();

        var sync = solver.Solve(matrix);
        var async = await ((ISolver<DistanceMatrix, PermutationResult>)solver).SolveAsync(matrix);

        Assert.Equal(sync.Distance, async.Distance);
        Assert.Equal(sync.Order, async.Order);
    }

    [Fact]
    public async Task GeneticAlgorithm_SolveAsync_MatchesSolve()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(10, seed: 2);
        var matrix = new DistanceMatrix(costs, nodes);
        var solver = new GeneticAlgorithmSolver();

        var sync = solver.Solve(matrix);
        var async = await ((ISolver<DistanceMatrix, PermutationResult>)solver).SolveAsync(matrix);

        Assert.Equal(sync.Distance, async.Distance);
        Assert.Equal(sync.Order, async.Order);
    }

    [Fact]
    public async Task IteratedLocalSearch_SolveAsync_MatchesSolve()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(10, seed: 3);
        var matrix = new DistanceMatrix(costs, nodes);
        var solver = new IteratedLocalSearchSolver();

        var sync = solver.Solve(matrix);
        var async = await ((ISolver<DistanceMatrix, PermutationResult>)solver).SolveAsync(matrix);

        Assert.Equal(sync.Distance, async.Distance);
        Assert.Equal(sync.Order, async.Order);
    }

    [Fact]
    public async Task SaSelectionSolver_SolveAsync_MatchesSolve()
    {
        var problem = TestHelpers.MakeKnapsack(12, seed: 4);
        var solver = new SaSelectionSolver();

        var sync = solver.Solve(problem);
        var async = await ((ISolver<SelectionProblem, SelectionResult>)solver).SolveAsync(problem);

        Assert.Equal(sync.TotalValue, async.TotalValue);
        Assert.Equal(sync.Selected, async.Selected);
    }

    [Fact]
    public async Task GaSelectionSolver_SolveAsync_MatchesSolve()
    {
        var problem = TestHelpers.MakeKnapsack(12, seed: 5);
        var solver = new GaSelectionSolver();

        var sync = solver.Solve(problem);
        var async = await ((ISolver<SelectionProblem, SelectionResult>)solver).SolveAsync(problem);

        Assert.Equal(sync.TotalValue, async.TotalValue);
        Assert.Equal(sync.Selected, async.Selected);
    }

    [Fact]
    public async Task SaAssignmentSolver_SolveAsync_MatchesSolve()
    {
        var problem = TestHelpers.MakeBipartiteGraph(8);
        var solver = new SaAssignmentSolver();

        var sync = solver.Solve(problem);
        var async = await ((ISolver<AssignmentProblem, AssignmentResult>)solver).SolveAsync(problem);

        Assert.Equal(sync.LabelCount, async.LabelCount);
        Assert.Equal(sync.Labels, async.Labels);
    }

    [Fact]
    public async Task GaAssignmentSolver_SolveAsync_MatchesSolve()
    {
        var problem = TestHelpers.MakeBipartiteGraph(8);
        var solver = new GaAssignmentSolver();

        var sync = solver.Solve(problem);
        var async = await ((ISolver<AssignmentProblem, AssignmentResult>)solver).SolveAsync(problem);

        Assert.Equal(sync.LabelCount, async.LabelCount);
        Assert.Equal(sync.Labels, async.Labels);
    }
}
