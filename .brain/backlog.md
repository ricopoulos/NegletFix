# Backlog

> Work that's blocked, on hold, or waiting for something.
>
> Use status:
> - **WAITING**: Blocked on external factor (API approval, user decision, etc.)
> - **BLOCKED**: Technical blocker needs resolution
> - **PAUSED**: Intentionally set aside, can resume anytime

---

## WIP-001: Audiovisual Training Module

**Status**: PILOT QUEST SESSION PASSED + READY PROMPT ADDED (Phase 1 scaffold complete 2026-05-16; headless PlayMode smoke passed 2026-05-30; Quest controller smoke passed 2026-05-30; 6-minute Session 1 pilot passed 2026-05-30; quick Ready check passed 2026-05-30)
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

#### Generated Visual Stimulus Upgrade (2026-05-30)
Replaced the programmatic Sphere fallback in `AudioVisualTraining` with a flat camera-facing quad that generates a controlled grayscale texture at runtime. It supports a solid disk target and a Gabor patch mode, fixed visual angle sizing, soft edges, and LogCS-derived contrast on a matched gray background. The manual smoke scene builder now configures the generated disk defaults.

#### Session 1 Pilot Scene (2026-05-30)
Added `Assets/Scenes/AVTrainingSession1Pilot.unity` and corresponding editor/batch helpers. This scene is intentionally between smoke and full protocol: 30s baseline, one 5-minute training block, 30s cooldown, 4° generated disk targets, and a separate `program_state_session1_pilot.json` state file.

#### Session 1 Pilot Quest Check Passed (2026-05-30)
Built, installed, and launched `Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk` on Quest 2 (`1WMHH831TR1047`). Unity logs confirmed:
- Quest 2 / Android 14 device path
- Right controller trigger response via `Oculus Touch Controller OpenXR/triggerButton`
- `training_end`, `Trial log closed`, `cooldown_end`, and `session_end`
- `Session complete. 136 trials, 135 hits (99%). Final staircase: 10.20 LogCS.`

Pulled pilot logs into `SmokeResults/Pilot/`:
- `av_training_2026-05-30_17-09-55.csv`: 136 rows, 135 hits, 1 miss, 99.3% hit rate, average RT 371ms, contrast range 0.15..10.20 LogCS
- `session_2026-05-30_17-09-21.csv`: 3233 samples, 360.0s duration, 135 rewards

No new extra trial CSV was created after training ended; the earlier empty `14-24-18` file remains from the pre-guard smoke run.

#### Ready Prompt / Countdown Added (2026-05-30)
Added a pre-baseline `Ready` phase to `TaskManager`, then enabled it for `AudioVisualTraining`. The AV task now shows a world-space headset prompt:
- "GET READY" with headset/controller instructions
- Waits for Quest trigger or Space confirmation
- Runs a countdown before baseline begins
- Keeps `sessionStartTime` and data logging after the ready phase, so setup time is not mixed into therapeutic data

Verification:
- PlayMode smoke passed after updating the test to confirm the Ready phase
- `Builds/AVTrainingSession1Pilot.apk` rebuilt successfully with the prompt included
- Quest install/launch confirmed the prompt works: trigger press registered, 5s countdown completed, baseline/data logging started only after Ready phase

UX follow-up from Eric:
- 5s delay after pressing ready felt too long
- Running a 6-minute pilot for non-result prompt/controller validation was too heavy

Changes applied:
- Reduced Session 1 pilot ready countdown from 5s to 2s
- Reduced manual smoke ready countdown from 3s to 2s
- Added `Assets/Scenes/AVTrainingQuickReadyCheck.unity`
- Added batch/menu build path for `Builds/AVTrainingQuickReadyCheck.apk`
- Quick readiness check timing: 1s countdown, 2s baseline, 10s training, 2s cooldown

Quick Ready Check Quest validation:
- Installed/launched `Builds/AVTrainingQuickReadyCheck.apk` on Quest 2 (`1WMHH831TR1047`)
- Prompt appeared and right trigger was detected at `17:49:09`
- Ready phase completed at `17:49:10` (~1s after trigger)
- Baseline ended/training began at `17:49:12` after the 2s mini-baseline
- Session completed at `17:49:24` with 9 trials / 9 hits and total runtime reported as 0.2 minutes
- Eric confirmed the timing felt better
- ADB later stopped listing the headset, likely after unplug/sleep; rerun `adb devices -l` before the next install/launch

#### Completion Message Added (2026-05-30)
Added a headset-visible end-of-run message so Eric knows when it is safe to remove the headset:
- `SESSION COMPLETE`
- `You can remove the headset now.`

Implementation:
- `AudioVisualTraining.OnTaskCompleted()` now keeps/recreates the world-space headset prompt and updates it with the completion text after cooldown and logging shutdown
- Smoke scene builder serializes completion prompt defaults for generated scenes
- PlayMode smoke test now asserts that the completion message exists and contains the remove-headset instruction
- `NeglectFix.PlayModeTests.asmdef` now references `Unity.TextMeshPro` for the assertion

