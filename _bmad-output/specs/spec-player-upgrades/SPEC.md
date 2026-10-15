---
id: SPEC-player-upgrades
companions:
  - ../../planning-artifacts/architecture/architecture-player-upgrades-2026-10-15/ARCHITECTURE-SPINE.md
  - ../spec-save-system/SPEC.md
  - ../spec-player-wallet/SPEC.md
sources: []
---

> **Canonical contract.** This SPEC and the files in `companions:` are the complete, preservation-validated contract for what to build, test, and validate. Source documents listed in frontmatter are for traceability — consult them only if you need narrative rationale or prose color this contract intentionally omits.

# Player Upgrades

## Why

A vision to realize: MiraculousSurfers needs a progression hook — players spend earned coins to incrementally strengthen four gameplay parameters (`LaunchPower`, `Speed`, `SteeringResponsiveness`, `CollectibleValue`), so runs visibly improve over time. This is the first consumer of the save system and the first real use of the game-configuration and currency seams. It's built now as a headless, testable service so the home-screen shop UI (later) and gameplay both bind to one contract. The technical shape is fixed by the Player Upgrades architecture spine (AD-1…AD-9), adopted as a companion; this spec states the behavior that shape must deliver.

## Capabilities

- **CAP-1** — Incremental param upgrade
  - **intent:** The player buys the next upgrade step for a param, raising its gameplay multiplier by one index.
  - **success:** After a successful `TryUpgrade(param)`, the param's index increments by 1 and `GetValue(param)` returns the config's value at the new index.

- **CAP-2** — Affordability & max gating
  - **intent:** An upgrade is purchasable only when the player can afford it and it isn't already maxed.
  - **success:** `TryUpgrade` with coins below the next price, or at max (`HasNext` false), returns `Ok` with `Applied=false`, charges nothing, and leaves state unchanged; `CanUpgrade(param)` reports the same.

- **CAP-3** — Charge on purchase
  - **intent:** A successful purchase deducts the next step's price from the player's coins.
  - **success:** Coins decrease by exactly `Price(param, index+1)` after a successful `TryUpgrade` (via `IWalletService.TrySpend`).

- **CAP-4** — Durable upgrade state
  - **intent:** Upgrade progress survives app restarts.
  - **success:** Buy an upgrade, restart the app, and the saved index — and its derived value — are restored.

- **CAP-5** — Config-driven tuning
  - **intent:** Per-param steps, values, prices, and max level are authored data, not code.
  - **success:** Editing the `GameConfiguration` ScriptableObject ladder changes the reported `Value`/`Price`/`MaxIndex` with no code change, and `HasNext` is false at the top of the ladder.

- **CAP-6** — Derived gameplay values
  - **intent:** Gameplay reads a param's current multiplier through the service, reflecting the live upgrade index.
  - **success:** A gameplay consumer obtains e.g. `LaunchPower` via `GetValue(param)` and it equals `config.Value(param, currentIndex)`.

- **CAP-7** — Change-notification seam
  - **intent:** Consumers (the future HomeScreenUI) are notified when a param upgrades, without the service knowing about UI.
  - **success:** A subscriber to `IUpgradeService.Changed` receives the `UpgradeParam` after a successful purchase — including when `Persisted=false`.

- **CAP-8** — Consistent purchase (B2)
  - **intent:** A purchase is all-or-nothing — it never leaves coins spent without the upgrade persisted.
  - **success:** With a forced upgrade-save failure, the coins are refunded and the index reverts, and `TryUpgrade` returns `Ok` with `Applied=false`.

## Constraints

Each cites the governing spine AD (see the adopted `ARCHITECTURE-SPINE.md`).

