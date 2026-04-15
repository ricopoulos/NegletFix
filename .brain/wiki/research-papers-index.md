---
title: Research Papers Index
last_updated: 2026-04-14
confidence: MIXED
sources:
  - RESEARCH_SUMMARY.md
  - NEUROFEEDBACK_PROTOCOL.md
  - VR_REHABILITATION_TASKS.md
  - PROJECT_SUMMARY.md
  - docs/research/scientific_foundation.md
  - docs/research/contrast_sensitivity_module.md
  - CLAUDE.md
  - .brain/cross-cutting.md
---

# Research Papers Index

Alphabetical catalog of every paper/study cited in the NegletFix project. Each entry lists the citation, DOI/URL (where available in source files), the key finding in 1-2 sentences, and which NegletFix module/decision it informs.

Confidence applies to the paper's evidentiary weight for this specific project, not the paper itself. DOIs flagged `[CITATION NEEDED — verify]` are cited in source files but without a verifiable DOI in the available materials.

---

## Bolognini et al. (2005)

**Citation**: Bolognini N., Rasi F., Coccia M., Làdavas E. Multisensory Integration in Hemianopia. *Brain*. 2005.
**DOI**: [CITATION NEEDED — verify] (listed in `docs/research/scientific_foundation.md:63` without DOI)
**Key finding**: Spatially and temporally aligned audiovisual stimuli enhance visual detection in the blind hemifield of hemianopia patients — clinical evidence that the preserved retinotectal pathway can mediate recovery.
**Informs**: [[scientific-foundation]], [[audiovisual-training-protocol]] — direct precedent for the audiovisual cross-modal approach targeting Eric's left hemifield.
**Confidence for NegletFix**: HIGH (peer-reviewed, direct condition match).

---

## Cuppini et al. (2017)

**Citation**: Cuppini C. et al. Audiovisual Rehabilitation Model. *Frontiers in Computational Neuroscience*. 2017.
**DOI**: [CITATION NEEDED — verify] (listed in `docs/research/scientific_foundation.md:64` without DOI)
**Key finding**: Computational model of how repeated multisensory training strengthens residual SC→extrastriate connections, providing a mechanistic basis for audiovisual rehabilitation protocols.
**Informs**: [[scientific-foundation]], [[audiovisual-training-protocol]] — the theoretical "why" behind the Daibert-Nido protocol's effectiveness.
**Confidence for NegletFix**: MEDIUM (modeling paper, mechanistic not outcome evidence).

---

## Daibert-Nido et al. (2021) — ANCHOR PAPER

**Citation**: Daibert-Nido M., et al. Home-Based Visual Rehabilitation in Patients With Hemianopia. *Frontiers in Neurology*. 2021.
**DOI**: 10.3389/fneur.2021.680211
**Key finding**: 20 VR sessions of 15 min (<5 hours total) using spatiotemporally congruent audiovisual stimulation produced +0.31 to +0.54 LogCS contrast sensitivity improvement, +7-9 Humphrey points in the blind hemifield, and +10-22% reading speed in hemianopia patients.
**Informs**: [[audiovisual-training-protocol]] (entire protocol parameters), [[erics-baseline]] (target improvement magnitude), [[rehabilitation-roadmap]] (timeline).
**Confidence for NegletFix**: HIGH — peer-reviewed, home-based, exact condition match, provides ready-to-implement parameters.

---

## Elliott, Sanderson & Conkey (1990)

**Citation**: Elliott DB, Sanderson K, Conkey A. The reliability of the Pelli-Robson contrast sensitivity chart. *Ophthalmic and Physiological Optics*. 1990;10(1):21-24.
**DOI**: [CITATION NEEDED — verify] (no DOI in source files)
**Key finding**: Establishes coefficient of repeatability for Pelli-Robson testing at ±0.24 LogCS — the psychometric basis for calling ≥0.30 LogCS a clinically significant change.
**Informs**: [[contrast-sensitivity-test]], [[erics-baseline]] — the 0.30 LogCS significance threshold used throughout this project.
**Confidence for NegletFix**: HIGH (standard clinical reference).

---

## Pelli, Robson & Wilkins (1988)

**Citation**: Pelli DG, Robson JG, Wilkins AJ. The design of a new letter chart for measuring contrast sensitivity. *Clinical Vision Sciences*. 1988;2(3):187-199.
**DOI**: [CITATION NEEDED — verify] (pre-DOI era publication)
**Key finding**: Original Pelli-Robson chart — Sloan letters at 2° visual angle, 0.15 logCS triplet steps, 2-of-3 correct scoring — the gold-standard instrument for peak contrast sensitivity.
**Informs**: [[contrast-sensitivity-test]] (exact protocol parameters in `ContrastSensitivityTest.cs`).
**Confidence for NegletFix**: HIGH (foundational method).

---

## REINVENT — Spicer et al. (2019)

