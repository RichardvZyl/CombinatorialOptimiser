# Reconcile — PermutationOptimiser (GitHub `RichardvZyl/CombinatorialOptimiser`)

> Probed 2026-10-22 ~03:30 SAST, after `git fetch --all --prune --tags`. Trunk: `main` (derived from `origin/HEAD -> origin/main`).
> Status: roadmap only. Nothing parked, committed, pushed or deleted by this run. Probe ran as `remote` with per-command `-c safe.directory=<path>` (no config written).

## Base

| | Commit | Relative to the other |
|---|---|---|
| `main` (local) | `f8aee33` | 0 / 0 |
| `origin/main` | `f8aee33` | — |

Convergence takes: nothing.

## Ledger

| Entry | What it is | Evidence | Disposition |
|---|---|---|---|
| `chore/agent-entry-files` | 1 commit, upstream in sync | cherry +1 | ready |
| `feature/add-rollout` … `feature/add-cp-sat` (9 branches) | each upstream in sync, 4–22 ahead / 7–11 behind, 2026-08-31/09-01, chained by merges | cherry +3…+20 each; not ancestors of each other's tips (merge chain) | your call (stale stack) |
| `backup-133552`, `backup-134516` | both `5c816a4`, **no upstream, unrelated history** (no merge base) | 3 unique patches (`13961ed`, `13c5193`, `19e2890`); on no remote | unpublished |
| `final-clean` | `e056d23`, no upstream, unrelated history | 4 unique patches; on no remote | unpublished |
| `wip/permutation-optimiser-loose-state` | `8e57303`, no upstream | holds all 33 loose files | unpublished |
| working tree | 2 M + 31 untracked (solver test files etc.) | wip already holds the loose files byte-for-byte (hash-object == park blob) | parked already |

## At risk

**Unreachable — no ref names it** (fixed by parking, phase 1)

- none

**This machine only** (reachable, not on any remote)

- `backup-133552`/`backup-134516` (same SHA) and `final-clean`: June pre-rewrite history, 3–4 unique patches, on no remote
- `wip/permutation-optimiser-loose-state`: 1 commit (33 files)

## Roadmap

### 1. Park — local only, nothing pushed
- [ ] nothing

### 2. Converge trunk
- [ ] nothing

### 3. Land
- [ ] PR `chore/agent-entry-files`
- [ ] decide the 9-branch solver stack: land as one PR from `feature/add-cp-sat`/`add-mc-sampling` or retire

### 4. Rebase survivors
- [ ] nothing

### 5. Prune (after re-probe)
- [ ] `git branch -D backup-134516` (exact duplicate of backup-133552)

## Your calls

| Decision | If yes | If no | What is lost either way |
|---|---|---|---|
| Publish any parked / unpublished branches? | they survive losing this machine | `wip/` names stay off the shared remote | nothing — parked work already survives this roadmap |
| Keep the June pre-rewrite branches | history of the original scaffold stays | delete | 3–4 June commits (scaffold, Held-Karp, metaheuristics) that exist nowhere else |
| Revive the solver stack | rebase onto main and land | delete | the ACO/GRASP/VNS/ILP/CP-SAT/… solvers |

## Restore point

Captured 2026-10-22 ~03:30 SAST, before any execution. `git branch <name> <sha>` restores any of these.

```
5c816a417b2e74ed8b6e15a8357e4aae99768bdf refs/heads/backup-133552
5c816a417b2e74ed8b6e15a8357e4aae99768bdf refs/heads/backup-134516
1daf067653ee1a27cf8f3906d9ec8a63c8af3791 refs/heads/chore/agent-entry-files
81db31d47e3b54ff2334b0875a82c834d2a75308 refs/heads/feature/add-aco
f86604ef1d19792167d28e81052cd704fa5b9864 refs/heads/feature/add-cp-sat
cd62cac2799c04a065493c46601934e05bfc3e7b refs/heads/feature/add-diverse-beam
f21471b61a409fd48a8b404a3caaa585cf3588f2 refs/heads/feature/add-gnn-policy
47996761907081b3320c926baf09d23d2ead6a6a refs/heads/feature/add-grasp
2c1e48183508baf3119d5580a8107e5901c7577f refs/heads/feature/add-ilp
cfe3c785b9e828e5d3f2be2cbbb5b1aa1bc43f21 refs/heads/feature/add-mc-sampling
a378007c5d2211fba2901acdeaa39489c83a5d2a refs/heads/feature/add-rollout
740b2ad08002f888c2e4033989fb09e1e827850d refs/heads/feature/add-vns
e056d23f5a48efc68ffa8db6700e040ad3f99e1f refs/heads/final-clean
f8aee335643de51dcafceb7f52c09f5dbbb5f72f refs/heads/main
8e57303794a4098b3f6f40cbfebd73f3557c6573 refs/heads/wip/permutation-optimiser-loose-state
f8aee335643de51dcafceb7f52c09f5dbbb5f72f refs/remotes/origin/HEAD
1daf067653ee1a27cf8f3906d9ec8a63c8af3791 refs/remotes/origin/chore/agent-entry-files
81db31d47e3b54ff2334b0875a82c834d2a75308 refs/remotes/origin/feature/add-aco
f86604ef1d19792167d28e81052cd704fa5b9864 refs/remotes/origin/feature/add-cp-sat
cd62cac2799c04a065493c46601934e05bfc3e7b refs/remotes/origin/feature/add-diverse-beam
f21471b61a409fd48a8b404a3caaa585cf3588f2 refs/remotes/origin/feature/add-gnn-policy
47996761907081b3320c926baf09d23d2ead6a6a refs/remotes/origin/feature/add-grasp
2c1e48183508baf3119d5580a8107e5901c7577f refs/remotes/origin/feature/add-ilp
cfe3c785b9e828e5d3f2be2cbbb5b1aa1bc43f21 refs/remotes/origin/feature/add-mc-sampling
a378007c5d2211fba2901acdeaa39489c83a5d2a refs/remotes/origin/feature/add-rollout
740b2ad08002f888c2e4033989fb09e1e827850d refs/remotes/origin/feature/add-vns
f8aee335643de51dcafceb7f52c09f5dbbb5f72f refs/remotes/origin/main
8e57303794a4098b3f6f40cbfebd73f3557c6573 refs/tags/park/2026-10-21/permutation-optimiser-loose-state
```

Stashes at capture time: none
