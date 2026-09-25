---
title: Clinical Trials Watchlist
last_updated: 2026-09-25
confidence: HIGH registry status / MIXED protocol impact
sources:
  - PubMed E-utilities live check 2026-06-11
  - ClinicalTrials.gov API live check 2026-06-11
  - PubMed E-utilities live check 2026-06-17
  - ClinicalTrials.gov API live check 2026-06-17
  - PubMed E-utilities live check 2026-07-09
  - ClinicalTrials.gov API live check 2026-07-09
  - PubMed E-utilities live check 2026-08-13
  - ClinicalTrials.gov API live check 2026-08-13
  - ClinicalTrials.gov API and PubMed E-utilities live check 2026-09-25
  - docs/research/research-refresh-2026-09-25.md
  - docs/research/clinical-trials-watchlist-2026-09-25.csv
  - docs/research/research-monitor-2026-09-25.html
  - docs/research/clinical-trials-watchlist-2026-08-13.csv
  - docs/research/clinical-trials-watchlist-2026-08-13.md
  - docs/research/research-monitor-2026-08-13.html
  - docs/research/research-refresh-2026-08-13.md
  - Mayo Clinic official neuro-ophthalmology / PM&R / ophthalmology trials pages checked 2026-06-17 and 2026-08-13
---

# Clinical Trials Watchlist

Registry-backed monitor for external evidence that could affect NegletFix. This page tracks clinical trials and active evidence leads; it is not a treatment recommendation and should not override the measurement-first Quest audiovisual protocol.

Current structured source: `docs/research/clinical-trials-watchlist-2026-09-25.csv` (36 NCT records). The August snapshot below is retained as history.

Current human-readable research note: `docs/research/clinical-trials-watchlist-2026-09-25.md`.

Current static HTML monitor: `docs/research/research-monitor-2026-09-25.html`.

Unified intake queue: `docs/research/source-queue-2026-05-25.csv` now includes 88 rows, including `PM-001..PM-012` and `CTG-001..CTG-036`. Source: `docs/research/research-refresh-2026-09-25.md`.

### September 25 current state

