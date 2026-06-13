---
title: Research Papers Index
last_updated: 2026-06-12
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
  - 2026-05-14 PubMed-verified research audit (see Recent Additions section)
  - 2026-06-11 PubMed + ClinicalTrials.gov refresh for AV+tDCS, AV multisensory, VRT, and VR cross-modal trials
  - 2026-06-11 LuMamba / BioFoundation EEG foundation-model watch lead
  - 2026-06-12 NeurIPS 2025 EEG-AI methods lane: BrainBodyFM, EEG Foundation Challenge, REVE, LUNA, BrainOmni, NeurIPT, CSBrain, EEG-Bench, critical review
  - 2026-06-12 ruv-neural GitHub analysis-side watch lead
---

# Research Papers Index

Alphabetical catalog of every paper/study cited in the NegletFix project. Each entry lists the citation, DOI/URL (where available in source files), the key finding in 1-2 sentences, and which NegletFix module/decision it informs.

Confidence applies to the paper's evidentiary weight for this specific project, not the paper itself. DOIs flagged `[CITATION NEEDED — verify]` are cited in source files but without a verifiable DOI in the available materials.

---

## Bolognini et al. (2005)

**Citation**: Bolognini N, Rasi F, Coccia M, Làdavas E. Visual search improvement in hemianopic patients after audio-visual stimulation. *Brain*. 2005;128(Pt 12):2830–2842.
**DOI**: 10.1093/brain/awh662 — PMID: 16014652 (verified 2026-05-14)
**Key finding**: Spatially and temporally aligned audiovisual stimuli enhance visual detection in the blind hemifield of hemianopia patients — clinical evidence that the preserved retinotectal pathway can mediate recovery.
**Informs**: [[scientific-foundation]], [[audiovisual-training-protocol]] — direct precedent for the audiovisual cross-modal approach targeting Eric's left hemifield.
**Confidence for NegletFix**: HIGH (peer-reviewed, direct condition match).

---

## Broustail, Tegon, Ingolfsson, Li & Benini (2026) — LuMamba EEG foundation model

**Citation**: Broustail D, Tegon A, Ingolfsson TM, Li Y, Benini L. LuMamba: Latent Unified Mamba for Electrode Topology-Invariant and Efficient EEG Modeling. *arXiv*. 2026.
**DOI**: 10.48550/arXiv.2603.19100 — arXiv:2603.19100 (added 2026-06-11)
**Key finding**: Self-supervised EEG foundation model combining topology-invariant channel unification, bidirectional Mamba temporal modeling, and LeJEPA + reconstruction objectives. Pretrained on 21,000+ hours of TUEG EEG and evaluated across 16-26 channel EEG configurations. The authors report a 4.6M parameter model with improved efficiency and cross-montage robustness.
**Informs**: [[eeg-neurofeedback]] — future EEG signal-analysis infrastructure watch lead only.
**Confidence for NegletFix**: LOW-MEDIUM for direct project impact. This is not hemianopia, audiovisual training, or neurofeedback efficacy evidence; it is a methods lead for possible future artifact/signal-quality/montage-adaptation tooling.

---

## NeurIPS 2025 EEG-AI methods lane

