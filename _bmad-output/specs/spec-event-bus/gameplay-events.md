# Gameplay Event Catalog

The catalog of events carried by `GameplayEventBus` (`EventBus<GameplayEventType>`). Each event is a `readonly struct : IEvent` in `MiraculousSurfers.Gameplay.Events`, bound to one `GameplayEventType` key. Adding an event means: add the enum key, add the payload struct, publish/subscribe by that key — no change to `EventBus<T>`.

## Buses

| Bus | Key enum | Namespace | Lifetime |
|-----|----------|-----------|----------|
| `GameplayEventBus` | `GameplayEventType` | `MiraculousSurfers.Gameplay.Events` | VContainer singleton in `GameplayLifetimeScope` |

## Events

| Event | Key | Payload fields | Published when | Typical subscribers |
|-------|-----|----------------|----------------|---------------------|
| `CoinCollected` | `GameplayEventType.CoinCollected` | `int Amount` | A coin is collected during gameplay | Wallet (credit balance), score/economy UI, pickup VFX/SFX |

## Conventions for adding an event

- Name the payload struct after the fact in the past tense (`CoinCollected`, not `CollectCoin`).
- One payload type per enum key — the bus binds the type on first use and throws on mismatch.
- Keep payloads minimal and immutable: only data a subscriber cannot derive itself.
- Add a concrete `EventBus<TNewEnum>` subclass (and register it as a singleton) only when a new *concern* appears; otherwise extend `GameplayEventType`.
