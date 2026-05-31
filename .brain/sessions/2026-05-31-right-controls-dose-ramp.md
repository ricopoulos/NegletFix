# Session: Sparse Right Controls + Dose Ramp

**Date:** 2026-05-31
**Branch:** `main`
**Status:** Implementation complete; Quest headset 5-minute, 8-minute, and 12-minute ramp validations passed

## Summary

Started the Sunday session from the guided-pilot next action: add sparse right-side control trials, keep them separately logged, and choose the first rehab dose ramp. Completed the code path, regenerated the Unity scenes, updated memory/wiki state, passed PlayMode validation, rebuilt the Session 1 pilot APK, installed it on Quest 2, completed the 5-minute control-ramp run on headset, then enabled Wi-Fi ADB and completed the 8-minute and 12-minute ramps.

## Session Start

- `git pull --ff-only` returned already up to date.
- Read `.brain/index.json`, `.brain/backlog.md`, recent sessions, crumbs, and the relevant wiki pages before editing.
- Branch audit found no unmerged local or remote work branches relative to `main`.
- Existing untracked/generated files were left untouched.

## Code Changes

- `AudioVisualTraining.cs`
  - Added sparse intact-hemifield control trial support.
  - For Eric's baseline, the intact hemifield resolves to right-side controls.
  - Default policy: 15% control probability, minimum 3 rehab trials between controls, first recorded trial always rehab-dose.
  - Control trials can reward but do not update the adaptive staircase.
  - Rehab and control hit rates are tracked and displayed separately.

- `DataLogger.cs`
  - Trial CSV now includes `trial_type`, `is_control_trial`, and `counts_for_rehab_dose`.
  - Tracks total, rehab-dose, and control trial counts.
  - Writes a commented trial summary when the trial file closes.

- `AudioVisualTrainingSmokeTest.cs`
  - Forces controls in the smoke test so both pathways are covered.
  - Asserts the CSV header includes the new fields.
  - Asserts at least one `rehab` row and one `right_control` row.

## Scene / Dose Changes

- `AVTrainingSession1Pilot.unity`
  - Recorded block changed first to 300s (5 minutes) for the first ramp, then to 480s (8 minutes) for the second ramp.
  - Current serialized ramp value is 720s (12 minutes).
  - Practice remains 15s.
  - Sparse right controls enabled.
  - High validation contrast floor remains for the first ramp.

- `AVTrainingQuickReadyCheck.unity`
  - Controls disabled so it stays a pure left-visibility/controller smoke check.

- `AVTrainingManualSmoke.unity`
  - Regenerated with the new control policy available.

Dose decision:
- Do not repeat the 2-minute guided validation block unless Eric feels fragile that day.
- Do not jump directly to 30 minutes.
- Run 5 minutes first, then ramp to 8-10, 12-15, 20, 25, and 30 minutes if tolerated.
- Reduce the validation contrast floor later only if right-control hit rate is near ceiling and left engagement is tolerable.

## Validation

- PlayMode smoke passed:
  - `Unity/NeglectFix/SmokeResults/playmode-right-control-ramp-results.xml`
  - Result: 1 test, 1 passed, 0 failed

- APK build passed:
  - `Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk`
  - Finder size: 64 MB
  - Unity build result: Success

- `git diff --check` passed after scene whitespace cleanup.

- First HTML rehab report prototype generated after the dose ramp:
  - Generator: `scripts/generate-rehab-report.js`
  - Report: `reports/rehab-dose-ramp-2026-05-31.html`
  - Inputs: the 5-minute, 8-minute, and 12-minute trial/session CSVs from `Unity/NeglectFix/SmokeResults/ControlRamp/`, `Ramp8/`, and `Ramp12/`
  - Output story: 25 recorded training minutes, 538 total trials, 242/486 left rehab hits, and 52/52 right-control hits.
  - Design direction: "premium evidence storytelling" using GSAP enhancement where available, with static evidence visible even if the CDN is unavailable.
  - Field map iteration added: Evidence / Retinal truth / Heat bloom modes. Accuracy caveat documented in-report: current logs provide horizontal eccentricity and hemifield, not true vertical retinal coordinates.

