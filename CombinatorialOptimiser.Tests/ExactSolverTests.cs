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

    [Fact]
    public void UnitSquare_OptimalIsPerimeter()
    {
        var matrix = new DistanceMatrix(UnitSquare);
        var result = new BruteForceSolver().Solve(matrix);
        Assert.Equal(4.0, result.Distance, precision: 6);
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
