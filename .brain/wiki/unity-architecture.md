---
title: Unity Architecture
last_updated: 2026-05-16
confidence: HIGH
sources:
  - Unity/NeglectFix/Assets/Scripts/**/*.cs
  - PROJECT_SUMMARY.md
  - CLAUDE.md
  - 2026-05-16 Phase 1 scaffold + XR config session
---

# Unity Architecture

Developer reference for the NegletFix Unity codebase. **13 C# scripts** (10 original + 3 added in Phase 1, 2026-05-16), ~4,300 LOC total, organized in 4 subsystems: Assessment, EEG, Tasks, Utils. Target platform: Meta Quest 2/3 (Android ARM64) via OpenXR; OpenXR + Meta Quest Support enabled as of 2026-05-16.

**Unity version**: 6.2 (6000.2.8f1) with VR template (`.brain/cross-cutting.md:43`).
**Project path**: `Unity/NeglectFix/` (note the casing: directory is `NeglectFix`, git repo is `NegletFix`).

See [[hardware-setup]] for device/OSC config, [[eeg-neurofeedback]] for signal processing details, [[contrast-sensitivity-test]] for assessment logic.

---

## 1. Script Inventory

All paths relative to `Unity/NeglectFix/Assets/Scripts/`.

### Assessment/ — Contrast sensitivity measurement

| Script | LOC | Responsibility |
|--------|-----|---------------|
| `Assessment/ContrastSensitivityTest.cs` | 868 | Core Pelli-Robson test engine: Sloan letter display, triplet scoring, LogCS→luminance conversion with gamma correction, hemifield positioning (the ±300px UI offset + fixation cross), calibration mode. Namespace `NeglectFix.Assessment`. |
| `Assessment/ContrastTestInput.cs` | 280 | Quest controller + keyboard input handling for test responses. Maps letter keys C-D-H-K-N-O-R-S-V-Z, Backspace = can't-see, Space = advance. Optional visual letter picker (`LetterPickerUI`). |
| `Assessment/ContrastResultsUI.cs` | 349 | Displays per-hemifield scores, visual bars, historical trend comparison. Integrates with `DataLogger` for CSV export. |

### EEG/ — Brain signal pipeline

| Script | LOC | Responsibility |
|--------|-----|---------------|
| `EEG/MuseOSCReceiver.cs` | 198 | OSC receiver on port 5000. Binds `/muse/elements/{alpha,beta,theta}_absolute` from Mind Monitor. Parses 4-channel arrays (TP9, AF7, AF8, **TP10**). Emits per-band events + combined `OnBandPowerReceived`. Compile-time guarded by `#define EXTOSC_INSTALLED` (default off). Namespace `NeglectFix.EEG`. |
| `EEG/EngagementCalculator.cs` | 315 | Subscribes to MuseOSC events. Computes engagement `(β/α) × (1 − θ·penalty)` with 10-sample moving average. Handles 120 s baseline, adaptive threshold targeting 40-60% success rate. Emits `OnEngagementThresholdExceeded`. |
| `EEG/EEGSimulator.cs` | 144 | Dev-time substitute for Muse. Generates realistic sinusoid+Perlin-noise variation for α/β/θ/γ. Presets for high/low engagement. NOTE: `OnKeyPress()` method never fires (not a Unity message) — E/R key shortcuts do not work until refactored. |

### Utils/ — Cross-cutting services

| Script | LOC | Responsibility |
|--------|-----|---------------|
| `Utils/GazeDetector.cs` | 164 | Quest head-yaw tracking. Fires `OnStartLookingLeft` / `OnStopLookingLeft` when yaw crosses ±15° threshold (configurable). Optional rotation smoothing. |
| `Utils/RewardController.cs` ✱ | ~350 | Visual/audio/haptic rewards. **Two modes** (2026-05-16 refactor): `OpenLoop` (v1 default — task triggers reward directly on detection, no EEG gating) and `EegGated` (v2 — gates on engagement + gaze, preserved for future use after Muse signal-quality validation per Treves 2025 caveat). Handles cooldown (default 1s), reward duration (default 2s), glow effects. |
| `Utils/DataLogger.cs` ✱ | ~470 | CSV export at 10 Hz for the neurofeedback CSV. **2026-05-16 extension**: added `LogTrainingTrial()` method + per-session AV training trial CSV writer (`Application.persistentDataPath/training_trials/av_training_{timestamp}.csv`). Trial schema: `timestamp_ms, session_index, block_index, trial_index, eccentricity_deg, hemifield, contrast_logcs, stimulus_onset_ms, audio_onset_ms, response_onset_ms, rt_ms, hit, av_delta_ms`. Device metadata in CSV header. |

