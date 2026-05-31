# Session: Quest AV Guided Pilot

**Date:** 2026-05-30 / 2026-05-31 local late evening
**Branch:** `main`
**Status:** Completed and validated on Quest 2

## Summary

Started with unstable USB/ADB friction, then restored Quest connectivity through Meta Quest Developer Hub and terminal ADB. Added a Wi-Fi ADB helper for future loops, rebuilt the AV training pilot multiple times, and ended with a working guided headset pilot that Eric completed.

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

## Next Actions

1. Add sparse right-hemifield control trials, around 10-20%, clearly logged separately from left rehab trials.
2. Keep the left hemifield as the main therapeutic dose.
3. Polish instructions and completion states based on the guided-pilot feedback.
4. Decide whether the first rehab session should use the 2-minute guided block, a 5-minute block, or a gradual ramp.
5. Preserve fixed-gaze contrast/field assessment as a separate mode; do not conflate it with the AV training task.
