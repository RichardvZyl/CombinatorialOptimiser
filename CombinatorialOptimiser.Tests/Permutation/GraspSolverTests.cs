using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.Permutation;

public class GraspSolverTests
{
    [Fact]
    public void Grasp_ProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(11, 5);
        var m = new DistanceMatrix(nodes);
        var solver = new GraspSolver { Iterations = 30 }; 
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
