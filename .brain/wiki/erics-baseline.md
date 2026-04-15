---
title: Eric's Baseline Measurement
last_updated: 2026-04-14
confidence: HIGH
sources:
  - CLAUDE.md
  - .brain/index.json
  - .brain/sessions/2025-12-15.md
  - .brain/cross-cutting.md
---

# Eric's Baseline — December 15, 2025

The anchor measurement of this project. First validated contrast sensitivity across all three visual fields **after** the hemifield positioning bug was fixed. Everything that follows — audiovisual training, EEG neurofeedback, every future measurement — gets compared against this baseline.

**Confidence: HIGH [MEASURED]** — validated instrument, bug-fixed code, console logs confirmed correct hemifield tagging, commit `cddc43c` on `main`.

See [[contrast-sensitivity-test]] for the instrument that produced these numbers.

---

## 1. Medical Context

| Field | Value |
|-------|-------|
| Patient | Eric Lespagnon |
| Age at baseline | ~56 (2025-12-15) |
| Diagnosis | Left Homonymous Hemianopia |
| Cause | Right PCA stroke (July 2021) with hemorrhagic transformation |
| MRI | Large area of encephalomalacia in right occipital lobe |
| Primary symptom | Complete left visual field blindness |
| Secondary symptom | Bilateral dimness / "gray overlay" — persistent, all day, not a comfort issue |
| Prior eye exam (Reina Eye Care, 2021-11-29) | 20/50 both eyes with spectacles; may be outdated |

Source: `CLAUDE.md:41-47`, `.brain/cross-cutting.md:8-15`, `docs/research/contrast_sensitivity_module.md:54-57`.

This is a **rehabilitation project, not accommodation** (`.brain/cross-cutting.md:17`).

---

## 2. Baseline Results (2025-12-15)

| Hemifield | LogCS Score | Contrast Threshold | Interpretation |
|-----------|-------------|--------------------|----------------|
| **Central** | ~1.05+ | ~9% | Good central vision |
| **Right (intact)** | **2.25** | **0.6%** | Excellent — maxed out the test |
| **Left (affected)** | **0.00** | **100%** | Severe deficit — could not see any letter at max contrast |

**Asymmetry: 2.25 LogCS** (right − left).

### What these numbers mean in plain language

- The right side sees better than a normal 56-year-old (normal range for age 50-60 is 1.65-1.95 LogCS, see [[contrast-sensitivity-test]]#clinical-reference-values). Eric maxed out the test ceiling at 2.25.
- The left side couldn't identify letters even at 100% contrast — **pure black on gray was invisible**. LogCS 0.00 means the test floor was never passed.
- The asymmetry (2.25) is **7.5x larger** than the clinically significant threshold of 0.30 LogCS (Elliott, Sanderson & Conkey 1990).

This objectively confirms the left homonymous hemianopia documented by MRI. It is also the first time this project produced a measurement that can be trusted (see §4).

---

## 3. Rehabilitation Target

Based on Daibert-Nido et al. (2021), DOI: 10.3389/fneur.2021.680211:
- Audiovisual home rehabilitation produces **+0.31 to +0.54 LogCS** improvement in contrast sensitivity after ~20 sessions (<5 hours total training time).
- Even the low end of that range (+0.31) is clinically significant (>0.30 threshold).

**Goal**: Move Eric's left hemifield score off 0.00. Any measurable improvement is neuroplastic evidence that the cross-modal pathway is engaging.

Secondary goal (unvalidated hypothesis, see [[scientific-foundation]]#relevance-to-erics-specific-case): Improve central contrast sensitivity as well, testing whether the "gray overlay" reflects a global gain-control deficit that audiovisual training might ameliorate.

See [[audiovisual-training-protocol]] for the implementation, [[rehabilitation-roadmap]] for the schedule.

---

## 4. Why This Baseline Can Be Trusted — The Bug That Lied

**All previous "baseline" attempts were invalid.** The hemifield positioning bug in `ContrastSensitivityTest.cs` meant that every letter rendered at screen center, regardless of which hemifield was being "tested" (`CLAUDE.md:87-98`).

Before the fix, Eric's left-hemifield score was artificially high because the letters were appearing where his vision is intact. The test reported success in a field that is functionally blind.

**The fix** (commit `cddc43c`, `.brain/sessions/2025-12-15.md:50-52`):
- Added `GetHemifieldUIOffset()` function
- Updated `ShowNextLetter()` to apply ±300px UI offset to the RectTransform.anchoredPosition
- Added fixation cross (red "+") for hemifield phases
- Added `[Central]` / `[LeftHemifield]` / `[RightHemifield]` console tags for verification

After the fix, the re-test produced the numbers above. The console logs confirmed correct hemifield placement letter-by-letter. The numbers match the MRI diagnosis. **This is the first run where the instrument was measuring what its label claimed.**

See [[contrast-sensitivity-test]]#the-hemifield-positioning-bug-fixed-2025-12-15 for full technical detail.

---

## 5. Progress Log

> **Append new measurements here, never overwrite.** Each re-assessment creates a new row. Do not edit past rows — a baseline is a historical artifact. If a measurement is invalidated (bad fixation, calibration change, etc.), add a new row with a "[INVALID]" note rather than deleting.

| Date | Central LogCS | Right LogCS | Left LogCS | Asymmetry | Notes |
|------|--------------|-------------|------------|-----------|-------|
| 2025-12-15 | ~1.05+ | 2.25 (maxed) | 0.00 | 2.25 | First validated baseline. Post hemifield-bug fix. Commit `cddc43c`. |
|  |  |  |  |  |  |

### Columns
- **Central** — foveal contrast sensitivity, letters at screen center
- **Right** — intact hemifield, letters ~10° right, fixation on central "+"
- **Left** — affected hemifield, letters ~10° left, fixation on central "+"
- **Asymmetry** — (Right − Left) LogCS. ≥0.30 is clinically significant; Eric's baseline 2.25 is ~7.5× that threshold.
- **Notes** — bug fixes, calibration changes, test conditions, perceived fixation quality, any deviation from standard protocol

### Recommended re-assessment cadence
- **Every ~5 training sessions** — quick-check (Central + Left only) to monitor affected field (`docs/research/contrast_sensitivity_module.md:650-653`)
- **Post-training (session 20+)** — full 3-hemifield reassessment for comparison to baseline
- **Ad-hoc** — after any hardware/display change, redo Central to detect calibration drift

---

## 6. Interpretation Guardrails

When adding new rows, watch for these failure modes:

1. **Fixation break** — if you felt your eyes drift toward the letter, the score is partly a central-vision score. Note it.
2. **Display change** — different monitor, different brightness, different viewing distance → contrast thresholds shift. Re-run the Central test to detect this before trusting hemifield deltas.
3. **Fatigue** — the test is sensitive to vigilance. Morning runs are more repeatable than evening runs.
4. **Learning effect** — small first-few-session gains can come from better test-taking strategy, not real neural change. Look for sustained improvement across ≥3 sessions before calling it progress.
5. **Coefficient of repeatability** — ±0.24 LogCS is the noise floor of the Pelli-Robson instrument (Elliott et al. 1990). Changes smaller than this are not signal.

---

## 7. Related Wiki Pages

- Instrument: [[contrast-sensitivity-test]]
- Why this is possible despite V1 damage: [[scientific-foundation]]
- The intervention targeting the Left 0.00 score: [[audiovisual-training-protocol]]
- Overall plan: [[rehabilitation-roadmap]]
- Paper citations (Daibert-Nido, Pelli-Robson, Elliott): [[research-papers-index]]
