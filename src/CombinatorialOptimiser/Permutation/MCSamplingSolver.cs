using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Monte Carlo sampling solver: samples random tours and keeps the best found within budget.
/// Deterministic seed ensures test reproducibility.
/// </summary>
public sealed class MCSamplingSolver : ISolver<DistanceMatrix, PermutationResult>
{
    /// <inheritdoc/>
    public string Name => "Monte Carlo Sampling";

    /// <inheritdoc/>
    public SolverParadigm Paradigm => SolverParadigm.Construction;

    /// <summary>Number of random tour samples to draw.</summary>
    public int Samples { get; init; } = 1000;

    /// <inheritdoc/>
    public PermutationResult Solve(DistanceMatrix matrix) =>
        SolverRunner.Timed(Name, Paradigm, matrix, () => SolveInternal(matrix, Samples));

    private static int[] SolveInternal(DistanceMatrix matrix, int samples)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        var n = matrix.Count;
        var rng = new Random(1234);
        var best = Enumerable.Range(0, n).ToArray();
        var bestCost = matrix.TourLength(best);

        var arr = Enumerable.Range(0, n).ToArray();
        for (var s = 0; s < samples; s++)
        {
            for (var i = n - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }

            var cost = matrix.TourLength(arr);
            if (cost < bestCost)
            {
                bestCost = cost;
                best = arr.ToArray();
            }
        }

        return best;
    }
}
