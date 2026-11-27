# Monte Carlo Tree Search (MCTS)

Description
- Monte Carlo Tree Search (MCTS) is a simulation-based tree search algorithm combining selection, expansion, simulation (rollout) and backpropagation (UCT). It balances exploration and exploitation and can handle large branching factors.

Complexity
- Time: depends on simulation budget; each iteration runs a rollout which may be O(n)–O(n^2).
- Space: grows with the explored tree (number of nodes visited).

Typical practical limits
- Workable n: scales to problems with large branching factors when using limited simulation budgets; practical for medium-to-large combinatorial problems when rollouts are cheap.

Parameters / options
- Simulation budget (iterations), exploration constant (C), rollout policy (random, heuristic, learned), rollout depth.

Top use cases
1. Planning and decision making where model rollouts provide useful approximate evaluation (e.g., route planning with stochastic costs).
2. Hybrid LLM pipelines (Tree of Thoughts style) where LLM proposes actions and MCTS evaluates/searches candidate action sequences.
3. Problems with large branching factors where selective deepening is preferable to exhaustive enumeration.

CLI example
- dotnet run -- --solver MCTS

Notes on LLM integration
- MCTS is commonly paired with learned policies or value estimates. In LLM contexts, the LLM can provide proposal distributions or rollout heuristics while MCTS aggregates search statistics to choose actions.

***
