# BruteForceSolver

Description
- Exhaustively enumerates all permutations (tours) using Heap's algorithm. Guarantees optimal solution but has factorial runtime.

Complexity
- Time: O(n!)
- Space: O(n)

Typical practical limits
- Workable n: ≤ 10 for reasonable runtimes; used for correctness and small instances.

Parameters / options
- None.

Top use cases
1. Correctness baseline and unit tests (compare heuristic outputs).
2. Tiny production tasks where n is small and optimality is required.
3. Educational demonstrations of exhaustive search.

CLI example
- dotnet run -- --solver BruteForce
