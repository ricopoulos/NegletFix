---
title: Audiovisual Training Protocol (Daibert-Nido family)
last_updated: 2026-05-14
confidence: MIXED
sources:
  - NEUROFEEDBACK_PROTOCOL.md
  - VR_REHABILITATION_TASKS.md
  - docs/research/scientific_foundation.md
  - docs/research/contrast_sensitivity_module.md
  - CLAUDE.md
  - .brain/backlog.md
  - 2026-05-14 research audit — see [[research-papers-index]] Recent Additions
---

# Audiovisual Training Protocol

The primary intervention in NegletFix. Originally based on Daibert-Nido et al. (2021), DOI: 10.3389/fneur.2021.680211. **The 2021 paper was an N=2 pediatric brain-tumor pilot, not a powered adult stroke trial.** NegletFix implements a *synthesized protocol* drawing from the Daibert-Nido family + adjacent chronic-stroke work.

**Evidence base for the family** (all verified 2026-05-14):
- **Daibert-Nido 2021** — N=2 pediatric, 3D-MOT in IVR (~5 hr total). Original anchor.
- **Misawa/Daibert-Nido 2024 EClinicalMedicine** — N=10 pediatric on **Meta Quest 2/Pro at home**, 6 weeks, 9/10 with clinically meaningful gains, durable at 6 months. Validates Eric's hardware choice for home delivery.
- **Alharshan/Alwashmi 2026 NeuroImage** — N=15 adult stroke HH, **30 min/day × 5 days/wk × 6 weeks (15 hr total, ~3× Daibert-Nido)**, with DTI evidence of microstructural change. First mechanism paper in stroke.
- **Diana 2025 Eur J Neurol** — N=18 chronic HVFDs, AV training + tDCS RCT. Adjunct evidence.
- **Rowland/Bushnell/Duncan/Stein 2023 J Neurosci** — N=2 chronic stroke (≥8 mo), 2-h sessions, "dramatic" recovery throughout blind field. Alternative paradigm.
- **Yang/Cavanaugh/Saionz/Huxlin 2023 medRxiv** — N=12 chronic V1 stroke, **only 58% responded at any trained location, blind-field CS stayed ~4× lower than intact-field**. Critical reality check.

See [[research-papers-index]] for full citations.

> **⚠ Reframing (2026-05-14)**: This page previously presented Daibert-Nido 2021 as the "anchor" producing reliable +0.31 to +0.54 LogCS gains, with HIGH confidence. That was overstated. The effect-size range is plausible but **not independently replicated at scale in chronic adult stroke**. Realistic targets for Eric's chronic case: detection-RT improvements, ADL transfer, and modest CS gains at the scotoma border — not a left-field LogCS jump from 0.00 to anywhere near the intact field's 2.25.

**Status**: Specification family is complete; Unity implementation is PAUSED as `WIP-001` in `.brain/backlog.md` — ready to resume post-baseline.

See [[scientific-foundation]] for the "why," [[erics-baseline]] for the starting point, [[unity-architecture]] for where to build it.

---

## 1. Protocol Parameters [MIXED — dose options across the family]

There is **no consensus dose** across the Daibert-Nido protocol family. Three reference points:

### Option A — Original Daibert-Nido 2021 (low dose, pediatric pilot)
| Parameter | Value | Source |
|-----------|-------|--------|
| Session duration | 15 minutes | Daibert-Nido 2021 (N=2 pediatric) |
| Blocks per session | 3 × 5 min | Daibert-Nido 2021 |
| Session frequency | Every 2 days | Daibert-Nido 2021 |
| Total program | 6-7 weeks (~20 sessions) | Daibert-Nido 2021 |
| Total training time | < 5 hours | Daibert-Nido 2021 |

### Option B — Misawa/Daibert-Nido 2024 (medium dose, pediatric Quest at home)
| Parameter | Value | Source |
|-----------|-------|--------|
| Session duration | 20 minutes | Misawa 2024 (N=10 pediatric, Meta Quest 2/Pro home) |
| Session frequency | Every other day | Misawa 2024 |
| Total program | 6 weeks (21 sessions) | Misawa 2024 |
| Total training time | ~5 h 15 min | Misawa 2024 |

### Option C — Alharshan/Alwashmi 2026 (high dose, adult stroke evidence) ← RECOMMENDED for Eric
| Parameter | Value | Source |
|-----------|-------|--------|
| Session duration | 30 minutes | Alharshan 2026 (N=15 stroke HH) |
| Session frequency | 5 days/week | Alharshan 2026 |
| Total program | 6 weeks (30 sessions) | Alharshan 2026 |
| Total training time | ~15 hours | Alharshan 2026 |

### Expected outcomes (honestly framed)
| Outcome | Original framing | Revised framing (2026-05-14) |
|---------|------------------|------------------------------|
| Primary | +0.31 to +0.54 LogCS (Daibert-Nido 2021) | Plausible but **unreplicated in chronic adult stroke**. Realistic chronic target: ≥+0.30 LogCS at scotoma-border locations in 50-60% of responders (Yang/Cavanaugh/Saionz 2023, n=12 chronic) |
| Secondary | +7-9 Humphrey points blind hemifield; +10-22% reading speed | Detection-RT improvements, scanning efficiency, ADL transfer — better-supported by chronic-stroke evidence than perimetric field-area expansion |

