@perseus

@prompt
This document was rendered by Perseus for the Generic markdown profile. The resolved
content below reflects the workspace at render time. Avoid re-discovering the
same facts, but verify anything stale, surprising, or load-bearing with live
tools before relying on it — rendered context is a snapshot, not ground truth.
@end

# Workspace Context — @date format="YYYY-MM-DD HH:mm z"

**Profile:** generic

## Project
- **Purpose:** CombinatorialOptimiser is a dependency-free .NET 10 combinatorial optimisation library plus a demo CLI. It solves Permutation (TSP-style), SubsetSelection (0/1 knapsack) and ConstraintAssignment (graph colouring) problems with 20+ solvers across four paradigms (Exact, Construction, Improvement, Reduction), reporting cost, gap-to-optimal and run time.
- **Layout:**
  - `src/CombinatorialOptimiser/` - the packable NuGet library.
  - `src/CombinatorialOptimiser/Core/` - `ISolver<TProblem,TResult>`, `SolverResult`, `Node`, `DistanceMatrix`, `SolverRegistry`; `Core/Metaheuristics/` holds shared SA/GA/ILS base classes.
  - `src/CombinatorialOptimiser/Permutation/` - 11 TSP solvers.
  - `src/CombinatorialOptimiser/SubsetSelection/` - 5 knapsack solvers.
  - `src/CombinatorialOptimiser/ConstraintAssignment/` - 4 graph-colouring solvers.
  - `CombinatorialOptimiser.Cli/` - demo console app.
  - `CombinatorialOptimiser.Tests/` - xUnit tests.
  - `docs/solvers` (per-solver docs), `PROJECT_INFO.md` (architecture), `local-feed/` (local NuGet feed).
- **Stack and build:** .NET 10, C#, xUnit. Build with `dotnet build CombinatorialOptimiser.slnx -c Release` and test with `dotnet test CombinatorialOptimiser.slnx -c Release`. Run the demo with `dotnet run --project CombinatorialOptimiser.Cli -c Release -- --cities 10`. Pack with `dotnet pack src/CombinatorialOptimiser/CombinatorialOptimiser.csproj -c Release`.
- **Conventions and constraints:**
  - `AGENTS.md` is the canonical agent entry; read it first each session instead of re-scanning the tree.
  - Architecture is the Strategy pattern: every algorithm implements `ISolver<TProblem,TResult>`; SA/GA/ILS solvers are thin subclasses of the shared base classes.
  - British spelling in domain language (optimiser, colouring), but `NearestNeighbor` keeps the US spelling; match existing names.
  - Strict build: Nullable on, ImplicitUsings on, `TreatWarningsAsErrors=true`, analyzers at AllEnabledByDefault; read the csproj NoWarn justifications before adding suppressions.
  - Format with `CultureInfo.InvariantCulture`.
  - The permutation domain type is `Node` (not `City`) on purpose.
- **Status:** On branch `main`. Recent commits pack CombinatorialOptimiser 1.0.1 into `local-feed`, fix the CP-SAT solver identity (`ISolver.Name`/`Paradigm`), register `CpSatSolver`, and merge the MCTS and CP-SAT feature branches.

---

## Last Checkpoint
@waypoint ttl=86400

---

## Workspace State

@query "git log --oneline -5 2>/dev/null || echo '(no git repo)'" fallback="git log unavailable"
@query "git status --short 2>/dev/null || true" fallback="clean"

---

## Task Board
@agora status=open,in_progress

---

## Project Memory
@memory focus=recent ttl=300
