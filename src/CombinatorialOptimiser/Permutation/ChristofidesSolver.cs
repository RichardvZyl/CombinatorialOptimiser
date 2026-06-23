using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

// Reduction algorithm with a 1.5× optimality guarantee on metric instances. Builds a
// minimum spanning tree, finds minimum-weight perfect matching on its odd-degree vertices,
// combines them into an Eulerian multigraph, then shortcuts repeated nodes to a Hamiltonian tour.
internal sealed class ChristofidesSolver : ISolver<DistanceMatrix, PermutationResult>
{
    public bool UseExactMatching { get; init; } = true;
    public string Name => UseExactMatching ? "Christofides (reduction, exact matching)" : "Christofides (reduction, greedy matching)";
    public SolverParadigm Paradigm => SolverParadigm.Reduction;
    public PermutationResult Solve(DistanceMatrix m) => SolverRunner.Timed(Name, Paradigm, m, () => SolveImpl(m));
    private int[] SolveImpl(DistanceMatrix m) { var n = m.Count; if (n <= 1) return Enumerable.Range(0, n).ToArray(); if (n == 2) return new[] { 0, 1 };
        var parent = new int[n]; var key = new double[n]; var inTree = new bool[n]; for (var i = 0; i < n; i++) key[i] = double.PositiveInfinity; key[0] = 0;
        for (var count = 0; count < n; count++) { var u = -1; var best = double.PositiveInfinity; for (var i = 0; i < n; i++) if (!inTree[i] && key[i] < best) { best = key[i]; u = i; } if (u < 0) break; inTree[u] = true; for (var v = 0; v < n; v++) if (!inTree[v] && m[u, v] < key[v]) { parent[v] = u; key[v] = m[u, v]; } }
        var mstAdj = new List<int>[n]; for (var i = 0; i < n; i++) mstAdj[i] = new List<int>(); for (var v = 1; v < n; v++) { var u = parent[v]; mstAdj[u].Add(v); mstAdj[v].Add(u); }
        var oddV = new List<int>(); for (var i = 0; i < n; i++) if (mstAdj[i].Count % 2 != 0) oddV.Add(i);
        var matching = UseExactMatching && oddV.Count <= 20 ? ExactWeightMatching(m, oddV) : GreedyMatching(m, oddV);
        var eulerAdj = new List<int>[n]; for (var i = 0; i < n; i++) eulerAdj[i] = new List<int>(mstAdj[i]); foreach (var (a, b) in matching) { eulerAdj[a].Add(b); eulerAdj[b].Add(a); }
        var circuit = new List<int>(); var stack = new Stack<int>(); var remaining = new List<int>[n]; for (var i = 0; i < n; i++) remaining[i] = new List<int>(eulerAdj[i]); stack.Push(0);
        while (stack.Count > 0) { var v = stack.Peek(); if (remaining[v].Count > 0) { var u = remaining[v][^1]; remaining[v].RemoveAt(remaining[v].Count - 1); remaining[u].Remove(v); stack.Push(u); } else { circuit.Add(stack.Pop()); } }
        var seen = new bool[n]; var hamiltonian = new List<int>(n); foreach (var v in circuit) if (!seen[v]) { seen[v] = true; hamiltonian.Add(v); } return hamiltonian.ToArray();
    }
    private static List<(int,int)> GreedyMatching(DistanceMatrix m, List<int> odd) { var k = odd.Count; var matched = new bool[k]; var pairs = new List<(int,int)>(); for (var i = 0; i < k; i++) { if (matched[i]) continue; var bestJ = -1; var bestD = double.PositiveInfinity; for (var j = i + 1; j < k; j++) if (!matched[j]) { var d = m[odd[i], odd[j]]; if (d < bestD) { bestD = d; bestJ = j; } } if (bestJ >= 0) { matched[i] = matched[bestJ] = true; pairs.Add((odd[i], odd[bestJ])); } } return pairs; }
    private static List<(int,int)> ExactWeightMatching(DistanceMatrix m, List<int> odd) { var k = odd.Count; if (k == 0) return new List<(int,int)>(); var size = 1 << k; var dp = new double[size]; var choice = new int[size]; for (var mask = 1; mask < size; mask++) dp[mask] = double.PositiveInfinity; dp[0] = 0; for (var mask = 0; mask < size; mask++) { if (double.IsPositiveInfinity(dp[mask])) continue; var first = -1; for (var i = 0; i < k; i++) if ((mask & (1 << i)) == 0) { first = i; break; } if (first < 0) continue; for (var j = first + 1; j < k; j++) { if ((mask & (1 << j)) != 0) continue; var nm = mask | (1 << first) | (1 << j); var cand = dp[mask] + m[odd[first], odd[j]]; if (cand < dp[nm]) { dp[nm] = cand; choice[nm] = j; } } } var pairs = new List<(int,int)>(); var cm = size - 1; while (cm > 0) { var first = 0; while ((cm & (1 << first)) == 0) first++; var partner = choice[cm]; pairs.Add((odd[first], odd[partner])); cm &= ~(1 << first); cm &= ~(1 << partner); } return pairs; }
}
