# GNN-guided Constructive Policy

Description
- Scaffold and integration points for a learned Graph Neural Network (GNN) policy that predicts next-node probabilities or value estimates to guide constructive search.

Complexity
- Training: depends on dataset size and model; inference: typically O(n)–O(n log n) per decision depending on architecture.

Typical practical limits
- Workable n: scales well for larger instances at inference time; training scales with dataset and model capacity.

Parameters / options
- Model architecture, training data generation, learning rate, inference batch size, beam/guided decoding integration.

Top use cases
1. Learned heuristics that outperform hand-crafted heuristics on domain-specific distributions.
2. Hybrid systems where LLMs provide problem descriptions and a GNN policy provides structured action recommendations.
3. Fast inference for large-scale routing when a trained policy replaces expensive search.

CLI example
- dotnet run -- --solver GNNPolicy

Notes on LLM integration
- GNN policies complement LLMs: use LLMs to interpret high-level constraints or generate training instances, and use GNNs for fast, structured decision making.

***
