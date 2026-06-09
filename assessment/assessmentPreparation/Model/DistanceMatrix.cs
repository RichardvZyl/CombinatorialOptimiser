namespace AssessmentPreparation.Model;

/// <summary>
/// Pre-computed cost lookup for a complete TSP graph.
/// Computing Euclidean distances once up front (O(n²)) keeps the hot
/// loops of every solver free of sqrt calls.
/// </summary>
public class DistanceMatrix
{
    private readonly double[,] _costs;

    public IReadOnlyList<Node> Nodes { get; }
    public int Count => Nodes.Count;

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

    public double this[int from, int to] => _costs[from, to];

    public double TourLength(IReadOnlyList<int> order)
    {
        ArgumentNullException.ThrowIfNull(order);
        var total = 0.0;
        for (var i = 0; i < order.Count; i++)
            total += _costs[order[i], order[(i + 1) % order.Count]];
        return total;
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
