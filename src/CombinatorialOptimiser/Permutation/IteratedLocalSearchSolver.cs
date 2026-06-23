using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.Permutation;

// Metaheuristic: alternates between Lin-Kernighan local search and a double-bridge
// perturbation (a 4-opt kick that splits the tour into four segments and reassembles them
// in a new order). Accepts only improvements after each perturbation, balancing
// intensification with escape from local optima.
internal sealed class IteratedLocalSearchSolver : IteratedLocalSearchBase<DistanceMatrix, int[]>, ISolver<DistanceMatrix, PermutationResult>
{
    public string Name => "Iterated Local Search (LK + double-bridge)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    public int[]? Seed { get; init; }
    public PermutationResult Solve(DistanceMatrix m) => Solve(m, CancellationToken.None);

    public PermutationResult Solve(DistanceMatrix m, CancellationToken ct) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var initial = Seed ?? new NearestNeighborSolver().Solve(m).Order.ToArray();
            return initial.Length < 4 ? initial : RunIteratedLocalSearch(m, initial, ct);
        });
    protected override int[] LocalSearch(DistanceMatrix m, int[] solution) => new LinKernighanSolver { Seed = solution }.Solve(m).Order.ToArray();
    protected override int[] Perturb(DistanceMatrix m, int[] solution, Random rng) => PermutationUtils.DoubleBridge(solution, rng);
    protected override double Evaluate(DistanceMatrix m, int[] solution) => m.TourLength(solution);
    protected override int[] Clone(int[] solution) => (int[])solution.Clone();
}
