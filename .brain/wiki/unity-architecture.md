---
title: Unity Architecture
last_updated: 2026-04-14
confidence: HIGH
sources:
  - Unity/NeglectFix/Assets/Scripts/**/*.cs
  - PROJECT_SUMMARY.md
  - CLAUDE.md
---

# Unity Architecture

Developer reference for the NegletFix Unity codebase. 10 C# scripts, ~3,340 LOC total, organized in 4 subsystems: Assessment, EEG, Tasks, Utils. Target platform: Meta Quest 2/3 (Android ARM64) via OpenXR; currently developed and tested in Unity Editor on macOS.

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
| `Utils/RewardController.cs` | 332 | Closed-loop gatekeeper. Fires visual/audio/haptic rewards only when (engagement > threshold) AND (looking left). Handles cooldown (default 1s), reward duration (default 2s), glow effects on registered `rewardObjects`. |
| `Utils/DataLogger.cs` | 364 | CSV export at 10 Hz. Columns: `timestamp_ms, tp10_alpha, tp10_beta, tp10_theta, engagement_score, threshold, head_yaw, head_pitch, left_gaze, reward_triggered, event`. Timestamped session files + event log. Has hook for contrast sensitivity result export. |

### Tasks/ — Rehabilitation task framework

| Script | LOC | Responsibility |
|--------|-----|---------------|
| `Tasks/TaskManager.cs` | 325 | Abstract base class. Session phases (`NotStarted` → `Baseline` → `Training` → `Cooldown` → `Completed`), default durations (120s / 900s / 180s per Daibert-Nido). Concrete tasks (KitchenDiscovery, etc.) inherit. Hooks for `dataLogger`, `engagementCalculator`, `rewardController`. |

**Total**: 10 scripts, 3,339 lines.

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

### Required Unity packages
- **XR Plugin Management** — with OpenXR + Meta Quest Support enabled
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

## 6. Where to Extend for the Audiovisual Module

From [[audiovisual-training-protocol]], the missing piece is an audiovisual training task. Build it as:

1. **New script**: `Tasks/AudioVisualTraining.cs` inheriting from `TaskManager`
2. **New prefab**: `AVStimulus` — child of Quest camera rig with:
   - `AudioSource` (spatial, 400 Hz 250ms 55-75 dB clip, generated at runtime)
   - `MeshRenderer` on a simple quad for the visual target
3. **Configuration method**: `ConfigureFromContrastResults(ContrastSensitivityResults)` — sketched in `docs/research/contrast_sensitivity_module.md:605-637`
4. **Trial loop**: position stimulus at progressive eccentricity (8° → 24° → 56°), fire coincident audio+visual pulse, optionally gate by `engagementCalculator.IsEngaged()`
5. **Logging**: extend `DataLogger` with per-trial records (eccentricity, onset delays, engagement at onset)

Integrate via existing hooks — `TaskManager` already wires `dataLogger`, `engagementCalculator`, `rewardController`.

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
