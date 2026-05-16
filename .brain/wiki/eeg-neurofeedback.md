---
title: EEG Neurofeedback (Muse TP10)
last_updated: 2026-05-14
confidence: MIXED — see §2 and §10 caveats added 2026-05-14
sources:
  - NEUROFEEDBACK_PROTOCOL.md
  - RESEARCH_SUMMARY.md
  - Unity/NeglectFix/Assets/Scripts/EEG/MuseOSCReceiver.cs
  - Unity/NeglectFix/Assets/Scripts/EEG/EngagementCalculator.cs
  - Unity/NeglectFix/Assets/Scripts/EEG/EEGSimulator.cs
  - CLAUDE.md
  - 2026-05-14 audit — Treves 2025 JMIR consumer-NF meta, Yang/Kastner 2024 PNAS alpha gating, Robineau 2014 (corrected citation), Muse signal-quality 2024 comparisons
---

# EEG Neurofeedback

The secondary/optional layer in NegletFix: real-time EEG from Muse → adaptive engagement score → gates the audiovisual reward. **Not yet tested on Eric** — confidence is deliberately MIXED.

> **⚠ Category-mismatch note (added 2026-05-14)**: This protocol leans on neurofeedback literature for **hemispatial neglect** (Ros 2017 n=5, Network Neuroscience 2022). Eric's diagnosis is **left homonymous hemianopia** from PCA stroke — *not* hemispatial neglect (which typically follows MCA stroke). The strongest direct precedent for Eric's exact condition is **Robineau et al. 2014** (Neuropsychologia, real-time fMRI NF in left HH patients) — fMRI, not EEG, and not what NegletFix implements. Treat the EEG-NF layer as **an open hypothesis repurposing neglect protocols for a hemianopia case**, not a validated intervention. See [[research-papers-index]] for the corrected Robineau 2014 citation and the Treves 2025 consumer-NF reality check.

See [[scientific-foundation]]#the-eeg-neurofeedback-rationale for the "why," [[audiovisual-training-protocol]] for the intervention it gates, [[hardware-setup]] for device connection, [[unity-architecture]] for the scripts.

---

## 1. Pipeline

```
Muse 2 / Muse S (head)
      │ Bluetooth LE
      ▼
Mind Monitor app (iOS/Android, ~$15)
      │ WiFi / UDP / OSC protocol
      ▼ /muse/elements/{alpha,beta,theta}_absolute  [4 channels: TP9 AF7 AF8 TP10]
Unity MuseOSCReceiver  (port 5000)
      │ OnTP10{Alpha,Beta,Theta}Received events
      ▼
EngagementCalculator  (adaptive threshold logic)
      │ OnEngagementThresholdExceeded
      ▼
RewardController  (+ GazeDetector for combined trigger)
      ▼
VR reward (visual/audio/haptic) + DataLogger CSV
```

All code paths: see [[unity-architecture]].

---

## 2. The TP10 Rationale [MEDIUM — revised 2026-05-14]

Muse 2/S has 4 dry electrodes: **TP9** (left temporal-parietal), **AF7** (left frontal), **AF8** (right frontal), **TP10** (right temporal-parietal).

**TP10 is the primary target** (`MuseOSCReceiver.cs:55`). Why:
- Overlies right posterior parietal cortex — a spatial-attention hub
- Near the lesion site in many right-hemisphere stroke patients
- Theoretical support from **Yang/Fiebelkorn/Kastner et al. 2024 PNAS** (intracranial ECoG, n=8): alpha is **bidirectional** — desynchronization contralateral to attended hemifield + synchronization ipsilateral both serve attention gating. For Eric (training left-field attention), right-hemisphere alpha desync is the correct direction.
- Closest condition-matched NF study: **Ros et al. 2017** trained 5 stroke patients with hemispatial neglect to down-regulate rPPC alpha — but Eric has **hemianopia, not neglect** (see top-of-page category-mismatch note).
- For pure hemianopia, the direct precedent is **Robineau et al. 2014** (Neuropsychologia, n small): real-time fMRI NF in left HH — patients could up-regulate right occipital cortex activity. *Not EEG, not TP10, but same diagnosis.*

> ***Citation correction (2026-05-14)***: The prior wiki described "Ros et al. 2017 trained 5 stroke-neglect patients to down-regulate rPPC alpha over 6 days." That sentence conflated two different papers. Ros 2017 (Neural Plasticity) is the alpha-NF for neglect work; Ros 2014 was a single-session study; Robineau 2017 was healthy-volunteer fMRI; Robineau 2014 was the hemianopia fMRI-NF paper. The above text uses the corrected attributions.

### Consumer-grade EEG signal-quality caveat [DOWNGRADED to LOW–MEDIUM, 2026-05-14]

Two independent 2024 device-comparison papers (Sokolova 2024 Frontiers Neuroscience; Pavlov 2024 Sensors 24:8108) found **Muse S Gen 2 ranks lowest among tested consumer-EEG headbands** when benchmarked against research-grade systems (Brain Products, NVX): poorest log-SNR (p<0.001), weak correlation with research-grade PSD, lower-amplitude alpha spindles even with eyes closed (the easiest case to detect alpha). The TP9/TP10 dry-electrode contact through hair is the documented weak point.

