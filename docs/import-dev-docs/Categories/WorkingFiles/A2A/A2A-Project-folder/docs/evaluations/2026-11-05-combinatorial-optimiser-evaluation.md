> Source: E:\Development\Projects\A2A Project folder\docs\evaluations\2026-11-05-combinatorial-optimiser-evaluation.md | Branch: main | Commit: 12c1a42 2026-11-05 | Status: committed on main | Copied: 2026-11-06 07:57 SAST (working copy - edit the source, not this file)

# Evaluation: `CombinatorialOptimiser` (`PermutationOptimiser`) in the A2A & Agentic Development Harness

* **Date:** 2026-11-05  
* **Status:** Evaluation from repository Markdown documentation only (per owner instruction; no source `.cs` files read, no code or git state modified)  
* **Repository Evaluated:** `RichardvZyl/CombinatorialOptimiser` (local folder `PermutationOptimiser/root`, mapped in [AGENTS.md](../../AGENTS.md) and [Executable DAG Plan v4.2](../agent_harness_dag_plan_v4.2.md) to `D19`)  
* **Markdown Sources Read:** `AGENTS.md`, `README.md`, `PROJECT_INFO.md`, `live-context.md`, `.perseus/context.md`, `.agents/reconcile.md`, `docs/SolversOverview.md`, `docs/solver-roadmap-tasks.md`, and `docs/solvers/*.md` (`MCTS.md`, `CP-SAT-Wrapper.md`, `BeamSearchSolver.md`, `GeneticAlgorithmSolver.md`, `SimulatedAnnealingSolver.md`, `IteratedLocalSearchSolver.md`)

---

## 1. Executive Summary & Verdict

> **Verdict:** **`CombinatorialOptimiser` is a high-leverage, zero-dependency .NET 10 asset that fits FOUR distinct deterministic optimisation problems across the A2A harness — not only `D19` routing.**  
>  
> Because it is a pure, dependency-free `.NET 10` NuGet library (`MIT`, strict analyzers, cancellation-aware `SolveAsync`, generic `Node` / Strategy pattern), it drops directly into `agent-harness` ([ADR 0002](../adr/0002-harness-code-in-agent-harness-repo.md)) and `OrchestrationSystem` (`Q00–Q07`) to replace LLM guesswork with provable or bounded-time combinatorial math (`§2.4 Deterministic-first execution`).  
>  
> **Key Gap Identified for `D19`:** While its current **3 domains** (`Permutation`, `SubsetSelection`, `ConstraintAssignment`) directly solve **sequence/ladder ordering**, **token-budgeted context packing (`D12.9`)**, and **conflict-free parallel wave / provider-independence scheduling (`D10.5`/`D11`)**, multi-task **quota-constrained provider assignment (`D19`)** is mathematically a **Generalized Assignment / Bipartite Matching** problem. Adding a 4th domain (`ResourceAllocation` / `GeneralizedAssignment`) on top of the existing `Core/Metaheuristics/*Base` (`SA`, `GA`, `ILS`) and exact solvers will make it a complete engine for `D19`.

---

## 2. What `CombinatorialOptimiser` Currently Does (From Its `.md` Documentation)

### 2.1 Architecture & Engineering Posture
* **Stack:** Dependency-free `.NET 10` library (`src/CombinatorialOptimiser/`, packed to `local-feed/` as `v1.0.1`) + demo CLI (`CombinatorialOptimiser.Cli/`) + xUnit suite (`CombinatorialOptimiser.Tests/`, `62+` tests).
* **Build Quality:** `Nullable` enabled, `ImplicitUsings` on, `TreatWarningsAsErrors=true`, .NET analyzers at `AllEnabledByDefault`, XML documentation generation on, `CultureInfo.InvariantCulture` enforced, and `CancellationToken` wired through `Solve(problem, ct)` / `SolveAsync`.
* **Core Pattern (Strategy + Shared Metaheuristic Base):**
  * Every solver implements `ISolver<TProblem, TResult>` (plus non-generic `ISolver` exposing `Name` and `SolverParadigm`: `Exact`, `Construction`, `Improvement`, `Reduction`).
  * `SolverRegistry` recommends and enumerates solvers by problem size $n$ (`RecommendPermutation`, `RecommendSelection`, `RecommendAssignment`, `AllPermutationSolvers`) and wires Christofides-seeded local-search variants.
  * `Core/Metaheuristics/` provides generic `SimulatedAnnealing<TProblem, TSolution>`, `GeneticAlgorithm<TProblem, TChromosome>`, and `IteratedLocalSearch<TProblem, TSolution>` base classes so domain solvers only define domain-specific moves and objective functions.
  * Multidimensional arrays (`[,]`) are used intentionally for CPU cache locality in cost, conflict, and dynamic-programming tables.
  * **Strict Domain Agnosticism (`AGENTS.md`):** Uses `Node` (not `City`) and carries zero coupling to any consuming application.

