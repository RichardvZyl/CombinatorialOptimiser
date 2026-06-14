using CombinatorialOptimiser.ConstraintAssignment;
using CombinatorialOptimiser.Permutation;
using CombinatorialOptimiser.SubsetSelection;

namespace CombinatorialOptimiser.Core;

/// <summary>Provides solver recommendations based on problem size.</summary>
public static class SolverRegistry
{
    /// <summary>Returns the recommended solver for a permutation problem of the given size.</summary>
    public static ISolver<DistanceMatrix, PermutationResult> RecommendPermutation(int nodeCount) => nodeCount switch
    {
        <= 10 => new BruteForceSolver(),
        <= 16 => new HeldKarpSolver(),
        <= 40 => new ChristofidesSolver { UseExactMatching = true },
        _ => new LinKernighanSolver(),
    };

    /// <summary>Returns the recommended solver for a subset selection (0/1 knapsack) problem.</summary>
    public static ISolver<SelectionProblem, SelectionResult> RecommendSelection(int itemCount)
    {
        // DP is exact and fast for moderate item counts, falls back to GA for larger problems.
        return itemCount <= 30 ? new DpSelectionSolver() : new GaSelectionSolver();
    }

    /// <summary>Returns the recommended solver for a constraint assignment (graph colouring) problem.</summary>
    public static ISolver<AssignmentProblem, AssignmentResult> RecommendAssignment(int entityCount)
    {
        // Backtracking is exact for n <= 20, otherwise use GA.
        return entityCount <= 20 ? new BacktrackingSolver() : new GaAssignmentSolver();
    }

    /// <summary>Returns all solvers appropriate for the given permutation problem size,
    /// optionally including Christofides-seeded variants if a distance matrix is provided.</summary>
    public static IReadOnlyList<ISolver<DistanceMatrix, PermutationResult>> AllPermutationSolvers(
        int nodeCount, DistanceMatrix? matrix = null)
    {
        var all = AllSolvers();
        var filtered = nodeCount switch
        {
            <= 10 => all,
            <= 16 => all.Where(s => s is not BruteForceSolver).ToArray(),
            <= 18 => all.Where(s => s is not BruteForceSolver and not BranchAndBoundSolver).ToArray(),
            _ => all.Where(s => s is not BruteForceSolver and not BranchAndBoundSolver and not HeldKarpSolver and not ChristofidesSolver { UseExactMatching: true }).ToArray(),
        };

        return nodeCount >= 4 && matrix is not null
            ? filtered.Concat(ChristofidesSeededVariants(matrix)).ToArray()
            : filtered;
    }

    private static ISolver<DistanceMatrix, PermutationResult>[] AllSolvers() =>
    [
        new BruteForceSolver(),
        new BranchAndBoundSolver(),
        new HeldKarpSolver(),
        new NearestNeighborSolver(),
        new ChristofidesSolver { UseExactMatching = true },
        new ChristofidesSolver { UseExactMatching = false },
        new TwoOptSolver(),
        new ThreeOptSolver(),
        new LinKernighanSolver(),
        new IteratedLocalSearchSolver(),
        new SimulatedAnnealingSolver(),
        new GeneticAlgorithmSolver(),
    ];

    private static ISolver<DistanceMatrix, PermutationResult>[] ChristofidesSeededVariants(DistanceMatrix matrix)
    {
        try
        {
            var seedSolver = new ChristofidesSolver { UseExactMatching = matrix.Count <= 20 };
            var seed = seedSolver.Solve(matrix).Order.ToArray();
            return
            [
                new TwoOptSolver { Seed = seed },
                new ThreeOptSolver { Seed = seed },
                new LinKernighanSolver { Seed = seed },
                new IteratedLocalSearchSolver { Seed = seed },
            ];
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return [];
        }
    }
}
