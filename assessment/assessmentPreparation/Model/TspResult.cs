using System.Diagnostics;

namespace AssessmentPreparation.Model;

/// <summary>Outcome of a single solver run.</summary>
public sealed record TspResult(
    string Algorithm,
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
    TspResult Solve(DistanceMatrix matrix);
}

/// <summary>Shared timing helper so every solver reports elapsed time the same way.</summary>
public static class SolverRunner
{
    public static TspResult Timed(string name, DistanceMatrix m, Func<int[]> solve)
    {
        ArgumentNullException.ThrowIfNull(solve);
        var sw = Stopwatch.GetTimestamp();
        var order = solve();
        return new TspResult(name, order, m.TourLength(order), Stopwatch.GetElapsedTime(sw));
    }
}
