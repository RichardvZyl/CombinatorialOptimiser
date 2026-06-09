using AssessmentPreparation.Algorithms;
using AssessmentPreparation.Model;

namespace PermutationOptimiser.Tests;

public class ExactSolverTests
{
    private static readonly Node[] UnitSquare = [new Node("SW",0,0),new Node("SE",1,0),new Node("NE",1,1),new Node("NW",0,1)];
    [Fact] public void UnitSquare_OptimalIsPerimeter() { Assert.Equal(4.0, new BruteForceSolver().Solve(new DistanceMatrix(UnitSquare)).Distance, 6); }
    [Fact] public void UnitSquare_AllExactSolversAgree()
    {
        var m = new DistanceMatrix(UnitSquare);
        Assert.Equal(new BruteForceSolver().Solve(m).Distance, new HeldKarpSolver().Solve(m).Distance, 4);
        Assert.Equal(new BruteForceSolver().Solve(m).Distance, new BranchAndBoundSolver().Solve(m).Distance, 4);
    }
    [Theory][InlineData(4,99)][InlineData(5,17)]
    public void SmallRandom_AllExactSolversAgree(int n, int seed)
    {
        var m = new DistanceMatrix(TestHelpers.MakeSeededNodes(n, seed));
        Assert.Equal(new BruteForceSolver().Solve(m).Distance, new HeldKarpSolver().Solve(m).Distance, 4);
        Assert.Equal(new BruteForceSolver().Solve(m).Distance, new BranchAndBoundSolver().Solve(m).Distance, 4);
    }
}
