using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.ConstraintAssignment;

internal sealed class DsaturSolver : ISolver<AssignmentProblem, AssignmentResult>
{
    public string Name => "DSatur (greedy)";
    public SolverParadigm Paradigm => SolverParadigm.Construction;

    public AssignmentResult Solve(AssignmentProblem problem) =>
        AssignmentSolverRunner.Timed(Name, Paradigm, problem, () => SolveImpl(problem));

    private static int[] SolveImpl(AssignmentProblem problem)
    {
        var n = problem.Count;
        var labels = new int[n];
        Array.Fill(labels, -1);

        var degree = new int[n];
        for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                if (i != j && problem.HasConflict(i, j)) degree[i]++;

        for (var step = 0; step < n; step++)
        {
            var best = -1;
            for (var i = 0; i < n; i++)
            {
                if (labels[i] != -1) continue;
                if (best == -1) { best = i; continue; }
                var satI = SaturationDegree(problem, labels, i);
                var satBest = SaturationDegree(problem, labels, best);
                if (satI > satBest || (satI == satBest && degree[i] > degree[best])) best = i;
            }
            labels[best] = SmallestAvailableColour(problem, labels, best);
        }
        return labels;
    }

    private static int SaturationDegree(AssignmentProblem problem, int[] labels, int v)
    {
        var used = new HashSet<int>();
        for (var j = 0; j < problem.Count; j++)
            if (problem.HasConflict(v, j) && labels[j] != -1) used.Add(labels[j]);
        return used.Count;
    }

    private static int SmallestAvailableColour(AssignmentProblem problem, int[] labels, int v)
    {
        var used = new HashSet<int>();
        for (var j = 0; j < problem.Count; j++)
            if (problem.HasConflict(v, j) && labels[j] != -1) used.Add(labels[j]);
        var colour = 0;
        while (used.Contains(colour)) colour++;
        return colour;
    }
}
