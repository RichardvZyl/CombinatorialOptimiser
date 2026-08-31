# Diverse Beam Search

Description
- Diverse Beam Search aims to increase hypothesis diversity compared to standard beam search by enforcing diversity-promoting constraints or splitting the beam into diverse groups.

Complexity
- Time: similar to Beam Search; O(beam * n^2) with added overhead for diversity maintenance.
- Space: O(beam * n).

Typical practical limits
- Workable n: similar to beam search; effective when beam width is moderate.

Parameters / options
- beamWidth, diversity strength, grouping strategy (k groups), sampling vs deterministic expansion.

Top use cases
1. Produce diverse candidate tours for downstream reranking to improve final solution quality.
2. Avoid beam collapse in LLM decoding analogues where diversity leads to better coverage of solution space.
3. Exploration in hybrid search pipelines where multiple distinct hypotheses are valuable.

CLI example
- dotnet run -- --solver DiverseBeam

Notes on LLM integration
- Directly applicable: diverse beam variants are commonly used in text generation to avoid near-duplicate outputs and improve downstream metrics after reranking.

***
