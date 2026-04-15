---
title: Scientific Foundation
last_updated: 2026-04-14
confidence: HIGH
sources:
  - docs/research/scientific_foundation.md
  - RESEARCH_SUMMARY.md
  - NEUROFEEDBACK_PROTOCOL.md
---

# Scientific Foundation

The theoretical backbone of NegletFix. Why audiovisual rehabilitation can restore partial visual function despite cortical (V1) damage, and why EEG neurofeedback targets the right posterior parietal cortex.

See also: [[research-papers-index]], [[erics-baseline]], [[audiovisual-training-protocol]], [[eeg-neurofeedback]].

---

## 1. The Two Visual Pathways

Conscious vision normally flows through the **geniculostriate pathway**. But a second, evolutionarily older pathway — the **retinotectal pathway** — runs in parallel and is typically **preserved** in occipital stroke.

### Damaged pathway (Eric's stroke killed this route for the left hemifield)

```
Retina → Lateral Geniculate Nucleus (LGN) → V1 (occipital cortex) → Conscious vision
```

In Eric's case, right-PCA stroke (July 2021) produced a large area of encephalomalacia in the right occipital lobe, destroying V1 input from the left visual field (`CLAUDE.md:42-47`). Result: complete left homonymous hemianopia [MEASURED — see [[erics-baseline]]].

### Preserved pathway (the rehabilitation target)

```
Retina → Superior Colliculus (SC) → Pulvinar → Extrastriate cortex (V5/MT, V4) → Detection / orienting
```

The retinotectal pathway bypasses V1 entirely. It mediates reflexive orienting, motion detection, and blindsight phenomena (Stein & Meredith 1993). **More than 70% of SC output neurons respond to multisensory input** (`docs/research/scientific_foundation.md:22`), making the SC the anatomical substrate for cross-modal rehabilitation. [HIGH]

**Implication for NegletFix**: Even though Eric's V1-route for the left hemifield is gone, the SC pathway is almost certainly intact. Audiovisual training aims to strengthen SC→extrastriate signaling as a functional workaround.

---

## 2. Multisensory Integration — Stein & Meredith's Three Principles

Stein & Meredith (1993, *The Merging of the Senses*, MIT Press) established three empirically robust principles governing when multisensory stimuli produce **supra-additive** responses in SC neurons — responses larger than the sum of each unisensory input.

### Spatial principle [HIGH]
Stimuli from the **same spatial location** produce enhanced responses. Spatially disparate stimuli may *depress* responses. Practical consequence: the audio cue must emanate from the same eccentricity as the visual target.

### Temporal principle [HIGH]
Stimuli must occur within a binding window — commonly stated as ~100-500ms in SC neurons (Stein & Meredith 1993; see `docs/research/scientific_foundation.md:30`). For perceptual binding in humans, tighter windows (~16-100ms) are often used in the VR rehabilitation literature.

> **Source conflict**: `docs/research/scientific_foundation.md:30` states "~100-500ms" for the SC temporal window. The project's research-synthesis prompt references "~16ms" for perceptual binding. These are not contradictory — the SC neural window is wider than the perceptual binding threshold — but the distinction matters when implementing [[audiovisual-training-protocol]]. Aim for audiovisual sync within one frame (~16ms at 60 Hz) to be safely inside both windows.

### Inverse effectiveness [HIGH]
**Multisensory enhancement is greatest when the individual unisensory signals are weakest.** This principle is the most important for hemianopia: exactly in the blind field, where residual visual input falls below detection threshold, a paired auditory stimulus produces the maximum boost. The damaged hemifield is precisely where inverse effectiveness predicts the biggest therapeutic gain (Stein & Meredith 1993; Bolognini et al. 2005).

---

## 3. Clinical Evidence That This Works

### Daibert-Nido et al. 2021 [HIGH — peer-reviewed, home-based]
20 VR sessions of 15 min each (<5 hours total training time) using spatiotemporally congruent audiovisual stimulation produced:
- **+0.31 to +0.54 LogCS** contrast sensitivity improvement
- **+7 to +9 points** on Humphrey visual field testing in the blind hemifield
- **+10-22%** reading speed improvement

DOI: 10.3389/fneur.2021.680211 — this is the protocol NegletFix implements. See [[audiovisual-training-protocol]].

