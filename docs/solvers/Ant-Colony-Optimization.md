# Ant Colony Optimization (ACO)

Description
- Population-based constructive metaheuristic that builds solutions using probabilistic decisions reinforced by pheromone trails and heuristic desirability.

Complexity
- Time: O(iter * ant_count * n) per construction pass; local search adds to cost.
- Space: O(n^2) for pheromone matrix.

Typical practical limits
- Workable n: hundreds depending on ant count and iterations; suitable for routing benchmarks.

Parameters / options
- AntCount, pheromone evaporation, pheromone update rules, heuristic weight, local search integration.

Top use cases
1. Robust constructive metaheuristic for routing and scheduling benchmarks.
2. Generating diverse high-quality seeds for refinement or reranking.
3. Hybridization with learned heuristics (e.g., use GNN for heuristic desirability updates).

CLI example
- dotnet run -- --solver ACO

Notes on LLM integration
- ACO can be combined with LLM-provided priors as initial pheromone biases, or LLMs can propose heuristic desirabilities used in probabilistic construction.

***
