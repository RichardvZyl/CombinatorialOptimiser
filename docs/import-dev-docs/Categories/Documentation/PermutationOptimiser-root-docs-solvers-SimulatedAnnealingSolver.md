# SimulatedAnnealingSolver

Description
- Stochastic metaheuristic that accepts worsening moves with a probability that decreases over time (temperature schedule) to escape local minima.

Complexity
- Time: O(steps * cost_of_move) where steps is the number of iterations.
- Space: O(n).

Typical practical limits
- Workable n: hundreds to thousands; runtime scaled by step budget.

Parameters / options
- InitialTemperature, CoolingRate, Step budget, Move neighbourhood (e.g., 2‑opt swaps).

Top use cases
1. Global search where deterministic local search gets stuck.
2. Noisy or approximate objectives where stochasticity helps find robust solutions.
3. Time‑bounded optimization with tunable exploration/exploitation tradeoffs.

CLI example
- dotnet run -- --solver SimulatedAnnealing
