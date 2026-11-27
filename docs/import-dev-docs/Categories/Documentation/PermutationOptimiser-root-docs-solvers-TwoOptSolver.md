# TwoOptSolver

Description
- Local improvement heuristic that iteratively removes two edges and reconnects the tour to reduce length (2‑opt swap).

Complexity
- Time: O(n^2) per improvement sweep; running multiple sweeps increases time linearly.
- Space: O(n).

Typical practical limits
- Workable n: thousands; fast in practice and commonly used as a refinement step.

Parameters / options
- Seed tour: accepts an initial tour to improve (e.g., nearest neighbour, Christofides).

Top use cases
1. Post‑processing of constructed tours to quickly reduce cost.
2. Realtime refinement in interactive routing tools.
3. Component of larger metaheuristics (ILS, GA repair).

CLI example
- dotnet run -- --solver TwoOpt
