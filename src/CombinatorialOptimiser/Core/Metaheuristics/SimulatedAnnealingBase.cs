namespace CombinatorialOptimiser.Core.Metaheuristics;

/// <summary>Shared simulated annealing loop. Domains subclass this and supply cost evaluation, the move/acceptance step, and cloning.</summary>
/// <typeparam name="TProblem">The problem instance type.</typeparam>
/// <typeparam name="TSolution">The candidate solution type.</typeparam>
public abstract class SimulatedAnnealingBase<TProblem, TSolution>
{
    /// <summary>The starting temperature. If <c>null</c>, <see cref="ComputeDefaultInitialTemperature"/> is used.</summary>
    public double? InitialTemperature { get; init; }

    /// <summary>The multiplicative factor applied to the temperature after each temperature level (0 &lt; rate &lt; 1).</summary>
    public double CoolingRate { get; init; } = 0.99;

    /// <summary>The number of candidate moves attempted at each temperature level.</summary>
    public int StepsPerTemperature { get; init; } = 100;

    /// <summary>The temperature at which annealing stops.</summary>
    public double MinTemperature { get; init; } = 0.1;

    /// <summary>The seed used to initialise the random number generator, for reproducible runs.</summary>
    public int RandomSeed { get; init; } = 42;

    /// <summary>Computes the cost (to be minimised) of a candidate solution.</summary>
    protected abstract double GetCost(TProblem problem, TSolution solution);

    /// <summary>Computes a problem-appropriate starting temperature when <see cref="InitialTemperature"/> is not set.</summary>
    protected abstract double ComputeDefaultInitialTemperature(TProblem problem, TSolution initial, Random rng);

    /// <summary>Proposes and (probabilistically) accepts a single move, mutating <paramref name="current"/> in place, and returns the resulting change in cost.</summary>
    protected abstract double Step(TProblem problem, TSolution current, double temperature, Random rng);

    /// <summary>Creates a deep copy of a solution.</summary>
    protected abstract TSolution Clone(TSolution solution);

    /// <summary>Runs the annealing schedule starting from <paramref name="initial"/> and returns the best solution found.</summary>
    /// <param name="problem">The problem instance.</param>
    /// <param name="initial">The starting solution.</param>
    /// <param name="ct">A token that, when cancelled, stops the search early and returns the best solution found so far.</param>
    protected TSolution RunAnnealing(TProblem problem, TSolution initial, CancellationToken ct = default)
    {
        var rng = new Random(RandomSeed);
        var current = Clone(initial);
        var currentCost = GetCost(problem, current);
        var best = Clone(current); var bestCost = currentCost;
        var temperature = InitialTemperature ?? ComputeDefaultInitialTemperature(problem, current, rng);
        while (temperature > MinTemperature)
        {
            ct.ThrowIfCancellationRequested();
            for (var step = 0; step < StepsPerTemperature; step++)
            {
                currentCost += Step(problem, current, temperature, rng);
                if (currentCost < bestCost) { bestCost = currentCost; best = Clone(current); }
                if (ct.IsCancellationRequested) break;
            }
            if (ct.IsCancellationRequested) break;
            temperature *= CoolingRate;
        }
        return best;
    }
}
