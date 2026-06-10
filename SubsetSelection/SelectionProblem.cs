namespace CombinatorialOptimiser.SubsetSelection;

public sealed record SelectionItem(string Name, double Cost, double Value);

public sealed class SelectionProblem
{
    public IReadOnlyList<SelectionItem> Items { get; }
    public double Capacity { get; }

    public SelectionProblem(IReadOnlyList<SelectionItem> items, double capacity)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
        Items = items;
        Capacity = capacity;
    }
}
