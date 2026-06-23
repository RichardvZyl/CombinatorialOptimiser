using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.SubsetSelection;

internal sealed class BnBSelectionSolver : ISolver<SelectionProblem, SelectionResult>
{
    public string Name => "Branch and Bound (exact)";
    public SolverParadigm Paradigm => SolverParadigm.Exact;

    public SelectionResult Solve(SelectionProblem problem) =>
        SelectionSolverRunner.Timed(Name, Paradigm, problem, () => SolveImpl(problem));

    private static bool[] SolveImpl(SelectionProblem problem)
    {
        var n = problem.Items.Count;
        if (n > 60) throw new InvalidOperationException($"Branch and Bound requires n ≤ 60 (got {n}). Use GreedySelectionSolver for larger inputs.");
        var capacity = problem.Capacity;

        // Sort by value/cost ratio descending: gives the tightest fractional-knapsack bound.
        var order = Enumerable.Range(0, n)
            .OrderByDescending(i => Ratio(problem.Items[i]))
            .ToArray();
        var items = order.Select(i => problem.Items[i]).ToArray();

        var bestValue = 0.0;
        var bestSelected = new bool[n];
        var current = new bool[n];

        Search(0, 0.0, 0.0);

        var result = new bool[n];
        for (var i = 0; i < n; i++) result[order[i]] = bestSelected[i];
        return result;

        void Search(int index, double cost, double value)
        {
            if (value > bestValue)
            {
                bestValue = value;
                Array.Copy(current, bestSelected, n);
            }
            if (index == n) return;
            if (Bound(index, cost, value) <= bestValue) return;

            if (cost + items[index].Cost <= capacity)
            {
                current[index] = true;
                Search(index + 1, cost + items[index].Cost, value + items[index].Value);
                current[index] = false;
            }
            Search(index + 1, cost, value);
        }

        double Bound(int index, double cost, double value)
        {
            var remaining = capacity - cost;
            var bound = value;
            for (var i = index; i < n; i++)
            {
                if (items[i].Cost <= remaining)
                {
                    bound += items[i].Value;
                    remaining -= items[i].Cost;
                }
                else
                {
                    if (items[i].Cost > 0) bound += items[i].Value * (remaining / items[i].Cost);
                    break;
                }
            }
            return bound;
        }
    }

    private static double Ratio(SelectionItem item) => item.Cost > 0 ? item.Value / item.Cost : double.PositiveInfinity;
}
