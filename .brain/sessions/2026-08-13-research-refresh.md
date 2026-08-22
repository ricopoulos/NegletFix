# Session: August 13 Research Refresh

**Date**: 2026-08-13
**Status**: Complete

## Objectives
- Refresh the NegletFix research monitor after the July 9 pass.
- Check for new clinical trials, papers, AI/tooling watch leads, and institutional signals relevant to post-stroke hemianopia and field-guided audiovisual rehabilitation.
- Account for the Paris transition and the temporary delay before the Quest setup arrives by maritime shipment.

## Outcomes
- Rechecked all existing CTG-001 through CTG-027 ClinicalTrials.gov watchlist rows; no tracked trial had a meaningful status or results change after the July 9 refresh.
- Added one true new-since-July registry lead:
  - CTG-028 / NCT07752563: Glasgow Caledonian University pilot RCT of computer-based eye movement training for stroke-related homonymous visual field loss.
- Added one new PubMed lead:
  - PM-010 / PMID42571232: DRIVE-study RCT protocol, linked to NCT07147660, using novel VR functional-vision evaluation and delayed-start home-based rehabilitation.
- Added gap-fill registry rows CTG-029 through CTG-035 for BITS field awareness, Paris AP-HP tACS, Fondation Rothschild blindsight/fMRI, INSERM European pediatric 3D-MOT AV telerehab, Rochester/Huxlin home blind-field training, Toronto MAIA biofeedback, and Schepens/Mass Eye prism feasibility.
- Parked a Paris LinkedIn discovery lead for LMC2 / Universite Paris Cite IRON as LI-002 because no primary NCT, PMID, DOI, or protocol page was found.
- Confirmed Neuro-JEPA and ruv-neural remain watch-only technical leads with no current protocol impact.
- Confirmed no protocol change for NegletFix: keep the active Quest plan focused on field-guided AV training near the measured border, with catch/probe trials and separate function/scanning metrics.

## Files Modified
- `.brain/index.json`
- `.brain/wiki/index.md`
- `.brain/wiki/clinical-trials-watchlist.md`
- `.brain/wiki/research-papers-index.md`
- `docs/research/source-queue-2026-05-25.csv`
- `docs/research/clinical-trials-watchlist-2026-08-13.csv`
- `docs/research/clinical-trials-watchlist-2026-08-13.md`
- `docs/research/research-refresh-2026-08-13.md`
- `docs/research/research-monitor-2026-08-13.html`
- `.brain/sessions/2026-08-13-research-refresh.md`

## Validation
- `.brain/index.json` parses successfully.
- `docs/research/source-queue-2026-05-25.csv`: 85 rows, no duplicate source IDs.
- `docs/research/clinical-trials-watchlist-2026-08-13.csv`: 35 rows, no duplicate trial IDs.
- `xmllint --html --noout docs/research/research-monitor-2026-08-13.html` exited successfully; it emitted expected HTML5 tag warnings from the legacy HTML parser.

## Remote Drift Check
- Skipped: no remote server configured for NegletFix.

## Branch Status
- Current branch: `main`.
- No unmerged or unpushed Codex/feature branches were found during the branch audit.

## Dirty Tree Status
- Session-owned research refresh files remain uncommitted pending user wrap-up or explicit commit approval.
- Pre-existing/user-owned WIP remains present from earlier June/July research sessions and Unity smoke artifacts; these were not staged, reverted, or cleaned.

## Wiki / Relationships
- Updated clinical-trials and research-paper wiki pages with the August 13 findings.
- Updated wiki index pointers to the August 13 monitor artifacts.
- No relationship update.

## Next Steps
- When the Quest setup arrives in Paris, resume the headset validation path rather than changing protocol from research alone.
- Retune field-guided AV training around the measured border, with practical probes around `-5 deg` and harder boundary targets around `-8 deg`.
- Track CTG-028, DRIVE/NCT07147660, Paris AP-HP tACS, Fondation Rothschild, and INSERM 3D-MOT AV as watchlist leads for the next research refresh.
