using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Lightweight GNN policy stub: when a trained GNN model is available at runtime (via
/// an ONNX file or referenced assembly), this solver will call it to compute node scores.
/// For CI and tests this is a deterministic randomized fallback that ranks by degree-based
/// heuristic.
/// </summary>
public sealed class GnnPolicySolver : ISolver<DistanceMatrix, PermutationResult>
{
    /// <inheritdoc/>
    public string Name => "GNN Policy";

    /// <inheritdoc/>
    public SolverParadigm Paradigm => SolverParadigm.Construction;

    /// <summary>Seed for any stochastic fallback behaviour.</summary>
    public int Seed { get; }

    /// <summary>Create a new GNN policy solver stub.</summary>
    /// <param name="seed">Seed for any stochastic fallback behaviour.</param>
    public GnnPolicySolver(int seed = 42) => Seed = seed;

    /// <inheritdoc/>
    public PermutationResult Solve(DistanceMatrix matrix) =>
        SolverRunner.Timed(Name, Paradigm, matrix, () => SolveInternal(matrix));

    private static int[] SolveInternal(DistanceMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        var n = matrix.Count;
        var scores = new double[n];
        for (var i = 0; i < n; i++)
        {
            var sum = 0.0;
            for (var j = 0; j < n; j++)
                sum += matrix[i, j];
            scores[i] = -sum;
        }

        return scores.Select((s, i) => (s, i)).OrderByDescending(x => x.s).Select(x => x.i).ToArray();
    }
}
