# Clinical Trials Watchlist Refresh - 2026-08-13

This is the August 13 refresh of the NegletFix registry-backed watchlist. It is a research operations artifact, not medical advice.

## Bottom Line

- The meaningful new-since-July registry update is `CTG-028` / [NCT07752563](https://clinicaltrials.gov/study/NCT07752563), first posted 2026-08-07: a Glasgow pilot RCT of computer-based eye-movement training after stroke using NeuroEyeCoach/VISIOcoach plus usual care versus usual care.
- Existing `CTG-001` through `CTG-027` rows were rechecked against ClinicalTrials.gov on 2026-08-13. No tracked row had a new status or posted-results change after the July 9 refresh.
- `CTG-015` / [NCT07147660](https://clinicaltrials.gov/study/NCT07147660) gained a new PubMed protocol paper, [PMID 42571232](https://pubmed.ncbi.nlm.nih.gov/42571232/), for the DRIVE-study: an open delayed-start RCT of eight-week home-based visual rehabilitation with a novel VR functional-vision test in stroke survivors affected by loss of driving privileges.
- Several older but newly surfaced rows were added because they sharpen useful lanes: BITS touchscreen field-awareness results (`CTG-029`), Paris AP-HP chronic hemianopia tACS (`CTG-030`), Fondation Rothschild blindsight/fMRI (`CTG-031`), INSERM-led VR audiovisual telerehab (`CTG-032`), Rochester home blind-field retraining (`CTG-033`), Toronto microperimetry biofeedback (`CTG-034`), and Schepens prism feasibility (`CTG-035`).
- Neuro-JEPA moved from arXiv v2 to v3 on 2026-07-19. `ruvnet/ruv-neural` is active with August commits and a shifted public description around closed-loop/gamma-entrainment research. Both remain future analytics/tooling watch items only.
- No August finding changes the active NegletFix protocol. The practical stance remains: use the shipping delay to refine the next Quest build and keep research in watch/comparator lanes.

## New Rows Added

| ID | NCT | Status | Evidence Role | Protocol impact |
|----|-----|--------|---------------|-----------------|
| CTG-028 | [NCT07752563](https://clinicaltrials.gov/study/NCT07752563) | Not yet recruiting | New August 7 pilot RCT of computer-based scanning/eye-movement training after stroke | Function/scanning comparator only; no visual restoration or AV protocol implication |
| CTG-029 | [NCT04930822](https://clinicaltrials.gov/study/NCT04930822) | Completed with results posted | New-to-monitor posted-results digital scanning comparator using Bells Test and kinetic field metrics | Inpatient acute/subacute field-awareness comparator only; not chronic restoration evidence |
| CTG-030 | [NCT04043689](https://clinicaltrials.gov/study/NCT04043689) | Completed | Paris AP-HP chronic HH neuromodulation study aligned with Raffin 2025 cf-tACS paper | Clinician/research watch only; do not adopt stimulation; useful local Paris expert/context lead |
| CTG-031 | [NCT06636994](https://clinicaltrials.gov/study/NCT06636994) | Active not recruiting | Fondation Rothschild Paris study mapping blindsight capability to functional MRI connectivity | Mechanism/local-clinician watch only; no treatment protocol impact |
| CTG-032 | [NCT07558395](https://clinicaltrials.gov/study/NCT07558395) | Not yet recruiting | INSERM multicenter VR audiovisual telerehab trial with Strasbourg site; pediatric tumor etiology not Eric stroke lane | AV/VR design inspiration only; not adult stroke or current Quest protocol evidence |
| CTG-033 | [NCT06121219](https://clinicaltrials.gov/study/NCT06121219) | Recruiting | Rochester/Huxlin recruiting home visual retraining after stroke; strong dose and staircase comparator | Home visual-training comparator only; not audiovisual and requires months-long dose |
| CTG-034 | [NCT05397873](https://clinicaltrials.gov/study/NCT05397873) | Recruiting | Toronto microperimetry biofeedback trial for fixation stability, retinal sensitivity, reading, and quality of life | Compensation/oculomotor comparator only; no restoration claim until results |
| CTG-035 | [NCT04424979](https://clinicaltrials.gov/study/NCT04424979) | Recruiting | Schepens prism configuration feasibility trial with primary completion expected 2026-08 | Accommodation/mobility comparator only; separate from restoration and AV dose |

## PubMed / Paper Updates

| ID | Source | Extract | Interpretation |
|----|--------|---------|----------------|
| PM-010 | [PMID 42571232](https://pubmed.ncbi.nlm.nih.gov/42571232/) / [NCT07147660](https://clinicaltrials.gov/study/NCT07147660) | DRIVE-study protocol paper entered PubMed on 2026-08-09. It describes a delayed-start RCT with 52 stroke survivors, eight-week home-based vision rehabilitation, functional vision outcomes, and a novel VR vision test. | Strengthens the driving/function measurement lane. It is not driving clearance and not restorative AV evidence. |
| PM window | PubMed date-window checks 2026-07-09 through 2026-08-13 | Strict hemianopia/stroke rehabilitation queries found no direct new AV, tDCS, VR-restoration, or neurofeedback outcome paper. BCI/prosthetic and retinal items were excluded from protocol impact. | No protocol change. |

## Paris / France Context

Eric is back in Paris while the Quest setup is in maritime shipment. The local relevance changed even though the protocol did not:

- `CTG-030` / [NCT04043689](https://clinicaltrials.gov/study/NCT04043689): AP-HP / Pitie-Salpetriere chronic post-stroke hemianopia tACS trial, completed. Treat as clinician/research context only.
- `CTG-031` / [NCT06636994](https://clinicaltrials.gov/study/NCT06636994): Fondation Rothschild active-not-recruiting blindsight/fMRI mechanism study after vascular retrochiasmatic lesions.
- `CTG-032` / [NCT07558395](https://clinicaltrials.gov/study/NCT07558395): INSERM-led European VR audiovisual telerehab trial using Unity HMD 3D-MOT with spatial sound; pediatric brain tumor etiology, so design inspiration only.
- `LI-002`: LMC2 / Universite Paris Cite social lead for IRON, an oculomotor and neurovisual rehabilitation tool for homonymous hemianopia. No primary PMID/NCT/DOI was found in this pass, so it stays a discovery lead.

## Protocol Decision

Keep the active protocol unchanged: open-loop, field-map-guided Quest audiovisual training remains the core. New eye-movement, driving/function, BITS, prism, tACS, blindsight/fMRI, and home visual-retraining rows are watchlist or comparator material only.

When the Quest arrives in Paris, the next practical build should still focus on the already planned retuning: mix the easy `-5 deg` target with harder boundary targets such as `-8 deg`, add catch/probe trials, and cap/redesign the staircase so audio-guided prediction cannot masquerade as visual recovery.

## Source

- Live ClinicalTrials.gov v2 endpoint checks on 2026-08-13.
- PubMed E-utilities checks on 2026-08-13.
- arXiv API check for Neuro-JEPA and LuMamba on 2026-08-13.
- GitHub API check for `ruvnet/ruv-neural` on 2026-08-13.
- Web search for Mayo, YouTube, X/LinkedIn, and Paris institutional/social leads on 2026-08-13.
- Structured CSV: `docs/research/clinical-trials-watchlist-2026-08-13.csv`.
- Unified source queue: `docs/research/source-queue-2026-05-25.csv`.
