# Monte Carlo Sampling + Reranking

Description
- Sampling-based approach: draw many stochastic tours from a distribution (e.g., softmax over -cost), then rerank samples by objective or external metric to choose final solution.

Complexity
- Time: O(samples * cost_of_evaluate).
- Space: O(samples * n).

Typical practical limits
- Workable n: scales well if sampling is cheap; sample count often limited by time budget.

Parameters / options
- SampleCount, sampling temperature, proposal distribution, reranking metric.

Top use cases
1. Useful when objective is noisy or when multiple diverse candidates improve final selection.
2. Combine LLM or learned policy sampling with exact reranking to enforce constraints or improve objectives.
3. Quick parallel exploration where many inexpensive samples can be evaluated and aggregated.

CLI example
- dotnet run -- --solver MCSampling

Notes on LLM integration
- Direct analogue to sampling-based decoding (top-k, nucleus) followed by reranking using downstream metrics or symbolic checks.

***
