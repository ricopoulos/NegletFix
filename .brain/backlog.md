# Backlog

> Work that's blocked, on hold, or waiting for something.
>
> Use status:
> - **WAITING**: Blocked on external factor (API approval, user decision, etc.)
> - **BLOCKED**: Technical blocker needs resolution
> - **PAUSED**: Intentionally set aside, can resume anytime

---

## WIP-001: Audiovisual Training Module

**Status**: MANUAL QUEST SMOKE PASSED (Phase 1 scaffold complete 2026-05-16; headless PlayMode smoke passed 2026-05-30; Quest controller smoke passed 2026-05-30)
**Created**: 2025-12-15
**Priority**: High

#### Context
Main rehabilitation module — cross-modal audiovisual stimulation for left hemifield recovery. **Reframed 2026-05-14**: was previously framed around Daibert-Nido 2021, now correctly built as Paradigm B (congruent-pair detection, Wake Forest / Rowland 2023 lineage) at the Alharshan/Alwashmi 2026 dose for chronic adult stroke.

#### Phase 1 Complete (2026-05-16)
Scaffolded in `Unity/NeglectFix/Assets/Scripts/Tasks/`:
- `AudioVisualTraining.cs` — main task, 30-min sessions, 3×10-min blocks, 2-up/1-down weighted staircase, sub-50ms AV sync, baseline-driven personalization
- `ProgramScheduler.cs` — session state JSON
- `EccentricityProgression.cs` — CS asymmetry → severity-classified ladder

Plus: `DataLogger.LogTrainingTrial()` added; `RewardController.RewardMode` enum (OpenLoop default, EEG decoupled). Unity 6.2 imports clean, XR Plug-in Management configured (OpenXR + Meta Quest Support). Commits `ecf327f`, `7ea389c`, `24a3075` on `main`.

#### Headless Smoke Test Complete (2026-05-30)
Added repeatable PlayMode smoke coverage in `Unity/NeglectFix/Assets/Tests/PlayMode/` and a `NeglectFix.Runtime` assembly definition. Batchmode command passed:
`/Applications/Unity/Hub/Editor/6000.2.8f1/Unity.app/Contents/MacOS/Unity -batchmode -projectPath Unity/NeglectFix -runTests -testPlatform PlayMode -testResults Unity/NeglectFix/playmode-smoke-results.xml -logFile -`

Smoke test creates a short AV training scene, runs baseline/training/cooldown, verifies session completion, and verifies `DataLogger` writes a trial CSV. It initially failed because `AudioVisualTraining.Start()` assigned `trialLogger = dataLogger` before `TaskManager` populated `dataLogger`; fixed by resolving `dataLogger` before assigning `trialLogger`.

#### Controller Input Binding Complete (2026-05-30)
Added an `InputActionProperty` response path to `AudioVisualTraining.cs`. If no action is assigned in the Inspector, the task creates a runtime default bound to generic XR controller triggers, Oculus Touch, Quest Touch Plus/Pro triggers, Space, and Enter. Legacy keyboard/Submit fallback remains enabled for quick Editor testing.

Strengthened the PlayMode smoke test to use `Unity.InputSystem.TestFramework`, simulate a Space response during training, and verify at least one hit/reward plus hit/RT columns in the trial CSV. Batchmode PlayMode run passed with `Total rewards: 1`.

#### Manual Smoke Scene Ready (2026-05-30)
Generated `Unity/NeglectFix/Assets/Scenes/AVTrainingManualSmoke.unity` with a short 5s baseline, 60s training block, and 5s cooldown. The scene contains `AVTrainingSystem_ManualSmoke` with `DataLogger`, `RewardController`, `ProgramScheduler`, and `AudioVisualTraining` wired together. The scene is first in Build Settings for a smoke build/run. A reusable editor menu item exists at `NeglectFix > Testing > Create AV Training Manual Smoke Scene`.

