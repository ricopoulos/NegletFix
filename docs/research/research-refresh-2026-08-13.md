# NegletFix Research Refresh - 2026-08-13

**Run time**: 2026-08-13 Europe/Paris
**Scope**: ClinicalTrials.gov, PubMed, arXiv, GitHub watch repo, Mayo/institutional web search, YouTube/X/LinkedIn discovery, and Paris/France local relevance.
**Rule**: intake is broad, but protocol claims require PubMed, NCT, DOI/arXiv/OpenReview, guidelines, institutional sources, or NegletFix measurement data.

## Bottom Line

- Yes, there are updates, but no protocol-changing result.
- The only true new-since-July registry row is `CTG-028` / `NCT07752563`: a Glasgow pilot RCT of computer-based eye-movement training after stroke, first posted on 2026-08-07.
- The strongest paper update is `PM-010` / `PMID42571232`: a DRIVE-study protocol paper for `CTG-015` / `NCT07147660`, entered PubMed on 2026-08-09.
- Existing `CTG-001..CTG-027` rows were rechecked. No tracked trial had a new status/result change after July 9.
- The broader pass found useful older rows missing from the watchlist: BITS touchscreen field-awareness results, AP-HP Paris chronic hemianopia tACS, Fondation Rothschild blindsight/fMRI, INSERM VR audiovisual telerehab, Rochester home blind-field retraining, Toronto microperimetry biofeedback, and Schepens prism feasibility.
- Paris matters now: `CTG-030`, `CTG-031`, `CTG-032`, and `LI-002` give us a local French/Paris research-context lane while the Quest is unavailable.
- Neuro-JEPA updated to arXiv v3 on 2026-07-19; still MRI-side analytics only. LuMamba remains v2. `ruvnet/ruv-neural` is active with an August commit and a public description that now emphasizes research-grade closed-loop/gamma-entrainment tooling; still no NegletFix protocol impact.
- Mayo official search did not produce a new Mayo hemianopia/stroke-rehab protocol or clinical trial. The Mayo Connect July discussion is patient/community signal only.

## Run Log

| Lane | Check | Result | Action |
|------|-------|--------|--------|
| ClinicalTrials saved IDs | Rechecked `CTG-001..CTG-027` by NCT ID | No status or posted-result changes after 2026-07-09 | Updated `last_checked` in dated CSV |
| ClinicalTrials broad search | Hemianopia, stroke visual-field rehab, occipital stroke restoration, AV/VR/tDCS/neurofeedback/scanning terms | One true August row plus several older relevant gaps | Added `CTG-028..CTG-035` |
| PubMed strict date window | 2026-07-09..2026-08-13 hemianopia/stroke rehab queries | No direct new AV/tDCS/VR-restoration outcome paper | Recorded no protocol-changing paper |
| PubMed targeted follow-up | `VISIOcoach`, NeuroEyeCoach, driving/function rows | `PMID42571232` maps to `NCT07147660`; NeuroEyeCoach older papers support compensation context | Added `PM-010`; keep NeuroEyeCoach as scanning/function lane |
| Paris/France | ClinicalTrials + web search for local leads | AP-HP tACS, Rothschild blindsight/fMRI, INSERM VR AV telerehab, and LMC2 IRON social lead | Added local context rows/watch leads |
| Mayo Clinic | Official Mayo and Mayo Connect searches | No new official protocol/trial; Mayo low-vision page remains service/context, Connect is community-only | No protocol row added |
| YouTube/X/LinkedIn | Web discovery | Most results were education, generic stroke, or social posts; LMC2 IRON was the only local lead worth parking | Added `LI-002` as lead-needs-primary |
| arXiv | LuMamba and Neuro-JEPA metadata | Neuro-JEPA is now v3, updated 2026-07-19; LuMamba remains v2 | Keep MRI/EEG analytics watch lanes only |
| GitHub | `ruvnet/ruv-neural` API | Repo active, 28 stars, latest commit 2026-08-04, pushed 2026-08-09, no protocol evidence | Keep offline tooling watch only |

## Protocol Decision

Keep the active protocol unchanged: open-loop, field-map-guided Quest audiovisual training remains the core. The current shipping delay is fine. There is no research reason to rush into a new device, stimulation adjunct, or clinical claim.

The next Quest-side work remains the pre-shipping plan:

1. Retune the next dose build around `-5 deg` plus harder boundary targets such as `-8 deg`.
2. Add catch/probe trials to separate audio-guided prediction from true visual confirmation.
3. Cap/redesign the staircase so high hit rate at an easy/cued location cannot inflate LogCS.
4. Keep functional/scanning measures separate from restoration claims.

## Sources Added

- `CTG-028` through `CTG-035` in `docs/research/clinical-trials-watchlist-2026-08-13.csv`.
- `PM-010` and `LI-002` in `docs/research/source-queue-2026-05-25.csv`.
- August monitor HTML: `docs/research/research-monitor-2026-08-13.html`.
