# CombinatorialOptimiser

A dependency-free .NET 10 console app that solves combinatorial optimisation
problems across three domains: **Permutation** (TSP-style ordering),
**SubsetSelection** (0/1 knapsack), and **ConstraintAssignment** (graph
colouring). The Permutation domain is the primary CLI focus and provides
**eleven classic algorithms** across **four paradigms**, printing distance,
gap-to-optimal, and run time for side-by-side comparison.

## Quick start

```bash
dotnet run -c Release                          # interactive prompt + demos
dotnet run -c Release -- --cities 10          # 10 random cities, all solvers
dotnet run -c Release -- --cities 8 --solver HeldKarp  # single solver
```

## Run the tests

```bash
dotnet test CombinatorialOptimiser.slnx -c Release
```

## Project layout

```
CombinatorialOptimiser/
├── Program.cs                    # entry point: CLI args, interactive prompt, demo datasets (Permutation)
├── Core/                          # shared model: ISolver<TProblem,TResult>, SolverResultBase, Node, DistanceMatrix
│   └── Metaheuristics/             # shared SA / GA / ILS base classes
├── Permutation/                   # 11 solver implementations (TSP-style)
├── SubsetSelection/                # 0/1 knapsack: 5 solvers
├── ConstraintAssignment/            # graph colouring: 4 solvers
└── CombinatorialOptimiser.Tests/  # xUnit test project (62 tests)
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
