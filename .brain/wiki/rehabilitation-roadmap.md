---
title: Rehabilitation Roadmap
last_updated: 2026-06-11
confidence: MIXED
sources:
  - .brain/index.json
  - .brain/backlog.md
  - .brain/sessions/2025-12-15.md
  - CLAUDE.md
  - PROJECT_SUMMARY.md
  - NEUROFEEDBACK_PROTOCOL.md
  - 2026-05-14 PubMed-verified audit — see [[research-papers-index]] Recent Additions
  - .brain/sessions/2026-05-30-quest-guided-pilot-wrap.md
  - 2026-06-11 clinical-trials watchlist refresh — see [[clinical-trials-watchlist]]
  - .brain/sessions/2026-06-11-field-map-guided-rehab-wrap.md
---

# Rehabilitation Roadmap

Honest progress map. What has been done, what is next, and what is genuinely unknown or at risk. Confidence is intentionally mixed: what's DONE is high (measured or committed); what's NEXT is medium (planned, parameters known, untested on Eric); what's HOPED FOR is low to unknown.

See [[erics-baseline]] for the measured starting point, [[audiovisual-training-protocol]] for the main intervention, [[unity-architecture]] for code readiness.

---

## 1. DONE — Measured / Committed [HIGH]

### Baseline measurement established (2025-12-15)
- Central ~1.05+ LogCS, Right 2.25 LogCS (maxed), Left 0.00 LogCS, Asymmetry 2.25
- First validated measurement — all prior tests invalidated by hemifield bug
- Commit `cddc43c` on `main`
- See [[erics-baseline]]

### Hemifield positioning bug fixed (2025-12-15)
- Letters now correctly render in left/right peripheral positions, not always at center
- Fixation cross added for valid hemifield testing
- Console tags (`[Central]`, `[LeftHemifield]`, `[RightHemifield]`) for verification
- See [[contrast-sensitivity-test]]#the-hemifield-positioning-bug-fixed-2025-12-15

