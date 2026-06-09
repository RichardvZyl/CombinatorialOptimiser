using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

public sealed class GeneticAlgorithmSolver : ISolver
{
    public string Name => "Genetic Algorithm (ERX + LK)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    public int PopulationSize { get; init; } = 20;
    public int Generations { get; init; } = 10;
    public double MutationRate { get; init; } = 0.1;
    public int RandomSeed { get; init; } = 42;
    public SolverResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var n = m.Count; if (n < 4) return Enumerable.Range(0, n).ToArray();
            var rng = new Random(RandomSeed);
            var pop = new List<int[]>(PopulationSize); var fitness = new List<double>(PopulationSize);
            var nnTour = new NearestNeighborSolver().Solve(m).Order.ToArray();
            pop.Add(nnTour); fitness.Add(m.TourLength(nnTour));
            while (pop.Count < PopulationSize)
            {
                var rand = RandomPerm(n, rng); var impr = RunLK(m, rand);
                if (!Dup(pop, impr)) { pop.Add(impr); fitness.Add(m.TourLength(impr)); }
                else { pop.Add(rand); fitness.Add(m.TourLength(rand)); }
            }
            var bestIdx = 0;
            for (var i = 1; i < pop.Count; i++) if (fitness[i] < fitness[bestIdx]) bestIdx = i;
            var bestPerm = (int[])pop[bestIdx].Clone(); var bestCost = fitness[bestIdx];
            for (var gen = 0; gen < Generations; gen++)
            {
                var newPop = new List<int[]>(); var newFit = new List<double>();
                var elite = fitness.Select((f,i)=>(f,i)).OrderBy(x=>x.f).Take(2).ToArray();
                foreach(var(_,idx)in elite){newPop.Add((int[])pop[idx].Clone());newFit.Add(fitness[idx]);}
                while (newPop.Count < PopulationSize)
                {
                    var p1 = Select(pop,fitness,rng); var p2 = Select(pop,fitness,rng);
                    var child = ERX(p1,p2,rng);
                    if (rng.NextDouble() < MutationRate) child = DBMutate(child,rng);
                    child = RunLK(m,child); var childCost = m.TourLength(child);
                    newPop.Add(child); newFit.Add(childCost);
                    if (childCost < bestCost) { bestCost = childCost; bestPerm = (int[])child.Clone(); }
                }
                pop = newPop; fitness = newFit;
            }
            return bestPerm;
        });

    private static int[] ERX(int[] p1, int[] p2, Random rng)
    {
        var n = p1.Length; var edges = new HashSet<int>[n];
        for (var i = 0; i < n; i++) edges[i] = new HashSet<int>();
        void Add(int[] t) { for (var i = 0; i < n; i++) { var a = t[i]; var b = t[(i+1)%n]; edges[a].Add(b); edges[b].Add(a); } }
        Add(p1); Add(p2);
        var used = new bool[n]; var child = new int[n]; var cur = rng.Next(n);
        child[0] = cur; used[cur] = true;
        for (var pos = 1; pos < n; pos++)
        {
            var un = edges[cur].Where(nx=>!used[nx]).ToList();
            if (un.Count > 0) cur = un.OrderBy(nx=>edges[nx].Count(e=>!used[e])).ThenBy(_=>rng.Next()).First();
            else { var rem = Enumerable.Range(0,n).Where(i=>!used[i]).ToList(); if (rem.Count == 0) break; cur = rem[rng.Next(rem.Count)]; }
            child[pos] = cur; used[cur] = true;
        }
        return child;
    }

    private static int[] Select(List<int[]> pop, List<double> fit, Random rng)
    {
        var bi = rng.Next(pop.Count);
        for (var i = 1; i < 3; i++) { var idx = rng.Next(pop.Count); if (fit[idx] < fit[bi]) bi = idx; }
        return pop[bi];
    }

    private static int[] DBMutate(int[] perm, Random rng)
    {
        var n = perm.Length; if (n < 4) return perm;
        var a = Math.Clamp(rng.Next(1, Math.Max(2, n/3)), 1, n-3);
        var b = Math.Clamp(rng.Next(Math.Max(a+1, n/3), Math.Min(2*n/3, n-1)), a+1, n-2);
        var c = Math.Clamp(rng.Next(Math.Max(b+1, 2*n/3), n), b+1, n-1);
        var r = new int[n]; r[0] = perm[0]; var pos = 1;
        for (var i = 1; i <= a; i++) r[pos++] = perm[i];
        for (var i = b+1; i <= c; i++) r[pos++] = perm[i];
        for (var i = a+1; i <= b; i++) r[pos++] = perm[i];
        for (var i = c+1; i < n; i++) r[pos++] = perm[i];
        return r;
    }

    private static int[] RandomPerm(int n, Random rng)
    {
        var r = new int[n]; r[0] = 0; var rest = Enumerable.Range(1, n-1).ToArray();
        for (var i = rest.Length-1; i > 0; i--) { var j = rng.Next(i+1); (rest[i], rest[j]) = (rest[j], rest[i]); }
        Array.Copy(rest, 0, r, 1, rest.Length); return r;
    }

    private static int[] RunLK(DistanceMatrix m, int[] seed) => new LinKernighanSolver { Seed = seed }.Solve(m).Order.ToArray();
    private static bool Dup(List<int[]> pop, int[] c)
    {
        foreach (var e in pop)
        {
            if (e.Length != c.Length) continue;
            var m = true; for (var i = 0; i < e.Length; i++) if (e[i] != c[i]) { m = false; break; }
            if (m) return true;
        }
        return false;
    }
}
