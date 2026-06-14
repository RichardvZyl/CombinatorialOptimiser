using System.Diagnostics;

namespace CombinatorialOptimiser.Core;

/// <summary>The algorithmic category a solver belongs to.</summary>
public enum SolverParadigm
{
    /// <summary>Guarantees an optimal solution (e.g. dynamic programming, branch and bound).</summary>
    Exact,

    /// <summary>Builds a solution incrementally using a heuristic (e.g. greedy, nearest neighbour).</summary>
    Construction,

    /// <summary>Iteratively improves an existing solution (e.g. local search, metaheuristics).</summary>
    Improvement,

    /// <summary>Solves via a reduction to another problem with a known approximation guarantee.</summary>
    Reduction
}

/// <summary>Common identity shared by all solvers, independent of problem/result types.</summary>
public interface ISolverBase
{
    /// <summary>A human-readable name for the algorithm.</summary>
    string Name { get; }

    /// <summary>The algorithmic category this solver belongs to.</summary>
    SolverParadigm Paradigm { get; }
}

/// <summary>A solver that maps a problem instance of type <typeparamref name="TProblem"/> to a result of type <typeparamref name="TResult"/>.</summary>
/// <typeparam name="TProblem">The problem instance type.</typeparam>
/// <typeparam name="TResult">The result type, derived from <see cref="SolverResultBase"/>.</typeparam>
public interface ISolver<TProblem, TResult> : ISolverBase where TResult : SolverResultBase
{
    /// <summary>Solves the given problem instance synchronously.</summary>
    TResult Solve(TProblem problem);

    /// <summary>Solves the given problem instance synchronously, stopping early and returning the best solution found so far if <paramref name="ct"/> is cancelled.</summary>
    /// <remarks>The default implementation ignores <paramref name="ct"/> and runs to completion. Solvers that support early stopping override this.</remarks>
    TResult Solve(TProblem problem, CancellationToken ct) => Solve(problem);

    /// <summary>Asynchronously solves the problem with support for cancellation.</summary>
    Task<TResult> SolveAsync(TProblem problem, CancellationToken ct = default)
    {
        // Default: wrap synchronous solve in Task.Run for cancellation support.
        return Task.Run(() => Solve(problem, ct), ct);
    }
}

/// <summary>Base record for all solver results, capturing the algorithm identity and timing.</summary>
/// <param name="Algorithm">The name of the algorithm that produced this result.</param>
/// <param name="Paradigm">The algorithmic category the solver belongs to.</param>
/// <param name="Elapsed">The wall-clock time taken to produce the result.</param>
public abstract record SolverResultBase(string Algorithm, SolverParadigm Paradigm, TimeSpan Elapsed);

/// <summary>The result of solving a permutation (TSP-style) problem.</summary>
/// <param name="Algorithm">The name of the algorithm that produced this result.</param>
/// <param name="Paradigm">The algorithmic category the solver belongs to.</param>
/// <param name="Order">The visiting order of node indices.</param>
/// <param name="Distance">The total tour distance for <paramref name="Order"/>.</param>
/// <param name="Elapsed">The wall-clock time taken to produce the result.</param>
public sealed record PermutationResult(string Algorithm, SolverParadigm Paradigm, IReadOnlyList<int> Order, double Distance, TimeSpan Elapsed)
    : SolverResultBase(Algorithm, Paradigm, Elapsed)
{
    /// <summary>Renders the tour as a human-readable "A -> B -> ... -> A" route description.</summary>
    /// <param name="nodes">The nodes referenced by <see cref="Order"/>.</param>
    public string RouteText(IReadOnlyList<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        var names = Order.Select(i => nodes[i].Name); return string.Join(" -> ", names) + " -> " + nodes[Order[0]].Name;
    }
}

/// <summary>Helper for running a permutation solver while measuring elapsed time.</summary>
public static class SolverRunner
{
    /// <summary>Runs <paramref name="solve"/>, measuring elapsed time, and wraps the resulting tour order in a <see cref="PermutationResult"/>.</summary>
    /// <param name="name">The algorithm name to record in the result.</param>
    /// <param name="paradigm">The algorithmic category to record in the result.</param>
    /// <param name="m">The distance matrix used to compute the tour length.</param>
    /// <param name="solve">A function that computes the tour order.</param>
    public static PermutationResult Timed(string name, SolverParadigm paradigm, DistanceMatrix m, Func<int[]> solve)
    {
        ArgumentNullException.ThrowIfNull(m);
        ArgumentNullException.ThrowIfNull(solve);
        var sw = Stopwatch.GetTimestamp(); var order = solve();
        return new PermutationResult(name, paradigm, order, m.TourLength(order), Stopwatch.GetElapsedTime(sw));
    }
}
