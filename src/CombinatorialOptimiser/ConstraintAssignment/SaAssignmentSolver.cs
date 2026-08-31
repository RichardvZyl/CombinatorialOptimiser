using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.ConstraintAssignment;

internal sealed class SaAssignmentSolver : SimulatedAnnealing<AssignmentProblem, int[]>, ISolver<AssignmentProblem, AssignmentResult>
{
    public string Name => "Simulated Annealing";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;

    public AssignmentResult Solve(AssignmentProblem problem) => Solve(problem, CancellationToken.None);

    public AssignmentResult Solve(AssignmentProblem problem, CancellationToken ct) =>
        AssignmentSolverRunner.Timed(Name, Paradigm, problem, () =>
        {
            var initial = new DsaturSolver().Solve(problem).Labels;
            return RunAnnealing(problem, initial, ct);
        });

    protected override double GetCost(AssignmentProblem problem, int[] solution) => Objective(problem, solution);
    protected override int[] Clone(int[] solution) => (int[])solution.Clone();

    protected override double ComputeDefaultInitialTemperature(AssignmentProblem problem, int[] initial, Random rng)
    {
        var n = initial.Length;
        var deltas = new List<double>();
        var samples = Math.Min(1000, n * n);
        var probe = (int[])initial.Clone();
        for (var s = 0; s < samples; s++)
        {
            var v = rng.Next(n);
            var newLabel = rng.Next(n);
            if (newLabel == probe[v]) continue;
            var before = Objective(problem, probe);
            var old = probe[v];
            probe[v] = newLabel;
            var delta = Objective(problem, probe) - before;
            if (delta > 0) deltas.Add(delta);
            probe[v] = old;
        }
        return deltas.Count == 0 ? 1.0 : -deltas.Average() / Math.Log(0.8);
    }

    protected override double Step(AssignmentProblem problem, int[] current, double temperature, Random rng)
    {
        var n = current.Length;
        var v = rng.Next(n);
        var newLabel = rng.Next(n);
        if (newLabel == current[v]) return 0;

        var before = Objective(problem, current);
        var old = current[v];
        current[v] = newLabel;
        var delta = Objective(problem, current) - before;

        if (delta < 0 || (temperature > 0 && rng.NextDouble() < Math.Exp(-delta / temperature))) return delta;
        current[v] = old;
        return 0;
    }

    // Lower is better: heavily penalise conflicting pairs, then prefer fewer distinct labels.
    private static double Objective(AssignmentProblem problem, int[] labels)
    {
        var n = labels.Length;
        var conflicts = 0;
        for (var i = 0; i < n; i++)
            for (var j = i + 1; j < n; j++)
                if (problem.HasConflict(i, j) && labels[i] == labels[j]) conflicts++;
        return conflicts * 1000.0 + labels.Distinct().Count();
    }
}
