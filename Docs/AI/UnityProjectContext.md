# Unity Project Context

<!-- unity-onboarding:generated:start -->

## Project Summary

- Project root: `C:/Users/ahmet/towerdefenseproject`
- Last analyzed: 2026-07-30
- Last analyzed commit: `56bacdef37421e40a4290c7638e073e566652979`
- Small runtime-composed 2D tower-defense prototype. `GameBootstrap` adds all gameplay systems to an otherwise minimal scene after load.

## Confirmed Environment

- Unity version: 6000.5.5f1
- Render pipeline: Universal Render Pipeline 17.6 with 2D packages installed; project graphics settings include URP global settings.
- Input system: Input System 1.19, new input backend active (`activeInputHandler: 1`), with `Assets/Settings/InputSystem_Actions.inputactions`.
- Target platform: Windows Editor; player target has not yet been validated.

## Important Packages And Frameworks

| Area | Finding | Confidence | Evidence |
| --- | --- | --- | --- |
| Rendering | URP 17.6 and Unity 2D package set | Confirmed | `Packages/manifest.json`, `ProjectSettings/GraphicsSettings.asset` |
| Input | Input System 1.19; UI and Player action maps exist | Confirmed | `Packages/manifest.json`, `Assets/Settings/InputSystem_Actions.inputactions` |
| UI | uGUI 2.5 and TextMeshPro runtime APIs through the uGUI package | Confirmed | `Packages/manifest.json` |
| Tests | Unity Test Framework 1.7 installed; no first-party tests or test assemblies found | Confirmed | `Packages/manifest.json`, repository search |
| Unity MCP | CoplayDev Unity MCP package installed and one Editor instance connected | Confirmed | `Packages/manifest.json`, `mcpforunity://instances` |
| Networking | No first-party networking implementation found | Confirmed | package and code inspection |

## Directory Structure

| Path | Purpose | Confidence | Evidence |
| --- | --- | --- | --- |
| `Assets/_Project/Scripts/Core` | Runtime composition and run state | Confirmed | `GameBootstrap.cs`, `GameManager.cs` |
| `Assets/_Project/Scripts/Managers` | Path and wave orchestration | Confirmed | `PathManager.cs`, `WaveSpawner.cs` |
| `Assets/_Project/Scripts/Towers` | Tower placement and attack behavior | Confirmed | first-party scripts |
| `Assets/_Project/Scripts/Enemies` | Enemy movement, health, and damage | Confirmed | first-party scripts |
| `Assets/_Project/Scripts/UI` | Runtime HUD | Confirmed | `RunHud.cs` |
| `Assets/_Project/Art` | Figma-derived UI, skill-tree, and background assets | Confirmed | asset inventory |
| `Assets/Scenes` | Build scene | Confirmed | Build Settings and repository |

## Assembly Boundaries

| Assembly | Responsibility | Key references | Notes |
| --- | --- | --- | --- |
| `Assembly-CSharp` | All first-party runtime code | UnityEngine, Input System, uGUI/TMP | No `.asmdef` or `.asmref` files exist |

## Scenes And Startup Flow

- Build scenes: `Assets/Scenes/SampleScene.unity` (enabled, index 0).
- Startup scene: `SampleScene`.
- Scene loading flow: `GameBootstrap.Create` runs after scene load, creates a `Tower Defense Game` object, and composes runtime systems. Restart reloads the active scene.

## Architecture

| Pattern | Finding | Confidence | Evidence |
| --- | --- | --- | --- |
| Composition root | `GameBootstrap` constructs all runtime managers and wires them through `Initialize` methods | Confirmed | `GameBootstrap.cs`, `GameManager.cs` |
| Runtime-generated presentation | Board sprites and HUD are created at runtime instead of serialized prefabs | Confirmed | `SpriteFactory.cs`, `PathManager.cs`, `RunHud.cs` |
| Event notifications | A small static `GameEvents` class broadcasts economy/run changes | Confirmed | `GameEvents.cs` and consumers |
| Persistence | JSON meta save stored under `Application.persistentDataPath` | Confirmed | `SaveSystem.cs` |
| Data authoring | Gameplay values are currently hardcoded; ScriptableObject data layer is absent | Confirmed | tower, wave, enemy, and path scripts |

## Coding Conventions

- Namespace style: file-scoped types inside `TowerDefenseIncremental`.
- Serialized fields: `[SerializeField] private` in authored components; prototype gameplay managers are runtime-configured.
- Async: no project async framework or asynchronous gameplay code.
- Comments/docs: concise XML summaries on reusable rendering/bootstrap helpers; gameplay code is lightly documented.
- Formatting: four-space indentation and Allman braces in maintained files; several prototype files are currently minified.

## Testing And Validation

- EditMode tests: none found.
- PlayMode tests: none found.
- CI/build validation: no repository CI or custom build scripts found.
- Current baseline: connected Editor idle in `SampleScene`; no Console errors or warnings at onboarding time.

## Available Unity Tooling

| Capability | Status | Evidence |
| --- | --- | --- |
| `unity.connection.status` | available | active instance `towerdefenseproject@a082f35a` |
| `unity.editor.version` | available | editor-state resource |
| `unity.console.read` | available | baseline Console read succeeded |
| `unity.scene.list` | available | loaded-scene/build-settings tools |
| `unity.scene.inspect` | available | scene hierarchy tool |
| `unity.buildsettings.read` | available | build-settings read succeeded |
| `unity.gameobject.inspect` | available | Unity MCP resource/tool group |
| `unity.asset.search` | available | Unity MCP asset tool group |
| `unity.package.read` | available | repository and Unity MCP evidence |
| `unity.tests.list` | available | Unity Test Framework and MCP test tools |
| `unity.tests.run` | available | Unity MCP test runner |
| `unity.playmode.read` | available | editor-state resource |
| `unity.profiler.read` | available | Unity MCP profiler tools |

## Important Constraints

- Preserve runtime bootstrap behavior unless a serialized scene composition is intentionally introduced.
- Serialized asset and input-action changes require Unity import and Console validation.
- There is no existing Press Start 2P TMP asset or UI prefab library.
- Existing Part 1 art changes are uncommitted user-requested work and must be preserved.

## Unknowns And Confidence

- Player build readiness and runtime UX have not yet been validated.
- There are no automated gameplay tests, performance budgets, or saved legacy fixtures.
- Exact target distribution platform beyond PC/Windows is not documented.

## Source Files Inspected

- `ProjectSettings/ProjectVersion.txt`
- `ProjectSettings/GraphicsSettings.asset`
- `ProjectSettings/ProjectSettings.asset`
- `Packages/manifest.json`
- `Assets/Settings/InputSystem_Actions.inputactions`
- `Assets/_Project/Scripts/**/*.cs`
- Unity Editor state, Console baseline, active scene, and Build Settings

<!-- unity-onboarding:generated:end -->
