---
title: Audiovisual Training Protocol (Daibert-Nido 2021)
last_updated: 2026-04-14
confidence: HIGH
sources:
  - NEUROFEEDBACK_PROTOCOL.md
  - VR_REHABILITATION_TASKS.md
  - docs/research/scientific_foundation.md
  - docs/research/contrast_sensitivity_module.md
  - CLAUDE.md
  - .brain/backlog.md
---

# Audiovisual Training Protocol

The primary intervention in NegletFix. Based on Daibert-Nido et al. (2021), DOI: 10.3389/fneur.2021.680211 — a peer-reviewed home-based VR rehabilitation study that produced +0.31 to +0.54 LogCS contrast sensitivity improvement in hemianopia patients. This is the protocol NegletFix is designed to implement.

**Status**: Specification is complete and peer-reviewed. Unity implementation is PAUSED as `WIP-001` in `.brain/backlog.md` — ready to resume post-baseline.

See [[scientific-foundation]] for the "why," [[erics-baseline]] for the target, [[unity-architecture]] for where to build it.

---

## 1. Protocol Parameters [HIGH — exact from Daibert-Nido 2021]

| Parameter | Value | Source |
|-----------|-------|--------|
| Session duration | 15 minutes | Daibert-Nido 2021 |
| Blocks per session | 3 × 5 min | Daibert-Nido 2021 |
| Session frequency | Every 2 days | Daibert-Nido 2021 |
| Total program | 6-7 weeks (~20 sessions) | Daibert-Nido 2021 |
| Total training time | < 5 hours | Daibert-Nido 2021 |
| **Target outcome** | **+0.31 to +0.54 LogCS** | Daibert-Nido 2021 |
| Secondary outcomes | +7-9 Humphrey points (blind hemifield), +10-22% reading speed | `docs/research/scientific_foundation.md:38-41` |

---

## 2. Stimulus Parameters [HIGH — exact from protocol]

### Auditory stimulus
- **Frequency**: 400 Hz sinusoidal tone
- **Duration**: 250 ms
- **Amplitude envelope**: 55 dB → 75 dB (looming — sound "approaches" the listener)
- **Spatial source**: Co-localized with the visual target (spatial principle of multisensory integration — Stein & Meredith 1993)

### Visual stimulus
- Personalized contrast from [[erics-baseline]] (starting contrast = 2× affected-hemifield threshold, i.e., easily visible — see `docs/research/contrast_sensitivity_module.md:605-617`)
- Co-localized with the auditory source in space AND time

### Temporal alignment (critical)
Audio and visual onsets must be **synchronized within one frame** (~16 ms at 60 Hz) to stay safely inside the perceptual binding window. The SC-level binding window is wider (~100-500 ms, Stein & Meredith 1993) but perceptual binding in humans uses the tighter threshold. See [[scientific-foundation]]#temporal-principle for the source-conflict note.

---

## 3. Eccentricity Progression [HIGH]

Stimuli are presented at increasing peripheral eccentricity across the program:

| Stage | Eccentricity | Purpose |
|-------|-------------|---------|
| Early | **8°** | Near-periphery, easier detection, build confidence |
| Mid | **24°** | Moderate periphery |
| Late | **56°** | Far periphery, closer to Eric's functional loss |

For Eric's severe asymmetry (baseline 2.25 LogCS — see [[erics-baseline]]), `docs/research/contrast_sensitivity_module.md:622-628` suggests starting closer to midline: `{ 8°, 16°, 24°, 32° }` instead of the standard `{ 8°, 24°, 40°, 56° }`. Personalize from the contrast sensitivity asymmetry.

Alternative parameter set from Wake Forest / Rowland & Stein protocol: 500 ms stimuli at 45° eccentricity, 600 trials/session; recovery began within 3-5 sessions (`docs/research/scientific_foundation.md:45-47`). Not the anchor protocol but an option if Daibert-Nido parameters plateau.

---

## 4. Session Structure [HIGH — derived from NEUROFEEDBACK_PROTOCOL.md]

Three phases per session (`NEUROFEEDBACK_PROTOCOL.md:109-127`):

### Phase 1 — Baseline (2 min)
- Eyes open, resting state, neutral VR environment
- Measure baseline TP10 alpha/beta/theta from Muse (if EEG enabled — see [[eeg-neurofeedback]])
- Compute personalized engagement threshold
- Optional: verify contrast sensitivity stability vs. [[erics-baseline]]

### Phase 2 — Training (15 min total, 3 × 5 min blocks)
- Active audiovisual exposure at progressive eccentricities
- Closed-loop: if EEG neurofeedback enabled, reward fires only when (engagement > threshold AND gaze/head in expected location)
- Adaptive difficulty targets 40-60% success rate (`NEUROFEEDBACK_PROTOCOL.md:142-146`)
- Short inter-block rests to prevent fatigue

### Phase 3 — Cool-down (3 min)
- Reduced stimulus intensity
- Optional post-session measurement
- Compare to pre-session baseline

---

## 5. What to Build in Unity

Target location: `Unity/NeglectFix/Assets/Scripts/Tasks/` (extend `TaskManager.cs` — see [[unity-architecture]]).

