using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.Permutation;

public class IlpSolverTests
{
    [Fact]
    public void Ilp_FallbackUsesHeldKarp_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(9, 4);
        var m = new DistanceMatrix(nodes);
        var solver = new IlpSolver(timeLimitMs: 100);
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
