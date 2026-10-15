# Save System — Stack & Conventions

Companion to `SPEC.md`: the concrete HOW the kernel keeps out of its intents. Reflects the code as built.

## Files (all under `Assets/MiraculousSurfers/Scripts/`)

| File | Namespace | Role |
| --- | --- | --- |
| `Core/Result.cs` | `MiraculousSurfers.Core` | `IResult<out T>` (`Success`/`Value`/`Error`), `readonly struct Result<T>`, static `Result.Ok<T>` / `Result.Fail<T>(Exception)` |
| `Data/IDataSaveSystem.cs` | `MiraculousSurfers.Data` | generic contract: `CreateDefault`, `LoadAsync`, `SaveAsync` → `UniTask<IResult<T>>` |
| `Data/DataSaveSystem.cs` | `MiraculousSurfers.Data` | abstract base: `StorageKey = $"{Key}.{Version}"`, `JsonConvert` (de)serialize, `Result` wrap; abstract `ReadRaw`/`WriteRaw` + `Key`/`Version`/`CreateDefault`; virtual `IsValid` |
| `Data/PlayerPrefsDataSaveSystem.cs` | `MiraculousSurfers.Data` | backend: `PlayerPrefs` — absent key → `Ok(null)`, read throw → `Fail`; `SetString` + `Save()` |
| `Data/InMemoryDataSaveSystem.cs` | `MiraculousSurfers.Data` | backend: `Dictionary<string,string>` for tests |
| `Data/PlayerUpgradesData.cs` | `MiraculousSurfers.Data` | plain class, public `int` auto-properties = upgrade indices (Newtonsoft-serializable) |
| `Data/PlayerUpgradesSaveSystem.cs` | `MiraculousSurfers.Data` | concrete: key `miraculoussurfers.player_upgrades`, version `v1`, defaults all `0`, `IsValid` = all `>= 0` |
| `Gameplay/GameplayLifetimeScope.cs` | `MiraculousSurfers.Gameplay` | VContainer `LifetimeScope`; registers the binding below |

No `.asmdef` in the project — everything compiles into `Assembly-CSharp`, so UniTask / VContainer / Newtonsoft are all available without assembly wiring.

## Dependencies

- **Newtonsoft** — `com.unity.nuget.newtonsoft-json` `3.2.2`, added explicitly to `Packages/manifest.json` (was only transitively present). The AOT-safe Unity fork; ships its own `link.xml`.
- **UniTask** — `com.cysharp.unitask` `2.5.11` (`Cysharp.Threading.Tasks`).
- **VContainer** — `jp.hadashikick.vcontainer` `1.19.0`.

## DI registration

```csharp
protected override void Configure(IContainerBuilder builder)
{
    builder.Register<IDataSaveSystem<PlayerUpgradesData>, PlayerUpgradesSaveSystem>(Lifetime.Singleton);
}
```

- `Register<TInterface, TImplement>(Lifetime)` is confirmed present in VContainer 1.19.0.
- The scope lives on a `GameplayLifetimeScope` GameObject in `Assets/Scenes/Gameplay.unity` (scene root). A service is resolvable only for objects *inside* the scope — a new consumer must be registered in this same scope to receive injection.

## Adding a new persisted type (the extensibility recipe — CAP-5)

1. Write a Newtonsoft-serializable data class (public properties).
2. Write `class FooSaveSystem : PlayerPrefsDataSaveSystem<FooData>` declaring `Key`, `Version`, `CreateDefault()`, optionally `IsValid`.
3. Register `IDataSaveSystem<FooData> → FooSaveSystem` in the appropriate `LifetimeScope`.
4. For tests, a parallel `: InMemoryDataSaveSystem<FooData>` concrete (or reuse) exercises the real serialize/version path.

## Build / AOT note (F1)

Mobile is IL2CPP. The Newtonsoft Unity package's bundled `link.xml` plus direct code references to data types (constructed in `CreateDefault`) are expected to cover stripping. Confirm on the first on-device build; if a type is ever (de)serialized purely via reflection with no direct construction, add it to a project `link.xml` or mark `[Preserve]`.

## Verified on build (2026-10-15)

- Compiles with 0 errors / 0 warnings (`Assembly-CSharp`).
- `IDataSaveSystem<PlayerUpgradesData>` resolves to `PlayerUpgradesSaveSystem` through a built VContainer.
- Default `PlayerUpgradesData` is four `int` indices defaulting to `0` and round-trips through Newtonsoft (revised from floats by player-upgrades AD-2).
- `GameplayLifetimeScope` GameObject present in `Gameplay.unity`, scene saved.
