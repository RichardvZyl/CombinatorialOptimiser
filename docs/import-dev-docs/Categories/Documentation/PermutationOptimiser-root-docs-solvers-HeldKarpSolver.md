# HeldKarpSolver

Description
- Dynamic programming exact solver for the travelling salesman problem (Held‑Karp). Computes optimal tour using subset DP.

Complexity
- Time: O(2^n * n^2)
- Space: O(2^n * n)

Typical practical limits
- Workable n: up to ~16–20 depending on memory and compute budget.

Parameters / options
- None in the default implementation; memory/time scale with n.

Top use cases
1. Medium‑sized instances requiring provable optimality (research, verification).
2. Generation of optimal baselines for benchmarking heuristics.
3. Small industrial problems (e.g., toolpath sequencing for CNC with limited points).

CLI example
- dotnet run -- --solver HeldKarp
