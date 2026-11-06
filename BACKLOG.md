# CombinatorialOptimiser — Backlog & Roadmap

> Companion to [`docs/solver-roadmap-tasks.md`](docs/solver-roadmap-tasks.md) and [`.agents/reconcile.md`](.agents/reconcile.md).
> Per [`AGENTS.md`](AGENTS.md), `CombinatorialOptimiser` is a general-purpose, domain-agnostic `.NET 10` optimisation library and carries no knowledge of any consuming application.

---

## Priority 1 — New Domain: `GeneralizedAssignment` / `ResourceAllocation`

Add a fourth problem domain under `src/CombinatorialOptimiser/ResourceAllocation/` (or `GeneralizedAssignment/`) to solve rectangular $M \times K$ task-to-agent / item-to-bin assignment under capacity constraints, alongside the existing `Permutation`, `SubsetSelection`, and `ConstraintAssignment` domains.

- [ ] **P1.1 Define problem and result contracts (`Core/` & `ResourceAllocation/`):**
  - `AllocationProblem`:
    - `double[,] CostMatrix` ($M$ tasks/items $\times$ $K$ targets/resources; multidimensional array for cache locality).
    - `double[,] ResourceConsumption` ($M \times K$ resource usage per `(item, target)` pair, with a helper constructor for uniform per-item weights `double[]`).
    - `double[] Capacities` ($K$ target capacity ceilings).
    - `bool Maximise` (or standardise on minimising cost / negating utility).
  - `AllocationResult` (inheriting `SolverResult`):
    - `int[] Assignments` (length $M$, mapping each item $m \in [0..M-1]$ to target $k \in [0..K-1]$, or `-1` if unassigned).
    - `double TotalCost`, `double[] TargetLoads`, `bool IsFeasible`.
  - Specialise `ISolver<AllocationProblem, AllocationResult>`.
- [ ] **P1.2 Exact Solvers (`SolverParadigm.Exact`):**
  - `HungarianSolver`: $O(N^3)$ Kuhn–Munkres / Jonker–Volgenant algorithm for 1-to-1 and unit-capacity bipartite minimum-cost assignment.
  - `BranchAndBoundAllocationSolver`: Exact depth-first branch-and-bound with linear relaxation / greedy lower bound for general weighted capacities ($M \le 30$).
- [ ] **P1.3 Construction Solver (`SolverParadigm.Construction`):**
  - `GreedyAllocationSolver`: Assigns items in decreasing order of regret (difference between best and second-best feasible target cost) or cost-to-weight ratio.
- [ ] **P1.4 Metaheuristic Solvers (`SolverParadigm.Improvement`):**
  - Subclass `Core/Metaheuristics/SimulatedAnnealing<AllocationProblem, int[]>`, `GeneticAlgorithm<AllocationProblem, int[]>`, and `IteratedLocalSearch<AllocationProblem, int[]>` using single-item reassignment and two-item target-swap moves with capacity-feasibility repair.
- [ ] **P1.5 Registry & CLI Integration:**
  - Add `SolverRegistry.RecommendAllocation(int itemCount, int targetCount)` and `SolverRegistry.AllAllocationSolvers()`.
  - Add `--domain allocation` demo and flags to `CombinatorialOptimiser.Cli`.
  - Add xUnit test coverage in `CombinatorialOptimiser.Tests` verifying optimality of `HungarianSolver` and `BranchAndBoundAllocationSolver` against brute-force baselines on small instances and feasibility of SA/GA/ILS on larger instances.

---

## Priority 2 — Deterministic RNG Seeding (`ZeroEntropy` Replay Support)

Ensure all stochastic and sampling-based solvers are 100% deterministic when supplied with an explicit seed so downstream callers can replay runs bit-for-bit.

- [ ] **P2.1 Explicit `int? Seed` parameter on metaheuristic base classes and options:**
  - Thread an optional `int? seed = null` (or `Random` instance created from `new Random(seed.Value)`) through `SimulatedAnnealing`, `GeneticAlgorithm`, `IteratedLocalSearch`, `MctsSolver`, and `BeamSearchSolver` (when stochastic sampling is enabled).
  - Never rely on ambient `Random.Shared` when a deterministic seed is provided.
- [ ] **P2.2 Determinism xUnit tests:**
  - Add tests in `CombinatorialOptimiser.Tests` asserting that two invocations of `SimulatedAnnealingSolver`, `GeneticAlgorithmSolver`, `IteratedLocalSearchSolver`, and `MctsSolver` with the same `seed` produce identical `Order` / `Selected` / `Labels` / `Assignments` and identical `Distance` / `TotalValue` / `LabelCount`.

---

## Priority 3 — Reconcile Parked Solver Branches & Documentation Hygiene

Per [`.agents/reconcile.md`](.agents/reconcile.md) and [`docs/solver-roadmap-tasks.md`](docs/solver-roadmap-tasks.md):

- [ ] **P3.1 Land or retire the 9 parked solver feature branches:**
  - Review and rebase onto `main` (or retire if superseded):
    - `feature/add-aco` (Ant Colony Optimisation)
    - `feature/add-cp-sat` (verify any remaining commits not yet in `main`)
    - `feature/add-diverse-beam` (Diverse / Stochastic Beam Search)
    - `feature/add-gnn-policy` (GNN-guided constructive policy scaffold)
    - `feature/add-grasp` (GRASP)
    - `feature/add-ilp` (ILP encoder + wrapper)
    - `feature/add-mc-sampling` (Monte Carlo sampling + reranking)
    - `feature/add-rollout` (Rollout policy + reranker)
    - `feature/add-vns` (Variable Neighbourhood Search)
  - Land `chore/agent-entry-files` and prune duplicate backup branch `backup-134516` once verified.
- [ ] **P3.2 Populate or remove 0-byte solver documentation files under `docs/solvers/`:**
  - Complete using `docs/solvers/TEMPLATE.md` (or remove until the corresponding branch merges):
    - `docs/solvers/Ant-Colony-Optimization.md`
    - `docs/solvers/Diverse-Beam-Search.md`
    - `docs/solvers/GNN-Guided-Policy.md`
    - `docs/solvers/GRASP.md`
    - `docs/solvers/ILP-Integration.md`
    - `docs/solvers/Monte-Carlo-Sampling.md`
    - `docs/solvers/Rollout-Policy-Reranker.md`
    - `docs/solvers/Variable-Neighbourhood-Search.md`
- [ ] **P3.3 Pack & Publish `v1.1.0` to `local-feed/`:**
  - Run `dotnet build CombinatorialOptimiser.slnx -c Release`, `dotnet test CombinatorialOptimiser.slnx -c Release`, and `dotnet pack src/CombinatorialOptimiser/CombinatorialOptimiser.csproj -c Release -o local-feed` once P1–P3 are complete.
