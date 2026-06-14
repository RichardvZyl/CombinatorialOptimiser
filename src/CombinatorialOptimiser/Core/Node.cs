namespace CombinatorialOptimiser.Core;

/// <summary>A point that can be visited, assigned, or selected by a solver, identified by name and an optional 2D position.</summary>
/// <param name="Name">A human-readable identifier for the node.</param>
/// <param name="X">The X coordinate, used for Euclidean distance calculations. Defaults to 0.</param>
/// <param name="Y">The Y coordinate, used for Euclidean distance calculations. Defaults to 0.</param>
public readonly record struct Node(string Name, double X = 0, double Y = 0)
{
    /// <inheritdoc/>
    public override string ToString() => Name;
}
