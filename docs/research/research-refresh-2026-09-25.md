# Research refresh — 2026-09-25

**Window:** 2026-08-14 through 2026-09-25. This is a source-graded watch update, not a medical recommendation. The structured snapshot is `clinical-trials-watchlist-2026-09-25.csv`; the patient-facing dashboard is `research-monitor-2026-09-25.html`.

## Decision

No new result justifies changing the current Quest audiovisual protocol. The next product work awaits the Quest's return and a measured reprise: retune the `-5°` target with a harder `-8°` probe, add catch/probe trials and a bounded staircase, and keep assessment separate from training. Recruitment and publication announcements are not efficacy data. Do not introduce home tDCS/tACS/tRNS, focused ultrasound, prisms, or medications into the active protocol.

## ClinicalTrials.gov audit

All 35 previously tracked NCT IDs were rechecked against the [ClinicalTrials.gov API](https://clinicaltrials.gov/data-api/api). A broader hemianopia search found one first-posted record since August 13.

| ID | Change since 2026-08-13 | Interpretation |
|---|---|---|
| `CTG-012` / [NCT06136169](https://clinicaltrials.gov/study/NCT06136169) | `ACTIVE_NOT_RECRUITING` → `COMPLETED`; registry updated September 18; results remain posted | Scanning/cue compensation, not restorative AV dose or driving clearance |
| `CTG-016` / [NCT04827147](https://clinicaltrials.gov/study/NCT04827147) | `ACTIVE_NOT_RECRUITING` → `COMPLETED`; registry updated August 19; results remain posted | Prism/accommodation comparator |
| `CTG-036` / [NCT07830745](https://clinicaltrials.gov/study/NCT07830745) | New record, first posted September 21; `NOT_YET_RECRUITING` | Clinique Les Trois Soleils, Boissise-le-Roi. Randomized crossover of 40-minute optokinetic sessions with moving letters versus scrolling-sentence reading for hemianopic alexia after stroke; reading/function comparator only |

`CTG-003`, `013`, `014`, `018`, `019`, `020`, `024`, `025`, `032`, and `033` had registry updates after August 13 without a status change. No other tracked NCT changed status or result-posting state. See the CSV for all 36 records and their protocol-impact classifications.

## Pitié-Salpêtrière and Paris

[AP-HP's HEMIANOTACS page](https://www.aphp.fr/registre-des-essais-cliniques/stimulation-transcranienne-par-courant-electrique-alternatif-tacs) labels follow-up *Suivi terminé*. [NCT04043689](https://clinicaltrials.gov/study/NCT04043689) is completed (primary completion April 3, 2025; last registry update May 8, 2025) and has no posted registry results. It is **not a currently recruiting option**. The study and its team are still relevant context for a [Pitié-Salpêtrière neuro-ophthalmology consultation](https://pitiesalpetriere.aphp.fr/consultation/83183/). A clinician can assess the visual-field and stroke context and identify suitable current studies; no eligibility is inferred here.

**Correction to the August snapshot:** [Raffin et al. 2025, PMID 41243213](https://pubmed.ncbi.nlm.nih.gov/41243213/) is a separate chronic-hemianopia cf-tACS paper. The official AP-HP/ClinicalTrials.gov sources do not establish it as a publication of HEMIANOTACS. Its linkage to `CTG-030` in the August CSV/wiki was too strong. The September snapshot and source queue remove that link; the dated August artifacts remain historical records with this explicit erratum.

[Fondation Rothschild NCT06636994](https://clinicaltrials.gov/study/NCT06636994) remains active but not recruiting, an observational blindsight/fMRI mechanism study. `LI-002` (LMC2/IRON) remains a social discovery lead; no PMID, NCT, DOI, or formal primary results page was identified in this pass. Neither is an intervention recommendation.

## Literature and methods

- `PM-011`: [Interventions for Visual Field Loss After Acquired Brain Injury: A Systematic Review, PMID 42652963](https://pubmed.ncbi.nlm.nih.gov/42652963/). Published July 31, 2026, but surfaced in a PubMed entry-date search for this window. Across 28 studies, compensation has the more consistent functional signal; evidence for expanding the visual field is more variable. Do not count this as a post-August publication.
- `PM-012`: [Complementary Visual Assessment: Validation of a High-Precision Eye-Tracking Kinetic Perimetry..., PMID 42684868](https://pubmed.ncbi.nlm.nih.gov/42684868/). Published September 2. Method validation in 37 participants, including 17 retrochiasmal lesions. Its suggested `>5°` meaningful boundary change belongs to that device and method; it is **not** a Quest threshold.
- [Neuro-JEPA arXiv:2606.14957](https://arxiv.org/abs/2606.14957v5) advanced from v3 to v5 on September 15. This remains an MRI foundation-model methods lead, not validated NegletFix rehabilitation software. LuMamba remained at v2; `ruv-neural` remains an analysis-side repository lead.

## Other source lanes

YouTube, X, LinkedIn, Mayo Clinic institutional pages, arXiv, GitHub, PubMed, and Paris institution/trial pages were checked for traceable new claims. No primary-source item found in those lanes changed the protocol. Social announcements remain intake until linked to a formal paper, registry, or institutional protocol. The PubMed search used entry date 2026-08-14..2026-09-25, so publication dates can predate this interval.

## Next watch

Check HEMIANOTACS for a posted result/publication, `CTG-036` recruitment and eligibility, the active AV lines (`CTG-003`, `004`, `018`, `023`) for results, and the two newly completed compensation trials for full outcome interpretation. A local neuro-ophthalmology consultation is reasonable separate from NegletFix research; bring the actual MRI reports, visual-field/perimetry documents, and Quest measurement summary for clinician review.
