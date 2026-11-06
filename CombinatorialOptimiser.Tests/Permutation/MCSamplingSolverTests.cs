using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.Permutation;

public class MCSamplingSolverTests
{
    [Fact]
    public void MCSampling_ProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(9, 3);
        var m = new DistanceMatrix(nodes);
        var solver = new MCSamplingSolver { Samples = 200 };
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
