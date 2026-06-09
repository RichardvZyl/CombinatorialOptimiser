using System.Diagnostics;

namespace PermutationOptimiser.Model;

public enum SolverParadigm { Exact, Construction, Improvement, Reduction }

public sealed record SolverResult(string Algorithm, SolverParadigm Paradigm, IReadOnlyList<int> Order, double Distance, TimeSpan Elapsed)
{
    public string RouteText(IReadOnlyList<Node> nodes) { var names = Order.Select(i => nodes[i].Name); return string.Join(" -> ", names) + " -> " + nodes[Order[0]].Name; }
}

public interface ISolver { string Name { get; } SolverParadigm Paradigm { get; } SolverResult Solve(DistanceMatrix matrix); }

public static class SolverRunner
{
    public static SolverResult Timed(string name, SolverParadigm paradigm, DistanceMatrix m, Func<int[]> solve)
    {
        ArgumentNullException.ThrowIfNull(solve);
        var sw = Stopwatch.GetTimestamp(); var order = solve();
        return new SolverResult(name, paradigm, order, m.TourLength(order), Stopwatch.GetElapsedTime(sw));
    }
}
