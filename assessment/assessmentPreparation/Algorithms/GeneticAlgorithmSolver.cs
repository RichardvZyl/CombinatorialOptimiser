using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

/// <summary>
/// Genetic Algorithm -- population-based meta-heuristic. Evolves tours through
/// tournament selection, Edge Recombination Crossover (ERX), double-bridge
/// mutation, and LK improvement of every offspring.
/// Complexity: O(popSize * generations * LK_cost).
/// </summary>
public sealed class GeneticAlgorithmSolver : ITspSolver
{
    public string Name => "Genetic Algorithm (ERX + LK)";
    public TspParadigm Paradigm => TspParadigm.Improvement;
    public int PopulationSize { get; init; } = 20;
    public int Generations { get; init; } = 10;
    public double MutationRate { get; init; } = 0.1;
    public int RandomSeed { get; init; } = 42;

    public TspResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var n = m.Count;
            if (n < 4) return Enumerable.Range(0, n).ToArray();
            var rng = new Random(RandomSeed);

            var pop = new List<int[]>(PopulationSize);
            var fitness = new List<double>(PopulationSize);

            var nnTour = new NearestNeighborSolver().Solve(m).Order.ToArray();
            pop.Add(nnTour); fitness.Add(m.TourLength(nnTour));

            while (pop.Count < PopulationSize)
            {
                var rand = RandomPermutation(n, rng);
                var improved = RunLK(m, rand);
                if (!IsDuplicate(pop, improved)) { pop.Add(improved); fitness.Add(m.TourLength(improved)); }
                else { pop.Add(rand); fitness.Add(m.TourLength(rand)); }
            }

            var bestIdx = 0;
            for (var i = 1; i < pop.Count; i++)
                if (fitness[i] < fitness[bestIdx]) bestIdx = i;
            var bestPerm = (int[])pop[bestIdx].Clone();
            var bestCost = fitness[bestIdx];

            for (var gen = 0; gen < Generations; gen++)
            {
                var newPop = new List<int[]>(); var newFitness = new List<double>();
                var elite = fitness.Select((f, i) => (f, i)).OrderBy(x => x.f).Take(2).ToArray();
                foreach (var (_, idx) in elite) { newPop.Add((int[])pop[idx].Clone()); newFitness.Add(fitness[idx]); }

                while (newPop.Count < PopulationSize)
                {
                    var p1 = TournamentSelect(pop, fitness, rng);
                    var p2 = TournamentSelect(pop, fitness, rng);
                    var child = EdgeRecombinationCrossover(p1, p2, rng);
                    if (rng.NextDouble() < MutationRate) child = DoubleBridgeMutate(child, rng);
                    child = RunLK(m, child);
                    var childCost = m.TourLength(child);
                    newPop.Add(child); newFitness.Add(childCost);
                    if (childCost < bestCost) { bestCost = childCost; bestPerm = (int[])child.Clone(); }
                }
                pop = newPop; fitness = newFitness;
            }
            return bestPerm;
        });

    private static int[] EdgeRecombinationCrossover(int[] p1, int[] p2, Random rng)
    {
        var n = p1.Length;
        var edges = new HashSet<int>[n];
        for (var i = 0; i < n; i++) edges[i] = new HashSet<int>();
        void AddEdges(int[] tour) { for (var i = 0; i < n; i++) { var a = tour[i]; var b = tour[(i + 1) % n]; edges[a].Add(b); edges[b].Add(a); } }
        AddEdges(p1); AddEdges(p2);
        var used = new bool[n]; var child = new int[n];
        var current = rng.Next(n); child[0] = current; used[current] = true;
        for (var pos = 1; pos < n; pos++)
        {
            var unused = edges[current].Where(nx => !used[nx]).ToList();
            if (unused.Count > 0) current = unused.OrderBy(nx => edges[nx].Count(e => !used[e])).ThenBy(_ => rng.Next()).First();
            else { var remaining = Enumerable.Range(0, n).Where(i => !used[i]).ToList(); if (remaining.Count == 0) break; current = remaining[rng.Next(remaining.Count)]; }
            child[pos] = current; used[current] = true;
        }
        return child;
    }

    private static int[] TournamentSelect(List<int[]> pop, List<double> fitness, Random rng)
    {
        var bestIdx = rng.Next(pop.Count);
        for (var i = 1; i < 3; i++) { var idx = rng.Next(pop.Count); if (fitness[idx] < fitness[bestIdx]) bestIdx = idx; }
        return pop[bestIdx];
    }

    private static int[] DoubleBridgeMutate(int[] permutation, Random rng)
    {
        var n = permutation.Length; if (n < 4) return permutation;
        var a = Math.Clamp(rng.Next(1, Math.Max(2, n / 3)), 1, n - 3);
        var b = Math.Clamp(rng.Next(Math.Max(a + 1, n / 3), Math.Min(2 * n / 3, n - 1)), a + 1, n - 2);
        var c = Math.Clamp(rng.Next(Math.Max(b + 1, 2 * n / 3), n), b + 1, n - 1);
        var result = new int[n]; result[0] = permutation[0]; var pos = 1;
        for (var i = 1; i <= a; i++) result[pos++] = permutation[i];
        for (var i = b + 1; i <= c; i++) result[pos++] = permutation[i];
        for (var i = a + 1; i <= b; i++) result[pos++] = permutation[i];
        for (var i = c + 1; i < n; i++) result[pos++] = permutation[i];
        return result;
    }

    private static int[] RandomPermutation(int n, Random rng)
    {
        var result = new int[n]; result[0] = 0;
        var rest = Enumerable.Range(1, n - 1).ToArray();
        for (var i = rest.Length - 1; i > 0; i--) { var j = rng.Next(i + 1); (rest[i], rest[j]) = (rest[j], rest[i]); }
        Array.Copy(rest, 0, result, 1, rest.Length);
        return result;
    }

    private static int[] RunLK(DistanceMatrix m, int[] seed) => new LinKernighanSolver().Solve(m).Order.ToArray();

    private static bool IsDuplicate(List<int[]> pop, int[] candidate)
    {
        foreach (var existing in pop)
        {
            if (existing.Length != candidate.Length) continue;
            var match = true;
            for (var i = 0; i < existing.Length; i++) if (existing[i] != candidate[i]) { match = false; break; }
            if (match) return true;
        }
        return false;
    }
}
