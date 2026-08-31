namespace CombinatorialOptimiser.Core.Metaheuristics;

/// <summary>Shared genetic algorithm loop. Domains subclass this and supply chromosome creation, fitness, crossover, mutation, and repair.</summary>
/// <typeparam name="TProblem">The problem instance type.</typeparam>
/// <typeparam name="TChromosome">The chromosome (candidate solution) type.</typeparam>
public abstract class GeneticAlgorithm<TProblem, TChromosome>
{
    /// <summary>The number of chromosomes maintained in each generation.</summary>
    public int PopulationSize { get; init; } = 20;

    /// <summary>The number of generations to evolve.</summary>
    public int Generations { get; init; } = 10;

    /// <summary>The probability of mutating a child chromosome after crossover.</summary>
    public double MutationRate { get; init; } = 0.1;

    /// <summary>The seed used to initialise the random number generator, for reproducible runs.</summary>
    public int RandomSeed { get; init; } = 42;

    /// <summary>Creates the initial seed chromosome, typically from a constructive heuristic.</summary>
    protected abstract TChromosome CreateSeed(TProblem problem, Random rng);

    /// <summary>Creates a random chromosome to help populate the initial population.</summary>
    protected abstract TChromosome CreateRandom(TProblem problem, Random rng);

    /// <summary>Computes the fitness (to be minimised) of a chromosome.</summary>
    protected abstract double Fitness(TProblem problem, TChromosome chromosome);

    /// <summary>Produces a child chromosome by combining two parents.</summary>
    protected abstract TChromosome Crossover(TProblem problem, TChromosome parent1, TChromosome parent2, Random rng);

    /// <summary>Applies a random mutation to a chromosome.</summary>
    protected abstract TChromosome Mutate(TProblem problem, TChromosome chromosome, Random rng);

    /// <summary>Repairs or locally improves a chromosome (e.g. restoring feasibility).</summary>
    protected abstract TChromosome LocalImprove(TProblem problem, TChromosome chromosome);

    /// <summary>Returns whether <paramref name="candidate"/> already exists in <paramref name="population"/>.</summary>
    protected abstract bool IsDuplicate(IReadOnlyList<TChromosome> population, TChromosome candidate);

    /// <summary>Creates a deep copy of a chromosome.</summary>
    protected abstract TChromosome Clone(TChromosome chromosome);

    /// <summary>Runs the evolution loop and returns the best chromosome found.</summary>
    /// <param name="problem">The problem instance.</param>
    /// <param name="ct">A token that, when cancelled, stops the search early and returns the best chromosome found so far.</param>
    protected TChromosome RunEvolution(TProblem problem, CancellationToken ct = default)
    {
        var rng = new Random(RandomSeed);
        ct.ThrowIfCancellationRequested();
        var population = new List<TChromosome>(PopulationSize); var fitness = new List<double>(PopulationSize);
        var seed = CreateSeed(problem, rng); population.Add(seed); fitness.Add(Fitness(problem, seed));
        while (population.Count < PopulationSize)
        {
            ct.ThrowIfCancellationRequested();
            var random = CreateRandom(problem, rng);
            var improved = LocalImprove(problem, random);
            if (!IsDuplicate(population, improved)) { population.Add(improved); fitness.Add(Fitness(problem, improved)); }
            else { population.Add(random); fitness.Add(Fitness(problem, random)); }
        }
        var bestIndex = 0; for (var i = 1; i < population.Count; i++) if (fitness[i] < fitness[bestIndex]) bestIndex = i;
        var best = Clone(population[bestIndex]); var bestFitness = fitness[bestIndex];
        for (var gen = 0; gen < Generations; gen++)
        {
            ct.ThrowIfCancellationRequested();
            var nextPopulation = new List<TChromosome>(); var nextFitness = new List<double>();
            var elite = fitness.Select((f, i) => (f, i)).OrderBy(x => x.f).Take(2).ToArray();
            foreach (var (_, idx) in elite) { nextPopulation.Add(Clone(population[idx])); nextFitness.Add(fitness[idx]); }
            while (nextPopulation.Count < PopulationSize)
            {
                var parent1 = Select(population, fitness, rng); var parent2 = Select(population, fitness, rng);
                var child = Crossover(problem, parent1, parent2, rng);
                if (rng.NextDouble() < MutationRate) child = Mutate(problem, child, rng);
                child = LocalImprove(problem, child);
                var childFitness = Fitness(problem, child);
                nextPopulation.Add(child); nextFitness.Add(childFitness);
                if (childFitness < bestFitness) { bestFitness = childFitness; best = Clone(child); }
            }
            population = nextPopulation; fitness = nextFitness;
        }
        return best;
    }

    private static TChromosome Select(List<TChromosome> population, List<double> fitness, Random rng)
    {
        var best = rng.Next(population.Count);
        for (var i = 1; i < 3; i++) { var idx = rng.Next(population.Count); if (fitness[idx] < fitness[best]) best = idx; }
        return population[best];
    }
}
