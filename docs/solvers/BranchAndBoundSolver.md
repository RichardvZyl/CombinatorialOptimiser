# BranchAndBoundSolver

Description
- Systematic search over the permutation tree with pruning based on lower/upper bounds. Pruning reduces the explored space compared to naive brute force.

Complexity
- Worst-case time: O(n!) but typically much faster when good bounds exist.
- Space: O(n)–O(n!) depending on implementation (stack/priority queue).

Typical practical limits
- Workable n: up to ~12–14 for many instances; performance depends on bound tightness and instance structure.

Parameters / options
- Bound heuristics and ordering (implementation-defined).

Top use cases
1. Small–medium routing tasks where optimality is required but full enumeration is too expensive.
2. Benchmarking heuristics by comparing to exact optimal tours on moderately sized instances.
3. Constrained scheduling problems where tight bounds enable strong pruning.

CLI example
- dotnet run -- --solver BranchAndBound
