---
id: SPEC-save-system
companions:
  - architecture-diagrams.md
  - stack.md
  - ../../../CLAUDE.md
sources: []
---

> **Canonical contract.** This SPEC and the files in `companions:` are the complete, preservation-validated contract for what to build, test, and validate. Source documents listed in frontmatter are for traceability — consult them only if you need narrative rationale or prose color this contract intentionally omits.

# Save System — Versioned Typed Persistence

## Why

A vision to realize: MiraculousSurfers needs one reusable foundation for durably storing game state — player progression, upgrades, and whatever comes next — instead of a one-off saver per feature. The force is extensibility: adding a new kind of saved data should cost a small subclass, not a new subsystem. This generalizes the proven Tower-of-Colors `DataLayer` pattern (per-type interface, swappable backend, `Result<T>`, key-suffix versioning) into a shared, DI-resolved stack. It matters now because the first consumer (the upgrades system) is being built, and everything persisted afterward should ride the same rails. The implementation already exists under `Assets/MiraculousSurfers/Scripts/{Core,Data}` with a `GameplayLifetimeScope`; this spec is the contract that code realizes.

## Capabilities

- **CAP-1** — Durable typed persistence
  - **intent:** A caller can save a serializable object and load it back intact across app restarts.
  - **success:** Save an instance, restart the app, load returns an equal instance (round-trips through serialization).

- **CAP-2** — Non-throwing outcomes
  - **intent:** Every load and save returns a result value that carries failure instead of throwing into gameplay code.
  - **success:** A corrupt or failed read yields a failed result (with the exception captured), never a thrown exception; the caller branches on success/failure.

- **CAP-3** — Data-format versioning without migration
  - **intent:** Each persisted type carries a format version; bumping it makes loads read a fresh slot and ignore pre-bump data.
  - **success:** After a version bump, a load returns defaults and the old stored blob is left untouched (abandoned), with no migration step.

- **CAP-4** — First-run safe defaults
  - **intent:** When nothing has been stored yet, a load returns the type's default instance as a success, not a failure.
  - **success:** On a fresh install, loading a type returns its constructed defaults wrapped as a success.

- **CAP-5** — Cheap extensibility
  - **intent:** A new persisted type is added by writing a serializable data class plus a small concrete system declaring its key, version, default, and validation — with no change to the shared base or backends.
  - **success:** A brand-new type becomes fully persistable without editing any shared/core code.

- **CAP-6** — Swappable backend with a production-faithful test double
  - **intent:** A runtime storage backend and an in-memory test backend satisfy the same read/write seam, so tests run a concrete system without touching real storage.
  - **success:** A test drives a concrete system against the in-memory backend and exercises the *real* serialize + version logic (only raw read/write differs).

- **CAP-7** — DI-resolved per-type systems
  - **intent:** Each type's save system is obtained through the DI container as a closed-generic service and injected into its consumers.
  - **success:** Resolving `IDataSaveSystem<PlayerUpgradesData>` yields `PlayerUpgradesSaveSystem` (verified in-editor).

- **CAP-8** — PlayerUpgrades pilot
  - **intent:** Persist the player's bought upgrade state as an upgrade **index** per param — `LaunchPower`, `Speed`, `SteeringResponsiveness`, `CollectibleValue` — each defaulting to `0`. (Revised from float values by the player-upgrades spec, AD-2; the multiplier value is derived from the index by the game-configuration system, not stored here.)
  - **success:** A default load returns all four indices at `0`, and a modified set round-trips.

## Constraints

- **Newtonsoft serialization.** Persisted types must be Newtonsoft-serializable; this is what makes "any serializable type" (dictionaries, nested generics, polymorphism, properties, null) actually hold. Build dependency: `com.unity.nuget.newtonsoft-json` (explicit in the manifest).
- **Exception-only result (R1).** The result type carries an exception, not an error code, so callers cannot branch on failure *cause* — corrupt vs. transient reads are indistinguishable. The chosen posture is **default-and-overwrite (F2a)**: on a corrupt read the base fails and the caller falls back to defaults, so a corrupt save can be overwritten with defaults (possible progress loss). This is a deliberate trade, not an oversight.
- **Key-suffix versioning, no migration.** The format version is part of the storage key; bumping it abandons pre-bump data and leaves orphaned keys (F3). Acceptable pre-launch; post-launch a bump wipes released players' saves.
- **Stateless (S1).** Loads always re-read/deserialize; saves write exactly what they are handed; nothing is cached between calls. Holding a live in-memory instance and deciding save cadence is the consumer's responsibility, not this system's.
- **Inheritance seam, not injected backend (for now).** The abstract base leaves `ReadRaw(key)` / `WriteRaw(key, json)` abstract; a concrete picks its backend by choosing a base class. Backend swap is a code/base choice, not yet a DI swap.
- **UniTask async surface.** Load/save signatures are `UniTask<IResult<T>>` (no coroutines, per project rule); the PlayerPrefs backend is synchronous under that surface.
- **VContainer DI.** Per-type systems register as closed generics in a `LifetimeScope`; a service is only resolvable/injectable for objects *inside* that scope.
- **IL2CPP/AOT build preservation (F1).** The mobile IL2CPP build must preserve `Newtonsoft.Json` and all persisted data types (via `link.xml`/`[Preserve]`) or on-device (de)serialize throws — a required build step.
- **Project conventions** (CLAUDE.md, adopted companion): no comments in code, underscore-prefixed private instance fields (incl. `[SerializeField] private`), UniTask over coroutines, all code under `Assets/MiraculousSurfers/`.

## Non-goals

- Cross-version data migration (a format bump abandons old data by design).
- Encryption, anti-cheat, or tamper protection.
- Multiple save slots / profiles.
- Binary / MessagePack serialization (Newtonsoft JSON only).
- In-memory live-instance caching and a save-cadence/service layer (a separate concern that sits *on top* of this system).
- Future extensions that hang off the same seam but are out of scope now: an injected `IStorageBackend`, backup-copy corrupt-recovery (F2c), and file/cloud backends.

## Success signal

A new serializable type becomes fully persistable by adding only a data class and a small concrete subclass registered in DI: its defaults load on first run, a saved instance round-trips intact after an app restart, and every failure arrives as a result value rather than a thrown exception. Demonstrated today by the pilot: `IDataSaveSystem<PlayerUpgradesData>` resolves to `PlayerUpgradesSaveSystem` via VContainer, a default load returns all four upgrade indices at `0`, the data round-trips through Newtonsoft, and the project compiles with zero errors.

## Assumptions

- F1 (AOT) is likely already covered: `com.unity.nuget.newtonsoft-json` ships its own `link.xml`, and the data types are referenced directly in code (`CreateDefault` constructs them, so they are not stripped). To be confirmed on the first IL2CPP device build.

## Open Questions

- Who is the first consumer of `IDataSaveSystem<PlayerUpgradesData>` — an upgrades/shop controller, `RunningCharacter` reading its multipliers at launch, or a boot-time loader? This decides whether `GameplayLifetimeScope` should also own/register those consumers so VContainer can inject into them.
- Add a "Clear Player Upgrades" editor menu item for testing (Tower-of-Colors had an equivalent `ClearPlayerData`)?
