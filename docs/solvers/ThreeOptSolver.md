# ThreeOptSolver

Description
- Local improvement heuristic that considers removing three edges and reconnecting segments (3‑opt) to escape 2‑opt local minima.

Complexity
- Time: O(n^3) per naive sweep; optimized implementations reduce constant factors.
- Space: O(n).

Typical practical limits
- Workable n: hundreds to thousands depending on optimization and time budget.

Parameters / options
- Seed tour: initial tour to improve.

Top use cases
1. Deeper local improvement when 2‑opt plateaus.
2. Backend batch optimization to increase solution quality.
3. Combined with heuristics as a refinement pass.

CLI example
- dotnet run -- --solver ThreeOpt
