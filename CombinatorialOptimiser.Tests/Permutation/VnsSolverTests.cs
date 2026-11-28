using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.Permutation;

public class VnsSolverTests
{
    [Fact]
    public void Vns_ProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(10, 4);
        var m = new DistanceMatrix(nodes);
        var solver = new VnsSolver { Iterations = 30 };
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
