# BeamSearchSolver

Description
- Beam search guided by transition log‑probabilities derived from the cost matrix (p(j|i) ∝ exp(−cost(i,j)/temperature)).
- Keeps a beam of the k best partial sequences and expands them until full tours are formed.

Complexity
- Time: O(beam * n^2) in typical implementations (beam * branching factor * steps).
- Space: O(beam * n).

Typical practical limits
- Workable n: tens to hundreds depending on beam width and time budget. Small beam widths scale better.

Parameters / options
- beamWidth (k): number of partial hypotheses to retain.
- temperature: softmax temperature applied to -cost when computing transition scores.
- useLogProbForFinalRanking: if true, select final tour by accumulated log‑prob; otherwise rank by tour length.
- disallowSelf: self‑transitions disabled by default.

Top use cases
1. Near‑real‑time constructive search balancing quality and latency (choose beam to match budget).
2. Hybrid pipelines where learned transition models produce logits that are combined with explicit costs and constraints.
3. Educational and research scenarios comparing probabilistic decoding notions (beam, temperature) against deterministic heuristics.

Notes
- Direct analogue to beam decoding in sequence models (LLMs). Useful when you want to prioritise high‑prob partial solutions but still enforce permutation constraints.

CLI example
- dotnet run -- --beam-width 8 --beam-temp 0.5 --beam-rank tour