### 2.2 The Three Implemented Problem Domains & 20+ Solvers

| Domain & Contract | Problem Formulation | Implemented Solvers (per `README.md`, `SolversOverview.md`, `live-context.md`) | Practical Scale ($n$) |
|---|---|---|---|
| **1. `Permutation`**<br/>`ISolver<DistanceMatrix, PermutationResult>` | Order $n$ nodes to minimise total transition cost (symmetric or asymmetric `DistanceMatrix`). | • **Exact:** `BruteForce` ($O(n!)$), `RecursiveBruteForce`, `BranchAndBound`, `HeldKarp` DP ($O(2^n n^2)$), `CpSatSolver` (CP-SAT wrapper)<br/>• **Construction:** `NearestNeighbor` ($O(n^2)$), `BeamSearchSolver` ($O(\text{beam}\cdot n^2)$ with softmax temperature over $-\text{cost}$)<br/>• **Reduction:** `Christofides` ($\le 1.5\times$ metric optimal)<br/>• **Improvement:** `TwoOpt` ($O(n^2)$), `ThreeOpt` ($O(n^3)$), `LinKernighan`<br/>• **Metaheuristics / Tree Search:** `IteratedLocalSearch`, `SimulatedAnnealing`, `GeneticAlgorithm`, `MCTS` (UCT + rollouts) | • Exact: $n \le 10$ (BF), $n \le 14$ (B&B), $n \le 20$ (Held-Karp), $n \le 100$ (CP-SAT)<br/>• Heuristics / Metaheuristics: $n$ up to hundreds or thousands |
| **2. `SubsetSelection`**<br/>`ISolver<SelectionProblem, SelectionResult>` | **0/1 Knapsack:** Select a subset of items to maximise `TotalValue` subject to `TotalCost <= Capacity`. | • **Construction:** `Greedy` (value/cost ratio)<br/>• **Exact:** `DynamicProgramming` (scaled by `Precision`, default 100), `BranchAndBound` (fractional-knapsack upper bound)<br/>• **Improvement / Metaheuristics:** `SimulatedAnnealing` (bit-flip from Greedy seed), `GeneticAlgorithm` (uniform crossover + greedy repair) | Exact up to hundreds/thousands of items (depending on capacity precision); SA/GA for very large sets |
| **3. `ConstraintAssignment`**<br/>`ISolver<AssignmentProblem, AssignmentResult>` | **Graph Colouring:** Assign labels (`Labels`) to `Entities` so no pair in `Conflicts` shares a label, minimising `LabelCount`. | • **Construction:** `DSatur` (greedy by saturation degree)<br/>• **Exact:** `Backtracking` (forward-checking search, $n \le 20$)<br/>• **Improvement / Metaheuristics:** `SimulatedAnnealing` (single-vertex recolouring from DSatur seed), `GeneticAlgorithm` (uniform crossover + conflict repair) | Exact for $n \le 20$; DSatur + SA/GA for hundreds of entities |

### 2.3 In-Flight / Parked Roadmap Branches (`.agents/reconcile.md` & `docs/solver-roadmap-tasks.md`)
* `live-context.md` notes that `MCTS` and `CpSatSolver` were merged to `main` and packed in `1.0.1`.
* `.agents/reconcile.md` (probed 2026-10-22) identifies 9 chained feature branches (`feature/add-aco`, `feature/add-cp-sat`, `feature/add-diverse-beam`, `feature/add-gnn-policy`, `feature/add-grasp`, `feature/add-ilp`, `feature/add-mc-sampling`, `feature/add-rollout`, `feature/add-vns`) plus `wip/permutation-optimiser-loose-state` (`8e57303`, holding 33 loose files) and 7 empty `0`-byte solver doc files in `docs/solvers/` (`Ant-Colony-Optimization.md`, `Diverse-Beam-Search.md`, `GNN-Guided-Policy.md`, `GRASP.md`, `ILP-Integration.md`, `Monte-Carlo-Sampling.md`, `Rollout-Policy-Reranker.md`, `Variable-Neighborhood-Search.md`).

---

## 3. Evaluation Against the Planned A2A & Harness Architecture

