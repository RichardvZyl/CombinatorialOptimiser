using AssessmentPreparation.Algorithms;
using AssessmentPreparation.Model;

namespace PermutationOptimiser.Tests;

public class EdgeCaseTests
{
    [Fact] public void SingleNode_ZeroDistance() { var r = new BruteForceSolver().Solve(new DistanceMatrix([new Node("A",0,0)])); Assert.Equal(0,r.Distance); Assert.Single(r.Order); }
    [Fact] public void TwoNodes_RoundTripIsDouble() { Assert.Equal(10.0, new BruteForceSolver().Solve(new DistanceMatrix([new Node("A",0,0),new Node("B",3,4)])).Distance, 6); }
    [Fact] public void Triangle_OptimalIsPerimeter() { Assert.Equal(12.0, new BruteForceSolver().Solve(new DistanceMatrix([new Node("A",0,0),new Node("B",3,0),new Node("C",0,4)])).Distance, 6); }
}
