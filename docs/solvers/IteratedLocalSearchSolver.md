# IteratedLocalSearchSolver

Description
- Metaheuristic that alternates local search with perturbations (restarts or shakes) to escape local minima and explore diverse basins.

Complexity
- Time: O(iter * cost_of_local_search) where local search is typically 2‑opt/3‑opt.
- Space: O(n) plus any bookkeeping for best solutions.

Typical practical limits
- Workable n: hundreds to thousands depending on iterations and local search complexity.

Parameters / options
- Iteration budget, perturbation strength, seed strategy.

Top use cases
1. Robust optimization when single local search is insufficient.
2. Time‑budgeted production optimization with multiple restarts.
3. Hybrid pipelines combining constructive heuristics and stochastic diversification.

CLI example
- dotnet run -- --solver IteratedLocalSearch
