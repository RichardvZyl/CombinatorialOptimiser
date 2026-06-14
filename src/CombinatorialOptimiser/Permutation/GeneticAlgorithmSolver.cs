using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.Permutation;

internal sealed class GeneticAlgorithmSolver : GeneticAlgorithmBase<DistanceMatrix, int[]>, ISolver<DistanceMatrix, PermutationResult>
{
    public string Name => "Genetic Algorithm (ERX + LK)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    public PermutationResult Solve(DistanceMatrix m) => Solve(m, CancellationToken.None);

    public PermutationResult Solve(DistanceMatrix m, CancellationToken ct) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var n = m.Count;
            return n < 4 ? Enumerable.Range(0, n).ToArray() : RunEvolution(m, ct);
        });
    protected override int[] CreateSeed(DistanceMatrix m, Random rng) => new NearestNeighborSolver().Solve(m).Order.ToArray();
    protected override int[] CreateRandom(DistanceMatrix m, Random rng) => RandPerm(m.Count, rng);
    protected override double Fitness(DistanceMatrix m, int[] chromosome) => m.TourLength(chromosome);
    protected override int[] Crossover(DistanceMatrix m, int[] parent1, int[] parent2, Random rng) => ERX(parent1, parent2, rng);
    protected override int[] Mutate(DistanceMatrix m, int[] chromosome, Random rng) => PermutationUtils.DoubleBridge(chromosome, rng);
    protected override int[] LocalImprove(DistanceMatrix m, int[] chromosome) => RunLK(m, chromosome);
    protected override bool IsDuplicate(IReadOnlyList<int[]> population, int[] candidate) => Dup(population, candidate);
    protected override int[] Clone(int[] chromosome) => (int[])chromosome.Clone();

    private static int[] ERX(int[] p1, int[] p2, Random rng) { var n = p1.Length; var edges = new HashSet<int>[n]; for (var i = 0; i < n; i++) edges[i] = new HashSet<int>(); void Add(int[] t) { for (var i = 0; i < n; i++) { var a = t[i]; var b = t[(i+1)%n]; edges[a].Add(b); edges[b].Add(a); } } Add(p1); Add(p2); var u = new bool[n]; var c = new int[n]; var cur = rng.Next(n); c[0] = cur; u[cur] = true; for (var pos = 1; pos < n; pos++) { var un = edges[cur].Where(nx=>!u[nx]).ToList(); if (un.Count > 0) cur = un.OrderBy(nx=>edges[nx].Count(e=>!u[e])).ThenBy(_=>rng.Next()).First(); else { var rem = Enumerable.Range(0,n).Where(i=>!u[i]).ToList(); if (rem.Count == 0) break; cur = rem[rng.Next(rem.Count)]; } c[pos] = cur; u[cur] = true; } return c; }
    private static int[] RandPerm(int n, Random rng) { var r = new int[n]; r[0] = 0; var rest = Enumerable.Range(1, n-1).ToArray(); for (var i = rest.Length-1; i > 0; i--) { var j = rng.Next(i+1); (rest[i], rest[j]) = (rest[j], rest[i]); } Array.Copy(rest, 0, r, 1, rest.Length); return r; }
    private static int[] RunLK(DistanceMatrix m, int[] seed) => new LinKernighanSolver { Seed = seed }.Solve(m).Order.ToArray();
    private static bool Dup(IReadOnlyList<int[]> pop, int[] c) { foreach (var e in pop) { if (e.Length != c.Length) continue; var m = true; for (var i = 0; i < e.Length; i++) if (e[i] != c[i]) { m = false; break; } if (m) return true; } return false; }
}
