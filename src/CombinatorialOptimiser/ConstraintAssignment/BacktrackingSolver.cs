using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.ConstraintAssignment;

internal sealed class BacktrackingSolver : ISolver<AssignmentProblem, AssignmentResult>
{
    public string Name => "Backtracking (exact)";
    public SolverParadigm Paradigm => SolverParadigm.Exact;

    public AssignmentResult Solve(AssignmentProblem problem) =>
        AssignmentSolverRunner.Timed(Name, Paradigm, problem, () => SolveImpl(problem));

    private static int[] SolveImpl(AssignmentProblem problem)
    {
        var n = problem.Count;
        if (n > 20) throw new InvalidOperationException($"Backtracking requires n <= 20 (got {n}).");
        if (n == 0) return [];

        for (var k = 1; k <= n; k++)
        {
            var labels = new int[n];
            Array.Fill(labels, -1);
            var domains = new bool[n][];
            for (var i = 0; i < n; i++) { domains[i] = new bool[k]; Array.Fill(domains[i], true); }
            if (TryColour(problem, labels, domains, k, 0)) return labels;
        }
        throw new InvalidOperationException("No valid colouring found.");
    }

    private static bool TryColour(AssignmentProblem problem, int[] labels, bool[][] domains, int k, int vertex)
    {
        var n = problem.Count;
        if (vertex == n) return true;

        for (var colour = 0; colour < k; colour++)
        {
            if (!domains[vertex][colour]) continue;

            labels[vertex] = colour;
            var removed = new List<int>();
            var feasible = true;
            for (var j = vertex + 1; j < n; j++)
            {
                if (problem.HasConflict(vertex, j) && domains[j][colour])
                {
                    domains[j][colour] = false;
                    removed.Add(j);
                    if (!domains[j].Contains(true)) { feasible = false; break; }
                }
            }
            if (feasible && TryColour(problem, labels, domains, k, vertex + 1)) return true;

            foreach (var j in removed) domains[j][colour] = true;
            labels[vertex] = -1;
        }
        return false;
    }
}
