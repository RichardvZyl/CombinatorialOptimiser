# CLAUDE.md — Session Context

Read this first each session instead of re-scanning the whole tree. For deeper
detail see `README.md` (usage + algorithm tables) and `PROJECT_INFO.md`
(architecture + abstractions). This file is the quick orientation.

## What this is

`CombinatorialOptimiser` — a **dependency-free .NET 10** combinatorial
optimisation library (plus a CLI). Solves three problem domains with 20+
classic algorithms across four paradigms (Exact, Construction, Improvement,
Reduction), reporting cost, gap-to-optimal, and run time for comparison.

| Domain | Problem | Solvers |
|--------|---------|---------|
| **Permutation** | TSP-style ordering (minimise tour cost) | 11 — primary focus |
| **SubsetSelection** | 0/1 knapsack (maximise value under capacity) | 5 |
| **ConstraintAssignment** | graph colouring (no conflicting pair shares a label) | 4 |

The Permutation domain type is `Node` (not `City`) on purpose — the same
algorithms serve scheduling, routing, wiring, etc.

## Solution layout (3 projects, `CombinatorialOptimiser.slnx`)

```
src/CombinatorialOptimiser/        # the LIBRARY (OutputType=Library, packable NuGet)
├── Core/                          # ISolver<TProblem,TResult>, ISolver, SolverResult, Node, DistanceMatrix, SolverRegistry
│   └── Metaheuristics/            # shared SA / GA / ILS base classes (domains subclass these)
├── Permutation/                   # 11 TSP solvers
├── SubsetSelection/               # 5 knapsack solvers
└── ConstraintAssignment/          # 4 graph-colouring solvers

CombinatorialOptimiser.Cli/        # console app (OutputType=Exe) — Program.cs, demos + CLI args
CombinatorialOptimiser.Tests/      # xUnit (~62+ tests)
```

## Architecture (Strategy pattern)

- Every algorithm implements `ISolver<TProblem, TResult>`, specialised per
  domain: `ISolver<DistanceMatrix, PermutationResult>`,
  `ISolver<SelectionProblem, SelectionResult>`,
  `ISolver<AssignmentProblem, AssignmentResult>`.
- `ISolver` is the non-generic marker (`Name`, `Paradigm`) for cross-domain
  collections; `SolverParadigm` enum classifies algorithms.
- `Core/SolverRegistry.cs` recommends/filters solvers by problem size and builds
  Christofides-seeded local-search variants.
- SA/GA/ILS solvers are thin subclasses of `Core/Metaheuristics/*Base` that
  supply only domain-specific moves + objective; the search loops are shared.

## Build / test / run

```bash
dotnet build CombinatorialOptimiser.slnx -c Release
dotnet test  CombinatorialOptimiser.slnx -c Release
dotnet run --project CombinatorialOptimiser.Cli -c Release -- --cities 10
dotnet run --project CombinatorialOptimiser.Cli -c Release -- --cities 8 --solver HeldKarp
dotnet pack src/CombinatorialOptimiser/CombinatorialOptimiser.csproj -c Release
```
The library lives in `src/CombinatorialOptimiser/` and has no `OutputType=Exe`
— run the CLI project for the demo app.

## Conventions / gotchas

- **British spelling** in domain language: *optimiser, colouring* (but
  `NearestNeighbor` keeps the US spelling — match existing names when editing).
- Strict build: `Nullable` enabled, `ImplicitUsings` on,
  `TreatWarningsAsErrors=true`, analyzers at `AllEnabledByDefault`. A handful of
  CA rules are suppressed via `NoWarn` with justifying comments in the csproj —
  read those before adding new suppressions.
- Always format with `CultureInfo.InvariantCulture` (CA1303/localisation is
  intentionally out of scope).
- Library exposes internals to Tests + Cli via `InternalsVisibleTo`.
- Multidimensional arrays (cost/conflict/DP tables) are intentional for cache
  locality — don't "fix" them to jagged.

## Current state (as of 2026-06-14)

Restructure to **NuGet-packable library + separate CLI** is complete and
green: library lives in `src/CombinatorialOptimiser/`, `dotnet build`/`test`/
`pack` all succeed, `LICENSE` is MIT (matches `PackageLicenseExpression`),
`GenerateDocumentationFile` is on with XML docs covering the public API, and
`Solve(problem, ct)`/`SolveAsync` cancellation is wired through the SA/GA/ILS
solvers.

The library is a general-purpose, domain-agnostic optimisation package — it
carries no knowledge of any consuming application. Keep it that way: anything
about *how* or *why* it's consumed belongs in the consuming code, not here.
