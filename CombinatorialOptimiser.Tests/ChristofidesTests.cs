using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests;

public class ChristofidesTests
{
    [Theory][InlineData(5,1)][InlineData(6,2)][InlineData(8,3)]
    public void ExactMatching_WithinOneAndHalfTimesOptimal(int n, int seed)
    {
        var m = new DistanceMatrix(TestHelpers.MakeSeededNodes(n, seed));
        var opt = new BruteForceSolver().Solve(m).Distance;
        var c = new ChristofidesSolver{UseExactMatching=true}.Solve(m).Distance;
        Assert.True(c <= opt * 1.5 + 1e-9);
    }
    [Fact] public void GreedyAndExact_BothProduceValidTours()
    {
        var m = new DistanceMatrix(TestHelpers.MakeSeededNodes(8,7));
        TestHelpers.AssertValidTour(new ChristofidesSolver{UseExactMatching=true}.Solve(m),8);
        TestHelpers.AssertValidTour(new ChristofidesSolver{UseExactMatching=false}.Solve(m),8);
    }
}
