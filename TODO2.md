# Inference — 1

> Seed charter. Minimal base for a future solution; **no implementation this session.**
> Spun out of the `CombinatorialOptimiser` scope review (2026-06-14).

## Purpose

A **probabilistic inference / MCMC** library: estimate **hidden / latent state
and posterior distributions** from noisy observations (e.g. market regime,
volatility, drift) — the *"infer a distribution"* bucket of the stack.

## In scope

- **Standalone Metropolis-Hastings** sampler — the *general* form with an
  arbitrary proposal `q` and the Hastings correction ratio `q(x|x')/q(x'|x)`
  for **asymmetric** proposals. (This is the part NOT present in
  `CombinatorialOptimiser` — its SA uses the symmetric *Metropolis* special
  case where the ratio is 1.)
- **Gibbs sampling** — full-conditional updates.
- **Markov chains** — transition matrices, stationary distribution, regime
  modelling.
- Diagnostics — convergence (R-hat), autocorrelation, effective sample size.
- *Later:* Kalman filter (linear-Gaussian), particle filter / sequential MC.

## Out of scope (delegate)

- Optimisation over tours / subsets / assignments → **CombinatorialOptimiser**.
- Forward scenario / P&L simulation → **Simulation** (2).
- Trade decisions → **TradingBotAPI** (3).

## Key recorded distinction

SA in `CombinatorialOptimiser` and MCMC here **share the acceptance kernel but
differ in purpose**: SA *cools* to converge on the mode (returns one optimum);
MCMC holds temperature fixed and **keeps the whole chain** to characterise the
distribution. So they are not the same algorithm — only the accept step.

## Relationships

- Standalone, ideally dependency-free, publishable NuGet package.
- **Consumed by** `TradingBotAPI` to estimate latent variables that feed
  decisions; **feeds** calibrated parameters to `Simulation`.

## Starter tasks

1. Create solution `Inference`.
2. `ISampler<TState>` abstraction; `MetropolisHastings`, `GibbsSampler`.
3. `MarkovChain` type (transition matrix → stationary distribution).
4. Convergence diagnostics (R-hat, ESS, autocorrelation).
5. xUnit tests against known posteriors (e.g. sample a Gaussian; check
   recovered mean/variance within tolerance).
