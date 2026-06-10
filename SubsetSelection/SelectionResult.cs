using System.Diagnostics;
using System.Globalization;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.SubsetSelection;

#pragma warning disable CA1819 // Selected array is a core solver output; switching to IReadOnlyList would break all solver and test code.
public sealed record SelectionResult(string Algorithm, SolverParadigm Paradigm, bool[] Selected, double TotalValue, double TotalCost, TimeSpan Elapsed)
#pragma warning restore CA1819
    : SolverResultBase(Algorithm, Paradigm, Elapsed)
{
    public string SummaryText(IReadOnlyList<SelectionItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var chosen = Selected.Select((selected, i) => (selected, i)).Where(x => x.selected).Select(x => items[x.i].Name);
        return string.Join(", ", chosen) + string.Format(CultureInfo.InvariantCulture, " (value={0}, cost={1})", TotalValue, TotalCost);
    }
}

#pragma warning disable CA1515 // Solver runners are public because they're called from tests.
public static class SelectionSolverRunner
#pragma warning restore CA1515
{
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