### Tasks/ — Rehabilitation task framework

| Script | LOC | Responsibility |
|--------|-----|---------------|
| `Tasks/TaskManager.cs` | 325 | Abstract base class. Session phases (`NotStarted` → `Baseline` → `Training` → `Cooldown` → `Completed`), default durations overridable by concrete tasks. Hooks for `dataLogger`, `engagementCalculator`, `rewardController`. |
| `Tasks/AudioVisualTraining.cs` ★ | ~320 | **Paradigm B** main task, congruent-pair detection. Extends `TaskManager`. Trial loop with random ISI (2-5s), 3×10-min blocks, 2-up/1-down weighted staircase converging on ~70.7%, sub-50ms AV sync, baseline-driven personalization. Procedural 400Hz tone with Hann window as audio fallback. Reward triggered directly on hit (open-loop, not EEG-gated). |
| `Tasks/ProgramScheduler.cs` ★ | ~200 | Program state machine — session count, last-session timestamp, paradigm choice, re-measurement triggers. Persisted as JSON to `Application.persistentDataPath/program_state.json`. Supports paradigm switching for Phase 3. |
| `Tasks/EccentricityProgression.cs` ★ | ~110 | Classifies CS asymmetry severity (Severe/Moderate/Mild), selects appropriate eccentricity ladder, progresses across sessions. Eric's case (asymmetry 2.25) → Severe → ladder `[5,8,12,16,20]°` starting at scotoma border per Yang/Cavanaugh/Saionz 2023. |

★ = added 2026-05-16 (Phase 1 scaffold).

**Total**: 13 scripts, ~4,300 lines.

---

## 2. Data Flow Diagram

```
┌─────────────────┐
│ Muse 2 / Muse S │   (or EEGSimulator in Editor)
│  TP9 AF7 AF8    │
│    TP10 ★       │  ← primary target (right posterior parietal)
└────────┬────────┘
         │ BLE
         ▼
┌─────────────────┐
│  Mind Monitor   │   iOS/Android app, OSC streamer
└────────┬────────┘
         │ WiFi UDP OSC → port 5000
         ▼
┌───────────────────────────┐
│     MuseOSCReceiver       │
│   /muse/elements/*        │   emits OnTP10{Alpha,Beta,Theta}Received
└──────────┬────────────────┘
           │
           ▼
┌───────────────────────────┐        ┌──────────────────┐
│   EngagementCalculator    │        │   GazeDetector   │
│  β/α ratio + θ penalty    │        │  head yaw >15°L  │
│  adaptive threshold       │        └────────┬─────────┘
│  120s baseline, 40-60%    │                 │
└──────────┬────────────────┘                 │
           │ OnEngagementThresholdExceeded    │ OnStartLookingLeft
           ▼                                  ▼
         ┌─────────────────────────────────────────┐
         │           RewardController              │
         │  if (engaged AND lookingLeft):          │
         │      TriggerReward()  (visual+audio)    │
         └──────────────┬──────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        ▼               ▼               ▼
    ┌───────┐      ┌─────────┐     ┌─────────────┐
    │ Glow  │      │ Haptic  │     │ DataLogger  │
    │ on    │      │ pulse   │     │ CSV 10 Hz   │
    │ objs  │      │         │     └─────────────┘
    └───────┘      └─────────┘

┌─────────────────────────────────┐
│   TaskManager (abstract)        │   provides Baseline→Training→Cooldown
│   KitchenDiscovery, etc. extend │   phasing for concrete tasks
└─────────────────────────────────┘

ASSESSMENT pipeline (independent of training):
┌──────────────────────┐
│ ContrastTestInput    │─────┐
└──────────────────────┘     │
                             ▼
┌──────────────────────┐     ┌──────────────────────┐
│ ContrastResultsUI    │◀────│ ContrastSensitivityTest │
└──────────────────────┘     │   Pelli-Robson + hemi   │
                             └──────────┬───────────────┘
                                        │ OnAllTestsComplete
                                        ▼
                              ┌──────────────────────┐
                              │ DataLogger           │
                              │ assessments/*.csv    │
                              └──────────────────────┘
```

---

## 3. Namespace Convention

- `NeglectFix.Assessment` — contrast sensitivity
- `NeglectFix.EEG` — signal pipeline
- `NeglectFix.Utils` — gaze, rewards, logging
- `NeglectFix.Tasks` — rehabilitation tasks

Cross-namespace references use fully qualified names (e.g., `NeglectFix.EEG.EngagementCalculator` referenced from `RewardController.cs:20`).

---

## 4. Dependencies

