namespace CombinatorialOptimiser.Core.Metaheuristics;

internal abstract class SimulatedAnnealingBase<TProblem, TSolution>
{
    public double? InitialTemperature { get; init; }
    public double CoolingRate { get; init; } = 0.99;
    public int StepsPerTemperature { get; init; } = 100;
    public double MinTemperature { get; init; } = 0.1;
    public int RandomSeed { get; init; } = 42;

    protected abstract double GetCost(TProblem problem, TSolution solution);
    protected abstract double GetInitialTemperature(TProblem problem, TSolution initial, Random rng);
    protected abstract double Step(TProblem problem, TSolution current, double temperature, Random rng);
    protected abstract TSolution Clone(TSolution solution);

    protected TSolution RunAnnealing(TProblem problem, TSolution initial)
    {
        var rng = new Random(RandomSeed);
        var current = Clone(initial);
        var currentCost = GetCost(problem, current);
        var best = Clone(current); var bestCost = currentCost;
        var temperature = InitialTemperature ?? GetInitialTemperature(problem, current, rng);
        while (temperature > MinTemperature)
        {
            for (var step = 0; step < StepsPerTemperature; step++)
            {
                currentCost += Step(problem, current, temperature, rng);
                if (currentCost < bestCost) { bestCost = currentCost; best = Clone(current); }
            }
            temperature *= CoolingRate;
        }
        return best;
    }
}
