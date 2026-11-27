# CP-SAT Wrapper (Constraint Programming)

Description
- Thin wrapper to call a constraint programming / CP-SAT solver (e.g., OR-Tools CP-SAT) to solve constrained permutation instances or to repair/rerank candidate tours to satisfy hard constraints.

Complexity
- Time: depends on model and solver; can be exponential in worst-case but effective for many practical constraints.
- Space: depends on the encoding and solver internal structures.

Typical practical limits
- Workable n: medium-sized instances (tens to low-hundreds) depending on constraint complexity and solver power.

Parameters / options
- Solver choice (OR-Tools, external executable), time limit, integer precision, constraint encoding options.

Top use cases
1. Enforce all-different, capacity, precedence or other hard constraints that are awkward to encode directly into heuristics.
2. Post-process LLM-generated candidate sequences to ensure feasibility (repair and rerank).
3. Hybrid pipelines where CP ensures global constraints while heuristics provide good initial seeds.

CLI example
- dotnet run -- --solver CP-SAT

Notes on LLM integration
- Use CP to filter or repair LLM outputs so that candidates satisfy domain invariants (counts, uniqueness, capacity). A common pattern: LLM proposes candidates -> CP refines to a feasible solution -> rerank.

***