### Wake Forest / Rowland & Stein Protocol [MEDIUM — ongoing trials]
500ms stimuli at 45° eccentricity, 600 trials/session. Recovery began within 3-5 sessions. Trials: NCT04963075, NCT05894434, IRB00061542 (`docs/research/scientific_foundation.md:45-47`).

### Bolognini et al. 2005 [HIGH — condition-matched]
Demonstrated that hemianopia patients can detect stimuli in the blind hemifield significantly better when paired with a co-localized auditory stimulus — the first clinical validation of the inverse-effectiveness principle in hemianopia (`docs/research/scientific_foundation.md:63`).

---

## 4. Cortical Reorganization and Blindsight

Blindsight — residual visual processing without conscious awareness — is the classical demonstration that the retinotectal pathway carries functional visual information even when V1 is destroyed. Hemianopia patients can often:
- Orient toward stimuli they report not seeing
- Detect motion in the blind field
- Show above-chance performance on forced-choice tasks

These capacities are the **raw material** audiovisual training is thought to amplify and push toward conscious awareness (Cuppini et al. 2017 — computational model of how repeated multisensory pairing strengthens SC→extrastriate connections). [MEDIUM — mechanistic, model-based]

---

## 5. The EEG Neurofeedback Rationale

Separately from the audiovisual pathway, NegletFix implements EEG neurofeedback targeting the right posterior parietal cortex (rPPC), which is:
- Anatomically near the lesion in many right-hemisphere stroke patients
- Accessible with consumer-grade Muse TP10 electrode
- Shown to be trainable via neurofeedback in Ros et al. (2017) — 5 stroke patients with neglect successfully learned to down-regulate rPPC alpha over 6 days [MEDIUM — small n, neglect not pure hemianopia]

**Target bands** (see [[eeg-neurofeedback]]):
- **↓ Alpha (8-12 Hz)** — alpha desynchronization = cortical activation
- **↑ Beta (13-30 Hz)** — beta increase correlates with alertness and attention recovery
- **Theta (4-8 Hz)** monitored as drowsiness proxy

The spectral signature targeted by this protocol (↓alpha, ↑beta at rPPC) matches the observed EEG disruption in stroke/neglect — ↑delta/theta + ↓alpha/beta/gamma in right posterior regions (Network Neuroscience 2022; `RESEARCH_SUMMARY.md:96-101`).

---

## 6. Relevance to Eric's Specific Case

**Diagnosis**: Right PCA stroke with hemorrhagic transformation → left homonymous hemianopia (July 2021).

**Key symptom beyond field loss**: Bilateral dimness / "gray overlay" — NOT a comfort complaint, a real daily struggle (`.brain/cross-cutting.md:12`).

**Hypothesis** (from `docs/research/scientific_foundation.md:54-58`): The gray overlay may reflect disrupted **gain control** in visual processing — the whole visual system is operating at reduced drive. If so, cross-modal training via the SC pathway could:
1. Amplify weak visual signals through multisensory enhancement (inverse effectiveness)
2. Strengthen SC→extrastriate feedback connections
3. Improve overall contrast/brightness perception — not just left-field detection but the global dimness [THEORETICAL — unconfirmed]

This hypothesis is **testable** via the contrast sensitivity instrument ([[contrast-sensitivity-test]]). Eric's baseline shows left = 0.00 LogCS, right = 2.25 LogCS ([[erics-baseline]]). If the gain-control hypothesis is correct, audiovisual training should improve both left-hemifield scores AND central contrast sensitivity.

---

## 7. Confidence Map for This Page

| Claim | Confidence | Basis |
|-------|-----------|-------|
| Retinotectal pathway is preserved in occipital stroke | HIGH | Classical neuroanatomy |
| SC neurons show supra-additive multisensory responses | HIGH | Stein & Meredith 1993 |
| Three principles of multisensory integration | HIGH | Stein & Meredith 1993 |
| Audiovisual training produces +0.3-0.5 LogCS in hemianopia | HIGH | Daibert-Nido et al. 2021 |
| EEG neurofeedback is trainable in neglect patients | MEDIUM-HIGH | Ros et al. 2017, n=5 |
| Gain-control hypothesis explains Eric's "gray overlay" | THEORETICAL | Project-internal, unvalidated |
| Transfer of training gains to Eric's daily function | UNKNOWN | Individual variability |
