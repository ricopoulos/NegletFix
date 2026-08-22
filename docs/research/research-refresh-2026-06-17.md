# NegletFix Research Refresh - 2026-06-17

**Run time**: 2026-06-17 23:26 EDT
**Scope**: ClinicalTrials.gov, PubMed, YouTube, X search discovery, LinkedIn/arXiv, OpenReview, GitHub watch repo checks, and Mayo Clinic institutional source intake.
**Rule**: intake is broad, but protocol claims require PubMed, NCT, DOI/arXiv/OpenReview, guidelines, institutional sources, or NegletFix measurement data.

## Bottom Line

- No tracked ClinicalTrials.gov status changed from the June 11 baseline.
- Eight new-to-monitor ClinicalTrials.gov rows were added: CTG-010 through CTG-017.
- Strict PubMed searches for 2026-06-11 through 2026-06-17 returned no new direct hits across the saved hemianopia/AV/tDCS/VR query set.
- A widened 2026 PubMed scan found five useful review/comparator/function papers, added as PM-001 through PM-005.
- YouTube search found five new discovery leads, added as YT-016 through YT-020.
- X search did not produce a traceable PMID/NCT/DOI/OpenReview lead worth adding.
- Mayo Clinic was added as a trusted institutional watch source, with rows INST-001 through INST-004 for neuro-ophthalmology, PM&R, ophthalmology trials, and official stroke-vision posts.
- LuMamba arXiv metadata resolves to v2 updated 2026-06-04; OpenReview pages still resolve; `ruvnet/ruv-neural` was active on GitHub at 2026-06-18T00:17Z, which is June 17 EDT.
- No finding changes the active NegletFix protocol.

## Run Log

| Lane | Check | Result | Action |
|------|-------|--------|--------|
| ClinicalTrials saved IDs | Rechecked CTG-001..CTG-009 by NCT ID | No status change | Updated last_checked to 2026-06-17 in new CSV |
| ClinicalTrials broad search | `hemianopia`, `homonymous hemianopia`, `visual field defect stroke rehabilitation` | Found 8 relevant new-to-monitor rows | Added CTG-010..CTG-017 |
| PubMed strict date window | 2026-06-11..2026-06-17 hemianopia/AV/tDCS/VR queries | 0 direct hits | Record empty strict window |
| PubMed 2026-to-date | Widened targeted queries | Found review/comparator/function candidates | Added PM-001..PM-005 |
| YouTube | Hemianopia/stroke/rehab search and metadata fetch | 5 new discovery leads | Added YT-016..YT-020 as leads/watch only |
| X | Saved-style link/PubMed/NCT searches | No traceable new lead | No new X row |
| Mayo Clinic | Official neuro-ophthalmology, PM&R, ophthalmology trials, and stroke-vision education/news pages | Trusted institutional source lane added | Added INST-001..INST-004; trace posts to clinician/PMID/NCT/DOI/guideline before promotion |
| LinkedIn/arXiv | LuMamba arXiv check | arXiv v2 updated 2026-06-04 | Keep LI-001 watch; no protocol change |
| OpenReview | Tracked NP pages | All tracked pages returned HTTP 200 | Keep NP-001..NP-009 watch |
| GitHub | `ruvnet/ruv-neural` API check | Repo active, 15 stars, 1 open issue, pushed 2026-06-18T00:17Z | Keep GH-001 as offline analysis inspiration only |

## Protocol Decision

Keep the active protocol unchanged: open-loop, field-map-guided Quest audiovisual training remains the core. New device, prism, scanning, driving, Mayo institutional, and EEG-AI leads are watchlist/comparator material only.
