---
id: SPEC-player-wallet
companions:
  - ../../planning-artifacts/architecture/architecture-player-wallet-2026-10-15/ARCHITECTURE-SPINE.md
  - ../spec-save-system/SPEC.md
  - ../spec-player-upgrades/SPEC.md
sources: []
---

> **Canonical contract.** This SPEC and the files in `companions:` are the complete, preservation-validated contract for what to build, test, and validate. Source documents listed in frontmatter are for traceability — consult them only if you need narrative rationale or prose color this contract intentionally omits.

# Player Wallet

## Why

A vision to realize: the player needs a real, persistent coin balance — the economy currency the upgrades system spends against. Until now the wallet was a non-persistent mock (player-upgrades AD-6). Making it durable turns coins into saved progression and, because a durable debit can now outlive a crash, forces the upgrade purchase to become genuinely consistent (the AD-7 D1→D2 revisit). The technical shape is fixed by the Player Wallet spine (AD-1…AD-4), adopted as a companion.

## Capabilities

- **CAP-1** — Durable balance
  - **intent:** The player's coin balance survives app restarts.
  - **success:** Add or spend coins, restart, and the balance is restored.

- **CAP-2** — Gated spend
  - **intent:** Spending atomically checks affordability and charges.
  - **success:** `TrySpend(price)` with `Coins >= price` returns `true` and decrements by `price` (persisted); with `Coins < price` returns `false` and changes nothing.

- **CAP-3** — Add coins
  - **intent:** Coins can be credited to the balance, not only spent.
  - **success:** `AddCoins(n)` raises `Coins` by `n`, persists, and fires `Changed`.

- **CAP-4** — Sync read + change notification
  - **intent:** The balance is readable synchronously; consumers are notified when it moves.
  - **success:** A subscriber receives `Changed` after a spend or add, and `Coins` reflects the new value.

- **CAP-5** — Starting grant
  - **intent:** A new player begins with a granted balance.
  - **success:** First-run load (no saved data) yields the default grant.

- **CAP-6** — Consistent purchase (B2)
  - **intent:** An upgrade purchase never leaves coins durably spent without the upgrade persisted.
  - **success:** With a forced upgrade-save failure, the coins are refunded and the index reverts, and `TryUpgrade` reports `Applied=false`.

## Constraints

Each cites the governing spine AD (see the adopted `ARCHITECTURE-SPINE.md`).

- **Durable stateful service (AD-1).** `WalletService` (`IWalletService` + VContainer `IAsyncStartable`) is the single owner of live `PlayerWalletData`; it loads on `StartAsync` (fallback `CreateDefault`) and persists every mutation via `IDataSaveSystem<PlayerWalletData>`. It replaces `MockWalletService`.
- **Async, persisted contract (AD-2, supersedes player-upgrades AD-6).** `IWalletService { int Coins { get; }` (sync read); `UniTask<bool> TrySpend(int)`; `UniTask AddCoins(int)`; `event Action Changed }`. A mutation persists before it is treated as committed; negative amounts are rejected.
- **Data & save (AD-3).** `PlayerWalletData { int Coins }`; `PlayerWalletSaveSystem : PlayerPrefsDataSaveSystem<PlayerWalletData>` (key `miraculoussurfers.player_wallet`, `v1`), `CreateDefault` = starting grant (const, default `1000`, tunable), `IsValid` = `Coins >= 0`. Added through the save-system extensibility pattern (CAP-5) — no change to the save core.
- **B2 refund integrity (AD-4, supersedes player-upgrades AD-7 D1).** `TryUpgrade` awaits `TrySpend` → bumps index → awaits upgrade `SaveAsync`; on save failure it awaits `AddCoins(price)` (refund) and reverts the index, returning `Applied=false`. Both sides persist or neither; the refund's own save is best-effort (logged).
- **Inherited, binding.** Save-system invariants (stateless S1, `UniTask<IResult<T>>`, Newtonsoft, key-suffix versioning with no migration, VContainer) and CLAUDE.md conventions (no comments, `var`, underscore privates, UniTask not coroutines, `int` coins, code under `Assets/MiraculousSurfers/`).

## Non-goals

- Coin HUD UI — out of scope; the seam is `IWalletService.Changed` + `Coins`.
- Routing `Changed` through the `GameplayEventBus` (added in parallel) — the wallet keeps a direct C# event for now.
- Multi-currency (single `int` coins).
- Economy balancing / earning sources — this is the ledger operations only.

## Success signal

A player's coins persist across restarts; spending is affordability-gated and the debit is on disk before it counts; crediting via `AddCoins` persists and notifies; a new player is granted a starting balance; and an upgrade purchase is all-or-nothing — a forced upgrade-save failure refunds the coins and reverts the index. Demonstrable with an edit-mode/play test driving `TrySpend`/`AddCoins` over the in-memory save backend and a forced-failure purchase.

## Assumptions

- Starting grant is `1000` coins (placeholder constant in `PlayerWalletSaveSystem.CreateDefault`), tunable.

## Open Questions

- Confirm/tune the starting grant value.
- Should balance-change notification route through the `GameplayEventBus` (now present) instead of `IWalletService.Changed`?