**Why Option C for Eric**: He is ~5y post-stroke (chronic). Saionz 2020 found chronics need ~93 sessions vs ~16 for subacutes for visual discrimination recovery. The Daibert-Nido pediatric dose is likely under-dosed for an adult chronic case. Alharshan 2026 is the closest direct evidence in Eric's population.

---

## 2. Stimulus Parameters [LOW–MEDIUM — partial misattribution flagged 2026-05-14]

> **⚠ Source-attribution concern**: The parameters listed below (400 Hz / 250 ms / 55→75 dB looming) **do not appear to come from Daibert-Nido 2021**. Daibert-Nido 2021 used a **3D Multiple-Object Tracking (3D-MOT) paradigm in immersive VR with spatially co-localized sound** (15 trials × 20 s × 3 blocks per session). The "400 Hz tone" parameters appear closer to the Wake Forest cat protocol (Stein/Rowland) or a synthesis from older literature. **Re-source or rewrite this section before building.** The cleanest options are: (a) implement 3D-MOT-IVR as in Daibert-Nido / Misawa 2024 / Alharshan 2026, or (b) implement the Wake Forest 500 ms / 45° / 600 trials paradigm cited in [[scientific-foundation]]#wake-forest, with proper attribution.

### Currently-cited (pending re-attribution)

#### Auditory stimulus
- **Frequency**: 400 Hz sinusoidal tone — *source: uncertain, NOT Daibert-Nido 2021*
- **Duration**: 250 ms — *source: uncertain*
- **Amplitude envelope**: 55 dB → 75 dB (looming) — *source: uncertain*
- **Spatial source**: Co-localized with the visual target (Stein & Meredith 1993 spatial principle — *this principle is well-sourced; the specific tone parameters are not*)

#### Visual stimulus
- Personalized contrast from [[erics-baseline]] (starting contrast = 2× affected-hemifield threshold, i.e., easily visible — see `docs/research/contrast_sensitivity_module.md:605-617`)
- Co-localized with the auditory source in space AND time

#### Temporal alignment
Audio and visual onsets synchronized within one frame (~16 ms at 60 Hz). **2026-05-14 update**: Bean/Stein/Rowland 2023 (Cereb Cortex) shows multisensory enhancement is **not prerequisite** for recovery — the SC integration window is ~100-500 ms and Wake Forest paradigms use 100 ms pairs successfully. **Strict ~16 ms binding may be over-engineered.** Aim for sub-50 ms sync; don't sweat single-frame precision.

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

## 9. Confidence Summary (revised 2026-05-14)

| Claim | Confidence | Note |
|-------|-----------|------|
| 400 Hz, 250 ms, 55-75 dB audio params | **LOW** ↓ | Source-attribution uncertain — see §2 callout |
| 3D-MOT-IVR + co-localized audio paradigm | MEDIUM-HIGH | Validated across Daibert-Nido 2021 / Misawa 2024 / Alharshan 2026 |
| 15 min × ~20 sessions × 6-7 weeks (Option A) | MEDIUM | Original Daibert-Nido dose; pediatric pilot only |
| 30 min × 30 sessions × 6 weeks (Option C) | **HIGH** ↑ | Alharshan 2026 in adult stroke HH — closest to Eric's case |
| +0.31 to +0.54 LogCS expected improvement | **LOW–MEDIUM** ↓ | Cohort range from N=2 pediatric. Yang/Cavanaugh/Saionz 2023 (n=12 chronic) showed ~58% response rate, blind-field CS stays ~4× lower than intact |
| Eric personally will improve at all | MEDIUM | Chronic responders exist (Saionz lab series); ~50-60% response rate |
| Eric will hit the cohort-mean Daibert-Nido number | LOW | Pediatric → chronic adult transfer is the weak link |
| Gain will generalize to central vision / "gray overlay" | THEORETICAL — unvalidated hypothesis, see [[scientific-foundation]] |
| Adding EEG closed-loop improves outcomes vs. open-loop | MEDIUM — REINVENT 2019 + Chen 2026 meta-analysis suggest yes for motor; untested for hemianopia AV; Treves 2025 tempers consumer-NF claims |
| Adding tDCS adjunct improves outcomes (Diana 2025) | MEDIUM-HIGH for chronic HVFD; hardware not part of NegletFix |

---

## 10. Cross-References

- Theoretical backbone: [[scientific-foundation]]
- Starting point and outcome anchor: [[erics-baseline]]
- Instrument for measuring effect: [[contrast-sensitivity-test]]
- Optional closed-loop layer: [[eeg-neurofeedback]]
- Where to build it: [[unity-architecture]]
- Overall schedule: [[rehabilitation-roadmap]]
- Paper citations: [[research-papers-index]]
