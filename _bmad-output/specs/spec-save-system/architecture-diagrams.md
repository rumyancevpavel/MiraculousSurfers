# Save System — Architecture Diagrams

Companion to `SPEC.md`. The whole design is one idea — a single `ReadRaw`/`WriteRaw` seam — wearing many hats.

## Class hierarchy

```mermaid
classDiagram
    class IResult~T~ {
        <<interface>>
        +bool Success
        +T Value
        +Exception Error
    }
    class IDataSaveSystem~TData~ {
        <<interface>>
        +TData CreateDefault()
        +UniTask~IResult~TData~~ LoadAsync(ct)
        +UniTask~IResult~bool~~ SaveAsync(data, ct)
    }
    class DataSaveSystem~TData~ {
        <<abstract>>
        #string Key*
        #string Version*
        #string StorageKey
        +TData CreateDefault()*
        #bool IsValid(data)
        #IResult~string~ ReadRaw(key)*
        #IResult~bool~ WriteRaw(key, json)*
    }
    class PlayerPrefsDataSaveSystem~TData~ {
        <<abstract>>
        #ReadRaw(key)  // PlayerPrefs
        #WriteRaw(key, json)
    }
    class InMemoryDataSaveSystem~TData~ {
        <<abstract>>
        #ReadRaw(key)  // Dictionary
        #WriteRaw(key, json)
    }
    class PlayerUpgradesSaveSystem {
        #Key = "miraculoussurfers.player_upgrades"
        #Version = "v1"
        +CreateDefault()  // all = 1
        #IsValid(data)    // all > 0
    }
    class PlayerUpgradesData {
        +float LaunchPower
        +float Speed
        +float SteeringResponsiveness
        +float CollectibleValue
    }

    IDataSaveSystem~TData~ <|.. DataSaveSystem~TData~
    DataSaveSystem~TData~ <|-- PlayerPrefsDataSaveSystem~TData~
    DataSaveSystem~TData~ <|-- InMemoryDataSaveSystem~TData~
    PlayerPrefsDataSaveSystem~TData~ <|-- PlayerUpgradesSaveSystem
    PlayerUpgradesSaveSystem ..> PlayerUpgradesData : persists
    DataSaveSystem~TData~ ..> IResult~T~ : returns
```

- `*` marks `abstract` members a concrete/backend subclass must fill.
- The base owns **all** version-key building, Newtonsoft (de)serialize, and `Result` wrapping. Backends fill only `ReadRaw`/`WriteRaw`. A concrete type fills only `Key`, `Version`, `CreateDefault`, and (optionally) `IsValid`.

## Load flow (where the seam decides behaviour)

```mermaid
flowchart TD
    A[LoadAsync] --> B[ReadRaw StorageKey]
    B -->|Fail| F[Result.Fail ex]
    B -->|Ok, value null/empty| D[Result.Ok CreateDefault]
    B -->|Ok, value present| C{JsonConvert.DeserializeObject}
    C -->|throws| F
    C -->|null| D
    C -->|ok| E[Result.Ok data]
```

- **Absent key → `Ok(default)`** (CAP-4, first-run safe).
- **Corrupt/throw → `Fail(ex)`** (CAP-2); caller falls back to `CreateDefault()` (F2a default-and-overwrite).
- `StorageKey = $"{Key}.{Version}"` — bump `Version` and the read targets a new key (CAP-3, data abandoned, no migration).

## The one seam, many hats

Every "flexible/extendable" property is latent in the single `ReadRaw`/`WriteRaw` hole:

- **New data type** = a new concrete subclass (CAP-5).
- **Backend swap** = a new abstract base over the same hole (CAP-6).
- **Test mock** = the in-memory base, ~2 lines, sharing the real serialize/version path (CAP-6).
- **Deferred trio** — injected `IStorageBackend`, backup-copy corrupt-recovery (F2c), file/cloud backend — all hang off this same seam, so none needs a redesign later.
