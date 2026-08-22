# Session: July 9 Research Refresh

**Date**: 2026-07-09
**Status**: Complete

## Objectives

- Run the delayed research update pass across ClinicalTrials.gov, PubMed, YouTube, X/LinkedIn discovery, Mayo institutional search, arXiv, OpenReview, and GitHub watch lanes.
- Update the NegletFix research monitor if live sources produced meaningful changes.
- Preserve the protocol boundary: new leads can update the watchlist, but only strong clinical evidence plus measurement context can change the active Quest AV protocol.

## Outcomes

- Rechecked `CTG-001` through `CTG-017` against ClinicalTrials.gov.
- Found the key same-day update: `CTG-012` / `NCT06136169` had registry results posted/updated on 2026-07-09 and linked `PMID42140549`.
- Extracted `CTG-012` functional scanning outcomes:
  - early large blind-side head scans: 18% to 84%;
  - blind-side hazard detection: 56% to 86%;
  - head scans per intersection: 2.2 to 3.5;
  - scan magnitude: 23.9 to 27.5 degrees;
  - response time: 2.7s to 1.9s.
- Extracted `CTG-016` / `NCT04827147` prism-comparator publication link, `PMID42274646`.
- Added PubMed function/comparator rows `PM-006` through `PM-009`.
- Added ClinicalTrials.gov rows `CTG-018` through `CTG-027`.
- Rechecked arXiv/OpenReview/GitHub watch lanes:
  - LuMamba remains arXiv v2 updated 2026-06-04.
  - Neuro-JEPA remains arXiv v2 updated 2026-06-18.
  - OpenReview EEG-FM critical review page still resolves.
  - `ruvnet/ruv-neural` remains active; no release found, latest visible default-branch commit was 2026-06-29.
- YouTube and X/LinkedIn discovery did not produce a stronger traceable lead than the NCT/PubMed updates.
- Mayo search did not produce a new Mayo-specific hemianopia/stroke-rehab protocol or trial.
- Protocol verdict: no change. Open-loop, field-map-guided Quest audiovisual training remains the active route.

## Files Modified

- `.brain/index.json`
- `.brain/wiki/clinical-trials-watchlist.md`
- `.brain/wiki/index.md`
- `.brain/wiki/research-papers-index.md`
- `docs/research/source-queue-2026-05-25.csv`
- `docs/research/clinical-trials-watchlist-2026-07-09.csv`
- `docs/research/clinical-trials-watchlist-2026-07-09.md`
- `docs/research/research-refresh-2026-07-09.md`
- `docs/research/research-monitor-2026-07-09.html`
- `.brain/sessions/2026-07-09-research-refresh.md`

## Remote Drift Check

- Skipped: no remote server configured for NegletFix.

## Branch Status

- Current branch: `main`.
- `origin/main...HEAD`: even at session start.
- Pull was skipped because the worktree already had pre-existing research/session dirty files from the June 17 and June 21 work.

## Dirty Tree Status

- Session-owned July 9 research files are uncommitted at the time of this summary.
- Pre-existing/session-owned research files from June 17 and June 21 were already dirty before this pass and were left in place.
- Generated/unrelated local artifacts remain untouched: `.codex/`, `SmokeResults/`, Unity smoke/log/screenshot artifacts, Compass artifacts, `docs/Eric Files/`, prior YouTube HTML, and report assets.

## Wiki / Relationships

- Wiki impact:
  - `clinical-trials-watchlist.md` now points to July 9 source files and includes `CTG-018..CTG-027`.
  - `research-papers-index.md` now maps `PM-006..PM-009`, `PMID39994637`, `PMID42140549`, and `PMID42274646`.
  - `index.md` now reports 75 source rows and 27 ClinicalTrials.gov rows.
- Relationship update: none.

## Next Steps

- Keep `CTG-012` as functional compensation/scanning evidence only; do not use it as visual restoration or driving-clearance evidence.
- Track `CTG-018` / `NCT07659691`, `CTG-019` / `NCT07635329`, `CTG-020` / `NCT06875206`, and `CTG-023` / `NCT06341777` in the next monthly refresh.
- Consider building a real CSV-to-HTML monitor generator before creating recurring automation.