Verification:
- PlayMode smoke passed: `Unity/NeglectFix/SmokeResults/playmode-finished-message-results.xml`
- Rebuilt `Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk` successfully with the completion message included

#### Left-Field Visibility Fix / Quick Check Passed (2026-05-30)
Eric reported hearing cues but not seeing markers in the rebuilt pilot. Root causes:
- `AVTrainingSession1Pilot.unity` had a serialized all-zero baseline object, which tied left/right and selected the right hemifield.
- The saved single-session scheduler state was already at `1/1`, so the next run advanced to stale session index 2 and far eccentricities.
- The 30s baseline looked like a silent delay after the ready countdown.

Fixes applied:
- `AudioVisualTraining` now ignores serialized all-zero baseline data, clamps current session index to planned sessions, starts below the affected threshold (`baseline - 0.30 LogCS`), supports a validation contrast floor, and shows a world-space `CALIBRATING` prompt during baseline.
- `ProgramScheduler` gained `resetStateOnAwake` for repeatable smoke/pilot builds.
- Smoke/quick scene builder now bakes Eric baseline (`left=0.00`, `right=2.25`, asymmetry `2.25`), resets smoke state, uses larger/higher-contrast validation markers, and keeps quick timing at 1s countdown + 2s baseline + 10s training + 2s cooldown.

Verification:
- Rebuilt `Unity/NeglectFix/Builds/AVTrainingQuickReadyCheck.apk` successfully.
- PlayMode smoke passed: `Unity/NeglectFix/SmokeResults/playmode-after-left-visible-fix-results.xml`.
- Quest quick check passed after applying proximity override with `adb shell am broadcast -a com.oculus.vrpowermanager.prox_close`.
- Trial log: `Unity/NeglectFix/SmokeResults/QuickReadyCheckLatest/av_training_2026-05-30_22-03-50.csv`.
- Summary: 9 rows, 9 hits, 100% hit rate, all left hemifield, eccentricity range `-8°..-5°`, average RT ~435ms.
- Eric confirmed the larger left-side markers were visible.
- Rebuilt full `Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk` after the same fixes; scene now has Eric baseline, reset-on-launch, 6° validation markers, `minimumGeneratedStimulusContrast=0.9`, and visible calibration text.

#### Controlled Background / Passthrough Visibility Fix (2026-05-30)
Eric reported that Quest camera/passthrough was visible behind the app, making low-contrast markers nearly impossible to see. Treat this as a clinical visibility blocker: AV training must render against a controlled background, not the headset's home/passthrough video.

Fixes applied:
- `AudioVisualTraining` now creates an opaque camera-locked gray quad named `AVTrainingControlledBackdrop` behind prompts and stimuli.
- Ready/baseline/completion prompt panels are nearly opaque instead of translucent, so passthrough cannot reduce text contrast.
- Smoke scene builder serializes the backdrop settings into generated Quick Ready Check and Session 1 pilot scenes.

Verification:
- Rebuilt `Unity/NeglectFix/Builds/AVTrainingQuickReadyCheck.apk` successfully.
- Rebuilt `Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk` successfully.
- Confirmed both generated scenes serialize `ensureOpaqueTrainingBackdrop: 1` with 2.4m distance, 5.2m width, 3.4m height, and 0.5 gray luminance.
- Unity CLI PlayMode command exited cleanly but did not emit the expected XML results file, so do not count a fresh PlayMode pass for this specific change yet.
- Quest Quick Ready Check was launched after the fix and completed cleanly: pulled `Unity/NeglectFix/SmokeResults/QuickReadyCheckOpaque/av_training_2026-05-30_22-41-21.csv` with 9 rows, 9 hits, all left hemifield, eccentricity `-8°..-5°`, average RT ~389ms. Eric confirmed the headset background was gray and the markers were visible; Quest mirror screenshot came back black, so rely on headset report + trial logs for this validation.

#### Quest USB Mitigation: Wi-Fi ADB Helper (2026-05-30)
Repeated USB detection failures are now a workflow blocker. Immediate mitigation:
- Added `scripts/quest-adb.sh`
- Added `.quest-adb-ip` to `.gitignore`
- Script supports `status`, `enable-wifi`, `connect`, `install-run`, `run`, and `logs`
- Once USB works one more time, run `scripts/quest-adb.sh enable-wifi` to switch the Quest to ADB-over-Wi-Fi, cache its IP, and use `scripts/quest-adb.sh install-run` for later install/launch loops

Current check:
- `adb connect 10.0.0.136:5555` failed with `Connection refused`
- `adb mdns services` found no wireless debugging services
- `adb devices -l` showed no USB device

