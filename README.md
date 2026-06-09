# Travelling Salesman -- POC

A dependency-free .NET 10 console app that solves the Travelling Salesman
Problem (TSP) using **twelve classic algorithms** across **four paradigms**
and prints distance, gap-to-optimal, and run time for side-by-side comparison.

## Quick start

```bash
dotnet run -c Release                          # interactive prompt + demos
dotnet run -c Release -- --cities 10          # 10 random cities, all solvers
dotnet run -c Release -- --cities 8 --solver HeldKarp  # single solver
```

## Run the tests

```bash
dotnet test PermutationOptimiser.slnx -c Release
```

## Project layout

```
PermutationOptimiser/
├── Program.cs            # entry point: CLI args, interactive prompt, demo datasets
├── Model/                # Node, DistanceMatrix, ISolver, SolverResult
├── Algorithms/           # 12 solver implementations
└── PermutationOptimiser.Tests/   # xUnit test project
```

## The 12 solvers

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
