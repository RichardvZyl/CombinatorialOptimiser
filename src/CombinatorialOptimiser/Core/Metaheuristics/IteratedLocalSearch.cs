namespace CombinatorialOptimiser.Core.Metaheuristics;

/// <summary>Shared iterated local search loop. Domains subclass this and supply local search, perturbation, and evaluation.</summary>
/// <typeparam name="TProblem">The problem instance type.</typeparam>
/// <typeparam name="TSolution">The candidate solution type.</typeparam>
public abstract class IteratedLocalSearch<TProblem, TSolution>
{
    /// <summary>The number of perturb-and-reoptimise iterations to run.</summary>
    public int MaxIterations { get; init; } = 20;

    /// <summary>The seed used to initialise the random number generator, for reproducible runs.</summary>
    public int RandomSeed { get; init; } = 42;

    /// <summary>Applies a local search procedure (e.g. 2-opt) until no further improvement is found.</summary>
    protected abstract TSolution LocalSearch(TProblem problem, TSolution solution);

    /// <summary>Applies a random perturbation (e.g. double-bridge) to escape the current local optimum.</summary>
    protected abstract TSolution Perturb(TProblem problem, TSolution solution, Random rng);

    /// <summary>Computes the cost (to be minimised) of a candidate solution.</summary>
    protected abstract double Evaluate(TProblem problem, TSolution solution);

    /// <summary>Creates a deep copy of a solution.</summary>
    protected abstract TSolution Clone(TSolution solution);

    /// <summary>Runs the perturb-and-reoptimise loop starting from <paramref name="initial"/> and returns the best solution found.</summary>
    /// <param name="problem">The problem instance.</param>
    /// <param name="initial">The starting solution.</param>
    /// <param name="ct">A token that, when cancelled, stops the search early and returns the best solution found so far.</param>
    protected TSolution RunIteratedLocalSearch(TProblem problem, TSolution initial, CancellationToken ct = default)
    {
        var rng = new Random(RandomSeed);
        var current = LocalSearch(problem, initial);
        var best = Clone(current); var bestCost = Evaluate(problem, best);
        for (var iter = 0; iter < MaxIterations; iter++)
        {
            ct.ThrowIfCancellationRequested();
            var perturbed = Perturb(problem, current, rng);
            var improved = LocalSearch(problem, perturbed);
            var cost = Evaluate(problem, improved);
            if (cost < bestCost) { bestCost = cost; best = Clone(improved); }
            current = improved;
        }
        return best;
    }
}
