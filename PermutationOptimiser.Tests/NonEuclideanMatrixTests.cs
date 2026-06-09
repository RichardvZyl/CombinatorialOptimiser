using PermutationOptimiser.Algorithms;
using PermutationOptimiser.Model;

namespace PermutationOptimiser.Tests;

public class NonEuclideanMatrixTests
{
    private static readonly double[,] Costs = new double[4,4]{{0,5,8,3},{5,0,2,7},{8,2,0,4},{3,7,4,0}};
    private static readonly Node[] Nodes = [new Node("Print"),new Node("Cut"),new Node("Weld"),new Node("Assemble")];
    private static DistanceMatrix M => new(Costs, Nodes);
    [Fact] public void FromMatrix_Symmetry() { var m = M; for (var i = 0; i < 4; i++) for (var j = 0; j < 4; j++) Assert.Equal(m[i,j], m[j,i]); }
    [Fact] public void FromMatrix_DiagonalIsZero() { var m = M; for (var i = 0; i < 4; i++) Assert.Equal(0.0, m[i,i]); }
    [Fact] public void ExactSolvers_FindKnownOptimal() { var m = M; Assert.Equal(14.0, new BruteForceSolver().Solve(m).Distance, 9); Assert.Equal(14.0, new HeldKarpSolver().Solve(m).Distance, 9); }
}
