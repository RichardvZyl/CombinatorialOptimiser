using System.Diagnostics;

namespace AssessmentPreparation.Model;

/// <summary>Broad algorithmic category for a TSP solver.</summary>
public enum SolverParadigm
{
    Exact,
    Construction,
    Improvement,
    Reduction,
}

/// <summary>Outcome of a single solver run.</summary>
public sealed record SolverResult(
    string Algorithm,
    SolverParadigm Paradigm,
    IReadOnlyList<int> Order,
    double Distance,
    TimeSpan Elapsed)
{
    public string RouteText(IReadOnlyList<Node> nodes)
    {
        var names = Order.Select(i => nodes[i].Name);
        return string.Join(" -> ", names) + " -> " + nodes[Order[0]].Name;
    }
}

public interface ISolver
{
    string Name { get; }
    SolverParadigm Paradigm { get; }
    SolverResult Solve(DistanceMatrix matrix);
}

/// <summary>Shared timing helper so every solver reports elapsed time the same way.</summary>
public static class SolverRunner
{
    public static SolverResult Timed(string name, SolverParadigm paradigm, DistanceMatrix m, Func<int[]> solve)
    {
        ArgumentNullException.ThrowIfNull(solve);
        var sw = Stopwatch.GetTimestamp();
        var order = solve();
        return new SolverResult(name, paradigm, order, m.TourLength(order), Stopwatch.GetElapsedTime(sw));
    }
}
