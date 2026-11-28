using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.Permutation;

public class RolloutSolverTests
{
    [Fact]
    public void Rollout_ProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(8, 3);
        var m = new DistanceMatrix(nodes);
        var solver = new RolloutSolver { BasePolicy = new NearestNeighborSolver() };
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