- **Persisted state is an int index per param (AD-2).** `PlayerUpgradesData` stores four `int` indices (default `0`); the multiplier value is derived via `IGameConfigurationSystem.Value`, never stored; a loaded index is clamped to `[0, MaxIndex(param)]`. This **revises already-shipped code** (`PlayerUpgradesData` floats→ints, `PlayerUpgradesSaveSystem` defaults/validation) and the **save-system spec's CAP-8** (defaults "all 1.0" → "indices default 0") — see Open Questions.
- **Single stateful owner (AD-1).** `UpgradeService` alone holds live `PlayerUpgradesData`; gameplay and UI read only through `IUpgradeService`.
- **Enum-keyed contract behaviour (AD-4).** Addressing is the `UpgradeParam` enum; `GetNextCost` returns `0` at max; `Changed` fires on any in-memory apply regardless of `Persisted`. The service also exposes `GetIndex(param)` and `GetMaxIndex(param)` so UI can render a `currentUpgrade/MaxUpgrades` progress bar per param.
- **Fallible ops reuse `Core.Result` (AD-5).** `TryUpgrade` returns `UniTask<IResult<UpgradePurchase>>`; `Ok` for every expected outcome (unaffordable, maxed), `Fail` only for an unexpected throw; pure queries return raw types. Under B2, `Applied` implies `Persisted`.
- **Tuning only via `IGameConfigurationSystem` (AD-3).** ScriptableObject-backed now (`LocalGameConfigurationSystem` reads a `GameConfiguration` SO); index-addressed `MaxIndex`/`Value`/`Price`/`HasNext`; no hardcoded steps or prices.
- **Durable async wallet (AD-6, realized).** Upgrades depend only on `IWalletService { int Coins; UniTask<bool> TrySpend(int); UniTask AddCoins(int); event Changed }`, realized by the durable `WalletService` (see the adopted `spec-player-wallet`). (Superseded the original sync, mocked shape.)
- **Refund-based purchase integrity, D2 (AD-7).** Order is `await TrySpend` → bump index → `await SaveAsync`; on save failure, refund via `AddCoins` and revert the index, returning `Applied=false`. Both sides persist or neither. (Tightened from D1 once the wallet became durable.)
- **DI & lifecycle (AD-8, AD-9).** All four services register as `Singleton` in the single existing `GameplayLifetimeScope`; `UpgradeService` loads on VContainer `IAsyncStartable.StartAsync`, falling back to `CreateDefault()` on load failure.
- **Inherited, binding.** Save-system invariants (stateless S1, Newtonsoft, key-suffix versioning with no migration, VContainer) and CLAUDE.md conventions (no comments, `var`, underscore privates, UniTask not coroutines, `int` coins/prices, code under `Assets/MiraculousSurfers/`).

## Non-goals

- HomeScreenUI — out of scope now; the seam is `IUpgradeService` + the `Changed` event.
- A real currency/balance system — the wallet is mocked and non-persistent behind `IWalletService`.
- Broader game-configuration authoring beyond the upgrade ladders.
- Root-scope promotion / multi-scene support — the prototype is single-scope.
- D1→D2 purchase rollback (refund + revert) while the wallet is non-persistent; and cross-version save migration (inherited — a format change abandons old saves).

## Success signal

A player spends coins to upgrade the four params: each affordable, non-maxed purchase raises the param's multiplier one step along the authored SO ladder, deducts the step's price, fires `Changed`, and survives an app restart; gameplay reads the boosted multipliers through `IUpgradeService.GetValue`; unaffordable or maxed attempts are no-ops reported as `Ok(Applied=false)`; and a forced save failure still raises the value in memory while reporting `Persisted=false`. Demonstrable with an edit-mode/play test driving `TryUpgrade` against the mock wallet and a test `GameConfiguration` SO.

## Assumptions

- Implementing AD-2 revises already-shipped code (`PlayerUpgradesData` floats→int indices, `PlayerUpgradesSaveSystem` defaults/`IsValid`) and the save-system spec's CAP-8. Pre-launch with no saved data on devices, so the format change is clean (old saves would simply be abandoned by the inherited no-migration rule).

## Open Questions

- Mock wallet starting coin balance — implemented as a serialized `_startingCoins` on the scope (default `1000`); confirm the value.
- The concrete SO ladders per param (number of steps, the value curve, the prices) — authored by design, not fixed by this spec.
