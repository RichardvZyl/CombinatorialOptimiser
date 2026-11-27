# Solver overview and real-world mappings

This document briefly explains what the project does and how each solver in SolverRegistry can be used in practice. The goal is to help a reader understand why these algorithms exist in the codebase and how they map to plausible real‑world use cases (three most probable when answers differ).

What this solution does
- Provides a catalogue of algorithms for permutation/TSP‑style problems (exact, constructive, and improvement heuristics).
- Exposes utilities for Euclidean and arbitrary cost matrices and a CLI to run and compare solvers.
- Includes a BeamSearch heuristic that mirrors the idea of guided decoding used in sequence models (LLMs) by treating -cost as a transition score.

Why it exists
- Benchmarking: compare algorithms across problem sizes to pick the right tool for a workload.
- Education and experimentation: a compact library to learn about exact algorithms (Held‑Karp, branch‑and‑bound) and heuristics (2‑opt, Lin‑Kernighan).
- Practical use: provide off‑the‑shelf solvers for routing, scheduling, assembly sequencing, and other permutation optimization tasks.

Per‑algorithm role and probable real‑world uses

- BruteForceSolver (Exact)
  - Role: Exhaustive enumeration of all tours; guaranteed optimal but factorial time.
  - Top 3 use cases: unit tests / correctness baselines, tiny production tasks (n ≤ 10), educational demonstrations.

- RecursiveBruteForceSolver (Exact)
  - Role: Recursive variant of brute force; same complexity, different implementation characteristics.
  - Top 3 use cases: correctness checks, recursion/stack behavior experiments, pedagogical examples.

- BranchAndBoundSolver (Exact / Pruned search)
  - Role: Systematically explores search tree while pruning using bounds; can solve larger instances than brute force in practice.
  - Top 3 use cases: small–medium routing tasks where optimality matters, test harness for heuristic quality, constrained scheduling with pruning-friendly bounds.

- HeldKarpSolver (Exact / DP)
  - Role: Dynamic programming exact solver (O(n^2 2^n))—practical up to ~16–20 nodes.
  - Top 3 use cases: medium‑sized instances where optimality is required (e.g., toolpath optimization for CNC with limited points), benchmarking optimal baselines, research experiments.

- NearestNeighborSolver (Construction / Greedy)
  - Role: Fast greedy construction; good initial solution, cheap runtime.
  - Top 3 use cases: quick approximate scheduling, seed for local improvement (2‑opt), low‑latency routing hints in interactive systems.

- BeamSearchSolver (Construction / Heuristic)
  - Role: Beam search guided by transition log‑probabilities (derived from costs). Mirrors LLM beam decoding: keeps k best partial sequences.
  - Top 3 use cases:
	1. Guided construction when you want a balance between greedy and exhaustive search (e.g., near‑real‑time sequencing where quality matters).
	2. As an analogue to LLM decoding for tasks that combine learned transition scores with combinatorial constraints (re‑ranking, constrained generation).
	3. Educational bridge between probabilistic decoding and combinatorial optimization (compare beam width / temperature effects).

- ChristofidesSolver (Reduction / Approximation)
  - Role: Metric TSP 1.5‑approximation (polynomial time) — useful when provable worst‑case bounds matter.
  - Top 3 use cases: large routing problems where approximation guarantee is acceptable, initial seed for local search, real‑time systems needing predictable worst‑case quality.

- TwoOptSolver, ThreeOptSolver (Improvement / Local search)
  - Role: Iteratively improve a given tour by exchanging edges; inexpensive and very effective in practice.
  - Top 3 use cases: post‑processing of constructed tours (e.g., from greedy or beam), iterative route refinement in logistics, fast on‑device improvement for mobile routing apps.

- LinKernighanSolver (Improvement / Powerful local search)
  - Role: State‑of‑the‑art local search, often produces near‑optimal tours for large instances.
  - Top 3 use cases: production route optimization at scale, backend batch optimization for delivery/logistics, component in hybrid pipelines (seed + LK + refinement).

- IteratedLocalSearchSolver, SimulatedAnnealingSolver (Improvement / Metaheuristics)
  - Role: Stochastic search strategies to escape local minima; configurable for time/quality tradeoffs.
  - Top 3 use cases: large combinatorial instances where exact methods are infeasible, scenarios that benefit from multiple restarts (robust scheduling), optimization under noisy/uncertain costs.

- GeneticAlgorithmSolver (Improvement / Population search)
  - Role: Population‑based search mixing and mutating candidate tours; parallelizable and flexible for hybrid objectives.
  - Top 3 use cases: multi‑objective scheduling, problems with custom crossover/repair rules (e.g., domain constraints), experimental pipelines combining learning and search.


Notes on LLM analogies
- Beam search maps directly to LLM decoding; several other concepts (reranking, sampling, temperature) have close analogues.
- Most exact combinatorial algorithms do not run inside token‑level LLM decoders — they are applied offline or as post‑processors for combinatorial tasks.


Complexity & practical limits

| Algorithm | Rough time complexity | Typical workable n |
|-----------|-----------------------:|------------------:|
| BruteForce / RecursiveBruteForce | O(n!) | n ≤ 10
| Branch & Bound | O(n!) worst, often much better with pruning | n ≤ 12–14 (problem dependent)
| Held‑Karp (DP) | O(2^n * n^2) | n ≤ 16–20
| Nearest Neighbor | O(n^2) | n up to thousands (fast)
| Beam Search (heuristic) | O(beam * n^2) typical | n up to hundreds (beam small)
| Christofides | O(n^2 + matching) | n up to thousands
| 2‑opt / 3‑opt | O(n^2) / O(n^3) per sweep | n up to thousands (2‑opt especially)
| Lin‑Kernighan | Practical near O(n^2) per improvement sweep | n up to tens of thousands (with engineering)
| ILS / SA / GA (metaheuristics) | O(iter * n^2) or O(gen * pop * n^2) | n up to thousands; scale depends on budget

These are conservative practical guidelines — actual limits depend on implementation details, time budget, and hardware.
