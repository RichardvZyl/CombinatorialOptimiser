using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Variable Neighborhood Search (VNS) simple implementation alternating 2-opt and 3-opt neighborhoods.
/// </summary>
public sealed class VnsSolver : ISolver<DistanceMatrix, PermutationResult>
{
    /// <inheritdoc/>
    public string Name => "Variable Neighborhood Search";

    /// <inheritdoc/>
    public SolverParadigm Paradigm => SolverParadigm.Improvement;

    /// <summary>Number of VNS iterations.</summary>
    public int Iterations { get; init; } = 50;

    /// <inheritdoc/>
    public PermutationResult Solve(DistanceMatrix matrix) =>
        SolverRunner.Timed(Name, Paradigm, matrix, () => SolveInternal(matrix, Iterations));

    private static int[] SolveInternal(DistanceMatrix matrix, int iterations)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        var n = matrix.Count;
        var rng = new Random(42);
        var current = Enumerable.Range(0, n).OrderBy(_ => rng.Next()).ToArray();
        var bestCost = matrix.TourLength(current);

        for (var it = 0; it < iterations; it++)
        {
            var k = (it % 2) + 2;
            var shaken = Shake(current, k, rng);
            var improved = LocalSearch(matrix, shaken);
            var cost = matrix.TourLength(improved);
            if (cost < bestCost)
            {
                current = improved;
                bestCost = cost;
            }
        }

        return current;
    }

    private static int[] Shake(int[] tour, int k, Random rng)
    {
        var n = tour.Length;
        var t = tour.ToArray();
        var i = rng.Next(0, n - k + 1);
        Array.Reverse(t, i, k);
        return t;
    }

    private static int[] LocalSearch(DistanceMatrix matrix, int[] tour)
    {
        var improved = true;
        var current = tour.ToArray();
        var n = tour.Length;
        while (improved)
        {
            improved = false;
            for (var i = 0; i < n - 1; i++)
            for (var j = i + 1; j < n; j++)
            {
                var candidate = current.ToArray();
                Array.Reverse(candidate, i, j - i + 1);
                if (matrix.TourLength(candidate) < matrix.TourLength(current))
                {
                    current = candidate;
                    improved = true;
                }
            }
        }

        return current;
    }
}
