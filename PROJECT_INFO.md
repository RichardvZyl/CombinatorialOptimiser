# PermutationOptimiser -- Codebase Overview

## Summary
A C# .NET console application that solves the Traveling Salesman Problem (TSP)
using **twelve algorithm variants** across four paradigms.

> **Generic model**: The domain type is `Node` (not `City`) because the same
> algorithms apply to scheduling, wiring, network routing, and any other
> permutation optimisation problem.

## Architecture
**Pattern**: Strategy -- each algorithm implements `ISolver`, and `Program.cs`
dispatches to all of them. Solvers are classified by `SolverParadigm` enum.

## Key Abstractions
- **ISolver**: Common contract with `Name`, `Paradigm`, `Solve(DistanceMatrix)`
- **SolverParadigm**: Exact, Construction, Improvement, Reduction
- **DistanceMatrix**: Precomputed cost lookup, Euclidean or raw matrix
- **SolverResult**: Immutable result with Algorithm, Paradigm, Order, Distance, Elapsed