Resolved later via MQDH:
- Meta Quest Developer Hub detected the Quest 2 over USB and restored the terminal ADB handshake
- `scripts/quest-adb.sh enable-wifi` succeeded
- Wi-Fi ADB connected at `10.0.0.136:5555`
- `scripts/quest-adb.sh run` launched the app over Wi-Fi
- Rebuilt `AVTrainingSession1Pilot.apk` installed successfully over Wi-Fi, and app process was running as PID `13066`
- Helper updated to prefer the cached Wi-Fi device when USB and Wi-Fi are both connected

Fallback if the old `adb tcpip 5555` path remains flaky:
- Try Android 11+ Wireless Debugging from headset Developer Options if exposed by Horizon OS: pair with `adb pair <ip>:<pair-port>` and then connect with `adb connect <ip>:<adb-port>`
- Use Meta Quest Developer Hub only if its ADB path is configured to the same `adb` used in terminal (`/opt/homebrew/bin/adb`), to avoid competing ADB daemons
- If the headset sleeps off-face during testing, reconnect Wi-Fi ADB and run `adb shell am broadcast -a com.oculus.vrpowermanager.prox_close`. This persists until headset reboot and can drain battery/burn display if left unattended.

#### Guided AV Pilot Flow (2026-05-30)
Eric stopped the longer Session 1 pilot because the task instruction was ambiguous: "look forward" sounded like a fixed-gaze contrast/field test, while the audiovisual rehab protocol is a training task where the patient starts centrally and may move the eyes toward the audiovisual marker while keeping the head still.

Decision:
- Keep fixed-gaze contrast/field measurement as a separate future assessment mode.
- Treat the current AV pilot as guided rehab training-flow validation, not a clinical contrast threshold test.

Fixes applied:
- `AudioVisualTraining` now renders a small center fixation cross as a starting anchor.
- Ready/practice instructions now say this is rehab training, not a contrast test.
- Instruction wording: start on the center cross, move only the eyes to the marker when it appears, keep the head still, press when detected.
- Added an optional unlogged practice block before recorded training trials.
- Practice trials can trigger normal reward feedback but do not write to the training-trial CSV and do not update the adaptive staircase.
- `AVTrainingSession1Pilot` preset shortened for validation: 5s calibration, 4s practice instructions, 15s practice, 120s recorded block, 5s cooldown.
- `AVTrainingQuickReadyCheck` remains a short prompt/controller/visibility check with no practice block.

Verification:
- PlayMode smoke passed: `Unity/NeglectFix/SmokeResults/playmode-av-guided-pilot-results.xml`.
- Rebuilt `Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk` successfully.
- Rebuilt `Unity/NeglectFix/Builds/AVTrainingQuickReadyCheck.apk` successfully.
- Installed updated Session1Pilot APK on Quest 2 over USB ADB at 23:20 local time.
- Guided pilot launched at 23:24 local time and completed cleanly: ready trigger detected, 5s baseline, 15s unlogged practice, recorded block, 5s cooldown, completion prompt shown.
- Trial CSV pulled to `Unity/NeglectFix/SmokeResults/GuidedPilot/av_training_2026-05-30_23-25-27.csv`.
- Session CSV pulled to `Unity/NeglectFix/SmokeResults/GuidedPilot/session_2026-05-30_23-24-58.csv`.
- Live device log saved at `Unity/NeglectFix/SmokeResults/guided-pilot-live.log`.
- Recorded block summary: 45 trials, 33 hits, 73.3% hit rate, all left hemifield, eccentricity range `-8°..-5°`, average hit RT ~475ms, contrast staircase `0.00..0.75 LogCS`, final log message `Final staircase: 0.45 LogCS`.
- App was force-stopped after pulling logs.
- Eric subjective feedback: guided flow is good. Center cross helped him stay anchored; he described it as focusing/gazing on the cross while trying not to chase. The experience was emotionally heavy because the sound made it clear a marker appeared while the left-side marker was still not visible without eye movement. Treat this as useful validation that the task is exposing the real deficit, not merely a UI problem.
- Candidate next iteration: add sparse random right-hemifield control markers to confirm attention/input/visibility, but keep them clearly marked as control trials and limit their proportion so they do not dilute left-field rehabilitation dose.

#### Remaining Work
1. **Use Wi-Fi ADB for Quest runs** — prefer `scripts/quest-adb.sh install-run` / `scripts/quest-adb.sh logs`; cached Quest IP is `10.0.0.136`
2. **Next headset validation** — launch the installed guided Session 1 pilot only when Eric has the headset adjusted and is ready for ~2.5 minutes
3. **Pilot interpretation** — after the guided run, decide the first therapeutic dose length and whether to keep high-contrast validation markers briefly or transition to adaptive clinical contrast immediately
4. **Phase 2 launch**: 30 sessions × 5 days/week × 6 weeks at the Alharshan dose; mid-program CS checks at sessions 5/10/15/20/25; full reassessment at session 30

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

*Last updated: 2026-05-31*
