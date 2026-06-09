namespace AssessmentPreparation.Model;

/// <summary>
/// Pre-computed cost lookup for a complete permutation graph.
/// Computing costs once up front (O(n²)) keeps the hot loops of every solver
/// free of sqrt calls.
/// Two construction paths:
///   1. new DistanceMatrix(nodes) — Euclidean from (X, Y) coordinates.
///   2. new DistanceMatrix(rawCosts, nodes) — raw symmetric cost table.
/// </summary>
public class DistanceMatrix
{
    private readonly double[,] _costs;

    public IReadOnlyList<Node> Nodes { get; }
    public int Count => Nodes.Count;

    /// <summary>Euclidean constructor.</summary>
    public DistanceMatrix(IReadOnlyList<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        if (nodes.Count == 0)
            throw new ArgumentException("Need at least one node.", nameof(nodes));

        Nodes = nodes;
        var n = nodes.Count;
        _costs = new double[n, n];

        for (var i = 0; i < n; i++)
        {
            _costs[i, i] = 0;
            for (var j = i + 1; j < n; j++)
                _costs[i, j] = _costs[j, i] = nodes[i].DistanceTo(nodes[j]);
        }
    }

    /// <summary>Non-Euclidean constructor — accepts any symmetric cost matrix.</summary>
    public DistanceMatrix(double[,] rawCosts, IReadOnlyList<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(rawCosts);
        ArgumentNullException.ThrowIfNull(nodes);

        var n = nodes.Count;
        if (rawCosts.GetLength(0) != n || rawCosts.GetLength(1) != n)
            throw new ArgumentException($"Cost matrix must be {n}×{n} (got {rawCosts.GetLength(0)}×{rawCosts.GetLength(1)}).", nameof(rawCosts));

        Nodes = nodes;
        _costs = rawCosts;
    }

    public double this[int from, int to] => _costs[from, to];

    public double TourLength(IReadOnlyList<int> order)
    {
        ArgumentNullException.ThrowIfNull(order);
        var total = 0.0;
        for (var i = 0; i < order.Count; i++)
            total += _costs[order[i], order[(i + 1) % order.Count]];
        return total;
    }

    /// <summary>
    /// Convenience factory: creates Node objects from string labels and builds
    /// a DistanceMatrix from the provided cost table.
    /// </summary>
    public static DistanceMatrix FromLabels(double[,] rawCosts, params string[] labels)
    {
        var n = rawCosts.GetLength(0);
        if (labels.Length == 0)
            labels = Enumerable.Range(0, n).Select(i => i.ToString()).ToArray();
        var nodes = labels.Select((name, _) => new Node(name)).ToArray();
        return new DistanceMatrix(rawCosts, nodes);
    }
}

public static class NodeExtensions
{
    public static double DistanceTo(this Node a, Node b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