**Citation / source cluster**: NeurIPS 2025 BrainBodyFM workshop; EEG Foundation Challenge 2025; OpenReview NeurIPS 2025 conference/workshop papers for REVE, LUNA, BrainOmni, NeurIPT, CSBrain, EEG-Bench, and EEG Foundation Models: A Critical Review.
**DOI / URLs**: source queue rows `NP-001` through `NP-009` in `docs/research/source-queue-2026-05-25.csv`; key traceable URLs include the [BrainBodyFM workshop](https://brainbodyfm-workshop.github.io/), [EEG Foundation Challenge](https://eeg2025.github.io), and OpenReview pages for each model/benchmark.
**Key finding**: NeurIPS 2025 showed a clear EEG/biosignal foundation-model wave: larger pretraining corpora, topology/electrode-layout handling, sensor encoders, cross-subject/cross-task transfer, and more explicit benchmark discipline. The most relevant methods leads are REVE, LUNA/BioFoundation, BrainOmni, NeurIPT, and CSBrain; the important guardrails are EEG-Bench and the critical review.
**Informs**: [[eeg-neurofeedback]], `docs/research/research-monitor-2026-06-11.html` — future EEG-AI analytics lane only.
**Confidence for NegletFix**: LOW-MEDIUM for direct project impact. The lane is useful for offline artifact detection, signal-quality scoring, montage adaptation, and higher-density EEG analytics. It is not clinical evidence for homonymous hemianopia recovery, audiovisual training efficacy, or Muse TP10 neurofeedback.

---

## ruvnet / ruv-neural (2026) — Rust EEG topology analysis repo

**Citation / source**: ruvnet. ruv-neural: Real-time brain network topology analysis in Rust. GitHub repository. 2026. [https://github.com/ruvnet/ruv-neural](https://github.com/ruvnet/ruv-neural)
**DOI / URLs**: no DOI. Source queue row `GH-001`; key traceable source is the GitHub repo and its `SECURITY_REVIEW.md`.
**Key finding**: Early Rust workspace with concrete modules for EEG/simulator signal processing, connectivity graphs, min-cut topology, embeddings, decoder logic, ESP32/WASM surfaces, and CLI tooling. It is interesting as analysis-side engineering inspiration, especially because Ruv/ruvnet tends to explore unconventional technical directions.
**Informs**: [[eeg-neurofeedback]], `docs/research/research-monitor-2026-06-11.html` — offline analysis inspiration only.
**Confidence for NegletFix**: LOW for direct project impact today. Watch the repo, harvest useful analysis ideas later, but do not use its clinical-risk scoring, do not store PHI in it, and do not let it drive live rehab or protocol decisions without local build/test evidence plus project-specific measurements.

---

## Magosso, Cuppini & Bertini (2017)

**Citation**: Magosso E, Cuppini C, Bertini C. Audiovisual Rehabilitation in Hemianopia: A Model-Based Theoretical Investigation. *Frontiers in Computational Neuroscience*. 2017;11:113.
**DOI**: 10.3389/fncom.2017.00113 — PMID: 29326578 (verified 2026-05-14)
**Key finding**: Computational model of how repeated multisensory training strengthens residual SC→extrastriate connections, providing a mechanistic basis for audiovisual rehabilitation protocols.
**Informs**: [[scientific-foundation]], [[audiovisual-training-protocol]] — the theoretical "why" behind the Daibert-Nido protocol's effectiveness.
**Confidence for NegletFix**: MEDIUM (modeling paper, mechanistic not outcome evidence).

> *Citation correction (2026-05-14)*: previously cited as "Cuppini et al. 2017" — first author is **Magosso**, not Cuppini.

---

## Daibert-Nido et al. (2021) — pilot, originally framed as anchor

**Citation**: Daibert-Nido M, et al. Home-Based Visual Rehabilitation in Patients With Hemianopia. *Frontiers in Neurology*. 2021.
**DOI**: 10.3389/fneur.2021.680211
**Key finding**: VR audiovisual stimulation (3D-MOT-IVR paradigm) produced contrast-sensitivity, Humphrey-field, and reading-speed improvements.
**Sample**: **N=2 pediatric brain tumor survivors** (one with hemianopia, one with bitemporal hemianopia) — not a powered adult-stroke trial.
**Informs**: [[audiovisual-training-protocol]] (paradigm template), [[erics-baseline]] (rough target magnitude only).
**Confidence for NegletFix**: **DOWNGRADED to LOW–MEDIUM** for chronic adult stroke (verified 2026-05-14). Effect-size range (+0.31–0.54 LogCS) is plausible but **not replicated at scale** in adults with V1 stroke; magnitude likely overstated for chronic case. See Cavanaugh/Yang/Saionz 2023, Alharshan/Alwashmi 2025, and Misawa/Daibert-Nido 2024 entries below for the broader evidence picture.

> *Reframing note (2026-05-14)*: this entry was previously labeled "ANCHOR PAPER" with HIGH confidence. That was overstated — the trial was N=2 pediatric, the stimulus parameters cited elsewhere in the wiki (400 Hz tone) appear misattributed (Daibert-Nido used 3D-MOT in IVR), and the +0.31–0.54 LogCS magnitude has not been independently replicated in adult stroke. The protocol family is real and promising; the original paper is a foundational pilot, not a definitive trial.

---

## Elliott, Bullimore & Bailey (1991)

**Citation**: Elliott DB, Bullimore MA, Bailey IL. Improving the reliability of the Pelli-Robson contrast sensitivity test. *Ophthalmic and Physiological Optics*. 1991;11(1):61–65.
**DOI**: 10.1111/j.1475-1313.1991.tb00368.x — PMID: 2062940 (verified 2026-05-14)
**Key finding**: Establishes coefficient of repeatability for Pelli-Robson testing at ±0.24 LogCS — the psychometric basis for calling ≥0.30 LogCS a clinically significant change.
**Informs**: [[contrast-sensitivity-test]], [[erics-baseline]] — the 0.30 LogCS significance threshold used throughout this project.
**Confidence for NegletFix**: HIGH (standard clinical reference).

> *Citation correction (2026-05-14)*: previously cited as "Elliott, Sanderson & Conkey 1990" — that paper does not appear in PubMed. The Pelli-Robson reliability paper is **Elliott, Bullimore & Bailey 1991**.

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

## Robineau et al. (2014) — direct hemianopia precedent

**Citation**: Robineau F, Saj A, Neveu R, Van De Ville D, Scharnowski F, Vuilleumier P. Using fMRI neurofeedback to restore right occipital cortex activity in patients with left homonymous hemianopia: proof-of-principle study. *Neuropsychologia*. 2014;62:227–233.
**DOI**: 10.1016/j.neuropsychologia.2014.07.020 — PMID: 25064604 (verified 2026-05-14)
**Key finding**: First demonstration that **left-hemianopia patients** can voluntarily up-regulate right occipital cortex activity via real-time fMRI neurofeedback — direct precedent for closed-loop neurofeedback in Eric's exact condition.
**Informs**: [[eeg-neurofeedback]] — strongest available evidence that neurofeedback in hemianopia is at least feasible.
**Confidence for NegletFix**: MEDIUM (fMRI not EEG; proof-of-principle, small n; same diagnosis).

> *Citation correction (2026-05-14)*: previously cited as "Robineau 2017 — Self-regulation of inter-hemispheric visual cortex balance" (a healthy-volunteer paper). The hemianopia precedent is the **2014 Neuropsychologia** paper, far more relevant to Eric.

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

**Citation**: Sitaram R, Ros T, Stoeckel L, Haller S, Scharnowski F, Lewis-Peacock J, et al. Closed-loop brain training: the science of neurofeedback. *Nature Reviews Neuroscience*. 2017;18(2):86–100.
**DOI**: 10.1038/nrn.2016.164 — PMID: 28003656 (verified 2026-05-14)
**Key finding**: Comprehensive review establishing closed-loop neurofeedback as a legitimate, mechanism-driven intervention paradigm, with best practices for threshold adaptation and session design.
**Informs**: [[eeg-neurofeedback]] — methodological rationale for the adaptive threshold (40-60% success rate target) in `EngagementCalculator.cs`.
**Confidence for NegletFix**: HIGH (high-impact review, methodology reference). *Note 2026-05-14*: Treves et al. 2025 JMIR meta-analysis tempers consumer-grade NF claims — see new entry below.

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
**ClinicalTrials.gov status refresh (2026-06-11)**: [NCT04963075](https://clinicaltrials.gov/study/NCT04963075) is **completed with posted results**; [NCT05894434](https://clinicaltrials.gov/study/NCT05894434) is **not yet recruiting**. The older "ongoing" status should no longer be reused without a fresh registry check. The NCT04963075 posted primary outcome reports affected-field detected points increasing from 14.0 mean at start to 63.6 mean at end of study, with high variance and n=5. See [[clinical-trials-watchlist]].
**Confidence for NegletFix**: MEDIUM (direct condition/protocol fit, but small/high-variance human evidence). The mechanism work is now confirmed in **Rowland, Bushnell, Duncan & Stein (2023)** — see new entry below — and **Bean, Stein & Rowland (2023)** Cerebral Cortex animal-model paper.

---

# Recent Additions (verified 2026-05-14)

The following entries were added after the deep research audit on 2026-05-14. Each citation has been individually PubMed-verified.

---

## Alharshan, Alwashmi et al. (2026) — first DTI/FC mechanism paper for AV training in stroke HH

**Citation**: Alharshan F, Alwashmi K, Rowe FJ, Meyer G, et al. Functional and structural adaptations following immersive audiovisual training in post-stroke hemianopia: A study of behaviour, DTI, and FC. *NeuroImage*. 2026;325:121669.
**PMID**: 41421499 (verified 2026-05-14)
**Key finding**: n=15 post-stroke hemianopia. **30 min/day × 5 days/week × 6 weeks (15 hr total)** immersive VR AV training. Significant reaction-time improvements; FA increases in occipital pole, thalamus, MTG; enhanced FC between medial visual cortex and auditory cortex. **First imaging evidence that AV training induces measurable structural plasticity in stroke HH.**
**Informs**: [[audiovisual-training-protocol]] (dose), [[scientific-foundation]] (mechanism), [[rehabilitation-roadmap]] (evidence tier).
**Confidence for NegletFix**: **HIGH** (peer-reviewed, condition match, imaging mechanism). The 15-hour total dose is ~3× the Daibert-Nido budget — suggests chronic stroke needs longer protocols.

---

## Alwashmi et al. (2024) — VR AV training mechanism in healthy subjects

**Citation**: Alwashmi K, Meyer G, Rowe F, Ward R. Enhancing learning outcomes through multisensory integration: A fMRI study of audio-visual training in virtual reality. *NeuroImage*. 2024;285:120483.
**PMID**: 38048921 (verified 2026-05-14)
**Key finding**: n=20 healthy. 30 min/day × 4 wk VR AV scanning training. Reaction time 1.63s → 1.23s (p=4×10⁻¹⁶); fMRI changes in thalamus, IPL, cerebellum; transfer to untrained tasks. Methodologically directly portable to NegletFix.
**Informs**: [[audiovisual-training-protocol]], [[scientific-foundation]].
**Confidence for NegletFix**: MEDIUM (healthy participants but exact methodological precedent for the 2026 stroke paper).

---

## Bagherzadeh et al. (2026) — closed-loop IAF neurofeedback enhances attention dynamics

**Citation**: Bagherzadeh Y, et al. Successful closed-loop neurofeedback alpha frequency modulation enhances the temporal dynamics of attention. *NeuroImage*. 2026;332:121912.
**Key finding**: n=108 healthy adults, 5 sessions individualized alpha-frequency (IAF) neurofeedback vs active placebo. Learners (~50% of cohort) showed faster RT and earlier alpha-ERD latency. **Causal evidence that NF can change attention timing**; replicates ~50% non-responder rate.
**Informs**: [[eeg-neurofeedback]] (closes the "does NF actually work" question for visual attention — yes, in healthy subjects).
**Confidence for NegletFix**: MEDIUM (healthy, not stroke; supports mechanism only).

---

## Bean, Stein & Rowland (2023) — mechanism update

**Citation**: Bean NL, Stein BE, Rowland BA. Cross-modal exposure restores multisensory enhancement after hemianopia. *Cerebral Cortex*. 2023;33(22):11036–11046.
**PMID**: 37724427 (verified 2026-05-14)
**Key finding**: Animal model. Recovery via cross-modal training happens, but **multisensory enhancement is NOT prerequisite** — mechanism is more complex than previously thought. Implication: strict ~16 ms perceptual binding may be less critical than NegletFix currently assumes.
**Informs**: [[scientific-foundation]] (mechanism revision), [[audiovisual-training-protocol]] (relaxes timing constraint).
**Confidence for NegletFix**: MEDIUM-HIGH (mechanistic, animal model).

---

## Chen et al. (2026) — BCI meta-analysis in chronic stroke

**Citation**: Chen H, Yun Y, et al. Efficacy of Brain-Computer Interface Therapy for Upper Limb Rehabilitation in Chronic Stroke: Systematic Review and Meta-Analysis of RCTs. *J Med Internet Res*. 2026;28:e79132.
**Key finding**: 21 RCTs (not 32 — see correction note below), chronic stroke. BCI vs control: FMA-UE MD 2.50 (95% CI 0.60–4.40, p=.01). BCI-FES subgroup: MD 5.00. Optimal protocol: 30-min sessions, 4–5×/week × 2 weeks.
**Informs**: [[eeg-neurofeedback]] (closest analogous pooled estimate for closed-loop NF + behavioral task).
**Confidence for NegletFix**: MEDIUM (motor stroke, not visual; supports combined-modality design philosophy).

> *Audit note (2026-05-14)*: An earlier research-agent draft cited "Iturrate 2026 Neurosurg Focus, 32 RCTs n=1187 FMA-UE MD 3.85". That citation could not be verified on PubMed and **was likely hallucinated**. The 21-RCT Chen meta-analysis is the real, verified pooled estimate for chronic stroke BCI.

---

## Diana et al. (2025) — AV training + tDCS RCT in chronic HVFD

**Citation**: Diana L, et al. Enhancing multisensory rehabilitation of visual field defects with transcranial direct current stimulation: A randomized clinical trial. *European Journal of Neurology*. 2025.
**DOI**: 10.1111/ene.16559 — PMID: 39607286 — PMC11625917 — [NCT06116760](https://clinicaltrials.gov/study/NCT06116760) (registry status refreshed 2026-06-11)
**Key finding**: n=18 chronic HVFDs. 2-week AV compensatory training with ipsilesional occipital OR parietal anodal tDCS (or sham), 2 h/day × 10 days. **Occipital tDCS produced longer-lasting effects on blind-hemifield visual processing.**
**Informs**: [[audiovisual-training-protocol]] (potential adjunct), [[rehabilitation-roadmap]] (chronic-actionable evidence tier).
**Confidence for NegletFix**: MEDIUM-HIGH as an adjunct signal (RCT, chronic, condition-matched, but n=18). tDCS hardware/montage/safety is separate from Quest/Muse and requires clinician supervision; it should not be added to NegletFix before a clean open-loop behavioral baseline and plateau.

---

## Clinical Trial Refresh (2026-06-11) — tDCS, AV, VRT, VR cross-modal

Source map: [[clinical-trials-watchlist]] and `docs/research/clinical-trials-watchlist-2026-06-11.csv`.

### NCT06116760 — exact AV+tDCS chronic HVFD lead

**Trial**: [Anodal tDCS With Compensatory Audio-visual Training for Acquired Visual Field Defects After Brain Injury](https://clinicaltrials.gov/study/NCT06116760)
**Status on 2026-06-11**: Completed; registry results not posted.
**Linked paper**: Diana et al. 2025, PMID 39607286.
**Key finding**: small randomized chronic-HVFD trial supports occipital/parietal anodal tDCS as an adjunct to AV training, with the strongest lasting blind-field detection signal for occipital tDCS.
**NegletFix implication**: clinician-supervised adjunct question only; no home tDCS protocol change.

### NCT04963075 — Wake Forest AV multisensory line

**Trial**: [Multisensory Rehabilitation of Hemianopia](https://clinicaltrials.gov/study/NCT04963075)
**Status on 2026-06-11**: Completed with posted results.
**Linked paper**: Rowland et al. 2023, PMID 36604169.
**Posted primary outcome**: affected-field detected points increased from 14.0 mean at start to 63.6 mean at end of study; intact field stayed near ceiling.
**NegletFix implication**: supports the audiovisual core, but remains small and high-variance.

### NCT07358832 — active subacute AV+tDCS trial

**Trial**: [Neuromodulation Through Multisensory Stimulation for Visual Field Deficits in the Subacute Stage of Disease](https://clinicaltrials.gov/study/NCT07358832)
**Status on 2026-06-11**: Recruiting.
**Key feature**: subacute stroke HVFD patients receive AV training plus real or sham occipital tDCS.
**NegletFix implication**: important watchlist item, but Eric's case is chronic, not subacute.

### NCT02935413 — subacute tDCS+VRT pilot

**Trial**: [Combined tDCS and Vision Restoration Training in Subacute Stroke Rehabilitation](https://clinicaltrials.gov/study/NCT02935413)
**Linked paper**: [PMID 28082176](https://pubmed.ncbi.nlm.nih.gov/28082176/).
**Key finding**: open-label subacute PCA-stroke pilot, 7 tDCS/VRT patients versus 7 matched controls; safe/accepted and larger perimetry recovery reported.
**NegletFix implication**: early-window adjunct evidence, not directly transferable to a chronic home protocol.

### NCT03350919 and NCT04230486 — comparator trials

**NCT03350919**: completed blind-field visual training trial with posted Humphrey PMD results; useful as a visual-restoration comparator and measurement-design reference.
**NCT04230486**: completed VR cross-modal rehabilitation trial with posted outcomes; useful Quest/cross-modal comparator, but extract effect sizes before citing strongly.

---

## Misawa, Daibert-Nido et al. (2024) — Daibert-Nido follow-up on Meta Quest at home

**Citation**: Misawa M, Yaman Bajin I, Zhang B, Daibert-Nido M, et al. A telerehabilitation program to improve visual perception in children and adolescents with hemianopia consecutive to a brain tumor: a single-arm feasibility and proof-of-concept trial. *EClinicalMedicine*. 2024;78:102955.
**PMID**: 39687429 (verified 2026-05-14)
**Key finding**: n=10 pediatric (stable hemianopia). 3D-MOT-IVR on **Meta Quest 2/Pro at home**, 20-min sessions every other day × 6 weeks. **9/10 showed clinically meaningful improvement**; benefits sustained at 6 months. Cybersickness rare.
**Informs**: [[hardware-setup]] (Quest 2 home delivery validated), [[audiovisual-training-protocol]] (direct extension of Daibert-Nido 2021).
**Confidence for NegletFix**: HIGH for **feasibility/safety** of the consumer-VR-at-home approach. MEDIUM for effect size in adult stroke (pediatric tumor cohort).

> *Citation note*: agents initially cited this as "Bélanger 2024 EClinicalMedicine" — first author is **Misawa**, with Daibert-Nido as senior author.

---

## Namgung et al. (2024) — VR-VPL multicenter RCT in chronic post-stroke VFD

**Citation**: Namgung E, et al. Digital therapeutics using virtual reality-based visual perceptual learning for visual field defects in stroke: A double-blind randomized trial. *Brain and Behavior*. 2024;14(1):e3525.
**DOI**: 10.1002/brb3.3525 — PMC11109502 (verified 2026-05-14)
**Key finding**: Multicenter RCT, **n=82 chronic (>6 mo post-stroke)**, VR-VPL (Nunap Vision, mobile head-mounted display), 5 days/week × 12 weeks. Both target and active-control arms improved (learning-transfer effect, no between-group difference) — **important caveat for trial design**.
**Informs**: [[rehabilitation-roadmap]] (strongest evidence yet for chronic responsiveness, with control-arm warning).
**Confidence for NegletFix**: HIGH for chronic responsiveness; MEDIUM caveat — active VR + attention itself drives improvement, not just the targeted training.

---

## Namgung et al. (2025) — personalized VR-VPL JAMA RCT in chronic stroke

**Citation**: Namgung E, et al. Personalized Visual Perceptual Learning Digital Therapy for Visual Field Defects Following Stroke: A Randomized Clinical Trial. *JAMA Network Open*. 2025;8(5):e2511068.
**DOI**: 10.1001/jamanetworkopen.2025.11068 (verified 2026-05-14)
**Key finding**: Multicenter RCT, **n=82 chronic post-stroke (>3 mo)**, **personalized** VR visual discrimination tasks via mobile HMD, 5 days/week × 12 weeks. Clinically meaningful field gains (median +72° squared improvement). **Strongest published RCT in Eric's exact population**.
**Informs**: [[rehabilitation-roadmap]] (chronic evidence tier), [[audiovisual-training-protocol]] (personalization rationale validated).
**Confidence for NegletFix**: HIGH (largest, most rigorous chronic-stroke VR-VPL trial as of 2026-05).

---

## Raffin et al. (2025) — cf-tACS in chronic hemianopia

**Citation**: Raffin E, Bevilacqua M, et al. Boosting hemianopia recovery: the power of interareal cross-frequency brain stimulation. *Brain*. 2025;148(12):4548–4561.
**PMID**: 41243213 (verified 2026-05-14)
**Key finding**: Double-blind crossover RCT, n=16 (median 11 mo post-stroke), forward cf-tACS (α-V1 + γ-MT) vs backward (γ-V1 + α-MT) concurrent with motion training, 10 sessions × ~2 weeks. **Forward cf-tACS produced greater improvements in kinetic visual field boundaries** — first clean tACS-in-hemianopia evidence. Much faster than typical months-long VPL protocols.
**Informs**: [[rehabilitation-roadmap]] (potential adjunct), [[scientific-foundation]] (V1↔MT pathway as causal target).
**Confidence for NegletFix**: MEDIUM-HIGH for the result; **not actionable for home setup** (tACS hardware is separate, requires expert placement).

---

## Rowland, Bushnell, Duncan & Stein (2023) — chronic AV training in adult stroke

**Citation**: Rowland BA, Bushnell CD, Duncan PW, Stein BE. Ameliorating Hemianopia with Multisensory Training. *Journal of Neuroscience*. 2023;43(6):1018–1026.
**PMID**: 36604169 (verified 2026-05-14)
**Key finding**: **n=2 chronic stroke patients (≥8 months post-stroke)**, 2-hour sessions of repeated congruent visual-auditory stimulation in blind hemifield over weeks. Both patients recovered detection, localization, identification within blind field. Strongest pure-AV chronic-stroke result published.
**Informs**: [[audiovisual-training-protocol]] (alternative Wake Forest paradigm), [[scientific-foundation]].
**Confidence for NegletFix**: MEDIUM (n=2 case report, but chronic adult stroke and dramatic effect sizes).

> *Citation note*: agents initially cited this as "Grasso/Dundon 2023" — actual first author is **Rowland**.

---

## Saionz et al. (2025) — natural-history baseline for occipital stroke VF

**Citation**: Saionz EL, Cavanaugh MR, Johnson BA, Harrington D, Aguirre GK, Huxlin KR. Evolution of Visual Field Defects After Occipital Stroke: A Quantitative Analysis. *Translational Vision Science & Technology*. 2025.
**PMID**: 40478590 (verified 2026-05-14)
**Key finding**: Retrospective chart review, n=532 (73 longitudinal). >77% of patients improve spontaneously in subacute window (<6 mo). **In chronic phase (>6 mo), VF is stable overall — about half show ±1 dB drift, which is test-retest noise, not real change.** Defines the no-treatment null comparator for any chronic rehab claim.
**Informs**: [[rehabilitation-roadmap]] (sets honest expectations), [[contrast-sensitivity-test]] (test-retest noise floor for chronic patients).
**Confidence for NegletFix**: HIGH (large retrospective sample, clear quantification of natural history).

> *Citation note*: agents initially cited this as "Rossi 2025" — actual first author is **Saionz** (Huxlin lab).

---

## Saionz et al. (2020) — the subacute >> chronic finding

**Citation**: Saionz EL, Tadin D, Melnick MD, Huxlin KR. Functional preservation and enhanced capacity for visual restoration in subacute occipital stroke. *Brain*. 2020;143(6):1857–1872.
**PMID**: 32428211 — DOI 10.1093/brain/awaa145 (verified 2026-05-14)
**Key finding**: Subacute patients needed ~16 sessions to achieve what chronic patients needed ~93 sessions for. **Subacute training is ~6× more efficient than chronic** for global motion discrimination recovery.
**Informs**: [[rehabilitation-roadmap]] (Eric is chronic, expectations should match), [[scientific-foundation]].
**Confidence for NegletFix**: HIGH (referenced repeatedly across the wiki; finally with verified citation).

---

## Yang, Cavanaugh, Saionz, Huxlin et al. (2023) — chronic CS reality check

**Citation**: Yang J, Saionz EL, Cavanaugh MR, Fahrenthold BK, Melnick MD, Tadin D, Briggs F, Carrasco M, Huxlin KR. Contrast sensitivity: a fundamental limit to vision restoration after V1 damage. *medRxiv preprint*. 2023.
**PMC**: PMC10491352 (verified 2026-05-14 — **preprint, not yet peer-reviewed**)
**Key finding**: n=12 chronic (mean 28±33 months post-V1-stroke). Training improved CS in only 7/12 (58%) at 10/22 trained locations (45%). **Post-training CS remained ~4× lower than intact-field** even at trained locations. *Eric's 2.25 LogCS asymmetry will not fully close.*
**Informs**: [[erics-baseline]] (sets honest expectations), [[audiovisual-training-protocol]] (downgrades the +0.31–0.54 LogCS framing).
**Confidence for NegletFix**: MEDIUM (preprint, but the most directly relevant chronic-CS data point we have).

> *Citation note*: agents initially cited this as "Cavanaugh/Saionz/Huxlin 2023" — first author is **Yang**; Cavanaugh and Saionz are co-authors.

---

## Yang, Fiebelkorn, Jensen, Knight & Kastner (2024) — alpha gating mechanism

**Citation**: Yang X, Fiebelkorn IC, Jensen O, Knight RT, Kastner S. Differential neural mechanisms underlie cortical gating of visual spatial attention mediated by alpha-band oscillations. *PNAS*. 2024;121(45):e2313304121.
**Key finding**: ECoG study in 8 epilepsy patients during Eriksen Flanker task. Shows alpha is **bidirectional** — desynchronization contralateral to attended hemifield AND synchronization ipsilateral — both contribute to attention gating.
**Informs**: [[eeg-neurofeedback]] (modern mechanistic basis for ↓alpha at TP10 for left-attention training).
**Confidence for NegletFix**: HIGH (intracranial, mechanistic). Strengthens the rationale for TP10 targeting.

---

## Treves et al. (2025) — consumer-grade neurofeedback reality check

**Citation**: Treves IN, et al. Consumer-Grade Neurofeedback With Mindfulness Meditation: Meta-Analysis. *Journal of Medical Internet Research*. 2025;27:e68204.
**PMID**: 40246295 — PMC12046271 (verified 2026-05-14)
**Key finding**: Meta-analysis of 16 RCTs (n=763) + 5 within-participant designs. 11/16 RCTs used Muse. **No evidence for cognition, mindfulness, or physiological improvements; only modest distress reduction.** Authors suggest effects "may rely on neurosuggestion."
**Informs**: [[eeg-neurofeedback]] (critical counterweight — consumer NF claims should be tempered).
**Confidence for NegletFix**: HIGH for the meta-analytic null result.

> *Citation note*: agents initially cited this as "Hardy 2025 JMIR" — first author is **Treves**.

---

## ESO 2025 — European Stroke Organisation guideline on visual impairment in stroke

**Citation**: Rowe FJ, Hepworth LR, Coco-Martín MB, Gillebert CR, Leal-Vega L, Palmowski-Wolfe A, Papageorgiou E, Ryan SJ, Skorkovska K, Aamodt AH. European Stroke Organisation (ESO) guideline on visual impairment in stroke. *European Stroke Journal*. 2025;10(4):1087.
**PMID**: 40401755 — DOI 10.1177/23969873251314693 (verified 2026-05-14)
**Key finding**: First international consensus guideline on post-stroke visual impairment. Evidence for VPL strongest in <12-month window; chronic gains modest but real. Visual impairment affects ~75% of stroke survivors.
**Informs**: [[rehabilitation-roadmap]] (clinical context), [[scientific-foundation]].
**Confidence for NegletFix**: HIGH (authoritative consensus).

---

## Laver et al. (2025) — Cochrane VR for stroke rehabilitation

**Citation**: Laver KE, Lange B, George S, Deutsch JE, Saposnik G, Crotty M. Virtual reality for stroke rehabilitation. *Cochrane Database of Systematic Reviews*. 2025;6:CD008349.pub5.
**DOI**: 10.1002/14651858.CD008349.pub5 — PMID: 40537150 (verified 2026-05-14)
**Key finding**: Latest Cochrane update; supersedes the 2017 pub4 commonly cited in stroke-VR literature.
**Informs**: [[rehabilitation-roadmap]] (general VR-stroke evidence base).
**Confidence for NegletFix**: HIGH (Cochrane).

---

## Scheidtmann et al. (2001) — Levodopa motor-stroke anchor (DOI fill-in)

**Citation**: Scheidtmann K, Fries W, Müller F, Koenig E. Effect of levodopa in combination with physiotherapy on functional motor recovery after stroke: a prospective, randomised, double-blind study. *Lancet*. 2001;358(9284):787–790.
**DOI**: 10.1016/S0140-6736(01)05456-X — PMID: 11476841 (verified 2026-05-14)
**Key finding**: Original positive levodopa + physiotherapy trial in motor stroke (n=53). Establishes the dopaminergic-adjunct hypothesis that NegletFix's [[pharmacological-adjuncts]] page references.
**Informs**: [[pharmacological-adjuncts]] (fills missing DOI).
**Confidence for NegletFix**: MEDIUM (motor stroke, not visual; pre-replication-failure era).

---

## Schneider et al. (2023) — FLUORESCE fluoxetine pilot (cross-ref)

**Citation**: Schneider CL, Prentiss EK, Busza A, Williams ZR, Mahon BZ, Sahin B. FLUORESCE: A Pilot Randomized Clinical Trial of Fluoxetine for Vision Recovery After Acute Ischemic Stroke. *Journal of Neuro-Ophthalmology*. 2023;43(2):237–242.
**PMID**: 36166771 (verified 2026-05-14)
**Key finding**: Pilot RCT, n=12 completers. 64.4% vs 26.0% perimetry improvement (p=0.06, NS trend). **Acute-only enrollment** (<10 d post-stroke).
**Informs**: [[pharmacological-adjuncts]] (already cited there in full; cross-listed here).
**Confidence for NegletFix**: LOW for chronic transfer (acute window only).

---

## Fluoxetine Individual Patient Data Meta-analysis (2024) — stroke recovery caution

**Citation**: Individual patient data meta-analysis of the FOCUS, AFFINITY, and EFFECTS randomized trials of fluoxetine after acute stroke.
**PMCID**: PMC11298115 (added 2026-05-26)
**Key finding**: Across 5907 recent-stroke patients, fluoxetine did not improve functional recovery and increased adverse events including seizures, fall-injury, fractures, and hyponatremia.
**Informs**: [[pharmacological-adjuncts]] and `docs/research/medication-neuroplasticity-doctor-brief-2026-05-26.html`.
**Confidence for NegletFix**: HIGH as a general SSRI recovery caution; LOW for direct HH because the target was broad stroke recovery, not chronic visual field restoration.

---

## DARS Trial (2019) — Co-careldopa stroke rehabilitation caution

**Citation**: Ford GA et al. Safety and efficacy of co-careldopa as an add-on therapy to occupational and physical therapy after stroke (DARS): a randomised, double-blind, placebo-controlled trial.
**PMID**: 31122493 (added 2026-05-26)
**Key finding**: Co-careldopa added to rehabilitation did not improve walking recovery versus placebo in a large stroke rehabilitation trial.
**Informs**: [[pharmacological-adjuncts]].
**Confidence for NegletFix**: HIGH as motor-stroke adjunct caution; LOW for direct visual transfer.

---

## ESTREL Trial (2025) — Levodopa added to stroke rehabilitation caution

**Citation**: ESTREL randomized clinical trial of levodopa added to stroke rehabilitation.
**PMID**: 40982270 (added 2026-05-26)
**Key finding**: Levodopa added to standardized rehabilitation did not reduce the proportion of patients with a modified Rankin Scale score of 3 or more at 90 days and had higher mortality, though not judged treatment-related by investigators.
**Informs**: [[pharmacological-adjuncts]] and the 2026-05-26 medication doctor brief.
**Confidence for NegletFix**: HIGH as the latest levodopa rehab caution; LOW for direct visual-field transfer.

---

## Summary Table

Refreshed 2026-06-12 — ruv-neural GitHub analysis-side watch lead and NeurIPS 2025 EEG-AI methods lane added; 2026-05-26 medication-adjunct caution entries added after the doctor-brief sprint; 2026-05-14 audit resolved all prior "[CITATION NEEDED]" rows.

### Foundational / pre-audit
| Paper | Year | DOI | Confidence | Primary Use |
|-------|------|------|------------|-------------|
| Daibert-Nido et al. | 2021 | 10.3389/fneur.2021.680211 | **LOW–MEDIUM** ↓ | AV protocol family origin (N=2 pediatric pilot) |
| Ros et al. | 2017 | 10.1155/2017/7407241 | MEDIUM-HIGH | EEG NF protocol (neglect, n=5) |
| Stein & Meredith | 1993 | N/A (book) | HIGH | Multisensory theory |
| Pelli, Robson & Wilkins | 1988 | Pre-DOI | HIGH | Contrast test method |
| Elliott, Bullimore & Bailey ✱ | 1991 | 10.1111/j.1475-1313.1991.tb00368.x | HIGH | CS test reliability |
| Bolognini et al. | 2005 | 10.1093/brain/awh662 | HIGH | Hemianopia AV facilitation |
| Broustail/Ingolfsson et al. | 2026 | 10.48550/arXiv.2603.19100 | LOW-MEDIUM | EEG foundation-model methods watch lead |
| NeurIPS 2025 EEG-AI lane | 2025 | NP-001..NP-009 | LOW-MEDIUM | EEG foundation-model methods lane and guardrails |
| ruvnet / ruv-neural | 2026 | GH-001 | LOW | Rust EEG topology analysis repo; offline inspiration only |
| Magosso, Cuppini & Bertini ✱ | 2017 | 10.3389/fncom.2017.00113 | MEDIUM | Computational mechanism |
| REINVENT (Spicer) | 2019 | 10.3389/fnhum.2019.00210 | MEDIUM | Closed-loop VR-BCI precedent (motor) |
| Robineau et al. ✱ | 2014 | 10.1016/j.neuropsychologia.2014.07.020 | MEDIUM | fMRI NF in left HH (direct precedent) |
| Sitaram et al. | 2017 | 10.1038/nrn.2016.164 | HIGH | NF methodology canon |
| VR-VET feasibility | 2023 | 10.3389/fnins.2023.1142663 | MEDIUM | VR for neglect (adjacent) |
| Arch PM&R meta | 2024 | (article ID S0003-9993(24)01311-X) | MEDIUM | VR stroke breadth |
| Topics Stroke Rehab | 2020 | 10.1080/10749357.2020.1716531 | MEDIUM | Ecological VR |
| Network Neuroscience | 2022 | (MIT direct/netn/6/1/69) | MEDIUM | EEG spectral signature in neglect |
| Wake Forest trials | refreshed 2026-06-11 | NCT04963075 / NCT05894434 | MEDIUM | Alternative AV parameters; NCT04963075 completed with results, NCT05894434 not yet recruiting |

### Added in 2026-05-14 audit
| Paper | Year | DOI / PMID | Confidence | Primary Use |
|-------|------|-----------|------------|-------------|
| Alharshan/Alwashmi et al. | 2026 | PMID 41421499 | HIGH | DTI mechanism — stroke HH (n=15) |
| Alwashmi et al. | 2024 | PMID 38048921 | MEDIUM | VR AV training mechanism (healthy) |
| Bagherzadeh et al. | 2026 | NeuroImage 332:121912 | MEDIUM | Closed-loop IAF-NF causal evidence |
| Bean, Stein & Rowland | 2023 | PMID 37724427 | MEDIUM-HIGH | Mechanism revision (animal) |
| Chen et al. (BCI chronic stroke) | 2026 | JMIR 28:e79132 | MEDIUM | BCI meta-analysis (21 RCTs) |
| Diana et al. | 2025 | 10.1111/ene.16559 | MEDIUM-HIGH | AV+tDCS RCT in chronic HVFD (n=18) |
| ESO Guideline (Rowe et al.) | 2025 | PMID 40401755 | HIGH | Authoritative consensus |
| Laver et al. (Cochrane) | 2025 | 10.1002/14651858.CD008349.pub5 | HIGH | Cochrane VR for stroke |
| Misawa/Daibert-Nido et al. | 2024 | PMID 39687429 | HIGH (feasibility) | Quest 2/Pro home delivery validation |
| Namgung et al. | 2024 | 10.1002/brb3.3525 | HIGH | Chronic VR-VPL multicenter RCT (n=82) |
| Namgung et al. | 2025 | 10.1001/jamanetworkopen.2025.11068 | HIGH | Personalized chronic VR-VPL RCT (n=82) |
| Raffin et al. | 2025 | PMID 41243213 | MEDIUM-HIGH | cf-tACS in chronic hemianopia (n=16) |
| Rowland/Bushnell/Duncan/Stein | 2023 | PMID 36604169 | MEDIUM | Chronic AV training in adult stroke (n=2) |
| Saionz et al. (chronic VF natural hx) | 2025 | PMID 40478590 | HIGH | Natural-history baseline |
| Saionz et al. (subacute >> chronic) | 2020 | 10.1093/brain/awaa145 | HIGH | The chronic-vs-subacute finding |
| DARS (co-careldopa) | 2019 | PMID 31122493 | HIGH (caution) | Levodopa adjunct caution |
| ESTREL (levodopa) | 2025 | PMID 40982270 | HIGH (caution) | Latest levodopa adjunct caution |
| Fluoxetine IPD meta-analysis | 2024 | PMC11298115 | HIGH (caution) | Routine fluoxetine recovery caution |
| Scheidtmann et al. (levodopa) | 2001 | PMID 11476841 | MEDIUM | Pharm adjunct anchor |
| Schneider et al. (FLUORESCE) | 2023 | PMID 36166771 | LOW (acute only) | Fluoxetine pilot (xref) |
| Treves et al. (consumer NF meta) | 2025 | PMID 40246295 | HIGH (null result) | Consumer-NF reality check |
| Yang/Cavanaugh/Saionz et al. | 2023 | PMC10491352 (preprint) | MEDIUM | Chronic CS reality check |
| Yang/Fiebelkorn/Kastner et al. | 2024 | PNAS 121(45):e2313304121 | HIGH | Alpha-gating mechanism (ECoG) |

✱ = citation corrected in 2026-05-14 audit. Previous entries had wrong first author or year.

**Follow-up open items**:
- Maintain [[clinical-trials-watchlist]] monthly or quarterly; NCT04963075 and NCT05894434 status was refreshed 2026-06-11.
- Watch for posted registry results or replication of NCT06116760 / Diana AV+tDCS.
- Watch NCT07358832 as the active AV+tDCS subacute trial.
- Watch for larger FLUORESCE follow-up trial registration (Schneider/Mahon/Sahin group, Rochester).
- Watch for adult-stroke replication of the Daibert-Nido 3D-MOT-IVR paradigm at adequate power.
