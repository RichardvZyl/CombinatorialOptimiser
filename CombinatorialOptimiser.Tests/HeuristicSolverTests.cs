using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests;

public class HeuristicSolverTests
{
    private static readonly ISolver<DistanceMatrix, PermutationResult>[] Heuristics = [new NearestNeighborSolver(),new TwoOptSolver(),new ThreeOptSolver(),new LinKernighanSolver(),new IteratedLocalSearchSolver(),new SimulatedAnnealingSolver(),new GeneticAlgorithmSolver()];
    public static IEnumerable<object[]> HeuristicData => Heuristics.Select(s => new object[] { s });
    [Theory][MemberData(nameof(HeuristicData))]
#pragma warning disable CA1062 // Solver parameters are injected by xUnit [MemberData] and will never be null.
    public void ProducesValidTour(ISolver<DistanceMatrix, PermutationResult> s) { TestHelpers.AssertValidTour(s.Solve(new DistanceMatrix(TestHelpers.MakeSeededNodes(10,42))), 10); }
    [Theory][MemberData(nameof(HeuristicData))]
    public void QualityWithinReasonableBound(ISolver<DistanceMatrix, PermutationResult> solver)
    {
        var m = new DistanceMatrix(TestHelpers.MakeSeededNodes(10,42));
        var bl = new NearestNeighborSolver().Solve(m).Distance;
        Assert.True(solver.Solve(m).Distance <= bl * 2.0);
    }
#pragma warning restore CA1062
}
