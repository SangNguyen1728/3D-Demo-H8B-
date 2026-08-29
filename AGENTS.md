# 3ddemo-bida — Agent Instructions

Unity 6 (6000.0.47f1) 3D billiards/pool game with PvP, character skills, and multiple game modes (8-ball, 9-ball).

## Build & Run
- **Open in Unity Hub** → select `3ddemo-bida` folder (Unity 6000.0.47f1 required)
- **Scenes**: `Assets/Scenes/HomeScene.unity` (main menu), `Assets/Scenes/PlayScene.unity` (gameplay), `Assets/Scenes/LoadingScene.unity`
- **Build Target**: StandaloneWindows64 (see `Assembly-CSharp.csproj:54`)

## Key Architecture
- **Entry Points**: `GameManager` (UI flow), `CueStickController` (shot logic), `PocketTowPs` (game rules)
- **Player Managers**: `GlaszekManager` (player 1/2, registered via `PlayerManagerRegistry`), `BeelzitaManager` (skill balls)
- **Registries**: `PlayerManagerRegistry`, `StaminaManagerRegistry`, `BuffManagerRegistry` — static dictionaries keyed by playerNumber
- **URP**: Universal Render Pipeline with separate PC/Mobile renderer assets (`Assets/Settings/*`)

## Code Conventions
- **Language**: C# 9.0 (`.csproj:5`)
- **Avoid** `FindObjectOfType`/`FindFirstObjectByType` in hot paths — use registries or cached refs
- **Coroutines**: Cache `WaitForSeconds` (many allocations in `BeelzitaManager`, `CueStickController`)
- **Events over polling**: Several `Update()` loops poll state that should be event-driven

## Known Performance Issues (fix before adding features)
1. `Aiming.DectectDangerousCircles()` — `FindObjectsOfType<Rigidbody>()` every frame + List rebuilds
2. `PocketTowPs.UpdateNextTarget()` / `HasClearedGroup()` — `FindObjectsByType<BallNo>()` repeatedly
3. `GlaszekManager.NotifyBallStopped()` / `NotifyShotStarted()` — `FindObjectsByType` in callbacks
4. `BeelzitaManager` — repeated `FindFirstObjectByType<PocketTowPs>` / `CueStickController`
5. `TargetBallFinder.GetTargetBallTransform()` — `GameObject.FindGameObjectWithTag()` with string concat

## Testing
- **Test Framework**: `com.unity.test-framework` (NUnit)
- **Run**: Unity Editor → Window → General → Test Runner
- No existing test files found — add under `Assets/Tests/` or `Packages/`

## Common Tasks
| Task | How |
|------|-----|
| Add new character skill | Create script under `Assets/Script/PlayscenePVP1D/{Character}/`, register in `PlayerSkillController` |
| Modify game rules | Edit `PocketTowPs.HandleStrokeResult()` and `NineBallRules` |
| Change UI flow | Modify `GameManager` coroutines or `UIFlowManager` |
| Adjust physics | `CueStickController.hitForceAmount`, `stopThreshold`, `CueBallController` |

## Gotchas
- **Scene loading**: `SceneLoader.Instance.LoadScene()` used throughout — singleton pattern
- **Input**: New Input System + legacy `Input.GetMouseButton` mixed (`CueStickInput`)
- **Visual Scripting**: Some logic in graphs — check `Assets/` for `.asset` graphs
- **PlayerPrefs keys**: `PocketedBallsSaved`, `RacksCountSaved` (typo in `HandleNineBallPotted`: `PocketdBallsSaved`)
- **Time.scale**: Paused via `Time.timeScale = 0f` in multiple places — ensure `UnlockShotProcess()` resets

## Directory Structure (scripts only)
```
Assets/Script/
├── HomeScript/           # Main menu, character select, mode select
│   ├── HomeScreen/       # UI, flow, spawner
│   ├── Character/        # ScriptableObjects: CharacterSO, SkillSlotSO, SkillVariantSO
│   └── LoadingScreen/
├── PlayscenePVP1D/       # Core gameplay
│   ├── Cue/              # CueStickController, CueBallController, input
│   ├── Pocket/           # PocketTowPs (rules), PocketDetector, PocketTrigger
│   ├── Beelzita/         # Character 1 skills & manager
│   ├── Glaszek/          # Character 2 skills & manager
│   ├── TakeDamage/       # BallHealth, IDamageable, BallCollisionDamage
│   └── Saving/           # SaveManager, SaveData, CharacterRegistrySO
├── Editor/               # PlayFromHomeScene (editor-only)
└── *.cs                  # Registries, StaminaManager
```

## External Dependencies
- `PinePie.SimpleJoystick` (local package at `Assets/PinePie/Simple Joystick/`)
- TextMesh Pro, Cinemachine, Input System, URP, AI Navigation, Visual Scripting