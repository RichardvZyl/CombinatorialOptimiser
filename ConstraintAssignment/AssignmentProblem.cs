namespace CombinatorialOptimiser.ConstraintAssignment;

#pragma warning disable CA1819 // Multidimensional array property is intentional — the conflict matrix is a core domain type used by all solvers.
public sealed class AssignmentProblem
{
    public IReadOnlyList<string> Entities { get; }
    public bool[,] Conflicts { get; }
#pragma warning restore CA1819
    public int Count => Entities.Count;

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

    public bool HasConflict(int a, int b) => Conflicts[a, b];

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