**Practical implication for Eric**: Real-world TP10 alpha/beta band powers may carry more low-frequency artifact than signal. **Plan a signal-validity check session before counting Muse data as the engagement gate** — eyes-open vs eyes-closed alpha modulation must be visibly detectable. If that simple eyes-closed alpha bump isn't there, the NF layer is operating on noise.

### NF replication-crisis context [added 2026-05-14]

**Treves et al. 2025 JMIR meta-analysis** (16 RCTs, n=763, 11/16 used Muse): consumer-grade neurofeedback shows **no benefit on cognition, mindfulness, or physiology** vs control. Modest distress reduction only. Authors suggest effects may rely on "neurosuggestion" (placebo of neurotechnology). Larger sham-controlled trials in adjacent fields (PTSD 2025 fMRI-NF vs sham = -0.05 ns; ADHD blinded RCTs = null) reinforce the pattern: **NF effects shrink or vanish under proper control**.

This argues for treating EEG-NF as an **exploratory adjunct layer** in NegletFix, not a primary mechanism. The audiovisual training has direct evidence (Daibert-Nido family, Diana 2025, Alharshan 2026); the EEG-NF layer is theoretical-with-caveats.

---

## 3. Engagement Score Formula

Implemented in `EngagementCalculator.cs:131-172`. All inputs are smoothed with a 10-sample moving average (`EngagementCalculator.cs:51`).

```
alpha  = smoothed TP10 alpha  (μV²)
beta   = smoothed TP10 beta   (μV²)
theta  = smoothed TP10 theta  (μV²)

if alpha < 0.01: alpha = 0.01     # prevent div by zero

betaAlphaRatio   = beta / alpha
thetaNormalized  = clamp01(theta / 10.0)
thetaFactor      = 1 - (thetaNormalized * thetaPenalty)   # thetaPenalty default 0.3

engagement = (betaAlphaRatio * betaWeight) * thetaFactor  # betaWeight default 1.0
```

Interpretation:
- **↑ beta / ↓ alpha → ↑ engagement** (matches the target neurophysiology — cortical activation, alertness)
- **↑ theta → ↓ engagement** (drowsiness penalty)

Note: the `alphaWeight` parameter (`EngagementCalculator.cs:28`) is declared but **not used** in the current formula — alpha appears only in the ratio denominator. This is a latent parameter for future tuning; treat as `1.0` for now.

### Alternate formula in EEGSimulator
`EEGSimulator.cs:86-99` uses a different formula: `engagement = (beta + gamma) / (alpha + theta)`. This is a **simulator-only** convenience formula for generating realistic variation; it does not match the production path. The real path flows through `EngagementCalculator.UpdateSimulatedValues()` which recomputes via the production formula. Be aware when reading logs.

---

## 4. Adaptive Threshold [MEDIUM]

Baseline phase (default 120 s, `EngagementCalculator.cs:37`):
1. User rests, eyes open, neutral environment
2. Collect engagement scores for the full duration
3. Compute mean μ and stddev σ
4. Initial threshold: `μ + 0.5σ` (`thresholdStdMultiplier` default 0.5, `EngagementCalculator.cs:181`)

Training phase (adaptive):
- Every 120 s (`adaptationInterval`), compute success rate (% of samples above threshold)
- If success rate < 0.4 → threshold *= 0.9 (easier)
- If success rate > 0.6 → threshold *= 1.1 (harder)
- Target: 40-60% success rate — optimal learning (Sitaram et al. 2017 principle; `NEUROFEEDBACK_PROTOCOL.md:142-146`)

This matches the standard closed-loop neurofeedback design. The exact multipliers (0.9, 1.1) are project-chosen tuning, not from a specific paper.

---

## 5. Frequency Bands

| Band | Range | Role in NegletFix |
|------|-------|-------------------|
| Delta | 1-4 Hz | Not used; artifact channel only |
| Theta | 4-8 Hz | Drowsiness penalty (monitored, not directly trained) |
| **Alpha** | 8-12 Hz | **Primary target — reduce** (desynchronization = cortical activation) |
| **Beta** | 13-30 Hz | **Primary target — increase** (alertness, attention) |
| Gamma | 30-50 Hz | Not used in V0 |

Rationale: Ros et al. 2017 and the Network Neuroscience 2022 spectral signature (↓alpha, ↓beta in right posterior regions post-stroke) — see [[research-papers-index]].

Muse delivers pre-computed band powers at ~10 Hz via Mind Monitor — NegletFix does NOT do raw FFT (`NEUROFEEDBACK_PROTOCOL.md:165-182`). Trust the pre-processing.

---

## 6. EEGSimulator — Development Without Hardware

`EEGSimulator.cs` generates realistic alpha/beta/theta/gamma variation via `Mathf.Sin` + `Mathf.PerlinNoise`, injected into `EngagementCalculator.UpdateSimulatedValues()` every frame.

