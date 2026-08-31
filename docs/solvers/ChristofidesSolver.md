# ChristofidesSolver

Description
- Polynomial‑time approximation algorithm for metric TSP with a worst‑case guarantee of 1.5× optimal. Combines minimum spanning tree, perfect matching, and shortcutting.

Complexity
- Time: dominated by matching; typically O(n^2) to O(n^3) depending on matching implementation.
- Space: O(n^2) for distance data.

Typical practical limits
- Workable n: thousands+; scales well for large metric instances.

Parameters / options
- UseExactMatching (bool): controls whether exact matching is used for the matching step; toggles quality vs runtime.

Top use cases
1. Large routing problems where a provable approximation bound is required.
2. Fast seeds for local improvement (follow with 2‑opt / LK to improve quality).
3. Systems needing predictable worst‑case solution quality (SLAs, contracts).

CLI example
- dotnet run -- --solver Christofides
