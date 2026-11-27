# GeneticAlgorithmSolver

Description
- Population‑based metaheuristic using crossover, mutation and selection to evolve tours over generations. Often combined with repair operators to maintain feasibility.

Complexity
- Time: O(gen * pop * cost_of_evaluate) where gen = generations, pop = population size.
- Space: O(pop * n).

Typical practical limits
- Workable n: hundreds to thousands depending on population/generation budgets and parallelism.

Parameters / options
- PopulationSize, Generations, Crossover type, Mutation rate, Repair strategy.

Top use cases
1. Multi‑objective or constrained problems where specialised crossover/repair encodes domain knowledge.
2. Parallelizable large‑scale optimization with population diversity.
3. Experimental pipelines combining learned models and evolutionary search.

CLI example
- dotnet run -- --solver GeneticAlgorithm
