using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.SubsetSelection;

internal sealed class DpSelectionSolver : ISolver<SelectionProblem, SelectionResult>
{
    public string Name => "Dynamic Programming (exact)";
    public SolverParadigm Paradigm => SolverParadigm.Exact;

    /// <summary>Number of DP table entries per unit of capacity/cost (for scaling fractional weights to integers).</summary>
    public int Precision { get; init; } = 100;

    public SelectionResult Solve(SelectionProblem problem) =>
        SelectionSolverRunner.Timed(Name, Paradigm, problem, () => SolveImpl(problem));

    private bool[] SolveImpl(SelectionProblem problem)
    {
        var n = problem.Items.Count;
        var capacity = (int)Math.Round(problem.Capacity * Precision);
        var weights = problem.Items.Select(item => (int)Math.Round(item.Cost * Precision)).ToArray();
        var values = problem.Items.Select(item => item.Value).ToArray();

        var tableSize = (long)(n + 1) * (capacity + 1);
        if (tableSize > 10_000_000)
            throw new InvalidOperationException($"DP table too large ({tableSize} entries); reduce Precision or Capacity.");

        var dp = new double[n + 1, capacity + 1];
        for (var i = 1; i <= n; i++)
        {
            var w = weights[i - 1];
            var v = values[i - 1];
            for (var c = 0; c <= capacity; c++)
            {
                dp[i, c] = dp[i - 1, c];
                if (w <= c) dp[i, c] = Math.Max(dp[i, c], dp[i - 1, c - w] + v);
            }
        }

        var selected = new bool[n];
        var rem = capacity;
        for (var i = n; i >= 1; i--)
        {
            if (dp[i, rem] != dp[i - 1, rem])
            {
                selected[i - 1] = true;
                rem -= weights[i - 1];
            }
        }
        return selected;
    }
}
