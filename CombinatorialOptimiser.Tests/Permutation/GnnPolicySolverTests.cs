using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.Permutation;

public class GnnPolicySolverTests
{
    [Fact]
    public void GnnPolicy_ProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(12, 5);
        var m = new DistanceMatrix(nodes);
        var solver = new GnnPolicySolver(seed: 7);
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
