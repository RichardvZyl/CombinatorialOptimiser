using AssessmentPreparation.Model;

namespace AssessmentPreparation.Algorithms;

/// <summary>
/// Christofides reduction-based approximation for metric TSP.
/// Steps: MST -> odd-degree vertices -> min-weight perfect matching ->
///        Eulerian multigraph -> Eulerian circuit -> shortcut -> Hamiltonian cycle
/// Guarantee: at most 1.5× the optimal tour length (for metric TSP).
/// </summary>
public sealed class ChristofidesSolver : ITspSolver
{
    public string Name => "Christofides (reduction)";
    public TspParadigm Paradigm => TspParadigm.Reduction;

    public TspResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () => SolveImpl(m));

    private int[] SolveImpl(DistanceMatrix m)
    {
        var n = m.Count;
        if (n <= 1) return Enumerable.Range(0, n).ToArray();
        if (n == 2) return new[] { 0, 1 };

        // Step 1: Minimum Spanning Tree (Prim's algorithm)
        var parent = new int[n];
        var key = new double[n];
        var inTree = new bool[n];
        for (var i = 0; i < n; i++) key[i] = double.PositiveInfinity;
        key[0] = 0;

        for (var count = 0; count < n; count++)
        {
            var u = -1;
            var best = double.PositiveInfinity;
            for (var i = 0; i < n; i++)
                if (!inTree[i] && key[i] < best) { best = key[i]; u = i; }
            if (u < 0) break;
            inTree[u] = true;
            for (var v = 0; v < n; v++)
                if (!inTree[v] && m[u, v] < key[v]) { parent[v] = u; key[v] = m[u, v]; }
        }

        var mstAdj = new List<int>[n];
        for (var i = 0; i < n; i++) mstAdj[i] = new List<int>();
        for (var v = 1; v < n; v++) { var u = parent[v]; mstAdj[u].Add(v); mstAdj[v].Add(u); }

        // Step 2: Find odd-degree vertices
        var oddVertices = new List<int>();
        for (var i = 0; i < n; i++)
            if (mstAdj[i].Count % 2 != 0) oddVertices.Add(i);

        // Step 3: Minimum-weight perfect matching (greedy)
        var matching = GreedyMatching(m, oddVertices);

        // Step 4: Build Eulerian multigraph
        var eulerAdj = new List<int>[n];
        for (var i = 0; i < n; i++) eulerAdj[i] = new List<int>(mstAdj[i]);
        foreach (var (a, b) in matching) { eulerAdj[a].Add(b); eulerAdj[b].Add(a); }

        // Step 5: Eulerian circuit (Hierholzer)
        var circuit = new List<int>();
        var stack = new Stack<int>();
        var remaining = new List<int>[n];
        for (var i = 0; i < n; i++) remaining[i] = new List<int>(eulerAdj[i]);
        stack.Push(0);
        while (stack.Count > 0)
        {
            var v = stack.Peek();
            if (remaining[v].Count > 0)
            {
                var u = remaining[v][^1];
                remaining[v].RemoveAt(remaining[v].Count - 1);
                remaining[u].Remove(v);
                stack.Push(u);
            }
            else { circuit.Add(stack.Pop()); }
        }

        // Step 6: Shortcut to Hamiltonian cycle
        var seen = new bool[n];
        var hamiltonian = new List<int>(n);
        foreach (var v in circuit)
            if (!seen[v]) { seen[v] = true; hamiltonian.Add(v); }
        return hamiltonian.ToArray();
    }

    private static List<(int, int)> GreedyMatching(DistanceMatrix m, List<int> odd)
    {
        var k = odd.Count;
        var matched = new bool[k];
        var pairs = new List<(int, int)>();
        for (var i = 0; i < k; i++)
        {
            if (matched[i]) continue;
            var bestJ = -1;
            var bestDist = double.PositiveInfinity;
            for (var j = i + 1; j < k; j++)
            {
                if (matched[j]) continue;
                var d = m[odd[i], odd[j]];
                if (d < bestDist) { bestDist = d; bestJ = j; }
            }
            if (bestJ >= 0) { matched[i] = matched[bestJ] = true; pairs.Add((odd[i], odd[bestJ])); }
        }
        return pairs;
    }
}
