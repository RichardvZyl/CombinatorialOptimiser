namespace CombinatorialOptimiser.Core.Metaheuristics;

internal abstract class IteratedLocalSearchBase<TProblem, TSolution>
{
    public int MaxIterations { get; init; } = 20;
    public int RandomSeed { get; init; } = 42;

    protected abstract TSolution LocalSearch(TProblem problem, TSolution solution);
    protected abstract TSolution Perturb(TProblem problem, TSolution solution, Random rng);
    protected abstract double Evaluate(TProblem problem, TSolution solution);
    protected abstract TSolution Clone(TSolution solution);

    protected TSolution RunIteratedLocalSearch(TProblem problem, TSolution initial)
    {
        var rng = new Random(RandomSeed);
        var current = LocalSearch(problem, initial);
        var best = Clone(current); var bestCost = Evaluate(problem, best);
        for (var iter = 0; iter < MaxIterations; iter++)
        {
            var perturbed = Perturb(problem, current, rng);
            var improved = LocalSearch(problem, perturbed);
            var cost = Evaluate(problem, improved);
            if (cost < bestCost) { bestCost = cost; best = Clone(improved); }
            current = improved;
        }
        return best;
    }
}
