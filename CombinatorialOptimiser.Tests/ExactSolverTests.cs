using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests;

public class ExactSolverTests
{
    private static readonly Node[] UnitSquare =
    [
        new Node("SW", 0, 0),
        new Node("SE", 1, 0),
        new Node("NE", 1, 1),
        new Node("NW", 0, 1),
    ];

    public static TheoryData<ISolver<DistanceMatrix, PermutationResult>> BruteForceSolvers =>
        new() { new BruteForceSolver(), new RecursiveBruteForceSolver() };

    [Theory]
    [MemberData(nameof(BruteForceSolvers))]
    public void UnitSquare_OptimalIsPerimeter(ISolver<DistanceMatrix, PermutationResult> solver)
    {
        ArgumentNullException.ThrowIfNull(solver);
        var matrix = new DistanceMatrix(UnitSquare);
        Assert.Equal(4.0, solver.Solve(matrix).Distance, precision: 6);
    }

    [Theory]
    [InlineData(4, 7)]
    [InlineData(5, 13)]
    public void BothBruteForce_SmallRandom_AgreeOnOptimal(int n, int seed)
    {
        var matrix = new DistanceMatrix(TestHelpers.MakeSeededNodes(n, seed));
        var heapDist = new BruteForceSolver().Solve(matrix).Distance;
        var recursiveDist = new RecursiveBruteForceSolver().Solve(matrix).Distance;
        Assert.Equal(heapDist, recursiveDist, precision: 6);
    }

    [Fact]
    public void UnitSquare_HeldKarpIsOptimal()
    {
        var matrix = new DistanceMatrix(UnitSquare);
        var result = new HeldKarpSolver().Solve(matrix);
        Assert.Equal(4.0, result.Distance, precision: 6);
    }

    [Theory]
    [InlineData(4, 7)]
    [InlineData(5, 13)]
    public void SmallRandom_TourIsValid(int n, int seed)
    {
        var matrix = new DistanceMatrix(TestHelpers.MakeSeededNodes(n, seed));
        TestHelpers.AssertValidTour(new HeldKarpSolver().Solve(matrix), n);
    }
}