**Citation**: Spicer R., et al. Effects of a Brain-Computer Interface With Virtual Reality (VR) Neurofeedback: A Pilot Study in Chronic Stroke Patients. *Frontiers in Human Neuroscience*. 2019.
**DOI**: 10.3389/fnhum.2019.00210 (derived from URL in RESEARCH_SUMMARY.md:114)
**PubMed**: https://pubmed.ncbi.nlm.nih.gov/31275126/
**Key finding**: Closed-loop VR-BCI (EEG drives virtual avatar in real time) is feasible in chronic stroke; patients with more severe motor impairment benefited most; real-time closed-loop beat open-loop feedback.
**Informs**: [[eeg-neurofeedback]], [[unity-architecture]] — precedent for the EEG→Unity→VR reward loop in NegletFix.
**Confidence for NegletFix**: MEDIUM (motor rehab, not visual; analogous architecture).

---

## Robineau et al. (2017)

**Citation**: Robineau F., et al. Self-regulation of inter-hemispheric visual cortex balance through real-time fMRI neurofeedback training. 2017. (Cited in CLAUDE.md/notes.)
**DOI**: [CITATION NEEDED — verify] (no DOI in source files)
**Key finding**: Healthy participants can voluntarily up/down-regulate inter-hemispheric balance in visual cortex using real-time neurofeedback — proof that cortical excitability can be self-regulated through feedback.
**Informs**: [[eeg-neurofeedback]] — supports the premise that Eric can learn to modulate his right-hemisphere EEG.
**Confidence for NegletFix**: MEDIUM (fMRI not EEG; healthy not stroke; mechanistic support).

---

## Ros et al. (2017)

**Citation**: Ros T., et al. Increased Alpha-Rhythm Dynamic Range Promotes Recovery from Visuospatial Neglect: A Neurofeedback Study. *Neural Plasticity*. 2017;2017:7407241.
**DOI**: 10.1155/2017/7407241 (derived from URL in RESEARCH_SUMMARY.md:60)
**PubMed**: https://pubmed.ncbi.nlm.nih.gov/28529806/
**Key finding**: First demonstration that 5 right-hemisphere stroke patients with visuospatial neglect can learn to down-regulate right posterior parietal cortex (rPPC) alpha power over 6 days (20 min/day), supporting alpha-reduction as a therapeutic target for neglect.
**Informs**: [[eeg-neurofeedback]] (entire protocol), [[unity-architecture]] (TP10 target rationale), [[audiovisual-training-protocol]] (session duration, adaptive-threshold model).
**Confidence for NegletFix**: MEDIUM-HIGH (small n=5, peer-reviewed, closest available condition match; applicability to pure hemianopia without neglect is uncertain).

---

## Sitaram et al. (2017)

**Citation**: Sitaram R., et al. Closed-loop brain training: the science of neurofeedback. *Nature Reviews Neuroscience*. 2017.
**DOI**: [CITATION NEEDED — verify] (cited in project notes; no DOI in source files)
**Key finding**: Comprehensive review establishing closed-loop neurofeedback as a legitimate, mechanism-driven intervention paradigm, with best practices for threshold adaptation and session design.
**Informs**: [[eeg-neurofeedback]] — methodological rationale for the adaptive threshold (40-60% success rate target) in `EngagementCalculator.cs`.
**Confidence for NegletFix**: HIGH (high-impact review, methodology reference).

---

## Stein & Meredith (1993)

**Citation**: Stein BE, Meredith MA. *The Merging of the Senses*. MIT Press, 1993.
**DOI**: N/A (monograph)
**Key finding**: Establishes the three principles of multisensory integration in the superior colliculus: (1) spatial principle — coincident stimuli produce supra-additive responses; (2) temporal principle — binding window ~100-500ms; (3) inverse effectiveness — enhancement is largest when individual signals are weak. >70% of SC output neurons respond to multisensory input.
**Informs**: [[scientific-foundation]] (theoretical backbone), [[audiovisual-training-protocol]] (why audio+visual must be spatiotemporally aligned).
**Confidence for NegletFix**: HIGH (foundational neuroscience monograph).

---

## Frontiers (2023) — VR-VET RCT for Neglect

**Citation**: "Feasibility of hemispatial neglect rehabilitation with virtual reality-based visual exploration therapy." *Frontiers in Neuroscience*. 2023.
**URL**: https://www.frontiersin.org/journals/neuroscience/articles/10.3389/fnins.2023.1142663/full
**DOI**: 10.3389/fnins.2023.1142663 (derived from URL)
**Key finding**: VR-based visual exploration therapy (VR-VET) shows feasibility and efficacy for hemispatial neglect rehabilitation post-stroke.
**Informs**: [[rehabilitation-roadmap]] (VR task designs for exploration training). Note: Eric's diagnosis is hemianopia, not neglect — applicability is partial.
**Confidence for NegletFix**: MEDIUM (adjacent condition, not identical).

---

## Archives PM&R (2024) — Meta-analysis

**Citation**: Meta-analysis of VR-based rehabilitation in stroke patients (29 studies, N=1,561). *Archives of Physical Medicine and Rehabilitation*. 2024.
**URL**: https://www.archives-pmr.org/article/S0003-9993(24)01311-X/abstract
**DOI**: [CITATION NEEDED — verify] (derivable from article ID but not listed in source)
**Key finding**: VR-based rehabilitation significantly reduces anxiety/depression and improves functional outcomes vs. standard rehab across 1,561 stroke patients.
**Informs**: [[rehabilitation-roadmap]] — general support for VR as the delivery modality.
**Confidence for NegletFix**: MEDIUM (broad meta, not visual-rehab specific).

