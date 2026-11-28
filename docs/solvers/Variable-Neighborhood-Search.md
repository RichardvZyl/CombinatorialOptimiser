# Variable Neighborhood Search (VNS)

Description
- VNS systematically changes neighborhood structures during the search to escape local minima and explore different solution neighborhoods.

Complexity
- Time: O(iter * cost_of_neighborhood_search) where neighborhood search depends on move types used.
- Space: O(n).

Typical practical limits
- Workable n: hundreds to thousands depending on neighborhoods and iterations.

Parameters / options
- Neighborhood set, shaking/perturbation strength, local search method, iteration budget.

Top use cases
1. Problems where single-neighborhood local search stagnates; VNS provides structured diversification.
2. Backend optimization for logistics where different neighborhood operators suit different route shapes.
3. Integration with metaheuristics as the intensification/diversification component.

CLI example
- dotnet run -- --solver VNS

Notes on LLM integration
- LLMs can suggest which neighborhood operators to apply based on problem description or instance features.

***
