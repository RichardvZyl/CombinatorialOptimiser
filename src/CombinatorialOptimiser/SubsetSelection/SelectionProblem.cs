namespace CombinatorialOptimiser.SubsetSelection;

/// <summary>An item that can be selected, with a cost (counted against capacity) and a value (to be maximised).</summary>
/// <param name="Name">A human-readable identifier for the item.</param>
/// <param name="Cost">The capacity consumed if this item is selected.</param>
/// <param name="Value">The value gained if this item is selected.</param>
public sealed record SelectionItem(string Name, double Cost, double Value);

/// <summary>A 0/1 knapsack problem instance: choose a subset of <see cref="Items"/> whose total cost does not exceed <see cref="Capacity"/>, maximising total value.</summary>
public sealed class SelectionProblem
{
    /// <summary>The candidate items.</summary>
    public IReadOnlyList<SelectionItem> Items { get; }

    /// <summary>The maximum total cost of selected items.</summary>
    public double Capacity { get; }

    /// <summary>Creates a new selection problem.</summary>
    /// <param name="items">The candidate items.</param>
    /// <param name="capacity">The maximum total cost of selected items. Must be positive.</param>
    public SelectionProblem(IReadOnlyList<SelectionItem> items, double capacity)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
        Items = items;
        Capacity = capacity;
    }
}