- Quest 2 control-ramp run passed:
  - APK installed/launched: `Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk`
  - Live log: `Unity/NeglectFix/SmokeResults/ControlRamp/control-ramp-live.log`
  - Trial CSV: `Unity/NeglectFix/SmokeResults/ControlRamp/av_training_2026-05-31_08-55-37.csv`
  - Session CSV: `Unity/NeglectFix/SmokeResults/ControlRamp/session_2026-05-31_08-55-10.csv`
  - Runtime: 5.5 minutes total including ready/baseline/practice/cooldown; recorded block closed cleanly at 110 trials.
  - Split: 96 rehab-dose left trials, 14 right-control trials.
  - Rehab result: 53/96 hits (55.2%), mean hit RT ~499 ms, final staircase 0.15 LogCS.
  - Right-control result: 14/14 hits (100%), mean hit RT ~380 ms.
  - CSV flags verified: `right_control` rows have `is_control_trial=1` and `counts_for_rehab_dose=0`; rehab rows have `counts_for_rehab_dose=1`.
  - Control spacing respected the minimum: at least 3 rehab trials before each control.

- Quest 2 8-minute ramp passed over Wi-Fi ADB:
  - APK installed/launched wirelessly after `scripts/quest-adb.sh enable-wifi` connected to `10.0.0.136:5555`.
  - Live log: `Unity/NeglectFix/SmokeResults/Ramp8/ramp8-live.log`
  - Trial CSV: `Unity/NeglectFix/SmokeResults/Ramp8/av_training_2026-05-31_09-13-38.csv`
  - Session CSV: `Unity/NeglectFix/SmokeResults/Ramp8/session_2026-05-31_09-12-41.csv`
  - Runtime: 8.5 minutes total including ready/baseline/practice/cooldown; recorded block closed cleanly at 173 trials.
  - Split: 157 rehab-dose left trials, 16 right-control trials.
  - Rehab result: 75/157 hits (47.8%), mean hit RT ~533 ms, final staircase 0.30 LogCS.
  - Right-control result: 16/16 hits (100%), mean hit RT ~392 ms.
  - CSV flags verified: `right_control` rows have `is_control_trial=1` and `counts_for_rehab_dose=0`; rehab rows have `counts_for_rehab_dose=1`.
  - Control spacing respected the minimum: at least 3 rehab trials before each control.
  - Session CSV logged 4570 samples.
  - Note: startup had a short Quest system/passthrough focus hiccup before practice; the app recovered, resumed focus, completed the run, and logs closed cleanly.

- Quest 2 12-minute ramp passed over Wi-Fi ADB:
  - APK installed/launched wirelessly with cached Wi-Fi ADB at `10.0.0.136:5555`.
  - Live log: `Unity/NeglectFix/SmokeResults/Ramp12/ramp12-live.log`
  - Trial CSV: `Unity/NeglectFix/SmokeResults/Ramp12/av_training_2026-05-31_09-33-25.csv`
  - Session CSV: `Unity/NeglectFix/SmokeResults/Ramp12/session_2026-05-31_09-32-56.csv`
  - Runtime: 12.5 minutes total including ready/baseline/practice/cooldown; recorded block closed cleanly at 255 trials.
  - Split: 233 rehab-dose left trials, 22 right-control trials.
  - Rehab result: 114/233 hits (48.9%), mean hit RT ~554 ms, final staircase 0.15 LogCS.
  - Right-control result: 22/22 hits (100%), mean hit RT ~444 ms.
  - CSV flags verified: `right_control` rows have `is_control_trial=1` and `counts_for_rehab_dose=0`; rehab rows have `counts_for_rehab_dose=1`.
  - Control spacing respected the minimum: at least 3 rehab trials before each control.
  - Session CSV logged 6725 samples.
  - No focus loss, Android runtime errors, or Unity exceptions were found in the live log.

Known build warnings remained non-blocking:
- Obsolete `Object.FindObjectOfType<T>()` warnings.
- Android input handling set to Both.
- OpenXR/URP project validation warning.
- TextMeshPro/IL2CPP size warnings.

## Next Actions

1. Build the quick field-mapping calibration scene next: fixed cross, controlled points left/right/up/down, separate from rehab.
2. Expand calibration trial logging: horizontal angle, vertical angle, stimulus world position, camera-relative direction, head yaw/pitch at stimulus onset, and head yaw/pitch at response.
3. Use the calibration map to choose the first defensible rehab training locations.
4. Get Eric's subjective report from the stacked 5+8+12-minute day: fatigue, emotional load, left-marker visibility, and whether the right controls felt obvious.
5. Prefer stopping for today after 25 minutes of recorded dose. Next dose decision after calibration: repeat 12 minutes if the stack felt heavy, or move to 15 minutes if tolerable.
6. Keep the high validation contrast floor for the next ramp unless subjective visibility is stable and the team explicitly wants the staircase to become more clinically meaningful.
7. Continue pulling trial CSVs after each run and review rehab/control split before increasing dose.
8. Turn the first report prototype into a reusable per-session report command once the next headset session produces fresh CSVs.
