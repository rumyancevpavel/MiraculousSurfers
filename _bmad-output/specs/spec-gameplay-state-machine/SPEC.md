---
id: SPEC-gameplay-state-machine
companions: [state-machines.md]
sources: []
---

> **Canonical contract.** This SPEC and the files in `companions:` are the complete, preservation-validated contract for what to build, test, and validate. Source documents listed in frontmatter are for traceability — consult them only if you need narrative rationale or prose color this contract intentionally omits.

# Gameplay State Machine

## Why

A vision to realize. MiraculousSurfers is a slingshot-launch surfer game whose core loop — wait, aim, charge, launch, slide, resolve, repeat — is the backbone the whole Gameplay scene hangs off. The developer wants that loop expressed as a **flexible, future-proof state machine** where each phase is a separate component wired from the Unity inspector, so any state can be customized, swapped, or extended later (e.g. adding a reward-collection flow) without reworking the others. Getting this architecture right now is what keeps future gameplay and juice cheap to add instead of forcing a rewrite.

## Capabilities

- **CAP-1** — Start a run
  - **intent:** From the initial Waiting state the camera frames the character and "Tap to begin" shows; a tap advances to Aim.
  - **success:** Before the tap no launch input is accepted; tapping transitions to Aim.

- **CAP-2** — Aim or back out
  - **intent:** In the behind-the-character Aim view the player can return to Waiting or begin a drag to charge.
  - **success:** Back returns to Waiting; starting a drag enters Charge.

- **CAP-3** — Charge and release
  - **intent:** Dragging sets launch power (pull distance) and direction (left/right); easing off reduces tension; releasing past a threshold launches, below it returns to Aim.
  - **success:** Releasing past the threshold launches the character with power and direction matching the drag; releasing below it returns to Aim with no launch.

- **CAP-4** — Flight and outcome routing
  - **intent:** After launch the character slides under its own physics; the machine reacts to the character's events and routes, with no return to pre-shot states.
  - **success:** Crashed routes to Crash; MomentumEnded routes to distance evaluation; SlowedDown produces feedback with no transition; no transition back to Charge/Aim/Waiting occurs during flight.

- **CAP-5** — Evaluate against session best
  - **intent:** When momentum ends, the travelled distance is compared to the session best to choose a new-record vs ordinary-end outcome.
  - **success:** Distance greater than the best routes to New Record and updates the best; otherwise routes to Try Ended.

- **CAP-6** — Resolve and loop
  - **intent:** New Record celebrates and marks the record on the level map, then returns to Waiting; Try Ended returns to Waiting; Crash plays a death animation then returns to Waiting.
  - **success:** Each terminal state completes its presentation and the machine returns to the initial Waiting state, ready for another run.

- **CAP-7** — Editor-configured, extensible states
  - **intent:** Each state is a separate component whose dependencies (next-state references, cameras, UI, animators, input maps, character, score) are wired from the Unity inspector, and states or variants can be added or swapped without changing other states.
  - **success:** Swapping a state's next-state reference, or replacing a state component with a variant (e.g. a rewards-enhanced Try Ended), changes the flow with zero code change to other states; the flow graph is readable in the inspector via the serialized references.

## Constraints

- One active state at a time per machine; parallel behavior is achieved via nested/referenced sub-machines (composition), never concurrent active states.
- Transitions are routed by serialized next-state references on each state (an inspector-visible flow graph); a state calls `machine.GoTo(next)` and its own logic picks which exit (Shot has onCrash/onStop; single-exit states have one reference).
- The machine never drives physics: the character is a self-driving actor that owns movement and animation and emits `SlowedDown`, `Crashed`, and `MomentumEnded(distance)`; the machine only commands (`Launch`) and reacts.
- Input is decentralized per state via Unity Input System Action Maps: each state enables its own map and disables the rest on enter/exit. Mobile-first; tap and drag are the primary inputs.
- Transitions are async via UniTask: a state awaits its presentation (e.g. the camera sweep) before enabling input. Coroutines are forbidden — UniTask only — per project convention.
- Camera uses Cinemachine: one shared `CinemachineCamera` with Orbital Follow, animated between orbit presets (not camera-swapping) to produce the circular Wait↔Aim sweep around the character.
- `SlowedDown` is feedback-only and must never cause a state transition.
- The session best lives in a `ScoreManager`, not on the character; per-run `TraveledDistance` lives on the character and resets each launch.

## Non-goals

- Cross-session persistence of the best distance — a future service will initialise `ScoreManager`.
- Slingshot visuals and art — explicitly deferred.
- Building the nested rewards sub-flow / sub-machines now — the architecture allows a state to reference one, but implementing it is later work.
- The character's internal movement, physics, and animation — owned by the character (a contract stub in this scope).

## Success signal

A player completes the full loop in the Gameplay scene: tap to begin → the camera sweeps around behind the character → drag to aim and charge → release to launch → slide → either crash (death animation) or stop (distance evaluated; a new record is marked on the level map or the try ends) → return to Waiting, ready to go again — all driven by the component state machine. A developer can rewire or extend any single state from the inspector without touching the others.

## Assumptions

- The character prefab exists (or will) as a self-driving actor matching the event + `Launch(power, direction)` contract; it is currently a stub (`Character.cs`).
- Charge computes power/direction and configures Shot, which subscribes before launching — i.e. Shot owns `Launch` and Charge owns the drag math, as built in the scaffold.

## Open Questions

- How does horizontal drag map to a world launch direction (ground-plane projection, sensitivity, clamps)? Left for tuning.
- Should terminal-state completion (the New Record celebration and the Crash death animation) be driven by animation events/signals rather than the scaffold's placeholder fixed delays?
