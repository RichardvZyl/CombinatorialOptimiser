# Solver roadmap — task list

This is a prioritized task list for adding new algorithms and integrations to the repository. Each item includes a short description, priority, and acceptance criteria.

- [ ] 1. Monte Carlo Tree Search (MCTS)
  - Priority: High
  - Description: Implement a generic MCTS framework for constructive permutation problems (UCT selection, rollout policy, backprop). Provide pluggable rollout policies.
  - Acceptance: MCTS solver class that implements ISolver<DistanceMatrix, PermutationResult>, unit tests showing improved solutions vs greedy on at least two seeded instances, README entry.

- [ ] 2. Constraint Programming (CP-SAT) wrapper
  - Priority: High
  - Description: Add a thin wrapper to call a CP/SAT solver (e.g., via OR-Tools or a small subprocess) to solve constrained permutation instances.
  - Acceptance: Example showing enforcing all-different and capacity constraints; a test that fails without constraints and passes with CP repair; docs and sample CLI usage.

- [ ] 3. Integer Linear Programming (ILP) integration
  - Priority: High
  - Description: Provide an ILP encoder for permutation/TSP and a wrapper to call a solver (e.g., local glpk or other) or accept external solutions. Provide LP relaxation for bounds.
  - Acceptance: ILP encoder, sample solve on small n using a bundled lightweight solver or instructions to use system solver; tests comparing optimality with HeldKarp on small instances.

- [ ] 4. Diverse / Stochastic Beam variants
  - Priority: Medium
  - Description: Implement Diverse Beam Search and stochastic beam variants (nucleus, sampled beam) as alternatives to deterministic BeamSearchSolver.
  - Acceptance: New solvers implementing ISolver, unit tests demonstrating increased diversity and at least one example where diversity yields a better reranked final solution.

- [ ] 5. GNN‑guided constructive policy (learned heuristic)
  - Priority: Medium
  - Description: Add a scaffold for integrating a learned policy (GNN) that predicts next-node probabilities for constructive search; provide an example training loop on synthetic instances.
  - Acceptance: Policy interface, trainer stub, and a small evaluation demonstrating the learned policy outperforms nearest neighbor on held-out synthetic data.

- [ ] 6. Ant Colony Optimization (ACO)
  - Priority: Medium
  - Description: Add ACO implementation with pheromone matrix and candidate lists; suitable for routing benchmarks.
  - Acceptance: ACO solver, tests showing it finds good tours on standard instances and documentation.

- [ ] 7. GRASP (Greedy Randomized Adaptive Search Procedure)
  - Priority: Low
  - Description: Implement GRASP to repeatedly generate randomized greedy constructs and apply local improvement.
  - Acceptance: GRASP solver, unit tests, example pipeline.

- [ ] 8. Variable Neighborhood Search (VNS)
  - Priority: Low
  - Description: Implement VNS with multiple neighborhood types to escape local minima.
  - Acceptance: VNS solver and tests demonstrating improvement over single‑neighborhood local search on at least one instance.

- [ ] 9. Monte Carlo sampling + reranking
  - Priority: Low
  - Description: Sampling-based solver that draws many stochastic tours (from softmax over -cost) and reranks by objective or external metric.
  - Acceptance: Solver and tests showing reranking improves best-of-N results vs naive sampling.

- [ ] 10. Rollout policy + reranker
  - Priority: Low
  - Description: Provide rollout infrastructure where a policy is used to complete partial sequences and a reranker evaluates rollouts for selection.
  - Acceptance: Rollout engine and example combining BeamSearch or GNN policy with rollouts for improved selection.

Notes
- Start with items 1–3 and 4 for best practical value and LLM synergy.
- Each task should include unit tests, a small README under docs/solvers, and at least one CLI example.