In [Executable DAG Plan v4.2](../agent_harness_dag_plan_v4.2.md), `CombinatorialOptimiser` is currently listed only under **`D19` (Intelligent Routing Optimisation)**. However, inspecting its three domains reveals **four immediate, high-value applications** across `D10`, `D11`, `D12.9`, `D19`, and `Q04`:

### 3.1 Application 1: `SubsetSelection` (0/1 Knapsack) -> `D12.9` Context Envelope & Token-Budget Packing (Ready Today)
* **The Harness Problem (`D12.9 ah-context-envelope-builder` & `plan-v3 §5`):**  
  At session start or during `memory_recall`, the harness retrieves $N$ candidate memories and LCM summary nodes. Each candidate $i$ has:
  - A **Value ($v_i$):** Fused RRF hybrid score $\times$ freshness (`last_verified`) $\times$ `source_trust` $\times$ project/lane match weight.
  - A **Cost ($c_i$):** Token count of the summary/snippet.
  - A **Capacity ($W$):** Hard token budget for Tier 2 / Tier 3 (`available_context` ceiling).
* **Why `SubsetSelection` Beats Greedy Truncation:**  
  Simple top-$k$ truncation stops as soon as one large memory overflows the remaining token budget, leaving unused capacity or dropping two high-value compact memories in favour of one verbose one. `CombinatorialOptimiser`'s `SubsetSelection` (`DynamicProgramming` or `BranchAndBound`) solves the exact 0/1 Knapsack problem in sub-millisecond time in `.NET`, maximising total relevance density inside the token envelope while remaining 100% deterministic (`ZeroEntropy`).

### 3.2 Application 2: `ConstraintAssignment` (Graph Colouring) -> `D10.5` Parallel Wave Scheduling & `D11` Provider-Independence Assignment (Ready Today)
* **The Harness Problem A — Conflict-Free Parallel Execution Waves (`D10.5`, `D13`):**  
  When the orchestrator decomposes work into $N$ tasks ($N \le 20$ is typical for a feature DAG), pairs of tasks that mutate overlapping files/worktrees or require the same single-flight CLI peer (`Shim :4010` enforces `1` in-flight per peer) have a **conflict edge**.
  - Feeding `(Entities = Tasks, Conflicts = OverlappingResourcePairs)` into `ConstraintAssignment` (`Backtracking` for $N \le 20$, `DSatur + SA` for $N > 20$) partitions the DAG into the **minimum number of conflict-free parallel waves (`LabelCount`)**.
* **The Harness Problem B — Builder / Tester / Reviewer Independence (`D11.4`, `D11.5`, `§2.8 Provider independence`):**  
  Your plan requires that the `Tester` and `Reviewer` use a different provider/model from the `Builder` where practical. Modelling `(Builder, Tester)` and `(Builder, Reviewer)` as conflict edges in `AssignmentProblem` guarantees that connected verification stages are assigned distinct provider labels.

### 3.3 Application 3: `Permutation` (`DistanceMatrix` + `BeamSearch` + `MCTS` + `CP-SAT`) -> `D10.4` / `D19` Escalation Ladders & Batch Sequencing (Ready Today)
* **The Harness Problem A — Minimising Context-Switch / Warmup Cost Across a Task Queue:**  
  When a worker or batch runner executes $n$ queued tasks sequentially, transitioning from task $i$ to task $j$ incurs a setup cost $d(i, j)$ (e.g., `0` if same repo and worktree, higher if switching repos, rebuilding containers, or switching Switchyard route pins `D19.8`). `SolverRegistry.RecommendPermutation(n)` (`HeldKarp` for $n \le 16$, `LinKernighan` / `ILS` for larger batches) finds the minimum-overhead execution order.
* **The Harness Problem B — Fallback Ladder & Plan Search (`ADR 0003`, `Switchboard :8096`, `Q04`):**  
  - `BeamSearchSolver` (which converts costs into transition log-probabilities $p(j\mid i) \propto \exp(-\text{cost}(i,j)/T)$) and `MCTS` + `CpSatSolver` directly support **deterministic capability composition (`Q04`)** and **plan validation (`D10.6`)**: when an LLM proposes candidate capability sequences, `CpSatSolver` can repair/filter sequences that violate hard precedence or capability-scope invariants.

### 3.4 Application 4: `D19` Quota-Aware Multi-Agent Routing — What Fits Today vs. What to Add

