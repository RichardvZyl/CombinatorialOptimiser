using System.Diagnostics;

namespace CombinatorialOptimiser.Core;

public enum SolverParadigm { Exact, Construction, Improvement, Reduction }

public interface ISolverBase { string Name { get; } SolverParadigm Paradigm { get; } }

public interface ISolver<TProblem, TResult> : ISolverBase where TResult : SolverResultBase
{
    TResult Solve(TProblem problem);
}

public abstract record SolverResultBase(string Algorithm, SolverParadigm Paradigm, TimeSpan Elapsed);

public sealed record PermutationResult(string Algorithm, SolverParadigm Paradigm, IReadOnlyList<int> Order, double Distance, TimeSpan Elapsed)
    : SolverResultBase(Algorithm, Paradigm, Elapsed)
{
    public string RouteText(IReadOnlyList<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        var names = Order.Select(i => nodes[i].Name); return string.Join(" -> ", names) + " -> " + nodes[Order[0]].Name;
    }
}

public static class SolverRunner
{
    public static PermutationResult Timed(string name, SolverParadigm paradigm, DistanceMatrix m, Func<int[]> solve)
    {
        ArgumentNullException.ThrowIfNull(m);
        ArgumentNullException.ThrowIfNull(solve);
        var sw = Stopwatch.GetTimestamp(); var order = solve();
        return new PermutationResult(name, paradigm, order, m.TourLength(order), Stopwatch.GetElapsedTime(sw));
    }
}
