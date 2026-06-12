# Session: NeurIPS 2025 EEG-AI Methods Lane

**Date**: 2026-06-12
**Status**: Complete

## Objectives

- Preserve the NeurIPS 2025 EEG/biosignal foundation-model discovery pass as a durable NegletFix research lane.
- Keep the classification explicit: useful future EEG signal-analysis infrastructure, not clinical or protocol evidence.

## Outcomes

- Added `NP-001` through `NP-009` to `docs/research/source-queue-2026-05-25.csv`.
- Updated `docs/research/research-monitor-2026-06-11.html` with a visible EEG-AI methods lane and five method cards:
  - BrainBodyFM + EEG Foundation Challenge.
  - REVE.
  - BioFoundation family: LuMamba + LUNA.
  - Architecture clues: BrainOmni, NeurIPT, CSBrain.
  - Guardrails: EEG-Bench and EEG-FM critical review.
- Updated `[[eeg-neurofeedback]]` with the practical interpretation and no-protocol-change boundary.
- Updated `[[research-papers-index]]` with a compact NeurIPS 2025 EEG-AI source cluster.
- Updated `[[index]]` and `.brain/index.json` so session-start scans show the NeurIPS lane.

## Evidence Classification

- Category: EEG-AI methods lane.
- Relevance: possible future EEG artifact detection, signal-quality scoring, montage adaptation, cross-session normalization, embeddings, or higher-density EEG analytics.
- Boundary: no change to active open-loop field-map-guided audiovisual Quest training; no evidence here for homonymous hemianopia recovery, audiovisual training efficacy, or Muse TP10 neurofeedback benefit.

## Files Modified

- `docs/research/source-queue-2026-05-25.csv`
- `docs/research/research-monitor-2026-06-11.html`
- `.brain/wiki/eeg-neurofeedback.md`
- `.brain/wiki/research-papers-index.md`
- `.brain/wiki/index.md`
- `.brain/index.json`
- `.brain/sessions/2026-06-12-neurips-eeg-ai-methods-lane.md`

## Remote Drift Check

- Skipped: no remote server configured for NegletFix.

## Branch Status

- `main` was even with `origin/main` at session start after `git fetch origin`.
- Branch audit found no unmerged or unpushed Codex/feature branches requiring decisions.

## Dirty Tree Status

- Session-owned work: tracked docs/brain files listed above.
- Pre-existing/generated local artifacts left untouched: `.codex/`, `SmokeResults/`, Unity `.utmp`, Unity tutorial/readme assets, Unity smoke logs/screenshots/CSV/XML artifacts, Compass artifacts, `docs/Eric Files/`, and the YouTube HTML artifact.

## Validation

- Source queue CSV parsed successfully after adding NeurIPS/OpenReview rows.
- `.brain/index.json` parsed successfully with `jq`.
- HTML monitor parsed successfully with Python's built-in `html.parser`.
- No protocol or Unity runtime files were changed.

## Next Steps

- If the EEG track becomes real, run a signal-quality session first; do not start from foundation-model inference.
- When the research monitor becomes generated from CSV, make `NP-*` rows a first-class platform group alongside YouTube, X, LinkedIn/arXiv, and ClinicalTrials.gov.
