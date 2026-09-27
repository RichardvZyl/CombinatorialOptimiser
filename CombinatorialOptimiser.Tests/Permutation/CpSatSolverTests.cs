using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Tests.Permutation;

public class CpSatSolverTests
{
    [Fact]
    public void CpSat_ExposesSolverIdentity()
    {
        var solver = new CpSatSolver();
        Assert.Equal("CP-SAT (OR-Tools)", solver.Name);
        Assert.Equal(SolverParadigm.Exact, solver.Paradigm);
    }

    [Fact]
    public void CpSat_FallbackProducesValidTour_SmallEuclidean()
    {
        var nodes = TestHelpers.MakeSeededNodes(8, 3);
        var m = new DistanceMatrix(nodes);
        var solver = new CpSatSolver(timeLimitMs: 100);
        var result = solver.Solve(m);
        TestHelpers.AssertValidTour(result, nodes.Length);
        Assert.Equal(solver.Name, result.Algorithm);
        Assert.Equal(solver.Paradigm, result.Paradigm);
        Assert.True(result.Elapsed >= TimeSpan.Zero);
    }
}
