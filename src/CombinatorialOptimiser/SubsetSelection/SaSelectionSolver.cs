using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.SubsetSelection;

internal sealed class SaSelectionSolver : SimulatedAnnealing<SelectionProblem, bool[]>, ISolver<SelectionProblem, SelectionResult>
{
    public string Name => "Simulated Annealing";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;

    public SelectionResult Solve(SelectionProblem problem) => Solve(problem, CancellationToken.None);

    public SelectionResult Solve(SelectionProblem problem, CancellationToken ct) =>
        SelectionSolverRunner.Timed(Name, Paradigm, problem, () =>
        {
            var initial = new GreedySelectionSolver().Solve(problem).Selected;
            return RunAnnealing(problem, initial, ct);
        });

    protected override double GetCost(SelectionProblem problem, bool[] solution) => Objective(problem, solution);
    protected override bool[] Clone(bool[] solution) => (bool[])solution.Clone();

    protected override double ComputeDefaultInitialTemperature(SelectionProblem problem, bool[] initial, Random rng)
    {
        var n = initial.Length;
        var deltas = new List<double>();
        var samples = Math.Min(1000, n * n);
        var probe = (bool[])initial.Clone();
        for (var s = 0; s < samples; s++)
        {
            var i = rng.Next(n);
            var before = Objective(problem, probe);
            probe[i] = !probe[i];
            var delta = Objective(problem, probe) - before;
            if (delta > 0) deltas.Add(delta);
            probe[i] = !probe[i];
        }
        return deltas.Count == 0 ? 1.0 : -deltas.Average() / Math.Log(0.8);
    }

    protected override double Step(SelectionProblem problem, bool[] current, double temperature, Random rng)
    {
        var i = rng.Next(current.Length);
        var before = Objective(problem, current);
        current[i] = !current[i];
        var delta = Objective(problem, current) - before;
        if (delta < 0 || (temperature > 0 && rng.NextDouble() < Math.Exp(-delta / temperature))) return delta;
        current[i] = !current[i];
        return 0;
    }

    // Lower is better: negative total value when feasible, otherwise a heavy capacity-violation penalty.
    private static double Objective(SelectionProblem problem, bool[] solution)
    {
        double value = 0, cost = 0;
        for (var i = 0; i < solution.Length; i++)
            if (solution[i]) { value += problem.Items[i].Value; cost += problem.Items[i].Cost; }
        return cost > problem.Capacity ? (cost - problem.Capacity) * 1_000_000 - value : -value;
    }
}
