# CombinatorialOptimiser

A dependency-free .NET 10 combinatorial optimisation **library**, plus a demo
**CLI**, solving problems across three domains: **Permutation** (TSP-style
ordering), **SubsetSelection** (0/1 knapsack), and **ConstraintAssignment**
(graph colouring). Provides **20+ solvers** across **four paradigms** (Exact,
Construction, Improvement, Reduction).

## Quick start (CLI demo)

```bash
dotnet run --project CombinatorialOptimiser.Cli -c Release                         # interactive prompt + demos
dotnet run --project CombinatorialOptimiser.Cli -c Release -- --cities 10          # 10 random cities, all solvers
dotnet run --project CombinatorialOptimiser.Cli -c Release -- --cities 8 --solver HeldKarp  # single solver
```

## Using the library

Reference the `CombinatorialOptimiser` NuGet package, then pick a solver
directly or let `SolverRegistry` recommend one based on problem size:

```csharp
using CombinatorialOptimiser.Core;

var matrix = new DistanceMatrix(nodes);
var solver = SolverRegistry.RecommendPermutation(matrix.Count);
var result = solver.Solve(matrix);
// or, with cancellation support:
var result = await solver.SolveAsync(matrix, cancellationToken);
```

`SolverRegistry` also exposes `RecommendSelection`, `RecommendAssignment`, and
`AllPermutationSolvers` for enumerating every applicable solver at a given
problem size.

## Run the tests

```bash
dotnet test CombinatorialOptimiser.slnx -c Release
```

## Project layout

```
CombinatorialOptimiser.slnx
├── src/CombinatorialOptimiser/    # the LIBRARY (packable NuGet project)
│   ├── Core/                       # ISolver<TProblem,TResult>, SolverResultBase, Node, DistanceMatrix, SolverRegistry
│   │   └── Metaheuristics/          # shared SA / GA / ILS base classes
│   ├── Permutation/                # 11 solver implementations (TSP-style)
│   ├── SubsetSelection/             # 0/1 knapsack: 5 solvers
│   └── ConstraintAssignment/         # graph colouring: 4 solvers
├── CombinatorialOptimiser.Cli/    # demo console app
└── CombinatorialOptimiser.Tests/  # xUnit test project
```

## The 11 Permutation solvers

| Algorithm | Paradigm | Complexity | Quality |
|-----------|----------|-----------|---------|
| Brute Force | Exact | O(n!) | Optimal |
| Branch & Bound | Exact | O(n!) worst | Optimal |
| Held-Karp DP | Exact | O(2^n * n^2) | Optimal |
| Nearest Neighbor | Construction | O(n^2) | ~25% above optimal |
| 2-opt | Improvement | O(n^2) / sweep | Local optimum |
| 3-opt | Improvement | O(n^3) / sweep | Local optimum |
| Lin-Kernighan | Improvement | O(n^2) / sweep | Local optimum (deeper) |
| Christofides | Reduction | O(n^2 + k*2^k) | <= 1.5x optimal |
| ILS | Meta-heuristic | O(iter * n^2) | Best of many local optima |
| Simulated Annealing | Meta-heuristic | O(steps) | Probabilistic global |
| Genetic Algorithm | Meta-heuristic | O(gen * pop * n^2) | Population diversity |

## The 5 SubsetSelection solvers

| Algorithm | Paradigm | Notes |
|-----------|----------|-------|
| Greedy | Construction | Sort by value/cost ratio, fill while it fits |
| Dynamic Programming | Exact | 0/1 knapsack DP (scaled by `Precision`, default 100) |
| Branch & Bound | Exact | Fractional-knapsack upper bound |
| Simulated Annealing | Improvement | Flip-bit moves from a Greedy seed |
| Genetic Algorithm | Improvement | Uniform crossover + greedy repair |

## The 4 ConstraintAssignment solvers

| Algorithm | Paradigm | Notes |
|-----------|----------|-------|
| DSatur | Construction | Greedy by saturation degree |
| Backtracking | Exact | Forward-checking search over colour count (n <= 20) |
| Simulated Annealing | Improvement | Single-vertex recolouring from a DSatur seed |
| Genetic Algorithm | Improvement | Uniform crossover + conflict repair |