#### Android Smoke APK + Quest Controller Smoke Passed (2026-05-30)
Added a batch APK build helper to `Assets/Editor/AvTrainingManualSmokeSceneBuilder.cs` and built `Unity/NeglectFix/Builds/AVTrainingManualSmoke.apk` successfully with Unity 6.2.8f1. The first Android build completed through shader compilation, IL2CPP, and Gradle packaging.

Physical Quest smoke initially launched but registered 0 hits because the Input System action did not surface Quest 2 trigger input on device. Added a UnityEngine.XR fallback response path that polls left/right controller `triggerButton`, analog trigger, grip, A/B, and corresponding left-hand buttons.

Patched APK run passed on Quest 2:
- Log detected `Oculus Touch Controller OpenXR/triggerButton`
- RewardController fired rewards during training
- Pulled `SmokeResults/av_training_2026-05-30_14-23-19.csv`
- CSV summary: 44 rows, 42 hits

Follow-up cleanup added after the successful smoke: `DataLogger.CloseTrainingTrialLog()` closes/flushed the training CSV at training end, player builds now use `Application.persistentDataPath` for the normal session log directory, and `AudioVisualTraining` now guards the block/trial coroutine so a held trigger cannot create an extra trial log after training has switched into cooldown. Rebuilt `Builds/AVTrainingManualSmoke.apk` successfully after these fixes.

#### Quest Performance Polish (2026-05-30)
Disabled the active Screen Space Ambient Occlusion renderer feature in `Assets/Settings/PC_Renderer.asset`. Android is already mapped to `Mobile_RPAsset`/`Mobile_Renderer`, which has no SSAO feature, but disabling the default PC renderer's SSAO removes the remaining validation/performance concern from the shared URP setup.

#### Remaining Work
1. **Optional final on-device check** — install/run the freshly rebuilt `Builds/AVTrainingManualSmoke.apk` once more only if we want direct confirmation that the post-training guard prevents extra empty CSV files
2. **Visual stimulus prefab** — replace programmatic Sphere fallback with Gabor patch / high-contrast disk
3. **Phase 2 launch** after controller smoke passes: 30 sessions × 5 days/week × 6 weeks at the Alharshan dose; mid-program CS checks at sessions 5/10/15/20/25; full reassessment at session 30

---

## WIP-002 (future): Paradigm A — 3D-MOT-IVR

**Status**: NOT-STARTED
**Created**: 2026-05-16 (Phase 3 in build plan)
**Priority**: Medium (deferred until Phase 2 results)

#### Context
After Phase 2 (Paradigm B) runs 30 sessions and produces responder data, optionally build Paradigm A (3D Multiple-Object Tracking in IVR, authentic Daibert-Nido). Significantly larger Unity build — 360° spherical environment, 3D physics for target spheres, distractor logic, gaze/controller pointer selection, adaptive difficulty via target count + sphere velocity. ~4-6 weeks dev time estimate.

#### Decision Gate
- Phase 2 gains ≥ +0.30 LogCS at any trained location → continue with A or extend B
- Phase 2 no detectable gain → switch to A (engages different mechanisms)

---

## WIP-003 (future): Official Unity MCP Migration Check

**Status**: PAUSED
**Created**: 2026-05-30
**Priority**: Low until Quest smoke path is stable

#### Context
Unity now ships an official MCP bridge through `com.unity.ai.assistant` with a relay binary under `~/.unity/relay/`. This may be a better long-term foundation than the current community `com.coplaydev.unity-mcp` package because it is Unity-supported and includes custom MCP tool registration through `[McpTool]` / `IUnityMcpTool`.

#### Recommendation
Do not switch before the manual Quest controller smoke test. After the rehabilitation flow is stable, test official Unity MCP on a branch and compare:
- Built-in tool coverage versus current `com.coplaydev.unity-mcp`
- Stability with Unity 6.2 and this project
- Whether custom tools can expose NegletFix-specific workflows, such as AV smoke test, Quest build validation, trial CSV summary, and scene setup checks

#### Caveat
Unity docs reviewed were for `com.unity.ai.assistant@2.9.0-pre.2`. Treat as pre-release until proven stable in this project.

---

*Last updated: 2026-05-30*
