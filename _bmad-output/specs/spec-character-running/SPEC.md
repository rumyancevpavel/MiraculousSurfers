---
id: SPEC-character-running
companions: [movement-model.md, ../spec-gameplay-state-machine/SPEC.md]
sources: []
---

> **Canonical contract.** This SPEC and the files in `companions:` are the complete, preservation-validated contract for what to build, test, and validate. Source documents listed in frontmatter are for traceability — consult them only if you need narrative rationale or prose color this contract intentionally omits.

# Character Running (Flight-Phase Movement)

## Why

A vision to realize. After the slingshot launch, the surfer's flight is the payoff of the whole loop — but MiraculousSurfers deliberately does *not* want Unity physics driving it: the developer wants every bit of the run (forward momentum, how it decays, how steering feels, where the track edges are) to be a hand-tunable number so the game feel can be dialed in and re-dialed without a rewrite. This spec defines the Character as a self-driving, fully parametrizable kinematic actor that the gameplay state machine commands and reacts to — the concrete movement behind the `Character` stub the state-machine spec already contracts against. Getting a clean, inspector-tunable movement model now is what keeps feel iteration and new levels cheap later.

## Capabilities

- **CAP-1** — Auto-run forward
  - **intent:** After `Launch(power, direction)` the character continuously runs forward under its own kinematic drive with no per-frame input; charge strength seeds initial speed/duration and aim direction seeds the heading.
  - **success:** On Launch the character advances along the aim heading and keeps moving with zero steering input until momentum ends.

- **CAP-2** — Lateral steering
  - **intent:** The player controls only left/right; input sets a target lateral offset the character eases toward each frame (target-X lerp).
  - **success:** Left/right input moves the character sideways smoothly toward the input offset; forward motion is unaffected; with no input there is no lateral drift.

- **CAP-3** — Stay within bounds
  - **intent:** The character cannot leave the playable lateral extent defined by the level's left/right boundary transforms.
  - **success:** Driving hard into an edge pins the character at the boundary; it never crosses or clips past the track edge.

- **CAP-4** — Tunable decaying momentum
  - **intent:** Forward speed is governed by a fuel reserve (`speed = fuel × k`) that drains per a designer-authored `AnimationCurve` until it reaches zero, ending the run (`MomentumEnded`).
  - **success:** With no obstacles a run decelerates to a stop purely from the curve; reshaping the curve or `k` in the inspector changes the feel with no code change; a harder charge (more initial fuel) runs both faster and longer.

- **CAP-5** — Obstacle reactions
  - **intent:** Slowing obstacles bleed fuel (feedback via `SlowedDown`); deadly obstacles end the run (`Crashed`); both detected by trigger colliders.
  - **success:** Entering a slowing obstacle's trigger drops fuel (visible deceleration) without ending the run; entering a deadly obstacle's trigger raises `Crashed` and the run ends.

- **CAP-6** — Distance tracking
  - **intent:** `TraveledDistance` accumulates total path length (sum of per-frame Move-delta magnitudes, lateral included) and resets on `Launch`.
  - **success:** `TraveledDistance` increases every frame the character moves, weaving adds to it, and it reads 0 immediately after a new `Launch`.

- **CAP-7** — Per-level layout via prefab
  - **intent:** A level is a prefab holding the track mesh, left/right boundary transforms, and spawn point; swapping the prefab swaps the level.
  - **success:** A new level prefab with different geometry/boundaries/spawn runs with zero code change to the character or the states.

- **CAP-8** — Clean respawn
  - **intent:** The character can be reset to its spawn position/state so a new run starts from the initial pose.
  - **success:** After a reset the character stands at spawn with fuel/distance/steer-target zeroed and the `CharacterController` in a valid state, ready for the next `Launch`.

## Constraints

- Movement is script-driven and kinematic via `CharacterController.Move()`. No Unity physics forces, no `Rigidbody`, no realistic bounce/collision response. Every movement number must be inspector-parametrizable.
- The Character is the self-driving actor — a concrete subclass of the existing `Character.cs` stub. The state machine never drives physics: it only calls `Launch(power, direction)` and reacts to `SlowedDown` / `Crashed` / `MomentumEnded`. (Honors `spec-gameplay-state-machine`.)
- Momentum: `speed = fuel × k`; each frame `fuel -= curve.Evaluate(fuel / initialFuel) × dt`; `initialFuel` is seeded by charge strength so one value scales both peak speed and run duration; slowing obstacles deduct fuel directly; `MomentumEnded` fires at `fuel ≤ 0`.
- The drain curve must stay `> 0` across its whole range (or apply a min-drain floor): otherwise fuel plateaus and `MomentumEnded` never fires. Guaranteed termination is required.
- Steering is a target-X lerp **clamped to the left/right boundary transforms before the move** — one mechanism serving steering, boundary enforcement, and collision safety, with `CharacterController` collide-and-slide only as a backstop.
- Distance is the sum of per-frame `Move`-delta magnitudes (lateral included), exposed as `TraveledDistance`, reset to 0 on `Launch`.
- Obstacles are authored in the level prefab and detected via trigger colliders — no physics collision response.
- Respawn resets safely: toggle `CharacterController.enabled` false around writing `transform.position/rotation = spawn`, then re-enable; also zero fuel, distance, and steer-target. Done on command (behind the state machine's fade).

## Non-goals

- Real drag-based Charge — a stub `ChargeState` with serialized power/direction stands in for now (owned by `spec-gameplay-state-machine`).
- Animations — the character is a bare prefab in this scope.
- Variable-width tracks (a series of boundary pairs) and multiple level layouts beyond one — the prefab-per-level architecture allows them; building them is later.
- Unity physics simulation (no `Rigidbody` forces/bounce), the rewards sub-flow, and cross-session persistence.

## Success signal

In the Gameplay scene, a launched character auto-runs forward along its aim heading; the player steers it left/right within the track without ever crossing the edges; `TraveledDistance` ticks up by total path; forward momentum decays to a stop purely from the tunable fuel curve — or a slowing obstacle visibly saps it, or a deadly one ends the run — after which the character resets cleanly to spawn for another go. All of it runs without Unity physics and every number is tunable from the inspector.

## Assumptions

- Charge strength and aim direction arrive via the existing `Character.Launch(power, direction)` / `ShotState.Configure(power, direction)` contract.
- `k` (fuel→speed) and the drain `AnimationCurve` are inspector fields; exact values and the `chargeStrength → initialFuel` mapping are tuning, not fixed here.

## Open Questions

- Exact `chargeStrength → initialFuel` mapping (linear, curve, clamps)? Left for tuning.
- Total-path distance lets weaving inflate the score — intended risk/reward, or should lateral be capped/weighted? Parked in the brainstorm, undecided.
- Steering responsiveness while slowed: is the target-X lerp rate constant, or scaled by current forward speed?
