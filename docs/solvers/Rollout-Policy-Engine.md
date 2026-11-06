# Rollout Policy Engine

Description
- Infrastructure to run rollouts from partial sequences using a policy (heuristic, LLM, or learned model) to complete sequences, evaluate outcomes, and use the rollout estimates to guide search.

Complexity
- Time: O(rollouts * cost_of_rollout).
- Space: O(rollouts * n).

Typical practical limits
- Workable n: depends on rollout cost; suited for problems where rollouts are informative and not too costly.

Parameters / options
- RolloutCount, rollout depth, policy choice (random, heuristic, LLM-driven), aggregation method.

Top use cases
1. Hybrid search where rollouts estimate the value of partial decisions (used in MCTS and policy rollouts).
2. Using LLMs as rollout policies to estimate downstream narrative or objective outcomes.
3. Combining cheap heuristic rollouts with expensive learned rollouts for budgeted evaluation.

CLI example
- dotnet run -- --solver Rollout

Notes on LLM integration
- LLMs can be used directly as rollout policies (generate completions) and the rollout engine aggregates generated outcomes to score partial sequences.

***
