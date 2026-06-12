# NegletFix Clinical Trials Watchlist

**Date**: 2026-06-11  
**Scope**: tDCS, audiovisual/multisensory training, visual restoration/restitution, and VR-based visual-field rehabilitation for homonymous hemianopia / visual-field defects after stroke or acquired brain injury.  
**Source check**: PubMed E-utilities and ClinicalTrials.gov API, live checked during this session.

## Working Conclusion

The research monitor now has one exact-match tDCS plus audiovisual trial to track as the main adjunct lead:

- [NCT06116760](https://clinicaltrials.gov/study/NCT06116760) / [PMID 39607286](https://pubmed.ncbi.nlm.nih.gov/39607286/) — completed chronic HVFD randomized clinical trial of audiovisual training plus sham/parietal/occipital anodal tDCS.

This does **not** change the active NegletFix protocol. It strengthens a clinician-supervised adjunct question: occipital anodal tDCS may be worth discussing only after a clean no-tDCS behavioral baseline and plateau.

## Trial Dashboard

| ID | Trial | Status | Evidence Role | Protocol Impact |
|---|---|---|---|---|
| CTG-001 | [NCT06116760](https://clinicaltrials.gov/study/NCT06116760) | Completed | Exact chronic HVFD AV+tDCS lead; linked to [PMID 39607286](https://pubmed.ncbi.nlm.nih.gov/39607286/) | Clinician-supervised adjunct candidate only |
| CTG-002 | [NCT04963075](https://clinicaltrials.gov/study/NCT04963075) | Completed with posted results | Wake Forest adult chronic AV multisensory line; linked to [PMID 36604169](https://pubmed.ncbi.nlm.nih.gov/36604169/) | Supports current AV core, but small/high-variance |
| CTG-003 | [NCT05894434](https://clinicaltrials.gov/study/NCT05894434) | Not yet recruiting | Wake Forest next-stage AV study | Watch for recruitment/protocol details |
| CTG-004 | [NCT07358832](https://clinicaltrials.gov/study/NCT07358832) | Recruiting | Active subacute stroke AV+tDCS trial | Important future signal, not Eric's chronic window |
| CTG-005 | [NCT02935413](https://clinicaltrials.gov/study/NCT02935413) | Completed | Subacute tDCS+VRT pilot; linked to [PMID 28082176](https://pubmed.ncbi.nlm.nih.gov/28082176/) | Park as early-window adjunct evidence |
| CTG-006 | [NCT03350919](https://clinicaltrials.gov/study/NCT03350919) | Completed with posted results | Blind-field training comparator with Humphrey PMD results | Supports measurement-first visual training |
| CTG-007 | [NCT04230486](https://clinicaltrials.gov/study/NCT04230486) | Completed with posted results | VR cross-modal visual-auditory rehabilitation comparator | Supports Quest/cross-modal direction |
| CTG-008 | [NCT05085210](https://clinicaltrials.gov/study/NCT05085210) | Recruiting | tRNS plus computer/VR visual training | Watch only; different stimulation modality |
| CTG-009 | [NCT04798924](https://clinicaltrials.gov/study/NCT04798924) | Active not recruiting | Occipital-stroke visual restoration timing/plasticity study | Watch for chronic-vs-subacute expectation refinement |

Structured source: `docs/research/clinical-trials-watchlist-2026-06-11.csv`.

## Evidence Threshold For Protocol Change

Do not change NegletFix's active protocol from this watchlist alone.

Minimum threshold before adding any stimulation adjunct:

1. Eric completes a clean open-loop AV baseline block with field-map-guided targets.
2. At least one mid-course and one post-course assessment exist in the same measurement instrument.
3. A clinician confirms tDCS/tACS/tRNS safety, montage, contraindications, and supervision.
4. The adjunct is introduced as a separate phase, not mixed into the first behavioral training phase.
5. The outcome metric and attribution question are written down before the adjunct starts.

## Dashboard / Automation Recommendation

Yes, this should become more real, but in stages.

Phase 1 is now in place: a data-first CSV watchlist plus a wiki page. This is enough for manual monthly review and avoids building a dashboard before the fields are stable.

Phase 2 should be a generated HTML dashboard from the CSV, similar to the existing research sprint reports. Recommended panels:

- changed trial statuses;
- new PubMed papers for tracked authors/trials;
- protocol-impact lane: core / adjunct / watch-only / parked;
- clinician questions generated since last review;
- active NegletFix measurement status beside external evidence.

Phase 3 can be a monthly Codex automation that refreshes PubMed and ClinicalTrials.gov, updates a proposed diff, and reports only meaningful changes. It should not auto-edit the protocol or create medical recommendations.

## Search Strings To Reuse

PubMed:

```text
(homonymous hemianopia OR homonymous hemianopsia OR visual field defect) AND (tDCS OR transcranial direct current stimulation) AND (audiovisual OR audio-visual OR multisensory OR rehabilitation OR visual restoration)
```

```text
(homonymous hemianopia OR homonymous hemianopsia) AND (audiovisual OR audio-visual OR multisensory) AND (stroke OR chronic OR rehabilitation)
```

ClinicalTrials.gov:

```text
condition: homonymous hemianopia
intervention: tDCS OR transcranial direct current stimulation OR audiovisual OR audio-visual OR multisensory OR visual restoration OR visual restitution OR visual perceptual learning
```
