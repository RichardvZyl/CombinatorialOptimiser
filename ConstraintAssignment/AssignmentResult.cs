using System.Diagnostics;
using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.ConstraintAssignment;

#pragma warning disable CA1819 // Labels array is a core solver output; switching to IReadOnlyList would break all solver and test code.
public sealed record AssignmentResult(string Algorithm, SolverParadigm Paradigm, int[] Labels, int LabelCount, TimeSpan Elapsed)
#pragma warning restore CA1819
    : SolverResultBase(Algorithm, Paradigm, Elapsed)
{
    public bool IsValid(AssignmentProblem problem)
    {
        ArgumentNullException.ThrowIfNull(problem);
        for (var i = 0; i < Labels.Length; i++)
            for (var j = i + 1; j < Labels.Length; j++)
                if (problem.HasConflict(i, j) && Labels[i] == Labels[j]) return false;
        return true;
    }

    public string SummaryText(AssignmentProblem problem)
    {
        var groups = Labels.Select((label, i) => (label, i))
            .GroupBy(x => x.label)
            .OrderBy(g => g.Key)
            .Select(g => $"Group {g.Key}: " + string.Join(", ", g.Select(x => problem.Entities[x.i])));
        return string.Join(" | ", groups) + $" (labels={LabelCount})";
    }
}

internal static class AssignmentSolverRunner
{
    public static AssignmentResult Timed(string name, SolverParadigm paradigm, AssignmentProblem problem, Func<int[]> solve)
    {
        ArgumentNullException.ThrowIfNull(solve);
        var sw = Stopwatch.GetTimestamp();
        var labels = solve();
        var elapsed = Stopwatch.GetElapsedTime(sw);
        return new AssignmentResult(name, paradigm, labels, labels.Distinct().Count(), elapsed);
    }
}
