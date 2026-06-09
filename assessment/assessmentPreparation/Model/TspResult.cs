using System.Diagnostics;

namespace AssessmentPreparation.Model;

/// <summary>Broad algorithmic category for a TSP solver.</summary>
public enum TspParadigm
{
    /// <summary>Enumerates the full search space to guarantee optimality.</summary>
    Exact,
    /// <summary>Builds a single tour greedily without backtracking.</summary>
    Construction,
    /// <summary>Takes an existing tour and repeatedly applies local moves to shorten it.</summary>
    Improvement,
    /// <summary>Reduces TSP to a different problem and transforms the result back.</summary>
    Reduction,
}

/// <summary>Outcome of a single solver run.</summary>
public sealed record TspResult(
    string Algorithm,
    TspParadigm Paradigm,
    IReadOnlyList<int> Order,
    double Distance,
    TimeSpan Elapsed)
{
    public string RouteText(IReadOnlyList<City> cities)
    {
        var names = Order.Select(i => cities[i].Name);
        return string.Join(" -> ", names) + " -> " + cities[Order[0]].Name;
    }
}

/// <summary>Common contract for all TSP solvers.</summary>
public interface ITspSolver
{
    string Name { get; }
    TspParadigm Paradigm { get; }
    TspResult Solve(DistanceMatrix matrix);
}

/// <summary>Shared timing helper so every solver reports elapsed time the same way.</summary>
public static class SolverRunner
{
    public static TspResult Timed(string name, TspParadigm paradigm, DistanceMatrix m, Func<int[]> solve)
    {
        ArgumentNullException.ThrowIfNull(solve);
        var sw = Stopwatch.GetTimestamp();
        var order = solve();
        return new TspResult(name, paradigm, order, m.TourLength(order), Stopwatch.GetElapsedTime(sw));
    }
}
// Paradigm classification applied