| Control | Effect |
|---------|--------|
| **E key** | Jump to high-engagement preset (β=0.8, α=0.3) |
| **R key** | Jump to low-engagement preset (β=0.3, α=0.8) |

**Known bug**: The key-handler method is named `OnKeyPress()` (`EEGSimulator.cs:131-143`), which is **not** a Unity message. It will never fire. The advertised E/R keys do not currently work. Fix: rename to `Update()` or add direct `Input.GetKeyDown` calls inside the existing `Update()` method. Low-priority since the simulator generates useful natural variation anyway.

The simulator is useful for developing [[audiovisual-training-protocol]] logic before the Muse arrives, but will **not** tell you whether the real-world TP10 signal is good.

---

## 7. extOSC Package Dependency

`MuseOSCReceiver.cs` requires the **extOSC** Unity package (https://github.com/Iam1337/extOSC) to receive OSC packets. The code uses a compile-time flag:

```csharp
// #define EXTOSC_INSTALLED   // uncomment when installed
#if EXTOSC_INSTALLED
using extOSC;
#endif
```

Until extOSC is installed AND the `#define` uncommented (`MuseOSCReceiver.cs:5`), the receiver falls back to simulator-only mode and emits a warning (`MuseOSCReceiver.cs:62-64`). This is an intentional safety default so the project compiles cleanly out-of-the-box.

Other possible packages: OscCore, UnityOSC — any UDP-OSC receiver on port 5000 parsing the Mind Monitor address format (`/muse/elements/alpha_absolute` etc.) will work.

---

## 8. What to Expect From Real Muse Data

[MEDIUM confidence — based on literature, not tested on Eric]

**Signal quality**:
- Fit matters enormously. Electrode contact quality <60% → unusable data (`NEUROFEEDBACK_PROTOCOL.md:303-308`)
- Dry hair, skin oils, or poor placement over the mastoid region for TP9/TP10 all degrade signal
- Jaw clenching, eye blinks, head movement → visible artifacts in the band powers
- Mind Monitor's "horseshoe" indicator must show all four electrodes connected before starting

**Latency**:
- Muse BLE → Mind Monitor: ~10-50 ms
- Mind Monitor → Unity OSC: <10 ms on LAN
- Total: comfortably <100 ms, sufficient for closed-loop neurofeedback (`RESEARCH_SUMMARY.md:217-220`)

**Calibration time**:
- 2 min baseline is the Daibert-Nido/Ros et al. standard (`EngagementCalculator.cs:37`)
- Baseline values vary session-to-session (hydration, alertness, time of day) — always re-baseline each session, never reuse

**Expected learning curve** (from Ros et al. 2017, `NEUROFEEDBACK_PROTOCOL.md:321-326`):
- Sessions 1-2: Inconsistent control, still finding the "engaged" state
- Sessions 3-4: Beginning to recognize the target brain state
- Sessions 5-6: Reliable voluntary modulation
- Session 10+: Sustained improvements

Not everyone learns neurofeedback. Some percentage of users are "non-responders" for reasons not fully understood.

---

## 9. Safety

- Non-invasive passive recording — no electrical stimulation (`NEUROFEEDBACK_PROTOCOL.md:289-292`)
- Consumer device, Bluetooth LE power levels
- No known contraindications for Eric's condition
- Skin irritation at electrode sites is the main physical risk; limit to <30 min sessions if skin reacts

---

## 10. Confidence Summary (revised 2026-05-14)

| Claim | Confidence | Note |
|-------|-----------|------|
| TP10 is accessible and measurable with Muse | HIGH | Anatomical placement is correct |
| Alpha/beta pattern targeted matches neglect EEG signature | HIGH | But Eric has hemianopia, not neglect (category mismatch) |
| Stroke patients can learn to modulate rPPC alpha via NFB | MEDIUM | Ros 2017 n=5 (neglect, medical EEG); Bagherzadeh 2026 confirms ~50% of healthy adults are NF learners |
| Muse (consumer) is adequate for this learning | **LOW–MEDIUM** ↓ | Sokolova/Pavlov 2024 — Muse S last among consumer devices on signal quality |
| Closed-loop consumer-NF improves cognition under sham control | **LOW** | Treves 2025 JMIR meta — null. NF effects shrink under proper control |
| Eric personally will respond to NFB | UNKNOWN | Individual variability; ~50% non-responder rate even in healthy |
| Adding NFB gate improves AV training outcomes | MEDIUM | REINVENT 2019 + Chen 2026 meta show BCI+task improves motor stroke; untested for hemianopia AV |
| Hemianopia (not neglect) is the right target population for this NF protocol | **MEDIUM (DOWNGRADED)** ↓ | Robineau 2014 supports fMRI-NF in HH, but EEG-NF in HH has no direct evidence — open hypothesis |

---

## 11. Cross-References

- Theoretical backbone: [[scientific-foundation]]
- Intervention this gates: [[audiovisual-training-protocol]]
- Code details: [[unity-architecture]]
- Hardware and OSC setup: [[hardware-setup]]
- Paper citations: [[research-papers-index]]
