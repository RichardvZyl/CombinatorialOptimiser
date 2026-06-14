namespace CombinatorialOptimiser.ConstraintAssignment;

/// <summary>A graph colouring problem instance: assign a label to each entity such that no two conflicting entities share a label.</summary>
#pragma warning disable CA1819 // Multidimensional array property is intentional — the conflict matrix is a core domain type used by all solvers.
public sealed class AssignmentProblem
{
    /// <summary>The entities to label.</summary>
    public IReadOnlyList<string> Entities { get; }

    /// <summary>The symmetric conflict matrix: <c>Conflicts[a, b]</c> is <c>true</c> if entities <c>a</c> and <c>b</c> must not share a label.</summary>
    public bool[,] Conflicts { get; }
#pragma warning restore CA1819

    /// <summary>The number of entities.</summary>
    public int Count => Entities.Count;

    /// <summary>Creates a new assignment problem.</summary>
    /// <param name="entities">The entities to label.</param>
    /// <param name="conflicts">The symmetric conflict matrix, sized <c>entities.Count x entities.Count</c>.</param>
    public AssignmentProblem(IReadOnlyList<string> entities, bool[,] conflicts)
    {
        ArgumentNullException.ThrowIfNull(entities);
        ArgumentNullException.ThrowIfNull(conflicts);
        var n = entities.Count;
        if (conflicts.GetLength(0) != n || conflicts.GetLength(1) != n)
            throw new ArgumentException($"Conflicts must be a {n}x{n} matrix matching Entities.Count.", nameof(conflicts));
        Entities = entities;
        Conflicts = conflicts;
    }

    /// <summary>Returns whether entities <paramref name="a"/> and <paramref name="b"/> conflict.</summary>
    public bool HasConflict(int a, int b) => Conflicts[a, b];

    /// <summary>Creates a problem from a list of conflicting entity-index pairs (edges of a conflict graph).</summary>
    /// <param name="entities">The entities to label.</param>
    /// <param name="edges">Pairs of entity indices that must not share a label.</param>
    public static AssignmentProblem FromEdges(string[] entities, (int A, int B)[] edges)
    {
        ArgumentNullException.ThrowIfNull(entities);
        ArgumentNullException.ThrowIfNull(edges);
        var n = entities.Length;
        var conflicts = new bool[n, n];
        foreach (var (a, b) in edges) { conflicts[a, b] = true; conflicts[b, a] = true; }
        return new AssignmentProblem(entities, conflicts);
    }
}
