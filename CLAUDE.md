# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project state

Unity **6000.6.0f1** project using **URP** (17.6). The active pipeline asset is `PC_RPAsset`; there are separate PC and Mobile URP assets. It also uses the new **Input System**.

## Coding conventions

- **No comments in generated code.** Do not add comments to code you write; keep names self-explanatory instead.
- **Never use coroutines** (`IEnumerator` / `StartCoroutine` / `yield`). Use **UniTask** for all async work.
- **Private field naming:** private instance data fields always start with an underscore, e.g. `_transitionCts` — **including `[SerializeField] private` fields** (e.g. `_initialState`; Unity still shows a clean inspector label).

## Validation

- **Don't make visual validations yourself.** Do not capture the game/scene view and judge whether a result looks correct on your own. Instead, set up the state to be checked, then **ask the user to look and approve or disapprove**, and wait for their verdict before treating it as validated. (Programmatic checks — compile status, transforms, positions, state values, console errors — are still yours to verify.)

## Driving the Unity Editor with `unity` CLI

The `unity` CLI (`~/.unity/bin/unity`) controls the **running** Editor through the `com.unity.pipeline` package (Editor server on port 7800). Use it to change scenes, prefabs, assets and settings instead of hand-editing `.unity`, `.prefab` or `.asset` YAML. The project-local skills `.claude/skills/unity-cli` and `.claude/skills/unity-pipeline` have the full reference.

```bash
unity --no-banner status                          # connected editors; state must be "ready"
unity --no-banner pipeline list                   # package version, server reachable, Safe Mode
unity --no-banner command                         # list all editor commands + params (--query <term> to filter)
unity --no-banner command <name> --param value    # run one
unity --no-banner command console --tail 20 --level error --result-only
```

- Add `--json` or `--result-only` when you need to parse the output. Add `--project-path <path>` if more than one Editor is open. `--timeout <s>` defaults to 30.
- Keep **autotick** on so the Editor keeps working while it's in the background: `unity command set_autotick --enable true`. It's already enabled.
- Commands that change settings or delete things need `--confirm true`. Most of them accept `--dry_run true`.
- **Async commands must be polled.** `recompile` → `recompile_status`, `run_tests --async_tests` → `test_status`, `build` → `build_status`. Connection errors during a domain reload are expected.
- **Adding a new script:** run `create_script` (or write the file), then `recompile`, then poll `recompile_status`, then `attach_script`. A type doesn't exist until the recompile finishes.
- **Arbitrary C#:** `unity command eval '<code>'`, `eval_file --file x.cs`, or `run_script` for bulk building with a static entry point.
- **Multi-step edits:** `batch --operations '[...]'` runs them as one Undo step. Later operations can reference earlier results with `"$0.instanceId"`.
- **Play and verify:** `editor_play` / `editor_stop`, `simulate_key`, `simulate_pointer`, `capture_game_view --save_path ...`, `console`.
- **If a command hangs,** check `editor_status`. `blocked_by_dialog` means a modal dialog is open; tell the user, because it can't be clicked from the CLI.
- **If the CLI can't connect,** the Editor probably started in **Safe Mode** because of compile errors. Run `unity pipeline list` to confirm, fix the errors, then restart Unity.

## Build & test

```bash
unity --no-banner test --mode EditMode --filter <TestName>                # headless run -> test-results.xml (exit 8 = tests failed)
unity --no-banner command run_tests --mode EditMode --filter <TestName>   # in the open Editor
unity --no-banner command build --dry_run true                            # validate a player build
```
