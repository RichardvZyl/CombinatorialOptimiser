using System;
using System.Linq;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

namespace CombinatorialOptimiser.Permutation;

/// <summary>
/// Thin ILP wrapper for permutation problems. Attempts to use an external ILP solver
/// (e.g., Google OR-Tools linear solver) if present; otherwise falls back to Held-Karp DP solver.
/// The reflection-based approach avoids a hard dependency for CI and unit tests.
/// </summary>
public sealed class IlpSolver : ISolver<DistanceMatrix, PermutationResult>
{
    private readonly int _timeLimitMs;

    public IlpSolver(int timeLimitMs = 2000)
    {
        _timeLimitMs = timeLimitMs;
    }

    public PermutationResult Solve(DistanceMatrix problem)
    {
        try
        {
            var ilpAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name?.IndexOf("OrTools", StringComparison.OrdinalIgnoreCase) >= 0
                                     || a.GetName().Name?.IndexOf("CoinUtils", StringComparison.OrdinalIgnoreCase) >= 0
                                     || a.GetName().Name?.IndexOf("Google.OrTools", StringComparison.OrdinalIgnoreCase) >= 0);

            if (ilpAssembly is not null)
            {
                return SolveWithIlp(problem);
            }
        }
        catch
        {
            // ignore and fall back
        }

        // Fallback to Held-Karp DP exact solver when ILP not available (good for small n)
        var hk = new HeldKarpSolver();
        return hk.Solve(problem);
    }

    private PermutationResult SolveWithIlp(DistanceMatrix problem)
    {
        // Placeholder for a future real ILP integration. For now, delegate to Held-Karp
        // to provide deterministic behaviour in tests and CI.
        return new HeldKarpSolver().Solve(problem);
    }
}