### Required Unity packages (installed 2026-05-16 in `Packages/manifest.json`)
- **`com.unity.xr.management 4.5.0`** — XR Plugin Management
- **`com.unity.xr.openxr 1.14.0`** — OpenXR Plugin (with Meta Quest Support feature enabled)
- **`com.unity.xr.interaction.toolkit 3.0.7`** — XR Interaction Toolkit
- **TextMeshPro** — letter rendering in contrast test (`using TMPro` in `ContrastSensitivityTest.cs:3`)

### Required third-party
- **extOSC** (https://github.com/Iam1337/extOSC) — OSC receiver. Install then uncomment `#define EXTOSC_INSTALLED` at top of `MuseOSCReceiver.cs`. Until installed, the receiver logs a warning and falls back to simulator-only mode — project still compiles.

### Not required (dev-only)
- Meta XR SDK — `ContrastTestInput.cs` has `#define OVR_INSTALLED` guards for Quest controller input; keyboard path works without it.

---

## 5. Quest 2 Deployment (Android)

1. Install Unity 6.2 (6000.2.8f1) with VR template and Android Build Support.
2. Enable Developer Mode on Quest via Meta Horizon (formerly Oculus) mobile app.
3. In Unity: **Build Settings → Android → Switch Platform**.
4. **Project Settings → XR Plug-in Management** → enable **OpenXR** on Android tab → add **Meta Quest Support** feature group.
5. Set Texture Compression: ASTC.
6. Set Scripting Backend: IL2CPP. Target Architecture: ARM64.
7. Set minimum API level per current Meta Quest requirements (check Meta docs; requirement has shifted several times).
8. Build and Run with Quest connected via USB (or use Meta Quest Link for in-editor testing).

See [[hardware-setup]] for Muse/Mind Monitor/OSC wiring.

---

## 6. AV Training Module (built 2026-05-16) — What Exists Now

The audiovisual training module is **built and committed** as of 2026-05-16 (Phase 1 scaffold, commits `ecf327f` + `7ea389c` + `24a3075` on `main`). Implements Paradigm B (congruent-pair detection, Wake Forest / Rowland 2023 lineage) at the Alharshan/Alwashmi 2026 dose for chronic adult stroke.

Files (see Section 1 for details):
- `Tasks/AudioVisualTraining.cs` — main task with trial loop, staircase, AV pair presentation
- `Tasks/ProgramScheduler.cs` — program state JSON persistence
- `Tasks/EccentricityProgression.cs` — baseline-driven eccentricity ladder

What's still on the to-do list:
- **Smoke test in Editor** — create a scene with the components wired up, hit Play, watch trials populate CSV. Deferred to a fresh-head session.
- **Quest controller input** — `DetectResponse()` currently falls back to keyboard SPACE/Return and legacy `Input.GetAxis("Submit")`. For real Quest deployment, this needs an `InputSystem.InputAction` binding to the Quest controller trigger.
- **Visual stimulus prefab** — fallback creates a programmatic Sphere; a proper Gabor patch or high-contrast disk prefab is a polish step.
- **Phase 3 — Paradigm A (3D-MOT-IVR)** — significantly larger build; deferred until Phase 2 (30 sessions of Paradigm B) is run and shows responder data.

Integration: `TaskManager` already wires `dataLogger`, `engagementCalculator`, `rewardController`. The new `AudioVisualTraining` inherits this plumbing and adds `programScheduler` + `trialLogger` (defaults to `dataLogger`). See [[audiovisual-training-protocol]] §5 for the build spec and runtime trial-loop pseudocode.

---

## 7. Known Code Issues / Tech Debt

| Issue | Location | Severity |
|-------|----------|----------|
| `EEGSimulator.OnKeyPress()` never fires (not a Unity message) | `EEG/EEGSimulator.cs:131` | Low — sim still useful |
| `alphaWeight` parameter declared but unused in engagement formula | `EEG/EngagementCalculator.cs:28` | Low — latent tuning knob |
| `hemifieldUIOffset` hardcoded to 300px, not calibrated to visual angle | `Assessment/ContrastSensitivityTest.cs:40` | Medium — VR migration will fix |
| `extOSC` not installed by default; falls back to simulator silently | `EEG/MuseOSCReceiver.cs:5` | By design — documented |
| `CalculateFontSize()` uses an approximation (`× 100 × 2`) not validated against VR FOV | `Assessment/ContrastSensitivityTest.cs:482-491` | Medium — recalibrate on Quest |

---

## 8. Cross-References

- EEG details: [[eeg-neurofeedback]]
- Assessment instrument: [[contrast-sensitivity-test]]
- Hardware/OSC wiring: [[hardware-setup]]
- What to build next: [[audiovisual-training-protocol]]
- Roadmap context: [[rehabilitation-roadmap]]
