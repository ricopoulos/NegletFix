# Session: Field Map Calibration + Field-Guided Rehab Run

**Date:** 2026-06-11
**Branch:** `main`
**Status:** Field-mapping scene built and Quest-validated; first field-guided 12-minute rehab run completed, but next build must retune targets/probes before increasing dose

## Summary

Built the separate quick field-mapping calibration scene requested after the 2026-05-31 dose ramp. The scene keeps assessment separate from rehab, uses a fixed center cross, tests controlled left/right/up/down points, logs spatial/head-pose fields, and writes a per-point recommendation. After a valid Quest run, the map recommended left `-5°`, vertical `0°` as the first boundary rehab location.

Then wired that calibration result into `AVTrainingSession1Pilot`: rehab-dose rows use the selected field-map location while sparse right controls remain separate and excluded from dose/staircase. The updated APK was installed and run on Quest 2 over Wi-Fi ADB on 2026-06-11.

## Code Changes

- `Assets/Scripts/Assessment/FieldMappingCalibration.cs`
  - New quick field-mapping task with fixed cross, randomized controlled point trials, response logging, and per-point summary/recommendation.
  - Logs horizontal/vertical angle, stimulus world position, camera world position, camera-relative direction, head yaw/pitch/roll at onset, and head pose/direction at response.

- `Assets/Scripts/Utils/DataLogger.cs`
  - Added field-mapping CSV logging and recommendation summary.
  - Extended AV trial logs with `horizontal_angle_deg` and `vertical_angle_deg`.

- `Assets/Scripts/Tasks/AudioVisualTraining.cs`
  - Added `useFieldMapGuidedRehabTargets` and `fieldMapGuidedRehabAnglesDeg`.
  - Rehab-dose trials can now use exact horizontal/vertical target angles from calibration.
  - Control trials keep the existing intact-field ladder and remain separately flagged.

- `Assets/Editor/AvTrainingManualSmokeSceneBuilder.cs`
  - Added menu/batch support for `FieldMappingCalibration.unity` and APK builds.
  - Regenerated `AVTrainingSession1Pilot.unity` with field-guided rehab target `(-5°, 0°)`.

- `Assets/Tests/PlayMode/FieldMappingCalibrationSmokeTest.cs`
  - New smoke coverage for the calibration CSV path.

- `Assets/Tests/PlayMode/AudioVisualTrainingSmokeTest.cs`
  - Updated to assert angle columns and the field-guided rehab target.

- `scripts/generate-rehab-report.js`
  - Parser now accepts the new horizontal/vertical angle columns and falls back to old CSVs.

## Validation

- PlayMode validation passed before the APK build:
  - `Unity/NeglectFix/SmokeResults/playmode-field-guided-rehab-results.xml`
  - Result: 2 tests, 2 passed, 0 failed.
- Field-mapping APK build passed:
  - `Unity/NeglectFix/Builds/FieldMappingCalibration.apk`
  - Build log: `Unity/NeglectFix/SmokeResults/build-field-mapping.log`
- Field-guided Session1Pilot APK build passed:
  - `Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk`
  - Build log: `Unity/NeglectFix/SmokeResults/build-session1-field-guided.log`
- `git diff --check` passed after scene whitespace cleanup.

## Quest Field Mapping

First headset attempt was invalid because Eric was not wearing the headset; all responses were misses. Those files were moved under:

- `Unity/NeglectFix/SmokeResults/FieldMapping/invalid_no_headset/`

Valid Quest 2 run:

- Trial CSV: `Unity/NeglectFix/SmokeResults/FieldMapping/valid_2026-06-02_15-35/field_mapping_2026-06-02_15-35-30.csv`
- Session CSV: `Unity/NeglectFix/SmokeResults/FieldMapping/valid_2026-06-02_15-35/session_2026-06-02_15-35-29.csv`
- Result: 19/26 hits.
- Right/up/down controls: all 6/6.
- Left axis:
  - `-5°`: 1/2 hits, mean hit RT 625ms.
  - `-8°`, `-12°`, `-16°`: 0/2 hits each.
- Recommendation written by the CSV: `left(h=-5.0,v=0.0,hit_rate=50 %,n=2,reason=boundary)`.

Interpretation: the map did its job as a quick orientation/visibility assessment. It does not replace formal perimetry, but it gave a defensible first rehab target.

## Field-Guided Rehab Run

Date/time: 2026-06-11 evening, Quest 2 over Wi-Fi ADB.

Files:

- Trial CSV: `Unity/NeglectFix/SmokeResults/FieldGuidedRehab/2026-06-11_21-46/av_training_2026-06-11_21-47-08.csv`
- Session CSV: `Unity/NeglectFix/SmokeResults/FieldGuidedRehab/2026-06-11_21-46/session_2026-06-11_21-46-40.csv`
- Device log: `Unity/NeglectFix/SmokeResults/FieldGuidedRehab/2026-06-11_21-46/field-guided-rehab-live.log`

Runtime summary:

- Total duration: 12.5 minutes.
- Session samples: 6744.
- Total recorded trials: 283.
- Rehab-dose trials: 259, all at left `-5.00°`, vertical `0.00°`.
- Rehab result: 230/259 hits (88.8%), mean hit RT ~559ms.
- Right controls: 24/24 hits (100%), excluded from rehab dose.
- Total rewards: 261.
- Live log ended cleanly: trial log closed, cooldown completed, session log closed, app force-stopped after pulling files.
- Headset battery after run: 87%.

## Subjective Report

Eric reported that he had to focus strongly on the center cross. Some responses felt more like reflex/prediction than true visual confirmation: the sound made him infer the marker location, his eyes shifted left, and he sometimes felt he was guessing rather than checking. He also felt the left marker was probably too close to the center. The run was exhausting, in the way real training is exhausting.

Interpretation: do not treat the high `-5°` hit rate as clinical recovery or true left-field detection. This run demonstrates that the task can drive leftward orienting and sustained effort, but the current location/audio cue can be too easy or too cueable.

## Launch Friction

- Cached Wi-Fi ADB at `10.0.0.136:5555` initially refused connection, likely because wireless ADB reset after headset sleep/reboot.
- A short USB reconnection restored terminal ADB.
- `scripts/quest-adb.sh enable-wifi` re-enabled Wi-Fi ADB.
- Horizon blocked the first launch behind a controllers-required dialog; Eric had to wake controllers and dismiss it in headset before Unity entered foreground.

Next live-run checklist:

1. Headset on and awake.
2. Controllers awake before launch.
3. If Wi-Fi ADB refuses, plug USB briefly and run `scripts/quest-adb.sh enable-wifi`.
4. Confirm Unity is foreground before starting log monitoring.

## Next Build Decision

Before increasing dose, retune the protocol:

1. Do not continue with `-5°` alone.
2. Mix `-5°` with harder boundary targets, likely prioritizing `-8°`.
3. Add catch/probe trials, for example sound-only or marker-only trials, to separate audio-guided prediction from visual confirmation.
4. Cap the contrast/staircase output so high hit rate at an easy/cued location does not run away to meaningless LogCS values.
5. Keep assessment/calibration separate from rehab.
6. Treat fatigue as a dose signal; do not stack another run after this session.