---

## Topics in Stroke Rehabilitation (2020) — Ecological VR

**Citation**: "Cognitive training in an everyday-like virtual reality enhances visual-spatial memory capacities in stroke survivors with visual field defects." *Topics in Stroke Rehabilitation*. 2020.
**URL**: https://www.tandfonline.com/doi/full/10.1080/10749357.2020.1716531
**DOI**: 10.1080/10749357.2020.1716531 (derived from URL)
**PubMed**: https://pubmed.ncbi.nlm.nih.gov/31960760/
**Key finding**: Everyday-like VR (kitchen, living room) training improves visual-spatial skills and shows real-world transfer in stroke survivors with visual field defects.
**Informs**: [[rehabilitation-roadmap]] — rationale for ecological (not abstract/gamified) rehab environments.
**Confidence for NegletFix**: MEDIUM (same patient population, different outcome measures).

---

## J. NeuroEngineering and Rehabilitation (2025) — Multisensory Telerehab

**Citation**: "Telerehabilitation for visual field defects with a multisensory training: a feasibility study." *Journal of NeuroEngineering and Rehabilitation*. 2025.
**DOI**: 10.1186/s12984-025-01573-4 (from URL in RESEARCH_SUMMARY.md:171)
**Key finding**: Combined audio+visual cues improve outcomes in visual field defect rehabilitation; home-based/remote delivery is feasible and effective.
**Informs**: [[audiovisual-training-protocol]], [[hardware-setup]] — validates the home-based consumer-hardware approach.
**Confidence for NegletFix**: MEDIUM (small feasibility study, directionally supportive).

---

## MIT Press / Network Neuroscience (2022)

**Citation**: "Disruption of large-scale electrophysiological networks in stroke patients with visuospatial neglect." *Network Neuroscience* (MIT Press). 2022;6(1):69.
**URL**: https://direct.mit.edu/netn/article/6/1/69/107407/
**DOI**: [CITATION NEEDED — verify] (derivable from article ID but not listed in source)
**Key finding**: EEG of neglect patients shows focal frequency slowing in right posterior regions: ↑delta/theta, ↓alpha/beta/gamma — providing the spectral signature the neurofeedback protocol targets.
**Informs**: [[eeg-neurofeedback]] — justifies targeting alpha-reduction and beta-enhancement at TP10.
**Confidence for NegletFix**: MEDIUM (observational, not interventional).

---

## Wake Forest / Rowland & Stein Protocol

**Citation**: Rowland BA, Stein BE et al. Multisensory training protocol. Trials: NCT04963075, NCT05894434, IRB00061542.
**DOI**: N/A (clinical trials, not papers)
**Key finding**: 500ms stimuli at 45° eccentricity, 600 trials/session; recovery began within 3-5 sessions in the blind hemifield.
**Informs**: [[audiovisual-training-protocol]] — alternative/complementary parameter set to Daibert-Nido.
**Confidence for NegletFix**: MEDIUM (ongoing trials, not yet fully published in source materials).

---

## Summary Table

| Paper | Year | DOI Status | Confidence | Primary Use |
|-------|------|------------|------------|-------------|
| Daibert-Nido et al. | 2021 | VERIFIED | HIGH | Audiovisual protocol (anchor) |
| Ros et al. | 2017 | DERIVED from URL | MEDIUM-HIGH | EEG neurofeedback protocol |
| Stein & Meredith | 1993 | N/A (book) | HIGH | Multisensory theory |
| Pelli et al. | 1988 | Pre-DOI | HIGH | Contrast test method |
| Elliott et al. | 1990 | NEEDED | HIGH | CS test reliability |
| Bolognini et al. | 2005 | NEEDED | HIGH | Hemianopia multisensory |
| Cuppini et al. | 2017 | NEEDED | MEDIUM | Computational mechanism |
| REINVENT (Spicer) | 2019 | DERIVED | MEDIUM | Closed-loop VR-BCI precedent |
| Robineau et al. | 2017 | NEEDED | MEDIUM | Self-regulation proof |
| Sitaram et al. | 2017 | NEEDED | HIGH | Neurofeedback methodology |
| VR-VET | 2023 | DERIVED | MEDIUM | VR for neglect (adjacent) |
| Arch PM&R meta | 2024 | NEEDED | MEDIUM | VR stroke rehab breadth |
| Topics Stroke Rehab | 2020 | DERIVED | MEDIUM | Ecological VR |
| J NeuroEng Rehab | 2025 | VERIFIED | MEDIUM | Multisensory home delivery |
| Network Neuroscience | 2022 | NEEDED | MEDIUM | EEG spectral signature |
| Wake Forest trials | ongoing | N/A | MEDIUM | Alternative AV parameters |

**Follow-up**: The DOIs flagged `[CITATION NEEDED — verify]` should be confirmed from primary sources before this wiki is used in any clinical or publication context.
