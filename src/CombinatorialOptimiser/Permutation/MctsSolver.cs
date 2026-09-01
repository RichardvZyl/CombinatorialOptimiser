using System.Collections.Generic;
using System.Linq;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Simple Monte Carlo Tree Search (MCTS) solver for permutation/TSP-style problems.
/// This implementation is intentionally small and conservative: it runs a specified
/// number of iterations, uses UCT selection, expands by adding a single child, and
/// performs rollouts with a greedy nearest-neighbour policy by default.
/// </summary>
/// <summary>
/// Simple Monte Carlo Tree Search (MCTS) solver for permutation/TSP-style problems.
/// This implementation is intentionally small and conservative: it runs a specified
/// number of iterations, uses UCT selection, expands by adding a single child, and
/// performs rollouts with a greedy nearest-neighbour policy by default.
/// </summary>
public sealed class MctsSolver : ISolver<DistanceMatrix, PermutationResult>
{
    /// <summary>Human-readable name of the solver.</summary>
    public string Name => "Monte Carlo Tree Search";

    /// <summary>The algorithmic paradigm of this solver.</summary>
    public SolverParadigm Paradigm => SolverParadigm.Construction;

    private readonly int _iterations;
    private readonly double _exploration;

    /// <summary>Create a new MCTS solver.</summary>
    /// <param name="iterations">Number of MCTS iterations / rollouts to run.</param>
    /// <param name="exploration">UCT exploration constant (positive).</param>
    public MctsSolver(int iterations = 1000, double exploration = 1.4142135623730951)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(iterations, 1, nameof(iterations));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exploration, nameof(exploration));
        _iterations = iterations;
        _exploration = exploration;
    }

    /// <summary>Solves the given distance matrix and returns a timed PermutationResult.</summary>
    public PermutationResult Solve(DistanceMatrix m) => SolverRunner.Timed(Name, Paradigm, m, () => SolveInternal(m));

    private int[] SolveInternal(DistanceMatrix m)
    {
        var n = m.Count;
        if (n <= 1) return new[] { 0 };

        var root = new NodeState(new List<int> { 0 }, 0.0);
        var rng = new Random(0);
        int[] best = null!;
        var bestDist = double.PositiveInfinity;

        for (var it = 0; it < _iterations; it++)
        {
            // Selection & expansion: traverse from root choosing child with highest UCT
            var path = new List<NodeState> { root };
            var node = root;

            while (node.IsFullyExpanded(m.Count) == false && node.Visits > 0)
            {
                // pick child by UCT if expanded, otherwise break to expand
                if (node.Children.Count == 0) break;
                node = node.SelectChild(_exploration);
                path.Add(node);
            }

            // If node not fully expanded, expand one child
            if (!node.IsFullyExpanded(m.Count))
            {
                var child = node.Expand(m.Count);
                node.Children.Add(child);
                node = child;
                path.Add(node);
            }

            // Rollout from node
            var tour = Rollout(node.Order, m, rng);
            var dist = m.TourLength(tour);

            // Backpropagate (we treat reward as negative distance so higher is better)
            var reward = -dist;
            foreach (var s in path)
            {
                s.Visits++;
                s.TotalValue += reward;
            }

            if (dist < bestDist)
            {
                bestDist = dist;
                best = (int[])tour.Clone();
            }
        }

        return best!;
    }

    // Greedy nearest neighbour rollout from current partial order
    private static int[] Rollout(IReadOnlyList<int> order, DistanceMatrix m, Random rng)
    {
        var n = m.Count;
        var used = new bool[n];
        var result = new List<int>(order);
        foreach (var v in order) used[v] = true;
        while (result.Count < n)
        {
            var last = result[^1];
            int best = -1; double bestCost = double.PositiveInfinity;
            for (var j = 0; j < n; j++) if (!used[j])
            {
                var c = m[last, j];
                if (c < bestCost) { bestCost = c; best = j; }
            }
            if (best == -1) break;
            used[best] = true; result.Add(best);
        }
        return result.ToArray();
    }

    private sealed class NodeState
    {
        public List<int> Order { get; }
        public List<NodeState> Children { get; }
        public int Visits { get; set; }
        public double TotalValue { get; set; }

        public NodeState(List<int> order, double totalValue = 0)
        {
            Order = order; Children = new List<NodeState>(); Visits = 0; TotalValue = totalValue;
        }

        public bool IsFullyExpanded(int n) => Order.Count >= n;

        public NodeState Expand(int n)
        {
            var used = new HashSet<int>(Order);
            for (var j = 0; j < n; j++) if (!used.Contains(j))
            {
                var newOrder = new List<int>(Order) { j };
                return new NodeState(newOrder);
            }
            // already full
            return new NodeState(new List<int>(Order));
        }

        public NodeState SelectChild(double exploration)
        {
            // UCT: value = avg + C * sqrt(ln(N)/n)
            var parentVisits = Math.Max(1, Visits);
            NodeState best = null!; double bestScore = double.NegativeInfinity;
            foreach (var c in Children)
            {
                var avg = c.Visits > 0 ? c.TotalValue / c.Visits : 0.0;
                var score = avg + exploration * Math.Sqrt(Math.Log(parentVisits) / Math.Max(1, c.Visits));
                if (score > bestScore) { bestScore = score; best = c; }
            }
            return best!;
        }
    }
}
