# Simulation — 2

> Seed charter. Minimal base for a future solution; **no implementation this session.**
> Spun out of the `CombinatorialOptimiser` scope review (2026-06-14).

## Purpose

A **simulation / what-if** library: sample **forward scenarios** to estimate
risk/reward distributions — the *"simulate scenarios"* bucket. This is the
legitimate home of **Monte Carlo** (which was rejected as a *solver* in
`CombinatorialOptimiser` because uniform random solution sampling is dominated
by every constructive heuristic there).

## In scope

- **Monte Carlo price-path simulation** — GBM first, pluggable stochastic
  processes.
- **Risk metrics** — VaR, CVaR / expected shortfall, max-drawdown distribution.
- **What-if / stress scenarios** over a strategy.
- Historical / bootstrap resampling.
- Reproducible RNG with explicit seeds.

## Out of scope (delegate)

- Optimisation → **CombinatorialOptimiser**.
- Posterior / latent-state estimation → **Inference** (1). Simulation *consumes*
  calibrated parameters; it does not estimate them.
- Decisions → **TradingBotAPI** (3).

## Key recorded distinction

Monte Carlo **simulation** (sampling many price paths to estimate a risk
distribution) is valuable and lives **here** — distinct from Monte Carlo as an
*optimiser* (random solution sampling), which was correctly excluded from
`CombinatorialOptimiser`.

## Relationships

- **Consumes** drift/vol/regime parameters from `Inference`.
- **Provides** scenario distributions to `TradingBotAPI` for risk-aware sizing
  and back-testing.

## Starter tasks

1. Create solution `Simulation`.
2. `IStochasticProcess` (GBM first), `IPathGenerator`.
3. `MonteCarloEngine` — N paths, horizon, seed.
4. Risk aggregations: VaR / CVaR / max-drawdown.
5. xUnit tests: GBM sample moments converge to analytic values; VaR sanity on
   distributions with known quantiles.
