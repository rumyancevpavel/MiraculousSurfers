---
id: SPEC-event-bus
companions: [gameplay-events.md]
sources: []
---

> **Canonical contract.** This SPEC and the files in `companions:` are the complete, preservation-validated contract for what to build, test, and validate. Source documents listed in frontmatter are for traceability — consult them only if you need narrative rationale or prose color this contract intentionally omits.

# Event Bus & Gameplay Events

## Why

A vision to realize: gameplay systems in MiraculousSurfers should communicate without holding direct references to each other, so independently-built units (a coin pickup, the wallet, score UI, VFX) stay consistent as the game grows. Direct wiring couples producers to every consumer and makes each new reaction a change to the producer. A typed, enum-keyed publish/subscribe bus — ported from the proven implementation in `hg-mobile-tower-of-colors` and provided through the project's existing VContainer DI — gives producers one call to raise an event and lets any number of consumers react independently. The immediate driver is the coin economy: collecting a coin must update the wallet and UI with no link between the pickup and those consumers.

## Capabilities

- **CAP-1** — Publish a typed event
  - **intent:** A system raises a gameplay event by enum key, passing a strongly-typed payload, so every interested system can react.
  - **success:** Every handler currently subscribed to that key is invoked with the payload, in subscription order; a handler that throws is logged and does not prevent the remaining handlers from running.
- **CAP-2** — Subscribe and unsubscribe
  - **intent:** A consumer registers a handler against an enum key and can later remove it.
  - **success:** The handler fires on every matching publish until removed; subscribing the same handler twice does not cause a double invocation; unsubscribing an unknown handler is a safe no-op.
- **CAP-3** — Per-key payload-type safety
  - **intent:** Each enum key is bound to exactly one payload type, so a key cannot be used with the wrong payload.
  - **success:** The first subscribe or publish binds the key's payload type; a later subscribe or publish on that key with a different payload type throws `InvalidOperationException` rather than silently doing nothing.
- **CAP-4** — DI-scoped bus lifetime
  - **intent:** A concrete bus is shared by all producers and consumers within a scope.
  - **success:** Resolving `GameplayEventBus` anywhere inside `GameplayLifetimeScope` yields the same singleton instance, so a publish from one injected reference reaches handlers registered through another.
- **CAP-5** — CoinCollected event
  - **intent:** Collecting a coin raises a `CoinCollected` event carrying the amount collected.
  - **success:** A subscriber to `GameplayEventType.CoinCollected` receives the `Amount` each time a coin is collected, with no direct reference between the pickup and the subscriber.

## Constraints

- Buses are keyed by a `struct, Enum` type; exactly one payload type per key, enforced at runtime by throwing on mismatch (one generic base `EventBus<T>`, concrete buses per enum).
- Payloads are immutable `readonly struct`s implementing the marker interface `IEvent`; no class payloads.
- A handler exception is caught and logged via `Debug.LogException` inside `Publish` and never propagates out, so one bad subscriber cannot break a publish.
- Dispatch snapshots the current handlers into a pooled buffer before invoking, so subscribing or unsubscribing during a publish is safe and the steady state allocates nothing per publish.
- Buses are provided as VContainer (1.19) singletons within a `LifetimeScope`; they are not static or globally accessible.
- Subscribers own their lifecycle and must `Unsubscribe`; there are no weak references and no automatic handler cleanup.
- Project conventions bind the code: no comments, no coroutines (UniTask only for async), underscore-prefixed private fields, `var` where permitted, all code under `Assets/MiraculousSurfers/`; Unity 6000.6 / URP.

## Non-goals

- No static or global singleton event access — buses are reachable only through DI.
- No async or awaitable handlers, and no ordering guarantee beyond subscription order.
- No event buffering, replay, or persistence: a publish with zero subscribers is dropped, and nothing is retained across scenes.
- No editor-wired `UnityEvent` inspector hookups — the bus is code-only.
- No handler priorities, payload filtering, or once-only subscriptions.
- Not a replacement for direct method calls where coupling between two systems is acceptable.

## Success signal

A coin pickup publishes `CoinCollected(amount)` and the wallet and score UI update in response, with no compile-time reference between the pickup and either consumer. Adding a new gameplay event requires only a new enum entry plus a payload struct (and a new concrete bus only for a new concern) — never a change to `EventBus<T>`. Misusing a key with the wrong payload throws on the first publish or subscribe during play testing instead of failing silently.

## Assumptions

- A single `GameplayEventBus` (one `GameplayEventType` enum, currently just `CoinCollected`) suffices for now; splitting into concern-specific buses is deferred until the event count warrants it.
- `CoinCollected.Amount` is an `int` count of coins, matching the wallet's integer coin balance.

## Open Questions

- One `GameplayEventBus` for all gameplay events, or split by concern as the reference project did (`GameplayEventBus` + `GameScoreEventBus`)? Currently single.
- Does `CoinCollected` need more than `Amount` — e.g. a world `Position` for VFX/SFX, or a source id? Currently `Amount` only.
- Should a bus auto-`Clear()` on scope disposal, or is manual `Clear` the contract and subscribers are fully responsible for unsubscribing? Currently manual.
