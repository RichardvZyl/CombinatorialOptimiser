# CombinatorialOptimiser -- Codebase Overview

## Summary
A C# .NET library (`src/CombinatorialOptimiser/`, packable as a NuGet
package), plus a demo console app (`CombinatorialOptimiser.Cli/`), that solves
combinatorial optimisation problems across three domains:
- **Permutation** (order all n nodes to minimise total cost, e.g. TSP) --
  eleven algorithm variants across four paradigms.
- **SubsetSelection** (0/1 knapsack: choose items to maximise value within a
  capacity) -- five solvers.
- **ConstraintAssignment** (graph colouring: label entities so no conflicting
  pair shares a label) -- four solvers.

> **Generic model**: The Permutation domain type is `Node` (not `City`)
> because the same algorithms apply to scheduling, wiring, network routing,
> and any other permutation optimisation problem.

## Architecture
**Pattern**: Strategy -- each algorithm implements `ISolver<TProblem, TResult>`,
specialised per domain (`ISolver<DistanceMatrix, PermutationResult>`,
`ISolver<SelectionProblem, SelectionResult>`,
`ISolver<AssignmentProblem, AssignmentResult>`). The CLI's `Program.cs`
dispatches across the Permutation solvers via the non-generic `ISolver`
marker, and `SolverRegistry` recommends/enumerates solvers by problem size.
Solvers are classified by the shared `SolverParadigm` enum.

Shared metaheuristic scaffolding lives in `Core/Metaheuristics/`:
`SimulatedAnnealing<TProblem, TSolution>`,
`GeneticAlgorithm<TProblem, TChromosome>`, and
`IteratedLocalSearch<TProblem, TSolution>` implement the
temperature/population/perturbation loops; each domain's SA/GA/ILS solvers are
thin subclasses that supply the domain-specific moves and objective.

## Key Abstractions
- **ISolver**: Non-generic marker (`Name`, `Paradigm`) for cross-domain collections
- **ISolver\<TProblem, TResult\>**: `Solve(TProblem) -> TResult`
- **SolverParadigm**: Exact, Construction, Improvement, Reduction
- **SolverResult**: Common `Algorithm`, `Paradigm`, `Elapsed` fields
- **DistanceMatrix / PermutationResult**: Precomputed cost lookup (Euclidean or raw) / Order, Distance (Permutation)
- **SelectionProblem / SelectionResult**: Items, Capacity / Selected, TotalValue, TotalCost (SubsetSelection)
- **AssignmentProblem / AssignmentResult**: Entities, Conflicts / Labels, LabelCount (ConstraintAssignment)
