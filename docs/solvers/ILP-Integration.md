# ILP Integration (Integer Linear Programming)

Description
- Encoder and wrapper to express permutation/TSP instances as integer linear programs (ILP) and call an external ILP solver (e.g., GLPK, CBC, or commercial solvers) for exact or bounded solutions.

Complexity
- Time: depends on solver and model; ILP is NP-hard in general but modern solvers are effective for many medium instances.
- Space: depends on model size and solver internals.

Typical practical limits
- Workable n: small-to-medium (up to a few dozen nodes) for exact ILP depending on solver; LP relaxations are useful for bounds on larger instances.

Parameters / options
- Solver choice, time limit, use of LP relaxation, cutting planes, branch-and-cut options.

Top use cases
1. Exact solving or proving optimality for small/medium instances where HeldKarp might be infeasible or where additional linear constraints are needed.
2. Use LP relaxation for strong lower bounds in branch-and-bound frameworks.
3. Repair or re-rank LLM outputs to satisfy numeric or linear constraints (budgets, capacities).

CLI example
- dotnet run -- --solver ILP

Notes on LLM integration
- ILP provides precise enforcement of linear constraints and can be used to certify or repair candidate schedules or allocations proposed by LLMs.

***
