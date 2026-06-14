using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.SubsetSelection;

internal sealed class GreedySelectionSolver : ISolver<SelectionProblem, SelectionResult>
{
    public string Name => "Greedy (value/cost ratio)";
    public SolverParadigm Paradigm => SolverParadigm.Construction;

    public SelectionResult Solve(SelectionProblem problem) =>
        SelectionSolverRunner.Timed(Name, Paradigm, problem, () =>
        {
            var n = problem.Items.Count;
            var selected = new bool[n];
            var order = Enumerable.Range(0, n).OrderByDescending(i => Ratio(problem.Items[i]));
            var remaining = problem.Capacity;
            foreach (var i in order)
            {
                if (problem.Items[i].Cost <= remaining)
                {
                    selected[i] = true;
                    remaining -= problem.Items[i].Cost;
                }
            }
            return selected;
        });

    private static double Ratio(SelectionItem item) => item.Cost > 0 ? item.Value / item.Cost : double.PositiveInfinity;
}
