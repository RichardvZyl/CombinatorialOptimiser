using PermutationOptimiser.Algorithms;
using PermutationOptimiser.Model;

namespace PermutationOptimiser.Tests;

public class HeuristicSolverTests
{
    private static readonly ISolver[] Heuristics = [new NearestNeighborSolver(),new TwoOptSolver(),new ThreeOptSolver(),new LinKernighanSolver(),new IteratedLocalSearchSolver(),new SimulatedAnnealingSolver(),new GeneticAlgorithmSolver()];
    public static IEnumerable<object[]> HeuristicData => Heuristics.Select(s => new object[] { s });
    [Theory][MemberData(nameof(HeuristicData))]
    public void ProducesValidTour(ISolver s) { TestHelpers.AssertValidTour(s.Solve(new DistanceMatrix(TestHelpers.MakeSeededNodes(10,42))), 10); }
    [Theory][MemberData(nameof(HeuristicData))]
    public void QualityWithinReasonableBound(ISolver solver)
    {
        var m = new DistanceMatrix(TestHelpers.MakeSeededNodes(10,42));
        var bl = new NearestNeighborSolver().Solve(m).Distance;
        Assert.True(solver.Solve(m).Distance <= bl * 2.0);
    }
}