### Audio source configuration
```
- AudioSource component
- Clip: generated 400 Hz sine wave, 250 ms, 55→75 dB linear envelope
- Spatial blend: 1.0 (fully 3D)
- Min/max distance tuned so perceived loudness matches 55-75 dB at Quest audio level
- Dopple-level: 0 (no pitch shift)
- Attach to the visual stimulus GameObject so spatial position = visual position
```

### Visual stimulus
```
- Quad or simple primitive (e.g., bright disk or Gabor patch)
- Contrast configurable via ConfigureFromContrastResults(ContrastSensitivityResults)
  (signature already sketched in docs/research/contrast_sensitivity_module.md:605)
- Position driven by eccentricity stage + hemifield (left-affected)
- Onset synchronized to audio: coroutine pattern,
    audioSource.Play();
    visualRenderer.enabled = true;
  in the same frame; do NOT yield between them
```

### Trial loop (pseudo)
```
for each block (3):
    for 5 minutes:
        pick eccentricity from current stage
        place stimulus in LEFT hemifield at that eccentricity
        wait random ISI (e.g., 2-5s)
        fire coincident audio+visual onset
        optionally check EEG engagement during window
        log trial (timestamp, eccentricity, response, engagement)
```

### Personalization hook
From `docs/research/contrast_sensitivity_module.md:605-635`:
```csharp
public void ConfigureFromContrastResults(ContrastSensitivityResults results) {
    float affectedThreshold = results.leftHemifieldLogCS;
    float startingLogCS = Mathf.Max(0, affectedThreshold - 0.30f);
    // ... set stimulus contrast, choose eccentricity array based on asymmetry
}
```

### Logging
Extend `DataLogger.cs` ([[unity-architecture]]) with per-trial records: timestamp, block, eccentricity, stimulus contrast, audio onset delay (ms), visual onset delay (ms), EEG engagement at onset, head yaw at onset, response (if any).

---

## 6. Measurement Schedule [HIGH]

From `docs/research/contrast_sensitivity_module.md:641-658`:

- **Day 0 (baseline)** — full [[contrast-sensitivity-test]] across Central + Right + Left → already done, see [[erics-baseline]]
- **Every 5 sessions** — quick-check (Central + Left only) to monitor affected-field trajectory without fatigue
- **Session 20+ (post-training)** — full 3-hemifield reassessment, compare to baseline

Append results to the progress log in [[erics-baseline]]#progress-log.

---

## 7. Safety & Limitations

From `NEUROFEEDBACK_PROTOCOL.md:287-313`:
- Non-invasive (no electrical stimulation; all training is sensory exposure)
- Start with shorter sessions if VR motion sickness emerges; build up to 15 min
- Skip sessions when fatigued (vigilance matters)
- **Contraindication**: Active seizure disorder (VR flashing/looming stimuli). No concern in Eric's case (confirm with physician if adding flash).

**Limitations specific to this implementation**:
- Daibert-Nido cohort size was modest (consult paper for exact n); individual response varies
- No guarantee of transfer to daily-life function even if LogCS improves — the contrast test measures sensitivity at a specific eccentricity and luminance, not "seeing at dinner"
- Eric has pure hemianopia, not neglect. The protocol was validated on hemianopia, so this is an on-label use.

---

## 8. Closed-Loop Integration with EEG (Optional, Phase 2)

If [[eeg-neurofeedback]] is enabled, the audiovisual reward fires only when:
```
(engagement > adaptive_threshold) AND (head_yaw > left_threshold OR eccentricity-appropriate gaze)
```
See `NEUROFEEDBACK_PROTOCOL.md:190-208` and `RewardController.cs` in [[unity-architecture]]. Without EEG, the training still runs as open-loop audiovisual exposure — the Daibert-Nido protocol did not use EEG, so open-loop is the minimum viable path.

---

## 9. Confidence Summary

| Claim | Confidence |
|-------|-----------|
| 400 Hz, 250 ms, 55-75 dB audio params | HIGH (Daibert-Nido 2021) |
| 15 min × ~20 sessions × 6-7 weeks schedule | HIGH (Daibert-Nido 2021) |
| +0.31 to +0.54 LogCS expected improvement | HIGH (Daibert-Nido 2021, cohort average) |
| Eric personally will improve by this magnitude | MEDIUM — individual variability |
| Gain will generalize to central vision / "gray overlay" | THEORETICAL — unvalidated hypothesis, see [[scientific-foundation]] |
| Adding EEG closed-loop improves outcomes vs. open-loop | MEDIUM — REINVENT 2019 suggests yes in motor rehab; untested for hemianopia AV |

---

## 10. Cross-References

- Theoretical backbone: [[scientific-foundation]]
- Starting point and outcome anchor: [[erics-baseline]]
- Instrument for measuring effect: [[contrast-sensitivity-test]]
- Optional closed-loop layer: [[eeg-neurofeedback]]
- Where to build it: [[unity-architecture]]
- Overall schedule: [[rehabilitation-roadmap]]
- Paper citations: [[research-papers-index]]
