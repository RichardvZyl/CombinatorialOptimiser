using System.Linq;

namespace CombinatorialOptimiser.Core;

/// <summary>A symmetric pairwise cost matrix over a set of nodes, used to evaluate tours.</summary>
public class DistanceMatrix
{
    private readonly double[,] _costs;

    /// <summary>The nodes indexed by this matrix.</summary>
    public IReadOnlyList<Node> Nodes { get; }

    /// <summary>The number of nodes in the matrix.</summary>
    public int Count => Nodes.Count;

    /// <summary>Creates a matrix by computing Euclidean distances between the given nodes' coordinates.</summary>
    /// <param name="nodes">The nodes to index. Must contain at least one node.</param>
    public DistanceMatrix(IReadOnlyList<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        if (nodes.Count == 0) throw new ArgumentException("Need at least one node.", nameof(nodes));
        Nodes = nodes; var n = nodes.Count; _costs = new double[n, n];
        for (var i = 0; i < n; i++) { _costs[i, i] = 0; for (var j = i + 1; j < n; j++) _costs[i, j] = _costs[j, i] = nodes[i].DistanceTo(nodes[j]); }
    }

    /// <summary>Creates a matrix from an explicit cost matrix, e.g. for non-Euclidean or asymmetric costs.</summary>
    /// <param name="rawCosts">An n x n cost matrix matching <paramref name="nodes"/> in size.</param>
    /// <param name="nodes">The nodes indexed by <paramref name="rawCosts"/>.</param>
    public DistanceMatrix(double[,] rawCosts, IReadOnlyList<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(rawCosts); ArgumentNullException.ThrowIfNull(nodes);
        var n = nodes.Count;
        if (rawCosts.GetLength(0) != n || rawCosts.GetLength(1) != n) throw new ArgumentException("Cost matrix must be " + n + "x" + n + ".", nameof(rawCosts));
        Nodes = nodes; _costs = rawCosts;
    }

    /// <summary>Gets the cost of travelling from node <paramref name="from"/> to node <paramref name="to"/>.</summary>
    public double this[int from, int to] => _costs[from, to];

    /// <summary>
    /// Computes row-wise log-probabilities for transitions using a softmax over -cost/temperature.
    /// Each row corresponds to probabilities conditioned on the current node.
    /// If <paramref name="disallowSelf"/> is true, self-transitions are given a large negative log-probability.
    /// </summary>
    public double[,] TransitionLogProbabilities(double temperature = 1.0, bool disallowSelf = true)
    {
        if (temperature <= 0) throw new ArgumentOutOfRangeException(nameof(temperature), "Temperature must be positive.");
        var n = Count;
        var logProbs = new double[n, n];
        for (var i = 0; i < n; i++)
        {
            // Build scores = -cost / temperature
            var max = double.NegativeInfinity;
            for (var j = 0; j < n; j++)
            {
                if (disallowSelf && i == j) continue;
                var s = -_costs[i, j] / temperature;
                if (s > max) max = s;
            }

            // If all transitions were disallowed (shouldn't happen), leave zeros
            if (double.IsNegativeInfinity(max))
            {
                for (var j = 0; j < n; j++) logProbs[i, j] = double.NegativeInfinity;
                continue;
            }

            // Compute normalized log-probs in a numerically stable way: log softmax(s)
            var sumExp = 0.0;
            for (var j = 0; j < n; j++)
            {
                if (disallowSelf && i == j) continue;
                sumExp += Math.Exp((-_costs[i, j] / temperature) - max);
            }
            var logSumExp = Math.Log(sumExp) + max;

            for (var j = 0; j < n; j++)
            {
                if (disallowSelf && i == j)
                {
                    logProbs[i, j] = double.NegativeInfinity;
                }
                else
                {
                    var s = -_costs[i, j] / temperature;
                    logProbs[i, j] = s - logSumExp;
                }
            }
        }
        return logProbs;
    }

    /// <summary>Computes the total cost of a closed tour that visits nodes in the given order and returns to the start.</summary>
    /// <param name="order">The visiting order of node indices.</param>
    public double TourLength(IReadOnlyList<int> order)
    {
        ArgumentNullException.ThrowIfNull(order); var total = 0.0;
        for (var i = 0; i < order.Count; i++) total += _costs[order[i], order[(i + 1) % order.Count]];
        return total;
    }

    /// <summary>Creates a matrix from an explicit cost matrix, naming nodes from <paramref name="labels"/> (or by index if none are given).</summary>
    /// <param name="rawCosts">An n x n cost matrix.</param>
    /// <param name="labels">Optional node names; if empty, nodes are named "0", "1", etc.</param>
    public static DistanceMatrix FromLabels(double[,] rawCosts, params string[] labels)
    {
        ArgumentNullException.ThrowIfNull(rawCosts);
        ArgumentNullException.ThrowIfNull(labels);
        var n = rawCosts.GetLength(0);
        if (labels.Length == 0) labels = Enumerable.Range(0, n).Select(i => i.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToArray();
        return new DistanceMatrix(rawCosts, labels.Select((name, _) => new Node(name)).ToArray());
    }
}

internal static class NodeExtensions
{
    public static double DistanceTo(this Node a, Node b) { var dx = a.X - b.X; var dy = a.Y - b.Y; return Math.Sqrt(dx * dx + dy * dy); }
}
