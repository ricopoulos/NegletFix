# Session: June 17 Research Refresh Pass

**Date**: 2026-06-17
**Status**: Complete

## Objectives

- Run a fresh research monitor pass across ClinicalTrials.gov, PubMed, YouTube, X, LinkedIn/arXiv, OpenReview, and GitHub/tooling leads.
- Keep broad intake separate from evidence that can influence the NegletFix protocol.
- Update the dashboard and source files so search runs are monitorable, not just discussed in chat.

## Outcomes

- Rechecked the original nine ClinicalTrials.gov rows (`CTG-001..CTG-009`) through live registry endpoints; no status changed from the June 11 baseline.
- Added eight new ClinicalTrials.gov rows (`CTG-010..CTG-017`) for post-stroke audiovisual stimulation, PAVE VR-HMD field enhancement, scanning training, occipital-stroke mechanism work, VR navigation/driving, prism compensation, and VIVID Brain digital training.
- Ran strict PubMed checks for 2026-06-11 through 2026-06-17 across hemianopia/AV/tDCS/VR queries; no direct new hits.
- Widened PubMed to 2026-to-date and added five relevant review/comparator/function rows (`PM-001..PM-005`).
- Added five YouTube intake rows (`YT-016..YT-020`) after metadata checks with `yt-dlp`; all remain discovery-only.
- X searches produced no traceable PMID/NCT/DOI/OpenReview lead worth adding.
- Added Mayo Clinic as a trusted institutional watch source (`INST-001..INST-004`) after Eric asked how to handle Mayo posts. The rows cover neuro-ophthalmology, PM&R, ophthalmology trials, and official stroke-vision education/news posts; promotion still requires a traceable clinician/source page, PMID, NCT, DOI/guideline, or formal institutional protocol.
- Rechecked LuMamba arXiv metadata, tracked OpenReview pages, and `ruvnet/ruv-neural`; all remain future analytics/tooling watch leads only.
- Protocol verdict unchanged: open-loop, field-map-guided Quest audiovisual training remains the active route; stimulation, device, prism, scanning, driving, and EEG-AI leads stay in watch/comparator lanes.

## Files Modified

- `docs/research/source-queue-2026-05-25.csv`
- `docs/research/clinical-trials-watchlist-2026-06-17.csv`
- `docs/research/clinical-trials-watchlist-2026-06-17.md`
- `docs/research/research-refresh-2026-06-17.md`
- `docs/research/research-monitor-2026-06-17.html`
- `.brain/wiki/clinical-trials-watchlist.md`
- `.brain/wiki/research-papers-index.md`
- `.brain/wiki/index.md`
- `.brain/index.json`
- `.brain/sessions/2026-06-17-research-refresh-pass.md`

## Remote Drift Check

- Skipped: no remote server configured for NegletFix.

## Branch Status

- Current branch: `main`.
- `git fetch origin` completed.
- Branch audit found no matching unmerged or unpushed `feature/`, `claude/`, `Codex/`, or `codex/` branches.

## Dirty Tree Status

- Session-owned work: the research refresh files and `.brain` updates listed above.
- Pre-existing/generated dirty paths left untouched: `.codex/`, `SmokeResults/`, Unity `.utmp`, Unity tutorial/readme assets, Unity smoke logs/screenshots/CSV/XML artifacts, Compass artifacts, `docs/Eric Files/`, prior YouTube HTML artifact, and condition-explainer report assets.

## Wiki / Relationships

- Wiki updated:
- `clinical-trials-watchlist.md` now points to June 17 sources and lists 17 NCT rows.
- `clinical-trials-watchlist.md` now has a Mayo institutional source section with `INST-001..INST-004`.
- `research-papers-index.md` now includes the June 17 PubMed refresh cluster.
- `index.md` now shows the updated research audit and Research Watchlist source paths.
- No relationship update.

## Validation

- CSV parsing passed for the 60-row source queue and 17-row clinical-trials watchlist.
- `.brain/index.json` parsed successfully with `jq`.
- `docs/research/research-monitor-2026-06-17.html` parsed successfully with Python's built-in `html.parser`.
- HTTP preview check at `http://127.0.0.1:8787/research-monitor-2026-06-17.html` confirmed the 60-row metric, Mayo institutional card, trusted-institutions lane, `INST-001..INST-004` mentions, and monthly Mayo checklist text.
- Browser render QA passed through a temporary localhost server:
  - desktop/current viewport before the Mayo addendum: 56/17/5/11 metrics rendered, 17 trial cards rendered, refresh section present, no console errors, no horizontal overflow;
  - trial filters: Watch narrowed to 8 rows and All restored 17 rows;
  - mobile 390px viewport: 17 trial cards rendered, no horizontal overflow, and no detected element overflow.
- Post-Mayo browser automation note: the in-app browser bridge reported no exposed tabs and could not attach a new page; a local headless Chrome render also hung before returning DOM, so no fresh screenshot/overflow pass was captured for the Mayo addendum.

## Next Steps

- Create a real CSV-to-HTML monitor generator before adding a recurring automation.
- In the next monthly refresh, extract posted results for `CTG-012` and `CTG-016` if the registry result tables are usable.
- Include `INST-001..INST-004` in the monthly source refresh and promote Mayo items only after tracing them to primary evidence or an official Mayo program/trial/clinician source.
