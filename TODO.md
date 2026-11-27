# Bedrock.Compute — 0

> Seed charter. Minimal base for a future extraction; **no implementation this session.**
> The shared **algorithm/strategy backbone** for generic stack — the "0" that
> `CombinatorialOptimiser` (1-adjacent), `Inference` (1), and `Simulation` (2) build on.
> Sibling family under the Bedrock brand (ADR-0007 + `ecosystem-map.md` in the Abstractions repo).

## Purpose

A dependency-free shared abstraction for the recurring shape across other libraries: **a
pluggable strategy that turns a problem into a result**, with a registry, reproducible seeded RNG,
cancellation/async, and run telemetry. Each library then implements only its *domain* moves, not
the scaffolding it currently duplicates.

The shape already exists three times:
- `CombinatorialOptimiser` — `ISolver<TProblem,TResult>` + `SolverRegistry` + `SolverParadigm` +
  `SolverResult` (with `Elapsed`) + `RandomSeed` on every metaheuristic.
- `Inference` (planned) — `ISampler<TState>`, convergence diagnostics, seeded RNG.
- `Simulation` (planned) — `IStochasticProcess` / `IPathGenerator` / `MonteCarloEngine`, seeded RNG.

## In scope

- **Strategy seam** — `IStrategy<TProblem,TResult>` plus a non-generic marker `IStrategy`
  (`Name`, `Kind`) for cross-domain collections. `ISolver<,>` / `ISampler<>` / path generators
  become specialisations.
- **Result base** — a common record carrying `Algorithm`, `Kind`, `Elapsed` (generalise
  `SolverResult`). **Name it `ComputeResult`, not `*Base`** (FDG "avoid Base suffix" — the same fix
  just applied in CombinatorialOptimiser).
- **Kind/paradigm marker** — generalise `SolverParadigm` (Exact/Construction/Improvement/Reduction)
  into an open classification.
- **Registry / recommendation** — generalise `SolverRegistry`: enumerate and recommend strategies
  by problem size/shape.
- **Reproducible RNG** — `ISeededRandomSource` (all three domains already standardise on
  `RandomSeed = 42` for reproducibility).
- **Cancellation + async** — `Solve/SolveAsync(…, CancellationToken)` conventions (already the
  pattern in CombinatorialOptimiser).
- *(optional)* run telemetry + convergence-diagnostic contracts shared with Inference.

## Out of scope (delegate, don't reimplement)

- Concrete optimisation solvers → **CombinatorialOptimiser**.
- MCMC / samplers → **Inference** (1).
- Monte Carlo / path generation → **Simulation** (2).
- **Service infrastructure** (data, outbox, sagas, tenancy, security, hosting) → **Bedrock.\***, the
  *other* backbone. `Bedrock.Compute` does **not** depend on Bedrock service-infra, and vice-versa;

## Key recorded distinction

Two backbones under one brand: **`Bedrock.*`** = service infrastructure; **`Bedrock.Compute.*`** =
algorithms. Neither depends on the other. Keeping them separate is what lets the math
libraries stay publishable and near-dependency-free while the service backbone carries EF/ASP.NET/
brokers.

## Relationships

- **Consumed by** `CombinatorialOptimiser`, `Inference`, `Simulation` — they implement its seams.
- **Near-dependency-free** — at most `Bedrock.Results` (`Result<T>`) + conventions; no service infra..

## Extraction gate (important)

Do **not** extract yet. Per ADR-0007 and the .NET Framework Design Guidelines ("DO NOT provide
abstractions unless tested by developing several concrete implementations"), wait until at least
**two** of the three libraries exist, so the shared seam is validated by real implementations
rather than guessed from one. The home and name are settled now; the extraction is the follow-up.

## Starter tasks (when the gate opens)

1. With Inference *or* Simulation in hand, diff CombinatorialOptimiser's
   `ISolver<,>`/`ComputeResult`/`SolverParadigm`/`SolverRegistry`/seeded-RNG against the
   sampler/process shapes; lift the common seam into `Bedrock.Compute.Abstractions`.
2. Define `IStrategy<TProblem,TResult>` (+ non-generic `IStrategy`), `ComputeResult` (with
   `Elapsed`), a kind marker, `IStrategyRegistry`, `ISeededRandomSource`.
3. Refactor `CombinatorialOptimiser` so `ISolver<,> : IStrategy<,>`; keep public API stable where
   possible, else major-version it.
4. Publishable, dependency-free package; xUnit tests proving each library's strategy plugs into the
   shared registry and RNG.
5. Record naming/versioning under the Bedrock ADR set (`Abstractions/root/docs/adr/`).