In `D19` ([Executable DAG Plan v4.2 §25](../agent_harness_dag_plan_v4.2.md#25-d19--intelligent-routing-optimisation)), the router scores eligible workers across 11 features (`capability fit`, `repo familiarity`, `historical success`, `first-pass test rate`, `review pass rate`, `security escape rate`, `latency`, `cost`, `context size`, `provider reliability`, `current load`) plus live quota headroom from `provider-usage-mcp :3061` ([ADR 0005](../docs/adr/0005-provider-availability-assume-until-recorded-limit.md)).

| `D19` Routing Scenario | Mathematical Formulation | Does `CombinatorialOptimiser` Solve It Today? | Recommended Action |
|---|---|---|---|
| **1. Single-task fallback ladder ordering (`Switchboard :8096` 0–5 rungs)** | Rank / order $k$ eligible peers considering transition penalties and failure correlations. | **Yes** (`Permutation`: `HeldKarp` / `BeamSearchSolver` for small $k$). | Use `Permutation` directly once step-to-step transition costs are defined. |
| **2. Choosing the best subset of peers for bounded `@all` fan-out (`concurrency = 3`)** | Pick $k=3$ diverse, high-utility peers under a total cost/quota budget. | **Yes** (`SubsetSelection`: 0/1 Knapsack under cost/quota budget). | Use `SubsetSelection` (`DynamicProgramming` / `BranchAndBound`). |
| **3. Assigning $M$ concurrent tasks across $K$ heterogeneous agents with quota caps (`D19` core)** | **Generalized Assignment Problem (GAP) / Min-Cost Bipartite Matching:** Each task $m \in \{1..M\}$ has a distinct cost/utility $c_{m,k}$ on provider $k \in \{1..K\}$, and each provider $k$ has remaining quota/concurrency capacity $Q_k$ from `provider-usage-mcp`. | **Partial (Not a native domain yet).** `Permutation` assumes a single tour visiting all nodes; `ConstraintAssignment` assigns unweighted colours without per-agent utility matrices $c_{m,k}$ or per-colour capacity caps $Q_k$. | **Add a 4th domain (`ResourceAllocation` / `GeneralizedAssignment`)** in `src/CombinatorialOptimiser/` implementing `ISolver<AllocationProblem, AllocationResult>`. |

---

## 4. Recommended Enhancements to `CombinatorialOptimiser` (Keeping Domain Agnosticism Intact)

Per `AGENTS.md` in `PermutationOptimiser/root`, the library must remain a **general-purpose, domain-agnostic optimisation package** with zero references to A2A, providers, or LLMs. All enhancements below preserve that boundary:

1. **Add Domain 4: `GeneralizedAssignment` (or `BipartiteAllocation`) for `D19`:**
   * **Problem Record (`AllocationProblem`):**
     - `CostMatrix[taskIndex, agentIndex]` (rectangular $M \times K$ `double[,]` matrix for cache locality, matching `DistanceMatrix` conventions).
     - `ResourceConsumption[taskIndex, agentIndex]` (or per-task weight) and `AgentCapacities[agentIndex]` (quota/concurrency ceiling per target).
   * **Solvers (`ISolver<AllocationProblem, AllocationResult>`):**
     - **Exact (`HungarianSolver` / `MinCostFlowSolver` or `BranchAndBound`):** Polynomial $O(N^3)$ exact assignment when capacities are unit/integer slots.
     - **Metaheuristics (`SimulatedAnnealing`, `GeneticAlgorithm`, `IteratedLocalSearch`):** Thin subclasses of your existing `Core/Metaheuristics/*Base` classes using task-reassignment and swap moves when capacities are fractional/weighted (Generalized Assignment Problem).
   * **Why this is high value:** It turns `CombinatorialOptimiser` into the exact mathematical solver needed by `D19` and `Switchboard :8096` without leaking any A2A concepts into the library.
2. **Enforce Deterministic Seed Option (`Seed` parameter) on Stochastic Solvers:**
   * For `SimulatedAnnealing`, `GeneticAlgorithm`, `IteratedLocalSearch`, and `MCTS`, ensure every solver constructor/options record accepts an optional deterministic RNG seed (`int? seed`). When invoked inside `agent-harness` (`D19`) or `OrchestrationSystem` (`Q02` `ZeroEntropy` admission gate), passing a seed derived from the ledger `run_id`/`event_hash` makes even metaheuristic routing decisions **100% reproducible on ledger replay (`D03`)**.
3. **Reconcile the 9 Parked Solver Feature Branches (`.agents/reconcile.md`):**
   * Decide whether to rebase/land the parked solver stack (`ACO`, `GRASP`, `VNS`, `ILP`, `DiverseBeam`, `MonteCarloSampling`, `Rollout`) and populate or remove the 0-byte `docs/solvers/*.md` placeholder files, then pack `CombinatorialOptimiser 1.1.0` into `local-feed/`.