### Unity project scaffold
- 10 C# scripts across Assessment, EEG, Tasks, Utils (3,339 LOC)
- MuseOSCReceiver + EngagementCalculator + RewardController + GazeDetector + DataLogger wired
- EEG simulator works for development without hardware (with the `OnKeyPress()` caveat noted in [[unity-architecture]]#known-code-issues)
- Unity 6.2 VR template + GitHub repo operational
- See [[unity-architecture]]

### Research synthesis
- 15+ papers catalogued, evidence graded
- Daibert-Nido 2021 protocol parameters extracted and documented (since reframed — see below)
- Ros et al. 2017 neurofeedback target identified (TP10, ↓α ↑β)
- See [[research-papers-index]], [[scientific-foundation]]

### Brain v2.0 + Obsidian bridge sync (2026-03-21)
- Memory system upgraded to v2 methodology
- Session rituals (`/start-session`, `/end-session`, `/crumb`, `/branch-audit`) operational

### 2026-05-14 — Research audit + literature expansion
- 4-agent parallel audit + PubMed-verified literature pass identified 20+ new papers and corrected 3 citation errors + 1 misframed anchor
- Daibert-Nido 2021 downgraded from HIGH "anchor" to LOW–MEDIUM (N=2 pediatric pilot, not adult stroke trial)
- Strong new evidence for chronic responsiveness: **Namgung 2024/2025** (n=82 RCTs each), **Alharshan/Alwashmi 2026** (n=15 stroke HH, DTI mechanism), **Raffin 2025** (cf-tACS chronic, n=16), **Diana 2025** (AV+tDCS chronic, n=18)
- Critical reality-check evidence: **Yang/Cavanaugh/Saionz 2023** (only 58% of chronic patients respond, blind-field CS stays ~4× lower than intact), **Saionz 2025 TVST** natural history (chronic VF stable), **Treves 2025 JMIR** consumer-NF meta (null under sham)
- Hemianopia-vs-neglect category mismatch flagged in [[eeg-neurofeedback]] — EEG-NF layer is now framed as exploratory adjunct
- See [[research-papers-index]] Recent Additions section for full citations

### 2026-05-16 — Quest dev mode resolved + Phase 1 Unity scaffold built
**Quest 2 dev mode** (April 27 blocker, finally closed):
- Verified the "only the owner can do it" error refers to the headset's primary Meta account, not the dev org owner (April 27 misdiagnosis retired)
- Working fix: previous owner unpaired from her Meta Horizon app → factory reset Quest 2 → re-paired to Eric's Meta account → dev mode toggled → adb authorized
- Quest 2 serial `1WMHH831TR1047`, Android 14, current firmware

**Phase 1 Unity scaffold built** (commits `ecf327f` + `7ea389c` + `24a3075` on `main`):
- New scripts in `Tasks/`: `AudioVisualTraining.cs` (Paradigm B main task), `ProgramScheduler.cs` (session state JSON), `EccentricityProgression.cs` (baseline-driven ladder)
- `DataLogger.cs` extended with `LogTrainingTrial()` + per-session AV trial CSV
- `RewardController.cs` decoupled from EEG — `RewardMode` enum with `OpenLoop` (v1 default) and `EegGated` (v2 reserved)
- `Packages/manifest.json` — added XR Management 4.5.0, OpenXR 1.14.0, Interaction Toolkit 3.0.7
- XR Plug-in Management configured: OpenXR enabled for Android, Oculus Touch Controller Profile, Meta Quest Support feature enabled
- Project Validation: 14/15 checks passing (SSAO is a deferred polish item)
- Unity 6.2 imported the project on first open with **zero errors, zero warnings**

**Build plan deliverable**: `docs/build-plan-2026-05-16.html` — styled HTML build plan capturing the paradigm decision (Both A+B sequential), the EEG deferral, the 5-phase build sequence, and honest expectations.

**Open from 2026-05-16**:
- Smoke test the AV training in Editor (fresh-head task — deferred to next session)
- Quest controller input binding via InputSystem (replaces keyboard SPACE fallback in `DetectResponse()`)
- Quest 3 acquisition (budget-dependent)

### 2026-05-30 — Quest AV guided pilot completed

Source map: `.brain/sessions/2026-05-30-quest-guided-pilot-wrap.md`.

This session moved the AV training path from code scaffold to a real headset-validated pilot:
- Restored ADB workflow after USB friction; added `scripts/quest-adb.sh` for cached Wi-Fi ADB.
- Added ready prompt/countdown, completion message, controlled gray backdrop, center fixation cross, and unlogged practice block.
- Added `AVTrainingQuickReadyCheck` for fast visual/controller smoke testing.
- Rebuilt and installed `AVTrainingSession1Pilot.apk`.
- Eric completed the guided Quest pilot.

Recorded pilot result:
- 45 recorded trials
- 33 hits (73.3%)
- all left hemifield
- eccentricity range `-8°..-5°`
- average hit RT ~475ms
- final staircase log `0.45 LogCS`

Interpretation:
- The UI/visibility/control path is now good enough to expose the real left-field deficit.
- This was emotionally heavy but productive; Eric explicitly framed the brutality as part of the rehab journey.
- Next iteration should add sparse right-control trials to confirm attention/input/visibility while preserving left-field therapeutic dose.

### 2026-05-31 — Sparse right controls + first dose ramps validated

The next concrete polish pass is implemented in code, scene presets, and Quest logs:
- `AudioVisualTraining` now inserts sparse intact-field controls during recorded blocks. For Eric's baseline this resolves to occasional right-side controls.
- Control rows are labeled in the trial CSV with `trial_type=right_control`, `is_control_trial=1`, and `counts_for_rehab_dose=0`.
- Control trials do not update the adaptive staircase; only affected-left rehab trials drive LogCS adaptation.
- `AVTrainingSession1Pilot.unity` was regenerated first as a 5-minute recorded Session 1 ramp, then as 8-minute and 12-minute recorded ramps with ~15% controls and 15s practice.
- `AVTrainingQuickReadyCheck.unity` remains left-only so it still tests prompt/controller/left-marker visibility without controls.
- Quest 2 5-minute control-ramp run passed: 110 recorded trials, 96 left rehab-dose trials, 14 right-control trials, left rehab hit rate 53/96 (55.2%), right-control hit rate 14/14 (100%), final staircase 0.15 LogCS.
- Logs pulled to `Unity/NeglectFix/SmokeResults/ControlRamp/`.
- Quest 2 8-minute ramp run passed over Wi-Fi ADB: 173 recorded trials, 157 left rehab-dose trials, 16 right-control trials, left rehab hit rate 75/157 (47.8%), right-control hit rate 16/16 (100%), final staircase 0.30 LogCS, 4570 session samples.
- Logs pulled to `Unity/NeglectFix/SmokeResults/Ramp8/`.
- Quest 2 12-minute ramp run passed over Wi-Fi ADB: 255 recorded trials, 233 left rehab-dose trials, 22 right-control trials, left rehab hit rate 114/233 (48.9%), right-control hit rate 22/22 (100%), final staircase 0.15 LogCS, 6725 session samples.
- Logs pulled to `Unity/NeglectFix/SmokeResults/Ramp12/`.

### 2026-06-11 — Field map + first field-guided rehab run completed

Built the separate quick field-mapping calibration scene and used its result to drive the next rehab run:
- `FieldMappingCalibration.unity` keeps assessment separate from rehab and uses a fixed center cross with controlled left/right/up/down points.
- Field-map trial CSV logs horizontal/vertical angle, stimulus world position, camera-relative direction, and head yaw/pitch/roll at onset/response.
- Valid Quest field map: 19/26 hits; right/up/down controls all 6/6; left `-5°` was 1/2; left `-8°`, `-12°`, `-16°` were 0/2.
- Recommendation: left `-5°`, vertical `0°` as the first boundary target.
- `AVTrainingSession1Pilot` now supports field-map-guided rehab target angles.
- First field-guided Quest run completed: 12.5 minutes, 283 recorded trials, 259 rehab-dose trials at left `-5°`, 24 right controls, 6744 session samples.
- Rehab hit rate was high at `230/259` (88.8%); right controls were 24/24 (100%).

Interpretation:
- Do not treat the high `-5°` hit rate as clinical recovery.
- Eric reported that some responses felt like audio-guided prediction/reflex rather than true visual confirmation, and that the target felt too close to center.
- This is useful training/orienting evidence, but the next build must make the task harder and more interpretable.

---

## 2. NEXT — Planned, Ready to Build [MEDIUM]

### WIP-001: Audiovisual training module — active polish before rehab start

The AV module is no longer paused or merely scaffolded. It is on-headset validated through field-guided rehab, but the next step is retuning before dose escalation:
1. Sparse right-control trials are implemented at ~15%, with at least 3 rehab-dose trials between controls.
2. Left hemifield remains the primary therapeutic dose; controls are excluded from the staircase and from rehab-dose counts.
3. First 5-minute, 8-minute, 12-minute, and field-guided 12.5-minute Quest runs are complete.
4. Next build: do not continue with left `-5°` alone; mix `-5°` with a harder boundary target such as `-8°`.
5. Add catch/probe trials, for example sound-only and/or marker-only trials, to separate audio-cued prediction from true visual confirmation.
6. Cap or redesign the contrast staircase so high hit rate at an easy/cued location cannot climb to meaningless LogCS values.
7. Keep fixed-gaze contrast/field assessment separate from AV training.

### WIP-002: Quick field-mapping calibration scene — completed, keep as assessment layer

The first GSAP-enhanced HTML report made the field map valuable enough to turn into a real calibration layer. That layer now exists as `FieldMappingCalibration.unity` and should remain separate from rehab.

Future calibration improvements:
1. Increase repeats if the result will choose several training points.
2. Keep logging horizontal angle, vertical angle, world position, camera-relative direction, and head pose at onset/response.
3. Use calibration output to select a small defensible training set, not a single easy target.
4. Do not present it as Humphrey/perimetry.

See [[audiovisual-training-protocol]]#what-to-build-in-unity for build spec.

### Periodic re-assessment cadence
Per [[contrast-sensitivity-test]] and [[audiovisual-training-protocol]]:
- **Every ~5 training sessions**: quick-check (Central + Left only) — monitor affected-field trajectory
- **Post session 20**: full 3-hemifield reassessment — compare to baseline
- Append each to [[erics-baseline]]#progress-log, never overwrite

### Quest VR deployment (current state)
- Quest AV training deployment is operational for APK install/launch/log pull.
- Contrast sensitivity test still needs its own Quest validation; do not infer assessment validity from the AV pilot.
- Replace ±300px UI offset with proper visual-angle positioning for future VR assessment mode.
- Verify fixation cross and letter rendering for the contrast test separately from AV training markers.

### Audiovisual training program
Three dose options, picked based on Eric's chronic profile (see [[audiovisual-training-protocol]] §1):
- **Option A — Daibert-Nido 2021 dose** (~5 hr total over 6-7 weeks): the original pediatric-pilot dose. Probably under-dosed for chronic adult.
- **Option B — Misawa/Daibert-Nido 2024 dose** (~5.25 hr, Meta Quest at home): same total budget, pediatric Quest validation.
- **Option C — Alharshan/Alwashmi 2026 dose** (~15 hr, 30 min × 5 days/wk × 6 weeks): **the only direct adult-stroke evidence**. Recommended for Eric.

Expected outcomes (revised 2026-05-14):
- Primary: ≥+0.30 LogCS at scotoma-border locations in ~50-60% of responders (Yang/Cavanaugh/Saionz 2023, n=12 chronic)
- Secondary: detection-RT improvements, scanning efficiency, ADL transfer
- **Not realistic**: closing the left-hemifield 0.00 → right-hemifield 2.25 asymmetry — chronic blind-field CS stays ~4× lower than intact even after training

### EEG integration (optional, Phase 2)
Currently `status: planned`, `health: gray` in `.brain/index.json:16-20`:
- Wire Muse → Mind Monitor → Unity via real (not simulated) OSC
- Uncomment `#define EXTOSC_INSTALLED` after installing extOSC
- First 2-3 sessions: baseline EEG without closed-loop, to validate signal quality on Eric's head
- Then enable closed-loop gating of audiovisual rewards

---

## 3. UNKNOWN / AT RISK [LOW — be honest]

### Potential adjuncts (not currently in the build, worth tracking)
Added 2026-05-14; registry refreshed 2026-06-11:
- **cf-tACS V1↔MT** (Raffin 2025 Brain, n=16 chronic): forward cf-tACS + motion training expanded kinetic field borders faster than typical VPL. **Not actionable for home setup** — tACS hardware separate, expert placement required, no consumer-grade equivalent. But a strong "if we ever get clinic time" option.
- **Anodal tDCS over ipsilesional occipital cortex** (Diana 2025 Eur J Neurol / [NCT06116760](https://clinicaltrials.gov/study/NCT06116760), n=18 chronic): added value over AV training alone, 2 h/day × 10 days. tDCS hardware/montage/safety is separate from Quest/Muse. Discuss with Eric's neurologist before any adoption. Current status: **clinician-supervised adjunct candidate only**, not a home protocol change.
- **Active AV+tDCS trial to watch**: [NCT07358832](https://clinicaltrials.gov/study/NCT07358832), recruiting subacute stroke HVFD patients for AV training plus real/sham occipital tDCS. Important future evidence, but not Eric's chronic window.
- **Pharmacological adjuncts**: see [[pharmacological-adjuncts]] (already separate). Levodopa, fluoxetine, citicoline — all have weak/conflicting evidence for chronic visual rehab; neurologist conversation territory only.

### Research tracking system [operational-lite, 2026-06-11]

The research watchlist is now data-backed rather than only narrative:
- Wiki source of truth: [[clinical-trials-watchlist]]
- Structured CSV: `docs/research/clinical-trials-watchlist-2026-06-11.csv`
- Source queue integration: `docs/research/source-queue-2026-05-25.csv` now includes CTG trial rows.

Recommended next system step: generate a static HTML dashboard from the CSV before creating automation. A monthly Codex automation is appropriate once the fields are stable; it should report registry/PubMed changes and proposed diffs, not auto-change the medical protocol.

### Muse signal quality on Eric specifically [DOWNGRADED to HIGH risk, 2026-05-14]
Ros et al. 2017 used medical-grade EEG. Muse is consumer-grade dry-electrode. **Sokolova 2024 + Pavlov 2024 device comparisons rank Muse S last among consumer EEG devices on signal quality** — log-SNR significantly lower than research-grade, weak alpha-spindle detection even eyes-closed. Treves 2025 JMIR meta (16 RCTs, 11 used Muse) showed null cognitive effects under sham control. The combination — weak signal + weak meta-analytic effects — argues for treating EEG-NF as exploratory only. **First session priority**: validate signal quality on Eric's head (visible eyes-open vs eyes-closed alpha modulation) before counting Muse data.

### Whether Eric is a "neurofeedback responder"
Not everyone learns EEG self-regulation. The literature on neurofeedback responders vs. non-responders is imperfect. Even for responders, learning curve is 3-10+ sessions (Ros et al. 2017; `NEUROFEEDBACK_PROTOCOL.md:321-326`). If Eric is a non-responder, the project's closed-loop layer adds no value — but the open-loop audiovisual layer (which worked in Daibert-Nido without EEG) still should. [MEDIUM risk — failure mode has a fallback]

### Whether Daibert-Nido gains reproduce in a chronic adult [revised 2026-05-14]
The published +0.31 to +0.54 LogCS range was the **cohort mean of N=2 pediatric brain-tumor survivors**, not adult stroke. Yang/Cavanaugh/Saionz 2023 (n=12 chronic adult stroke) found only 58% of patients responded at any trained location and blind-field CS stayed ~4× lower than intact-field after training. Realistic expectation for Eric: **~50-60% chance of any measurable gain; magnitude likely 0.20-0.40 LogCS at scotoma-border locations if responding, not full restoration**. Clinically significant change is still ≥0.30 LogCS (Elliott/Bullimore/Bailey 1991), so a responder outcome still counts as success. [HIGH risk for magnitude, MEDIUM risk for direction (was LOW)]

### Whether gains transfer to daily-life function
Contrast sensitivity is an eccentricity-specific, luminance-specific lab number. It may improve on the test without Eric subjectively noticing at dinner. Literature is mixed on transfer; Topics in Stroke Rehab 2020 argues ecological VR helps transfer, but that's indirect evidence. This is the biggest unknown — and the one that matters most to Eric. [HIGH uncertainty]

### The "gray overlay" hypothesis
Whether audiovisual training improves Eric's bilateral dimness is completely theoretical — see [[scientific-foundation]]#relevance-to-erics-specific-case. The Central LogCS in the progress log is the only way to test it. [THEORETICAL, unvalidated]

### Calibration drift
Display brightness, viewing distance, ambient light all affect contrast measurements. Without a calibrated test rig, small month-over-month changes could be noise not signal. Mitigation: re-run Central test frequently as a calibration check — if Central drifts without Left/Right changing, it's likely display-side. [MEDIUM risk, detectable]

---

## 4. Honest Summary

| Layer | Status | Evidence | Confidence |
|-------|--------|----------|------------|
| Instrument (contrast sensitivity test) | Working | Bug fixed, produces valid per-hemifield LogCS | HIGH |
| Baseline | Measured | Central ~1.05, Right 2.25, Left 0.00 | HIGH |
| Code for audiovisual training | Quest-guided pilot completed | 45 left-field recorded trials, clean CSV close, Eric confirmed flow works | MEDIUM-HIGH for flow / MIXED for rehab effect |
| Code for EEG pipeline | Written, untested with real Muse | Simulator path works | MEDIUM |
| Audiovisual training effect on Eric's left hemifield | Started as flow pilot, not full dose | 73.3% hit rate at `-5°/-8°`; no pre/post outcome claim yet | MEDIUM predict |
| Effect on central dimness ("gray overlay") | Not attempted | Theoretical only | UNKNOWN |
| Transfer to daily function | Not attempted | Mixed literature | UNKNOWN |

The project has the hardware, the research base, the instrument, the baseline, a Quest-validated AV training pilot, sparse right-side controls, headset-validated 5-minute, 8-minute, and 12-minute dose ramps, and the first HTML evidence report. What's missing before the official rehab phase is a quick field-mapping calibration scene, richer per-trial spatial logging, Eric's subjective tolerance report, the next ramp decision, the clinical contrast-floor decision, and 6 weeks of disciplined Eric-time.

---

## 5. Critical Path

1. **Build quick field-mapping calibration** — fixed cross, controlled left/right/up/down points, expanded spatial/head-pose trial schema.
2. **Generate a calibration map** — use it to choose defensible first rehab training locations.
3. **Collect subjective report** — fatigue, emotional load, left-marker visibility, and whether right-control markers felt clearly visible after the 25-minute same-day stack.
4. **Decide next dose** — repeat 12 minutes if heavy, or move to 15 minutes next session if tolerable.
5. **Keep reviewing CSV split** — rehab/control counts, control hit rate, left hit rate, final staircase, and new spatial map fields after every ramp.
6. **Mid-pilot re-test** — run full contrast sensitivity test, append to [[erics-baseline]]#progress-log, check signal in left-hemifield score.
7. **Wire Muse for real** — install extOSC, confirm signal quality on Eric.
8. **Enable closed-loop only after signal validation** — add engagement gating to the training task.
9. **Complete the 30-session Alharshan-style program** with mid-course quick-checks every 5 sessions if tolerated.
10. **Post-program full re-assessment** — compare to 2025-12-15 baseline.
11. **Document outcome honestly** — whatever the result is, log it.
12. **Refresh clinical-trial watchlist monthly or after major PubMed hits** — update [[clinical-trials-watchlist]] only when status, posted results, or protocol-impact claims change.

---

## 6. Cross-References

- Where we start: [[erics-baseline]]
- The instrument that tracks us: [[contrast-sensitivity-test]]
- The main intervention: [[audiovisual-training-protocol]]
- External evidence monitor: [[clinical-trials-watchlist]]
- The optional closed-loop layer: [[eeg-neurofeedback]]
- Theoretical backbone: [[scientific-foundation]]
- Build details: [[unity-architecture]]
- Hardware readiness: [[hardware-setup]]
