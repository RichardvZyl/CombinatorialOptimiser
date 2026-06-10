using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.Permutation;

internal sealed class IteratedLocalSearchSolver : IteratedLocalSearchBase<DistanceMatrix, int[]>, ISolver<DistanceMatrix, PermutationResult>
{
    public string Name => "Iterated Local Search (LK + double-bridge)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    public int[]? Seed { get; init; }
    public PermutationResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var initial = Seed ?? new NearestNeighborSolver().Solve(m).Order.ToArray();
            return initial.Length < 4 ? initial : RunIteratedLocalSearch(m, initial);
        });
    protected override int[] LocalSearch(DistanceMatrix m, int[] solution) => new LinKernighanSolver { Seed = solution }.Solve(m).Order.ToArray();
    protected override int[] Perturb(DistanceMatrix m, int[] solution, Random rng) => PermutationUtils.DoubleBridge(solution, rng);
    protected override double Evaluate(DistanceMatrix m, int[] solution) => m.TourLength(solution);
    protected override int[] Clone(int[] solution) => (int[])solution.Clone();
}
