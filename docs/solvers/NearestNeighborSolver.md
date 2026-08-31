# NearestNeighborSolver

Description
- Fast greedy constructive heuristic that repeatedly visits the nearest unvisited node.

Complexity
- Time: O(n^2)
- Space: O(n)

Typical practical limits
- Workable n: up to thousands; very fast and low-overhead.

Parameters / options
- Choice of start node (default: index 0). Can be used with multiple restarts.

Top use cases
1. Quick approximate routing where latency matters.
2. Seed generation for local improvement algorithms (2‑opt, 3‑opt, LK).
3. Interactive or embedded systems needing a cheap heuristic.

CLI example
- dotnet run -- --solver NearestNeighbor
