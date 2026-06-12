# Session: Research Monitor HTML

**Date**: 2026-06-11
**Status**: Complete

## Objectives

- Update the understandable HTML research output with the latest clinical-trial findings.
- Make the research/tracking system feel more real as a monitor view.
- Clarify whether YouTube and X searches belong in the research flow.

## Outcomes

- Added `docs/research/research-monitor-2026-06-11.html`, a self-contained static HTML monitor with:
  - current protocol verdict;
  - research metrics;
  - protocol impact lanes;
  - filterable clinical-trial cards for CTG-001 through CTG-009;
  - YouTube/X/ClinicalTrials.gov intake explanation;
  - monthly monitor checklist and source-file links.
- Confirmed the operational rule: YouTube and X are part of intake/discovery, not proof. They only become evidence after resolving to PubMed, ClinicalTrials.gov, DOI/guideline, or institutional sources.
- Aligned `docs/research/source-queue-2026-05-25.csv` with the clinical-trials watchlist by adding CTG-008 and CTG-009.

## Files Modified

- `docs/research/research-monitor-2026-06-11.html`
- `docs/research/source-queue-2026-05-25.csv`
- `.brain/wiki/clinical-trials-watchlist.md`
- `.brain/wiki/index.md`
- `.brain/index.json`
- `.brain/sessions/2026-06-11-research-monitor-html.md`

## Remote Drift Check

- Skipped: no remote server configured for NegletFix.

## Branch Status

- Current branch: `main`.
- `git fetch origin` completed.
- Branch audit reported no unmerged matching `feature/`, `claude/`, `Codex/`, or `codex/` branches and no unpushed matching local branches.
- This session is documentation/research-monitor work only.

## Dirty Tree Status

- Session-owned work: the research monitor HTML, source-queue CTG alignment, and brain/wiki updates listed above.
- Pre-existing/user-owned WIP left untouched: Unity AV training and data-logging edits already dirty at session start.
- Generated/ignored candidates left untouched: `.codex/`, `SmokeResults/`, Unity `.utmp`, smoke logs, screenshots, pulled CSV/XML artifacts, and other generated Unity validation files.
- Pre-existing docs/artifacts left untouched: `docs/Eric Files/`, `docs/research/youtube-episode-406-francois-couillard-vision-loss-2026-05-30.html`, and Compass artifacts.

## Wiki / Relationships

- Wiki updated:
  - `clinical-trials-watchlist.md` now points to the static HTML monitor and unified source queue.
  - `index.md` now lists the monitor as part of the Research Watchlist module.
- No relationship update.

## Validation

- CSV parsing passed for the source queue and clinical-trials watchlist.
- HTML parsed successfully with Python's built-in `html.parser`.
- Browser render QA passed through temporary localhost serving:
  - desktop: 9 cards rendered, Watch filter narrowed to CTG-003/CTG-008/CTG-009, All restored 9 cards, no horizontal overflow;
  - mobile 390px: 9 cards rendered, no horizontal overflow, and no detected text overflow in key containers.

## Next Steps

- Generate future research monitors from the same CSV shape instead of manually duplicating trial card content.
- Add a monthly automation only after the CSV-to-HTML generation path is stable.
- If automated, it should propose a review note/diff for meaningful changes, not alter the protocol automatically.
