# LinKernighanSolver

Description
- Powerful and widely used local search (Lin‑Kernighan) that adaptively chooses k‑opt moves to produce high‑quality tours.

Complexity
- Practical cost: often near O(n^2) per improvement sweep; implementation‑dependent.
- Space: O(n).

Typical practical limits
- Workable n: thousands to tens of thousands with tuned implementations.

Parameters / options
- Seed tour: provides starting solution; performance improves with a good seed.

Top use cases
1. Production route optimization where high quality is required.
2. Post‑processing at scale in logistics backends.
3. Component in hybrid pipelines (seed + LK + final refinement).

CLI example
- dotnet run -- --solver LinKernighan
