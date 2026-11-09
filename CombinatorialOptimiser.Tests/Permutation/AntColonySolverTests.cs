using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.Permutation;

public class AntColonySolverTests
{
    [Fact]
    public void AntColony_ProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(10, 3);
        var m = new DistanceMatrix(nodes);
        var solver = new AntColonySolver { Ants = 8, Iterations = 20 };
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
