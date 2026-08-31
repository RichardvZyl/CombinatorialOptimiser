using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.ConstraintAssignment;

internal sealed class GaAssignmentSolver : GeneticAlgorithm<AssignmentProblem, int[]>, ISolver<AssignmentProblem, AssignmentResult>
{
    public string Name => "Genetic Algorithm (uniform crossover + repair)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;

    public AssignmentResult Solve(AssignmentProblem problem) => Solve(problem, CancellationToken.None);

    public AssignmentResult Solve(AssignmentProblem problem, CancellationToken ct) =>
        AssignmentSolverRunner.Timed(Name, Paradigm, problem, () => RunEvolution(problem, ct));

    protected override int[] CreateSeed(AssignmentProblem problem, Random rng) => new DsaturSolver().Solve(problem).Labels;

    protected override int[] CreateRandom(AssignmentProblem problem, Random rng)
    {
        var n = problem.Count;
        var labels = new int[n];
        for (var i = 0; i < n; i++) labels[i] = rng.Next(n);
        return Repair(problem, labels);
    }

    protected override double Fitness(AssignmentProblem problem, int[] chromosome) => chromosome.Distinct().Count();

    protected override int[] Crossover(AssignmentProblem problem, int[] parent1, int[] parent2, Random rng)
    {
        var child = new int[parent1.Length];
        for (var i = 0; i < child.Length; i++) child[i] = rng.Next(2) == 0 ? parent1[i] : parent2[i];
        return child;
    }

    protected override int[] Mutate(AssignmentProblem problem, int[] chromosome, Random rng)
    {
        var child = (int[])chromosome.Clone();
        var i = rng.Next(child.Length);
        child[i] = rng.Next(child.Length);
        return child;
    }

    protected override int[] LocalImprove(AssignmentProblem problem, int[] chromosome) => Repair(problem, chromosome);
    protected override int[] Clone(int[] chromosome) => (int[])chromosome.Clone();
    protected override bool IsDuplicate(IReadOnlyList<int[]> population, int[] candidate) => population.Any(p => p.SequenceEqual(candidate));

    // Reassigns any vertex that conflicts with an earlier same-labelled vertex to the smallest label not used by its earlier neighbours.
    private static int[] Repair(AssignmentProblem problem, int[] chromosome)
    {
        var n = problem.Count;
        var result = (int[])chromosome.Clone();
        for (var i = 0; i < n; i++)
        {
            var used = new HashSet<int>();
            for (var j = 0; j < i; j++) if (problem.HasConflict(i, j)) used.Add(result[j]);
            if (used.Contains(result[i]))
            {
                var label = 0;
                while (used.Contains(label)) label++;
                result[i] = label;
            }
        }
        return result;
    }
}
