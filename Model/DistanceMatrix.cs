using System;
using System.Linq;

namespace PermutationOptimiser.Model;

public class DistanceMatrix
{
    private readonly double[,] _costs;
    public IReadOnlyList<Node> Nodes { get; }
    public int Count => Nodes.Count;

    public DistanceMatrix(IReadOnlyList<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        if (nodes.Count == 0) throw new ArgumentException("Need at least one node.", nameof(nodes));
        Nodes = nodes; var n = nodes.Count; _costs = new double[n, n];
        for (var i = 0; i < n; i++) { _costs[i, i] = 0; for (var j = i + 1; j < n; j++) _costs[i, j] = _costs[j, i] = nodes[i].DistanceTo(nodes[j]); }
    }

    public DistanceMatrix(double[,] rawCosts, IReadOnlyList<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(rawCosts); ArgumentNullException.ThrowIfNull(nodes);
        var n = nodes.Count;
        if (rawCosts.GetLength(0) != n || rawCosts.GetLength(1) != n) throw new ArgumentException("Cost matrix must be " + n + "x" + n + ".", nameof(rawCosts));
        Nodes = nodes; _costs = rawCosts;
    }

    public double this[int from, int to] => _costs[from, to];

    public double TourLength(IReadOnlyList<int> order)
    {
        ArgumentNullException.ThrowIfNull(order); var total = 0.0;
        for (var i = 0; i < order.Count; i++) total += _costs[order[i], order[(i + 1) % order.Count]];
        return total;
    }

    public static DistanceMatrix FromLabels(double[,] rawCosts, params string[] labels)
    {
        var n = rawCosts.GetLength(0);
        if (labels.Length == 0) labels = Enumerable.Range(0, n).Select(i => i.ToString()).ToArray();
        return new DistanceMatrix(rawCosts, labels.Select((name, _) => new Node(name)).ToArray());
    }
}

public static class NodeExtensions
{
    public static double DistanceTo(this Node a, Node b) { var dx = a.X - b.X; var dy = a.Y - b.Y; return Math.Sqrt(dx * dx + dy * dy); }
}
