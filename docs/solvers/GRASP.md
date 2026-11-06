# GRASP (Greedy Randomized Adaptive Search Procedure)

Description
- GRASP is a multi-start metaheuristic that repeatedly constructs randomized greedy solutions and improves them via local search, keeping the best found.

Complexity
- Time: O(restarts * (construction + local_search)).
- Space: O(n) per solution.

Typical practical limits
- Workable n: hundreds to thousands depending on restart count and local search complexity.

Parameters / options
- Restarts, randomness factor (alpha), local search method (2‑opt, 3‑opt), time budget.

Top use cases
1. Robust heuristic for large instances where diversification matters.
2. Produce a diverse set of high-quality seeds for downstream refinement.
3. Combine with learned heuristics for randomized construction.

CLI example
- dotnet run -- --solver GRASP

Notes on LLM integration
- LLMs can be used to suggest randomized construction heuristics or to parameterize the randomness/exploration strategy.

***
