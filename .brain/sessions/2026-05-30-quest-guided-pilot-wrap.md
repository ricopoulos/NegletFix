# Session: Quest AV Guided Pilot

**Date:** 2026-05-30 / 2026-05-31 local late evening
**Branch:** `main`
**Status:** Completed and validated on Quest 2

## Summary

Started with unstable USB/ADB friction, then restored Quest connectivity through Meta Quest Developer Hub and terminal ADB. Added a Wi-Fi ADB helper for future loops, rebuilt the AV training pilot multiple times, and ended with a working guided headset pilot that Eric completed.

## Objectives

- Restore a usable Quest deployment loop despite USB/ADB instability.
- Validate Quest controller input and marker visibility on-headset.
- Improve the pilot UX so Eric can adjust headset/controllers before trials begin and know when the run is complete.
- Clarify the AV task instructions so it is not confused with fixed-gaze contrast/perimetry assessment.
- Run a short guided pilot and pull the resulting logs.

## Key Changes

- Added a headset ready prompt/countdown before data collection.
- Added a clear completion prompt: `SESSION COMPLETE` / `You can remove the headset now.`
- Added an opaque gray camera-locked background so passthrough/home video cannot reduce marker visibility.
- Fixed repeatable pilot state by letting `ProgramScheduler` reset saved state for smoke/pilot scenes.
- Baked Eric's baseline into generated smoke/pilot scenes so the affected hemifield is left and the validation targets start at `-5°/-8°`.
- Added `AVTrainingQuickReadyCheck` for short visual/controller smoke tests.
- Added a small center fixation cross and explicit rehab-training instructions.
- Added a short unlogged practice block before recorded trials; practice rewards are allowed but practice does not write to the trial CSV or update the staircase.
- Shortened guided Session1Pilot to 5s calibration + 4s practice intro + 15s practice + 120s recorded block + 5s cooldown.
- Added `scripts/quest-adb.sh` to prefer cached Wi-Fi ADB once enabled.

## Files Modified

Key committed files:
- `.brain/backlog.md`, `.brain/index.json`, `.brain/crumbs/2026-05-30-1350.md`
- `.brain/sessions/2026-05-30-quest-guided-pilot-wrap.md`
- `.gitignore`
- `scripts/quest-adb.sh`
- `Unity/NeglectFix/Assets/Scripts/Tasks/TaskManager.cs`
- `Unity/NeglectFix/Assets/Scripts/Tasks/AudioVisualTraining.cs`
- `Unity/NeglectFix/Assets/Scripts/Tasks/ProgramScheduler.cs`
- `Unity/NeglectFix/Assets/Editor/AvTrainingManualSmokeSceneBuilder.cs`
- `Unity/NeglectFix/Assets/Scenes/AVTrainingQuickReadyCheck.unity`
- `Unity/NeglectFix/Assets/Scenes/AVTrainingSession1Pilot.unity`
- `Unity/NeglectFix/Assets/Tests/PlayMode/AudioVisualTrainingSmokeTest.cs`
- Project/URP settings touched by Unity import/build

Generated local artifacts kept out of git:
- `Unity/NeglectFix/SmokeResults/GuidedPilot/`
- `Unity/NeglectFix/SmokeResults/guided-pilot-live.log`
- APKs under `Unity/NeglectFix/Builds/` (ignored by `.gitignore`)

## Validation

- PlayMode smoke passed: `Unity/NeglectFix/SmokeResults/playmode-av-guided-pilot-results.xml`.
- Quick Ready Check after controlled backdrop: 9/9 hits, all left hemifield, Eric confirmed gray background and visible markers.
- Guided Session1Pilot completed on Quest 2:
  - Trial CSV: `Unity/NeglectFix/SmokeResults/GuidedPilot/av_training_2026-05-30_23-25-27.csv`
  - Session CSV: `Unity/NeglectFix/SmokeResults/GuidedPilot/session_2026-05-30_23-24-58.csv`
  - Live log: `Unity/NeglectFix/SmokeResults/guided-pilot-live.log`
  - Recorded block: 45 trials, 33 hits, 73.3% hit rate
  - All recorded trials were left hemifield
  - Eccentricity range: `-8°..-5°`
  - Average hit RT: ~475ms
  - Contrast staircase range: `0.00..0.75 LogCS`
  - Final staircase log: `0.45 LogCS`

## Eric Feedback

The guided flow is good. The center cross helped Eric stay anchored, described as focusing/gazing on the cross while trying not to chase. The task was emotionally heavy because the sound made the unseen left-side marker explicit. Eric framed this as part of the rehab journey: "if you can measure it, you can modify it."

## Wiki Updates

Follow-up audit after the initial commit found stale wiki claims. Updated:
- `wiki/index.md` — module readiness moved AV training and Quest deployment out of planned/gray status.
- `wiki/audiovisual-training-protocol.md` — added guided pilot structure, practice policy, fixed-gaze assessment separation, and Quest validation result.
- `wiki/unity-architecture.md` — updated TaskManager/AudioVisualTraining/ProgramScheduler state and Quest validation details.
- `wiki/hardware-setup.md` — added MQDH/ADB/Wi-Fi helper workflow and troubleshooting rows.
- `wiki/rehabilitation-roadmap.md` — moved AV training from paused/scaffold to guided Quest pilot completed; updated critical path.

## Relationship Check

- `relationships.md` updated for Eric (self): smoke test is no longer the blocker; next state is right-control polish + dose decision.
- No external human contact changed. Charlie and Dr. Jacquemin entries did not require updates.

## VPS / Remote Drift Check

- No deployed VPS or remote runtime exists for NegletFix in this session.
- Quest device state was checked via ADB; the app was force-stopped after logs were pulled.
- GitHub remote synced via push to `origin/main`.

## Branch Status

- Branch: `main`
- No `feature/*` or `claude/*` remote branches were unmerged into `main`.
- No local `feature/*` or `claude/*` branches with unpushed commits were found.
- Commit pushed: `73cfce8 Add guided Quest AV pilot flow`
- Follow-up wiki/memory audit completed in the final wrap-up pass and committed separately.

## Obsidian Sync

- Raw `.brain` mirror was copied to the Obsidian vault initially.
- Proper `/end-session` bridge completed during the final audit:
  - `populate .brain NegletFix` wrote `Projects/NegletFix.md`, copied raw `.brain/`, and produced `✓ Briefing generated`.
  - `done NegletFix "Guided Quest AV pilot completed; wiki, session memory, hardware notes, and roadmap reconciled"` produced `Logged:` and refreshed the dashboard.

## Next Actions

1. Add sparse right-hemifield control trials, around 10-20%, clearly logged separately from left rehab trials.
2. Keep the left hemifield as the main therapeutic dose.
3. Polish instructions and completion states based on the guided-pilot feedback.
4. Decide whether the first rehab session should use the 2-minute guided block, a 5-minute block, or a gradual ramp.
5. Preserve fixed-gaze contrast/field assessment as a separate mode; do not conflate it with the AV training task.
