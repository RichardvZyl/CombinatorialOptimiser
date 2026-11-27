# PermutationOptimiser — Todo

Solver roadmap (source: PermutationOptimiser-solver-roadmap.md). Each task needs unit tests, a `docs/solvers` README entry, and one CLI example.

## High priority (start here)
- [P1] [L] Implement generic MCTS framework (UCT selection, pluggable rollout policies, backprop) as `ISolver<DistanceMatrix, PermutationResult>`; must beat greedy on 2+ seeded instances
- [P1] [M] CP-SAT wrapper (OR-Tools or subprocess): all-different + capacity constraint example; test that fails without constraints and passes with CP repair
- [P1] [L] ILP integration: permutation/TSP encoder + solver wrapper (glpk or external); LP relaxation for bounds; optimality tests vs HeldKarp on small instances

## Medium priority
- [P2] [M] Diverse/stochastic beam variants (nucleus, sampled beam) vs deterministic BeamSearchSolver; demonstrate diversity win on reranked solution
- [P2] [L] GNN-guided constructive policy scaffold: policy interface, trainer stub, eval showing it beats nearest neighbour on held-out synthetic data
- [P2] [M] Ant Colony Optimization with pheromone matrix + candidate lists

## Low priority
- [P3] [M] GRASP: randomized greedy constructs + local improvement loop
- [P3] [M] Variable Neighborhood Search with multiple neighborhood types
- [P3] [S] Monte Carlo sampling + reranking solver (softmax over -cost, best-of-N)
- [P3] [M] Rollout policy + reranker infrastructure (combine BeamSearch or GNN policy with rollouts)
