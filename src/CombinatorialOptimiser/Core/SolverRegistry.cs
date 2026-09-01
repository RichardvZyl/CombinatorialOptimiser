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
            <= 16 => all.Where(s => s is not BruteForceSolver and not RecursiveBruteForceSolver).ToArray(),
            <= 18 => all.Where(s => s is not BruteForceSolver and not RecursiveBruteForceSolver and not BranchAndBoundSolver).ToArray(),
            _ => all.Where(s => s is not BruteForceSolver and not RecursiveBruteForceSolver and not BranchAndBoundSolver and not HeldKarpSolver and not ChristofidesSolver { UseExactMatching: true }).ToArray(),
        };

        return nodeCount >= 4 && matrix is not null
            ? filtered.Concat(ChristofidesSeededVariants(matrix)).ToArray()
            : filtered;
    }

    // Canonical list of available permutation solvers. Each entry below is intentionally
    // short-described so readers and tooling understand the purpose and tradeoffs.
    // When adding a new solver, also add a docs/solvers/<SolverName>.md file describing
    // complexity, parameters and typical use cases (a template exists at docs/solvers/TEMPLATE.md).
    private static ISolver<DistanceMatrix, PermutationResult>[] AllSolvers() =>
    [
        // Exact solvers (guarantee optimality; exponential cost)
        new BruteForceSolver(),            // exhaustive enumeration, small n only
        new RecursiveBruteForceSolver(),   // recursive variant of brute force
        new BranchAndBoundSolver(),        // pruned exact search using bounds
        new HeldKarpSolver(),              // DP exact solver (2^n * n^2)

        // Fast constructive heuristics (cheap, often used as seeds)
        new NearestNeighborSolver(),       // greedy nearest unvisited

        // Beam search: heuristic constructive strategy guided by transition log-probabilities
        // Useful as an LLM-style decoder analogue (beam, temperature) and as a tunable
        // compromise between greedy and exhaustive search.
        new BeamSearchSolver(),
        new DiverseBeamSolver(),            // Beam variant that encourages diversity across beam members

        // Approximation / reduction
        new ChristofidesSolver { UseExactMatching = true },
        new ChristofidesSolver { UseExactMatching = false },

        // Local improvement heuristics
        new TwoOptSolver(),
        new ThreeOptSolver(),
        new LinKernighanSolver(),

        // Metaheuristics / stochastic improvement
        new IteratedLocalSearchSolver(),
        new SimulatedAnnealingSolver(),
        new GeneticAlgorithmSolver(),
        new GnnPolicySolver(), // learned GNN policy (ONNX); falls back to cheap heuristic when model unavailable
    ];

    /// <summary>
    /// Produce solver variants seeded from a Christofides tour.
    /// Runs Christofides (uses exact matching when matrix.Count is 20 or less) to obtain a seed tour,
    /// then returns improvement solvers (TwoOpt, ThreeOpt, LinKernighan, IteratedLocalSearch)
    /// with their <c>Seed</c> property set to that tour. If seeding fails due to an
    /// invalid or degenerate matrix, the method returns an empty array so callers can
    /// continue without the seeded variants.
    /// </summary>
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
