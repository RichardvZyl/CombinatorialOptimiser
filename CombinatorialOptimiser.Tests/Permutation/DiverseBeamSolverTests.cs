using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests.Permutation;

public class DiverseBeamSolverTests
{
    [Fact]
    public void DiverseBeam_ProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(10, 4);
        var m = new DistanceMatrix(nodes);
        var solver = new DiverseBeamSolver { BeamWidth = 4, Temperature = 1.0, DiversityStrength = 0.3 };
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
    }
}
