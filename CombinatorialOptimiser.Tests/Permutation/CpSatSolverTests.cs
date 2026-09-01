using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.Permutation;

public class CpSatSolverTests
{
    [Fact]
    public void CpSat_FallbackProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(8, 3);
        var m = new DistanceMatrix(nodes);
        var solver = new CpSatSolver(timeLimitMs: 100);
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
