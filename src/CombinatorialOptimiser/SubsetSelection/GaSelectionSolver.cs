using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.SubsetSelection;

internal sealed class GaSelectionSolver : GeneticAlgorithmBase<SelectionProblem, bool[]>, ISolver<SelectionProblem, SelectionResult>
{
    public string Name => "Genetic Algorithm (uniform crossover + repair)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;

    public SelectionResult Solve(SelectionProblem problem) => Solve(problem, CancellationToken.None);

    public SelectionResult Solve(SelectionProblem problem, CancellationToken ct) =>
        SelectionSolverRunner.Timed(Name, Paradigm, problem, () => RunEvolution(problem, ct));

    protected override bool[] CreateSeed(SelectionProblem problem, Random rng) => new GreedySelectionSolver().Solve(problem).Selected;

    protected override bool[] CreateRandom(SelectionProblem problem, Random rng)
    {
        var chromosome = new bool[problem.Items.Count];
        for (var i = 0; i < chromosome.Length; i++) chromosome[i] = rng.Next(2) == 0;
        return Repair(problem, chromosome);
    }

    protected override double Fitness(SelectionProblem problem, bool[] chromosome)
    {
        double value = 0;
        for (var i = 0; i < chromosome.Length; i++) if (chromosome[i]) value += problem.Items[i].Value;
        return -value;
    }

    protected override bool[] Crossover(SelectionProblem problem, bool[] parent1, bool[] parent2, Random rng)
    {
        var child = new bool[parent1.Length];
        for (var i = 0; i < child.Length; i++) child[i] = rng.Next(2) == 0 ? parent1[i] : parent2[i];
        return child;
    }

    protected override bool[] Mutate(SelectionProblem problem, bool[] chromosome, Random rng)
    {
        var child = (bool[])chromosome.Clone();
        var i = rng.Next(child.Length);
        child[i] = !child[i];
        return child;
    }

    protected override bool[] LocalImprove(SelectionProblem problem, bool[] chromosome) => Repair(problem, chromosome);
    protected override bool[] Clone(bool[] chromosome) => (bool[])chromosome.Clone();
    protected override bool IsDuplicate(IReadOnlyList<bool[]> population, bool[] candidate) => population.Any(p => p.SequenceEqual(candidate));

    // Drops the worst value/cost-ratio selected items until feasible, then greedily fills any remaining capacity.
    private static bool[] Repair(SelectionProblem problem, bool[] chromosome)
    {
        var n = problem.Items.Count;
        var result = (bool[])chromosome.Clone();
        var cost = TotalCost(problem, result);

        while (cost > problem.Capacity)
        {
            var worst = -1;
            for (var i = 0; i < n; i++)
                if (result[i] && (worst == -1 || Ratio(problem.Items[i]) < Ratio(problem.Items[worst]))) worst = i;
            if (worst == -1) break;
            result[worst] = false;
            cost -= problem.Items[worst].Cost;
        }

        foreach (var i in Enumerable.Range(0, n).Where(i => !result[i]).OrderByDescending(i => Ratio(problem.Items[i])))
        {
            if (cost + problem.Items[i].Cost <= problem.Capacity)
            {
                result[i] = true;
                cost += problem.Items[i].Cost;
            }
        }
        return result;
    }

    private static double TotalCost(SelectionProblem problem, bool[] selection)
    {
        double cost = 0;
        for (var i = 0; i < selection.Length; i++) if (selection[i]) cost += problem.Items[i].Cost;
        return cost;
    }

    private static double Ratio(SelectionItem item) => item.Cost > 0 ? item.Value / item.Cost : double.PositiveInfinity;
}