- `CTG-012` / [NCT06136169](https://clinicaltrials.gov/study/NCT06136169) and `CTG-016` / [NCT04827147](https://clinicaltrials.gov/study/NCT04827147) moved to completed, with results posted. They remain compensation/accommodation comparators.
- `CTG-036` / [NCT07830745](https://clinicaltrials.gov/study/NCT07830745) is a newly registered, not-yet-recruiting French optokinetic reading study for hemianopic alexia at Boissise-le-Roi; it is not the Pitié study.
- `CTG-030` / [NCT04043689](https://clinicaltrials.gov/study/NCT04043689), HEMIANOTACS at AP-HP/Pitié-Salpêtrière, remains completed with no posted registry results. **Erratum:** [Raffin et al. 2025 / PMID 41243213](https://pubmed.ncbi.nlm.nih.gov/41243213/) is not verified as a publication of this trial. The August association below was too strong and is corrected in the September CSV and source queue.
- `PM-011` is a newly surfaced 28-study systematic review; `PM-012` validates an eye-tracking perimetry method. Neither changes the Quest protocol or provides a Quest-specific improvement threshold.
- No protocol change. Await Quest return and measured `-5°` / `-8°` probes, catch trials, and staircase retuning. See `docs/research/research-refresh-2026-09-25.md` for the source map and limits.

---

## 1. Current Verdict

The best exact-match adjunct lead remains:

- [NCT06116760](https://clinicaltrials.gov/study/NCT06116760) / [PMID 39607286](https://pubmed.ncbi.nlm.nih.gov/39607286/) — completed randomized clinical trial of audiovisual training plus anodal/sham tDCS in chronic homonymous visual-field defects.

August 13 refresh:

- Prior NCT rows `CTG-001` through `CTG-027` were rechecked. No tracked row had a new status or posted-results change after the July 9 refresh.
- The only true new-since-July registry row is `CTG-028` / [NCT07752563](https://clinicaltrials.gov/study/NCT07752563), first posted 2026-08-07: Glasgow Caledonian pilot RCT of NeuroEyeCoach/VISIOcoach computer-based eye-movement training after stroke.
- `CTG-015` / [NCT07147660](https://clinicaltrials.gov/study/NCT07147660) gained [PMID 42571232](https://pubmed.ncbi.nlm.nih.gov/42571232/), the DRIVE-study protocol paper for eight-week home-based visual rehabilitation and VR functional-vision testing.
- `CTG-029` through `CTG-035` were added as newly surfaced older/gap-fill rows: BITS touchscreen field-awareness results, AP-HP Paris tACS, Fondation Rothschild blindsight/fMRI, INSERM VR audiovisual telerehab, Rochester home blind-field retraining, Toronto microperimetry biofeedback, and Schepens prism feasibility.
- Mayo, YouTube, X/LinkedIn, arXiv, and GitHub watch lanes were rechecked. Mayo produced no new official protocol/trial; LMC2 IRON was parked as `LI-002` pending a traceable primary source; Neuro-JEPA moved to arXiv v3; ruv-neural remains tooling-only.
- None of the new rows changes the active NegletFix protocol.

Protocol impact:

- **Do not add home tDCS, tACS, tRNS, focused ultrasound, or other stimulation now.**
- Keep NegletFix on open-loop, field-map-guided Quest audiovisual training until there is a clean behavioral baseline, plateau evidence, and clinician review.
- Use the Paris move as context for possible clinician/research discovery, not as a reason to accelerate treatment changes.

---

## 2. Trial Dashboard

| ID | Trial | Status on 2026-08-13 | Evidence Role | NegletFix Action |
|----|-------|----------------------|---------------|------------------|
| CTG-001 | [NCT06116760](https://clinicaltrials.gov/study/NCT06116760) | Completed | Best exact-match tDCS plus audiovisual clinical trial lead; linked to [PMID 39607286](https://pubmed.ncbi.nlm.nih.gov/39607286/) | Clinician-supervised adjunct candidate only; do not add home tDCS before behavioral baseline and plateau |
| CTG-002 | [NCT04963075](https://clinicaltrials.gov/study/NCT04963075) | Completed with results posted | Closest Wake Forest adult chronic AV training registry line; linked to [PMID 36604169](https://pubmed.ncbi.nlm.nih.gov/36604169/) | Supports current AV core; sample/results still small and high-variance |
| CTG-003 | [NCT05894434](https://clinicaltrials.gov/study/NCT05894434) | Not yet recruiting | Wake Forest next-stage AV study | Watch only; no immediate protocol change |
| CTG-004 | [NCT07358832](https://clinicaltrials.gov/study/NCT07358832) | Recruiting | Best active AV+tDCS trial lead, but subacute not chronic | Watch as future evidence; not directly applicable to Eric's chronic window |
| CTG-005 | [NCT02935413](https://clinicaltrials.gov/study/NCT02935413) | Completed | Subacute VRT+tDCS feasibility and safety pilot; linked to [PMID 28082176](https://pubmed.ncbi.nlm.nih.gov/28082176/) | Park as early-window adjunct evidence; not a chronic home protocol |
| CTG-006 | [NCT03350919](https://clinicaltrials.gov/study/NCT03350919) | Completed with results posted | Modern blind-field training comparator with posted PMD results | Supports measurement-first visual training; not audiovisual and long dose |
| CTG-007 | [NCT04230486](https://clinicaltrials.gov/study/NCT04230486) | Completed with results posted | VR audiovisual/cross-modal comparator with posted outcomes | Supports Quest/cross-modal direction; needs careful effect-size review |
| CTG-008 | [NCT05085210](https://clinicaltrials.gov/study/NCT05085210) | Recruiting | Active neuromodulation plus VR/visual-training lead | Watch only; tRNS is not tDCS and is not part of NegletFix |
| CTG-009 | [NCT04798924](https://clinicaltrials.gov/study/NCT04798924) | Active not recruiting | Timing/plasticity study relevant to chronic-vs-subacute expectations | Watch for results to refine dose/timing expectations |
| CTG-010 | [NCT07317739](https://clinicaltrials.gov/study/NCT07317739) | Not yet recruiting | New exact post-stroke visual-field rehabilitation registry lead with early multisensory audiovisual stimulation | Watch only; reinforces AV research lane but has no results and does not change current Quest protocol |
| CTG-011 | [NCT07185971](https://clinicaltrials.gov/study/NCT07185971) | Recruiting | Recruiting HMD-based visual-field enhancement lead for chronic field loss | Compensation/device comparator only; do not treat as restorative AV evidence |
| CTG-012 | [NCT06136169](https://clinicaltrials.gov/study/NCT06136169) | Active not recruiting with results posted; registry results updated 2026-07-09 | Compensatory scanning-training comparator with posted registry outcomes and linked Archives PM&R publication; linked to [PMID 42140549](https://pubmed.ncbi.nlm.nih.gov/42140549/) | Functional compensation evidence only; useful for head-scan/hazard-detection metrics, not restorative AV dose or driving clearance |
| CTG-013 | [NCT07134777](https://clinicaltrials.gov/study/NCT07134777) | Recruiting | Mechanism/imaging watch lead for occipital-stroke visual restoration | Mechanism watch only; no intervention result yet |
| CTG-014 | [NCT06047717](https://clinicaltrials.gov/study/NCT06047717) | Recruiting | Functional navigation and steering measurement lead in VR | Useful for future function tracker, not treatment efficacy |
| CTG-015 | [NCT07147660](https://clinicaltrials.gov/study/NCT07147660) | Recruiting | Driving-focused visual rehabilitation/scanning-training trial; August PubMed RCT protocol paper added; linked to [PMID 42571232](https://pubmed.ncbi.nlm.nih.gov/42571232/) | Driving/function lane only; no driving clearance or restorative protocol implication |
| CTG-016 | [NCT04827147](https://clinicaltrials.gov/study/NCT04827147) | Active not recruiting with results posted | Prism/accommodation comparator with posted registry outcomes and randomized crossover publication; linked to [PMID 42274646](https://pubmed.ncbi.nlm.nih.gov/42274646/) | Accommodation evidence only; do not mix prism field expansion with restorative AV training claims |
| CTG-017 | [NCT05525949](https://clinicaltrials.gov/study/NCT05525949) | Completed | Completed digital visual perceptual learning comparator for post-stroke visual-field defect | Comparator only; wait for results/publication before use |
| CTG-018 | [NCT07659691](https://clinicaltrials.gov/study/NCT07659691) | Not yet recruiting | New Wake Forest multisensory rehabilitation registry lead first posted 2026-06-22 | High-priority AV watch only; no results and no current protocol change |
| CTG-019 | [NCT07635329](https://clinicaltrials.gov/study/NCT07635329) | Recruiting | BRIGHT home-based visual rehabilitation guided by brain imaging | Home-training/restoration comparator only; recruitment status does not change Quest AV protocol |
| CTG-020 | [NCT06875206](https://clinicaltrials.gov/study/NCT06875206) | Not yet recruiting | Neuromodulation plus VR visual-restoration trial lead | Clinician/research watch only; no stimulation/hardware change for NegletFix |
| CTG-021 | [NCT07237412](https://clinicaltrials.gov/study/NCT07237412) | Recruiting | Neurofeedback-based VRT registry comparator | Future EEG/neurofeedback research lane only; no Muse TP10 protocol impact |
| CTG-022 | [NCT06241209](https://clinicaltrials.gov/study/NCT06241209) | Recruiting | Low-tech visual stimulation comparator for hemianopsia rehabilitation | Comparator only; no evidence until results |
| CTG-023 | [NCT06341777](https://clinicaltrials.gov/study/NCT06341777) | Completed | Completed home AVT telerehabilitation comparator with published feasibility results; linked to [PMID 39994637](https://pubmed.ncbi.nlm.nih.gov/39994637/) | Supports home AV delivery and function improvements, but interpreted as compensatory oculomotor evidence |
| CTG-024 | [NCT07105358](https://clinicaltrials.gov/study/NCT07105358) | Recruiting | NIBS plus visual-training comparator | Watch only; do not add tRNS or stimulation to NegletFix |
| CTG-025 | [NCT06578117](https://clinicaltrials.gov/study/NCT06578117) | Enrolling by invitation | Visual learning/restoration comparator aiming to reduce training burden | Watch only until results |
| CTG-026 | [NCT06638619](https://clinicaltrials.gov/study/NCT06638619) | Not yet recruiting | Late-June update comparing eye-tracking biofeedback and saccadic reading training | Compensation/reading comparator only |
| CTG-027 | [NCT05141604](https://clinicaltrials.gov/study/NCT05141604) | Active not recruiting | VR/HMD obstacle-detection and field-expansion mobility comparator updated 2026-07-02 | Mobility/accommodation measurement lead only |
| CTG-028 | [NCT07752563](https://clinicaltrials.gov/study/NCT07752563) | Not yet recruiting | New August 7 pilot RCT of computer-based scanning/eye-movement training after stroke | Function/scanning comparator only; no visual restoration or AV protocol implication |
| CTG-029 | [NCT04930822](https://clinicaltrials.gov/study/NCT04930822) | Completed with results posted | New-to-monitor posted-results digital scanning comparator using Bells Test and kinetic field metrics | Inpatient acute/subacute field-awareness comparator only; not chronic restoration evidence |
| CTG-030 | [NCT04043689](https://clinicaltrials.gov/study/NCT04043689) | Completed | Paris AP-HP chronic HH neuromodulation study; no posted results. Raffin 2025 link retracted in September audit | Clinician/research watch only; do not adopt stimulation; useful local Paris expert/context lead |
| CTG-031 | [NCT06636994](https://clinicaltrials.gov/study/NCT06636994) | Active not recruiting | Fondation Rothschild Paris study mapping blindsight capability to functional MRI connectivity | Mechanism/local-clinician watch only; no treatment protocol impact |
| CTG-032 | [NCT07558395](https://clinicaltrials.gov/study/NCT07558395) | Not yet recruiting | INSERM multicenter VR audiovisual telerehab trial with Strasbourg site; pediatric tumor etiology not Eric stroke lane | AV/VR design inspiration only; not adult stroke or current Quest protocol evidence |
| CTG-033 | [NCT06121219](https://clinicaltrials.gov/study/NCT06121219) | Recruiting | Rochester/Huxlin recruiting home visual retraining after stroke; strong dose and staircase comparator | Home visual-training comparator only; not audiovisual and requires months-long dose |
| CTG-034 | [NCT05397873](https://clinicaltrials.gov/study/NCT05397873) | Recruiting | Toronto microperimetry biofeedback trial for fixation stability, retinal sensitivity, reading, and quality of life | Compensation/oculomotor comparator only; no restoration claim until results |
| CTG-035 | [NCT04424979](https://clinicaltrials.gov/study/NCT04424979) | Recruiting | Schepens prism configuration feasibility trial with primary completion expected 2026-08; linked to [PMID 38990239](https://pubmed.ncbi.nlm.nih.gov/38990239/) | Accommodation/mobility comparator only; separate from restoration and AV dose |

---

## 3. Evidence Quality Tiers

### Higher priority

- **NCT06116760 / PMID 39607286**: small but condition-matched RCT. Supports tDCS as an adjunct question, not a home protocol.
- **NCT04963075 / PMID 36604169**: Wake Forest AV multisensory line. Strong mechanistic fit for NegletFix but still small/high-variance.
- **NCT06341777 / PMID 39994637**: home AV telerehabilitation feasibility/comparator row. Supports home delivery, but published framing remains largely compensatory/oculomotor.
- **NCT07659691** and **NCT07635329**: closest active/newer AV/home-training watch rows, but no results yet.

### August 13 additions

- **NCT07752563**: new August pilot RCT of NeuroEyeCoach/VISIOcoach. Useful scanning/function comparator; not visual restoration or AV evidence.
- **PMID 42571232 / NCT07147660**: DRIVE-study protocol strengthens the driving/function measurement lane; no driving clearance implication.
- **NCT06121219**: Rochester home blind-field visual retraining trial is a strong home-dose/staircase comparator, but it is not audiovisual and may require months of training.

### Paris / France context

- **NCT04043689**: AP-HP/Pitie-Salpetriere chronic post-stroke hemianopia tACS. Clinician/research watch only; Raffin 2025 is a separate paper, not a verified trial result (September correction).
- **NCT06636994**: Fondation Rothschild blindsight/fMRI mechanism study. Local mechanism/context only.
- **NCT07558395**: INSERM-led Unity HMD audiovisual telerehab study for pediatric tumor hemianopsia. VR/AV design inspiration only.
- **LI-002**: LMC2 IRON social lead. Needs primary evidence before promotion.

### Comparator / design references

- **NCT03350919**, **NCT04230486**, **NCT04930822**, **NCT05397873**, and **NCT04424979**: useful visual training, digital scanning, biofeedback, and prism/accommodation comparators. Do not treat as direct proof of restoration.

### Watch-only neuromodulation

- **NCT07358832**: active AV+tDCS subacute stroke trial.
- **NCT05085210** and **NCT07105358**: tRNS plus visual/VR/perceptual learning rows, different modality.
- **NCT06875206**: focused ultrasound plus immersive VR. Clinician/research-only.

---

## 4. Protocol-Change Gate

Before any stimulation adjunct enters NegletFix:

1. Complete the open-loop AV baseline phase using field-map-guided targets.
2. Record at least one mid-course and one post-course assessment with the same instrument.
3. Document whether the behavior-only protocol has plateaued.
4. Review contraindications and montage with a clinician or legitimate research team.
5. Start stimulation as a separate phase with an explicit attribution question.

No device purchase or DIY stimulation protocol should be inferred from this page.

---

## 5. Institutional Watch Sources

These rows are high-quality source-discovery channels, not treatment evidence by themselves. A Mayo post or page can promote a lead only when the claim traces to a Mayo clinician/source page, PubMed, ClinicalTrials.gov, DOI/arXiv/OpenReview, guideline, or formal institutional protocol.

| ID | Source | Role | NegletFix Action |
|----|--------|------|------------------|
| INST-001 | [Mayo Clinic Neuro-Ophthalmology](https://www.mayoclinic.org/departments-centers/neuro-ophthalmology/overview/ovc-20524237) | Neuro-ophthalmic evaluation/team context; useful for parsing vision-loss posts and clinician terms | Watch official updates; promote only traceable clinician/PMID/NCT/official-page claims |
| INST-002 | [Mayo Clinic Physical Medicine and Rehabilitation](https://www.mayoclinic.org/departments-centers/physical-medicine-rehabilitation-mayo-clinic/sections/overview/ovc-20467039) | Brain/stroke rehabilitation and assistive/restorative technology context | Use as referral/source-discovery lane, not protocol proof |
| INST-003 | [Mayo Clinic Ophthalmology Clinical Trials](https://www.mayoclinic.org/departments-centers/ophthalmology/sections/clinical-trials/rsc-20518555) | Official trial-surface watch for visual-field, neuro-ophthalmology, and eye-disease studies | Add exact matched trials as NCT rows only when applicable |
| INST-004 | [Mayo Clinic News Network / Press stroke-vision posts](https://newsnetwork.mayoclinic.org/discussion/recognize-the-sudden-warning-signs-of-stroke-and-be-fast/) | Patient-facing education/news and named-clinician discovery | Keep as intake; trace any rehab or treatment claim before promotion |

---

## 6. Dashboard / Automation Direction

Recommended staged system:

1. **Now**: keep the September 25 CSV, wiki page, refresh log, and static HTML monitor as the source of truth.
2. **Next**: generate future monitors from the same CSV structure instead of manually copying trial rows.
3. **Then**: create a monthly Codex automation that checks only for meaningful changes:
   - trial status changes;
   - newly posted results, especially `CTG-012`, `CTG-015`, `CTG-016`, and `CTG-028..CTG-035`;
   - new linked PubMed publications;
   - new recruiting trials in AV+tDCS / visual restoration / VR visual training;
   - Mayo or Paris institutional source updates that resolve to a clinician, publication, trial, guideline, or formal program page.

Automation should produce a review note or proposed diff. It should not auto-change the protocol or make medical recommendations.
