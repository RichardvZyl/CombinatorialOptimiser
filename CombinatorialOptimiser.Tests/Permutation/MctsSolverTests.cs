using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Tests.Permutation;

public class MctsSolverTests
{
    [Fact]
    public void Mcts_ProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(6, 2);
        var m = new DistanceMatrix(nodes);
        var solver = new MctsSolver(iterations: 200, exploration: 1.0);
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }

    [Fact]
    public void Mcts_RunsOnRawMatrix()
    {
        var (costs, nodes) = TestHelpers.MakeRawMatrix(5, 5);
        var m = new DistanceMatrix(costs, nodes);
        var solver = new MctsSolver(iterations: 100);
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
