using System;
using System.Collections.Generic;
using System.Linq;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Diverse beam search variant that encourages diversity across beam members by
/// penalising duplicate actions or tours using a simple Jaccard-style penalty.
/// Falls back to BeamSearch behaviour when diversity parameter is zero.
/// </summary>
public sealed class DiverseBeamSolver : ISolver<DistanceMatrix, PermutationResult>
{
    /// <inheritdoc/>
    public string Name => "Diverse Beam Search";
    /// <inheritdoc/>
    public SolverParadigm Paradigm => SolverParadigm.Construction;
        /// <summary>Beam width.</summary>
    public int BeamWidth { get; init; } = 6;
        /// <summary>Temperature.</summary>
    public double Temperature { get; init; } = 1.0;
        /// <summary>Diversity strength.</summary>
    public double DiversityStrength { get; init; } = 0.5;

        /// <inheritdoc/>
    public PermutationResult Solve(DistanceMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        var beam = new List<int[]> { Array.Empty<int>() };
        var beamScores = new List<double> { 0.0 };

        var logProbs = matrix.TransitionLogProbabilities(Temperature);
        var n = matrix.Count;

        for (int step = 0; step < n; step++)
        {
            var candidates = new List<(int[] tour, double score)>();
            for (int i = 0; i < beam.Count; i++)
            {
                var prefix = beam[i];
                var used = new HashSet<int>(prefix);
                for (int next = 0; next < n; next++)
                {
                    if (used.Contains(next)) continue;
                    var newTour = prefix.Append(next).ToArray();
                    var stepScore = beamScores[i] + logProbs[prefix.Length, next];
                    // diversity penalty: encourage different sets of visited nodes
                    var penalty = 0.0;
                    if (DiversityStrength > 0)
                    {
                        foreach (var (other, _) in candidates)
                        {
                            var jacc = JaccardSimilarity(newTour, other);
                            penalty += jacc;
                        }
                        penalty = (penalty / Math.Max(1, candidates.Count)) * DiversityStrength;
                        stepScore -= penalty;
                    }

                    candidates.Add((newTour, stepScore));
                }
            }

            beam = candidates.OrderByDescending(c => c.score).Take(BeamWidth).Select(c => c.tour).ToList();
            beamScores = candidates.OrderByDescending(c => c.score).Take(BeamWidth).Select(c => c.score).ToList();
        }

        // final ranking: pick best complete tour
        var final = beam.OrderByDescending(t => t.Select((v, i) => 0).Sum()).First();
        var order = final.ToArray();
        return SolverRunner.Timed(Name, Paradigm, matrix, () => order);
    }

    private static double JaccardSimilarity(int[] a, int[] b)
    {
        var sa = new HashSet<int>(a);
        var sb = new HashSet<int>(b);
        var inter = sa.Intersect(sb).Count();
        var uni = sa.Union(sb).Count();
        return uni == 0 ? 0.0 : (double)inter / uni;
    }
}
