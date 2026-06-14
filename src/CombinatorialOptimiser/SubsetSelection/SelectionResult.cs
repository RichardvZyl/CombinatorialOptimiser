using System.Diagnostics;
using System.Globalization;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.SubsetSelection;

/// <summary>The result of solving a subset selection (0/1 knapsack) problem.</summary>
/// <param name="Algorithm">The name of the algorithm that produced this result.</param>
/// <param name="Paradigm">The algorithmic category the solver belongs to.</param>
/// <param name="Selected">Whether each item (by index) was selected.</param>
/// <param name="TotalValue">The total value of selected items.</param>
/// <param name="TotalCost">The total cost of selected items.</param>
/// <param name="Elapsed">The wall-clock time taken to produce the result.</param>
#pragma warning disable CA1819 // Selected array is a core solver output; switching to IReadOnlyList would break all solver and test code.
public sealed record SelectionResult(string Algorithm, SolverParadigm Paradigm, bool[] Selected, double TotalValue, double TotalCost, TimeSpan Elapsed)
#pragma warning restore CA1819
    : SolverResultBase(Algorithm, Paradigm, Elapsed)
{
    /// <summary>Renders the selected items as a human-readable summary including total value and cost.</summary>
    /// <param name="items">The items referenced by <see cref="Selected"/>.</param>
    public string SummaryText(IReadOnlyList<SelectionItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var chosen = Selected.Select((selected, i) => (selected, i)).Where(x => x.selected).Select(x => items[x.i].Name);
        return string.Join(", ", chosen) + string.Format(CultureInfo.InvariantCulture, " (value={0}, cost={1})", TotalValue, TotalCost);
    }
}

/// <summary>Helper for running a subset selection solver while measuring elapsed time.</summary>
#pragma warning disable CA1515 // Solver runners are public because they're called from tests.
public static class SelectionSolverRunner
#pragma warning restore CA1515
{
    /// <summary>Runs <paramref name="solve"/>, measuring elapsed time, and wraps the resulting selection in a <see cref="SelectionResult"/>.</summary>
    /// <param name="name">The algorithm name to record in the result.</param>
    /// <param name="paradigm">The algorithmic category to record in the result.</param>
    /// <param name="problem">The problem instance used to compute total value and cost.</param>
    /// <param name="solve">A function that computes which items are selected.</param>
    public static SelectionResult Timed(string name, SolverParadigm paradigm, SelectionProblem problem, Func<bool[]> solve)
    {
        ArgumentNullException.ThrowIfNull(problem);
        ArgumentNullException.ThrowIfNull(solve);
        var sw = Stopwatch.GetTimestamp();
        var selected = solve();
        var elapsed = Stopwatch.GetElapsedTime(sw);
        double value = 0, cost = 0;
        for (var i = 0; i < selected.Length; i++) if (selected[i]) { value += problem.Items[i].Value; cost += problem.Items[i].Cost; }
        return new SelectionResult(name, paradigm, selected, value, cost, elapsed);
    }
}
