# Character Running — Movement Model, Contract, and Component Map

Companion to `SPEC.md`. Holds the fuel/momentum math, the per-frame update order, the launch/obstacle/reset contract, the Unity component map, and the level-prefab layout.

## Fuel-based momentum

One reserve value drives the whole run. Charge strength seeds it; draining it *is* decelerating.

```
initialFuel = f(chargeStrength)        // mapping is tuning (linear or curve)
fuel        = initialFuel              // on Launch
speed       = fuel * k                 // forward speed is proportional to remaining fuel
drainRate   = curve.Evaluate(fuel / initialFuel)   // curve is a function of fuel fraction
```

Each tick: `fuel -= max(drainRate, minDrain) * dt`. Because `speed ∝ fuel`, a full tank starts fast and drains for longer, so **one seeded value scales both peak speed and duration** — no separate peak/duration curves. A slowing obstacle does `fuel -= penalty`, which drops speed immediately and permanently. The run ends (`MomentumEnded`) when `fuel ≤ 0`.

**Termination guard:** the drain curve must be `> 0` across `[0,1]`, or apply `minDrain`; otherwise fuel plateaus where `drainRate == 0` and the run never ends.

## Per-frame update order (while running)

1. Read steering input → set `targetX` (lateral offset), **clamp between the left/right boundary transforms**.
2. Ease current lateral position toward `targetX` (lerp).
3. Compute `forward = heading * speed` (heading seeded by aim direction at Launch).
4. Build the frame velocity (forward + lateral delta) and call `CharacterController.Move(velocity * dt)`.
5. `TraveledDistance += move.magnitude` (the actual applied delta, lateral included).
6. Drain fuel; if `fuel ≤ 0` raise `MomentumEnded(TraveledDistance)`.

Steering, boundary enforcement, and collision safety are the same mechanism (step 1's clamp); the `CharacterController`'s collide-and-slide is only a backstop.

## Launch / event / reset contract

Extends the `Character` actor contract in `spec-gameplay-state-machine` (`state-machines.md`). This spec defines the concrete behavior behind the stub.

| Member | Kind | Meaning |
|---|---|---|
| `Launch(float power, Vector3 direction)` | command | Seed `initialFuel` from `power`, set `heading = direction`, reset `TraveledDistance = 0`, begin running |
| `TraveledDistance` | state (per-run) | Total path length; reset on `Launch` |
| `SlowedDown` | event | Raised when a slowing obstacle trigger deducts fuel |
| `Crashed` | event | Raised when a deadly obstacle trigger is entered |
| `MomentumEnded(float distance)` | event | Raised when `fuel ≤ 0` |
| `ResetToSpawn()` *(new)* | command | `cc.enabled=false` → set `transform.position/rotation = spawn` → `cc.enabled=true`; zero fuel/distance/`targetX`. Called behind the state machine's fade. |

## Obstacles

Authored in the level prefab, detected by **trigger colliders** (no physics response):

| Class | On trigger enter | Effect |
|---|---|---|
| Slowing | `fuel -= penalty`; raise `SlowedDown` | Visible deceleration; run continues |
| Deadly | raise `Crashed` | Run ends → Crash state |

Serialized per obstacle: its class and (for slowing) the fuel penalty.

## Level prefab layout

Each level is a prefab (swap the prefab to swap the level — CAP-7):

- Track mesh / geometry.
- **Left and right boundary transforms** defining the lateral extent (clamp targets for `targetX`).
- **Spawn point** transform (the reset/initial pose).
- Obstacles (slowing / deadly trigger colliders) — later.

Later (non-goal now): a *series* of boundary pairs along the track gives variable width; multiple prefabs give multiple levels.

## Unity component map

| Concern | Unity component / API |
|---|---|
| Forward + lateral motion | `CharacterController.Move()` (kinematic, collide-and-slide backstop) |
| Momentum shaping | `AnimationCurve` (serialized; drain rate over fuel fraction) + `k` float |
| Steering input | Input System — `InputActionReference` (lateral axis), read in `Update` |
| Boundaries | `Transform` refs (empty GameObjects) for left/right edges → clamp `targetX` |
| Distance | plain accumulation on the `Character` (`TraveledDistance`) |
| Obstacles | Colliders set `isTrigger` + layers/tags → `OnTriggerEnter` → raise events |
| Run camera | owned by `spec-gameplay-state-machine` (dedicated chase `CinemachineCamera`) |
| Async / timing | UniTask (no coroutines, per project convention) |

Target runtime: Unity 6000.6.0f1 (URP), Input System 1.20.0, UniTask 2.5.11. Code lives under `Assets/MiraculousSurfers/Scripts/Gameplay/` in namespace `MiraculousSurfers.Gameplay.Characters` (concrete subclass of `Character`).
