> Source: E:\Development\Projects\A2A Project folder\docs\evaluations\2026-11-05-combinatorial-optimiser-evaluation.md_index.md | Branch: main | Commit: 12c1a42 2026-11-05 | Status: committed on main | Copied: 2026-11-06 07:57 SAST (working copy - edit the source, not this file)

# 2026-11-05-combinatorial-optimiser-evaluation.md — navigation aid

Source: [2026-11-05-combinatorial-optimiser-evaluation.md](2026-11-05-combinatorial-optimiser-evaluation.md).

Evaluation: `CombinatorialOptimiser` (`PermutationOptimiser`) in the A2A & Agentic Development Harness. Technology-fit assessments.

> Derived navigation snapshot. The source owns decisions, identifiers and current status.

## Find a section

- [Evaluation: `CombinatorialOptimiser` (`PermutationOptimiser`) in the A2A & Agentic Development Harness](2026-11-05-combinatorial-optimiser-evaluation.md#evaluation-combinatorialoptimiser-permutationoptimiser-in-the-a2a--agentic-development-harness)
- [1. Executive Summary & Verdict](2026-11-05-combinatorial-optimiser-evaluation.md#1-executive-summary--verdict)
- [2. What `CombinatorialOptimiser` Currently Does (From Its `.md` Documentation)](2026-11-05-combinatorial-optimiser-evaluation.md#2-what-combinatorialoptimiser-currently-does-from-its-md-documentation)
- [2.1 Architecture & Engineering Posture](2026-11-05-combinatorial-optimiser-evaluation.md#21-architecture--engineering-posture)
- [2.2 The Three Implemented Problem Domains & 20+ Solvers](2026-11-05-combinatorial-optimiser-evaluation.md#22-the-three-implemented-problem-domains--20-solvers)
- [2.3 In-Flight / Parked Roadmap Branches (`.agents/reconcile.md` & `docs/solver-roadmap-tasks.md`)](2026-11-05-combinatorial-optimiser-evaluation.md#23-in-flight--parked-roadmap-branches-agentsreconcilemd--docssolver-roadmap-tasksmd)
- [3. Evaluation Against the Planned A2A & Harness Architecture](2026-11-05-combinatorial-optimiser-evaluation.md#3-evaluation-against-the-planned-a2a--harness-architecture)
- [3.1 Application 1: `SubsetSelection` (0/1 Knapsack) -> `D12.9` Context Envelope & Token-Budget Packing (Ready Today)](2026-11-05-combinatorial-optimiser-evaluation.md#31-application-1-subsetselection-01-knapsack---d129-context-envelope--token-budget-packing-ready-today)
- [3.2 Application 2: `ConstraintAssignment` (Graph Colouring) -> `D10.5` Parallel Wave Scheduling & `D11` Provider-Independence Assignment (Ready Today)](2026-11-05-combinatorial-optimiser-evaluation.md#32-application-2-constraintassignment-graph-colouring---d105-parallel-wave-scheduling--d11-provider-independence-assignment-ready-today)
- [3.3 Application 3: `Permutation` (`DistanceMatrix` + `BeamSearch` + `MCTS` + `CP-SAT`) -> `D10.4` / `D19` Escalation Ladders & Batch Sequencing (Ready Today)](2026-11-05-combinatorial-optimiser-evaluation.md#33-application-3-permutation-distancematrix--beamsearch--mcts--cp-sat---d104--d19-escalation-ladders--batch-sequencing-ready-today)
- [3.4 Application 4: `D19` Quota-Aware Multi-Agent Routing — What Fits Today vs. What to Add](2026-11-05-combinatorial-optimiser-evaluation.md#34-application-4-d19-quota-aware-multi-agent-routing--what-fits-today-vs-what-to-add)
- [4. Recommended Enhancements to `CombinatorialOptimiser` (Keeping Domain Agnosticism Intact)](2026-11-05-combinatorial-optimiser-evaluation.md#4-recommended-enhancements-to-combinatorialoptimiser-keeping-domain-agnosticism-intact)
