using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Tests.Permutation;

public class BeamSearchSolverTests
{
    [Fact]
    public void BeamSearch_ProducesValidTour_EuclideanSmall()
    {
        var nodes = TestHelpers.MakeSeededNodes(6, 1);
        var m = new DistanceMatrix(nodes);
        var solver = new BeamSearchSolver(beamWidth: 3, temperature: 1.0, useLogProbForFinalRanking: false);
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }

    [Fact]
    public void BeamSearch_LogProbAndTourRanking_BothProduceValidTours()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(5, 7);
        var m = new DistanceMatrix(costs, nodes);
        var sTour = new BeamSearchSolver(beamWidth: 4, temperature: 1.0, useLogProbForFinalRanking: false);
        var sLog = new BeamSearchSolver(beamWidth: 4, temperature: 1.0, useLogProbForFinalRanking: true);
        var rTour = sTour.Solve(m);
        var rLog = sLog.Solve(m);
        TestHelpers.AssertValidTour(rTour, nodes.Length);
        TestHelpers.AssertValidTour(rLog, nodes.Length);
        Assert.True(rTour.Distance >= 0);
        Assert.True(rLog.Distance >= 0);
    }
}
