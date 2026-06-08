namespace CombinatorialOptimiser.Core.Metaheuristics;

internal abstract class GeneticAlgorithmBase<TProblem, TChromosome>
{
    public int PopulationSize { get; init; } = 20;
    public int Generations { get; init; } = 10;
    public double MutationRate { get; init; } = 0.1;
    public int RandomSeed { get; init; } = 42;

    protected abstract TChromosome CreateSeed(TProblem problem, Random rng);
    protected abstract TChromosome CreateRandom(TProblem problem, Random rng);
    protected abstract double Fitness(TProblem problem, TChromosome chromosome);
    protected abstract TChromosome Crossover(TProblem problem, TChromosome parent1, TChromosome parent2, Random rng);
    protected abstract TChromosome Mutate(TProblem problem, TChromosome chromosome, Random rng);
    protected abstract TChromosome LocalImprove(TProblem problem, TChromosome chromosome);
    protected abstract bool IsDuplicate(List<TChromosome> population, TChromosome candidate);
    protected abstract TChromosome Clone(TChromosome chromosome);

    protected TChromosome RunEvolution(TProblem problem)
    {
        var rng = new Random(RandomSeed);
        var population = new List<TChromosome>(PopulationSize); var fitness = new List<double>(PopulationSize);
        var seed = CreateSeed(problem, rng); population.Add(seed); fitness.Add(Fitness(problem, seed));
        while (population.Count < PopulationSize)
        {
            var random = CreateRandom(problem, rng);
            var improved = LocalImprove(problem, random);
            if (!IsDuplicate(population, improved)) { population.Add(improved); fitness.Add(Fitness(problem, improved)); }
            else { population.Add(random); fitness.Add(Fitness(problem, random)); }
        }
        var bestIndex = 0; for (var i = 1; i < population.Count; i++) if (fitness[i] < fitness[bestIndex]) bestIndex = i;
        var best = Clone(population[bestIndex]); var bestFitness = fitness[bestIndex];
        for (var gen = 0; gen < Generations; gen++)
        {
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
