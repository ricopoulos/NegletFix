---
title: Clinical Trials Watchlist
last_updated: 2026-06-11
confidence: HIGH registry status / MIXED protocol impact
sources:
  - PubMed E-utilities live check 2026-06-11
  - ClinicalTrials.gov API live check 2026-06-11
  - docs/research/clinical-trials-watchlist-2026-06-11.csv
  - docs/research/clinical-trials-watchlist-2026-06-11.md
  - docs/research/research-monitor-2026-06-11.html
---

# Clinical Trials Watchlist

Registry-backed monitor for external evidence that could affect NegletFix. This page tracks clinical trials and active evidence leads; it is not a treatment recommendation and should not override the measurement-first Quest audiovisual protocol.

Structured source: `docs/research/clinical-trials-watchlist-2026-06-11.csv`.

Human-readable research note: `docs/research/clinical-trials-watchlist-2026-06-11.md`.

Static HTML monitor: `docs/research/research-monitor-2026-06-11.html`.

Unified intake queue: `docs/research/source-queue-2026-05-25.csv` now includes the YouTube leads, X discovery queries, and CTG-001 through CTG-009.

---

## 1. Current Verdict

The best exact-match adjunct lead is:

- [NCT06116760](https://clinicaltrials.gov/study/NCT06116760) / [PMID 39607286](https://pubmed.ncbi.nlm.nih.gov/39607286/) — completed randomized clinical trial of audiovisual training plus anodal/sham tDCS in chronic homonymous visual-field defects.

Protocol impact:

- **Do not add home tDCS now.**
- Keep NegletFix on open-loop, field-map-guided Quest audiovisual training until there is a clean behavioral baseline, plateau evidence, and clinician review.
- Treat occipital anodal tDCS as a doctor/research question after baseline: "Would supervised ipsilesional occipital tDCS during AV training be appropriate, and under what montage/safety constraints?"

---

## 2. Trial Dashboard

| ID | Trial | Status on 2026-06-11 | Evidence Role | NegletFix Action |
|----|-------|----------------------|---------------|------------------|
| CTG-001 | [NCT06116760](https://clinicaltrials.gov/study/NCT06116760) | Completed | Exact chronic HVFD AV+tDCS lead; linked to [PMID 39607286](https://pubmed.ncbi.nlm.nih.gov/39607286/) | Track as clinician-supervised adjunct candidate only |
| CTG-002 | [NCT04963075](https://clinicaltrials.gov/study/NCT04963075) | Completed with posted results | Wake Forest adult chronic AV multisensory line; linked to [PMID 36604169](https://pubmed.ncbi.nlm.nih.gov/36604169/) | Core AV evidence, but keep n/sample caveat |
| CTG-003 | [NCT05894434](https://clinicaltrials.gov/study/NCT05894434) | Not yet recruiting | Wake Forest multisensory versus unisensory follow-up | Monthly status check |
| CTG-004 | [NCT07358832](https://clinicaltrials.gov/study/NCT07358832) | Recruiting | Active subacute stroke AV+tDCS trial | Watch for results; not Eric's chronic window |
| CTG-005 | [NCT02935413](https://clinicaltrials.gov/study/NCT02935413) | Completed | Subacute PCA stroke tDCS+VRT pilot; linked to [PMID 28082176](https://pubmed.ncbi.nlm.nih.gov/28082176/) | Park as early-window adjunct evidence |
| CTG-006 | [NCT03350919](https://clinicaltrials.gov/study/NCT03350919) | Completed with posted results | Blind-field visual training comparator | Use as measurement/design reference, not AV evidence |
| CTG-007 | [NCT04230486](https://clinicaltrials.gov/study/NCT04230486) | Completed with posted results | VR cross-modal visual-auditory comparator | Extract effect sizes before strong claims |
| CTG-008 | [NCT05085210](https://clinicaltrials.gov/study/NCT05085210) | Recruiting | tRNS plus computer/VR visual training | Watch only; different stimulation modality |
| CTG-009 | [NCT04798924](https://clinicaltrials.gov/study/NCT04798924) | Active not recruiting | Occipital-stroke visual restoration timing/plasticity | Watch for chronic-vs-subacute expectation updates |

---

## 3. Evidence Quality Tiers

### Higher priority

- **NCT06116760 / PMID 39607286**: small but condition-matched RCT. Supports tDCS as an adjunct question, not a home protocol.
- **NCT04963075 / PMID 36604169**: Wake Forest AV multisensory line. Strong mechanistic fit for NegletFix but still small/high-variance.
- **Alharshan/Alwashmi 2026 / PMID 41421499**: not an NCT row in this watchlist, but remains the closest adult-stroke immersive AV dose/mechanism anchor.

### Comparator / design references

- **NCT03350919**: blind-field training can improve Humphrey PMD versus intact-field control; useful for measurement discipline and dose expectations.
- **NCT04230486**: VR cross-modal rehab comparator; useful but should be quantitatively extracted before being used in doctor-facing claims.

### Watch-only neuromodulation

- **NCT07358832**: active AV+tDCS subacute stroke trial.
- **NCT05085210**: tRNS plus visual/VR training, different modality.
- **Raffin 2025 / PMID 41243213**: cf-tACS evidence outside the tDCS lane; lab-level watchlist only.

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

## 5. Dashboard / Automation Direction

Recommended staged system:

1. **Now**: keep the CSV, wiki page, and static HTML monitor as the source of truth.
2. **Next**: generate future monitors from the same CSV structure instead of manually copying trial rows.
3. **Then**: create a monthly Codex automation that checks only for meaningful changes:
   - trial status changes;
   - newly posted results;
   - new linked PubMed publications;
   - new recruiting trials in AV+tDCS / visual restoration / VR visual training.

Automation should produce a review note or proposed diff. It should not auto-change the protocol or make medical recommendations.
