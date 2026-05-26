# Session: Research Ops, YouTube/X Triage, Medication Brief

**Date**: 2026-05-26
**Status**: Complete

## Objectives
- Build a research-side strategy complementary to the Unity/Quest rehab work.
- Turn YouTube and X into discovery channels that produce traceable medical leads, not unsupported claims.
- Process the first five source-queue sprints.
- Refresh the pharmacological-adjunct question into a doctor-facing medication brief.
- Store a final HTML wrap-up in Obsidian for phone reading.

## Outcomes

### Research operations system created
- Created `docs/research/source-queue-2026-05-25.html` and `.csv` as the master queue.
- Established the operating rule: YouTube and X are discovery tools only; a claim becomes NegletFix knowledge only after it is traced to a paper, guideline, clinical trial, reputable medical organization, or Eric's app measurements.
- Created `docs/research/research-compass-2026-05-25.html` to summarize Eric's exact condition, research themes, discovery channels, and parking rules.
- Created `docs/research/youtube-recovery-after-stroke-triage-2026-05-25.html` for the Recovery After Stroke playlist method.

### Sprint reports completed
- Sprint 1: `sprint-1-medical-grounding-2026-05-25.html`
  - NANOS promoted as clean patient-facing HH explainer.
  - FLUORESCE/fluoxetine verified as a real acute-stroke pilot, but parked as clinician-only and not actionable for Eric's chronic case.
- Sprint 2: `sprint-2-function-mobility-2026-05-25.html`
  - Alex Bowers walking/driving material converted into functional metrics: blind-side detection, reaction time, head-scan angle, mobility confidence.
  - Explicit boundary: NegletFix is never driving clearance.
- Sprint 3: `sprint-3-daily-function-tracker-2026-05-25.html`
  - Recovery After Stroke episodes 274 and 156 converted into a daily-function tracker.
  - Created `daily-function-tracker-template-2026-05-25.csv`.
- Sprint 4: `sprint-4-paper-author-discovery-2026-05-25.html` and `.csv`
  - X queries reframed as paper/trial/author discovery prompts.
  - Captured Rowe/Liverpool, Huxlin/Rochester, Bowers/Schepens, Wake Forest multisensory group, Bolognini/Diana/Auxologico, Raffin/Bevilacqua/Guggisberg.
- Sprint 5: `sprint-5-qeeg-tpbm-evidence-2026-05-25.html` and `.csv`
  - QEEG/tPBM evaluated as watchlist-only.
  - Conclusion: tPBM is real but mixed; QEEG-guided laser targeting for chronic HH has no strong direct evidence.

### Medication adjunct brief refreshed
- Created `docs/research/medication-neuroplasticity-doctor-brief-2026-05-26.html` and `.csv`.
- Updated [[pharmacological-adjuncts]] with the 2026-05-26 evidence refresh:
  - Fluoxetine: FLUORESCE is direct visual-field but acute-only; 2024 fluoxetine IPD meta-analysis argues against routine recovery use.
  - Levodopa: older Scheidtmann rationale weakened by DARS 2019 and ESTREL 2025.
  - Modafinil: possible fatigue-management conversation only, not visual recovery.
  - Methylphenidate/rotigotine: only relevant if a clinician confirms true neglect/attention impairment distinct from HH.
- Doctor conversation reframed from "neuroplasticity booster" to safer questions about medication cleanup, fatigue, sleep, mood, and attention barriers to training.

### Obsidian mobile output
- Created `docs/research/research-session-wrapup-2026-05-26.html`.
- Added durable brain artifact copy:
  - `.brain/artifacts/research-session-wrapup-2026-05-26.html`
- Copied it into the Obsidian vault at:
  - `Projects/NegletFix/brain/artifacts/research-session-wrapup-2026-05-26.html`
- Added `.brain/obsidian-custom.md` so the Obsidian project note exposes the mobile HTML report after bridge populate.

## Files Modified

### New research artifacts
- `docs/research/research-compass-2026-05-25.html`
- `docs/research/youtube-recovery-after-stroke-triage-2026-05-25.html`
- `docs/research/source-queue-2026-05-25.html`
- `docs/research/source-queue-2026-05-25.csv`
- `docs/research/sprint-1-medical-grounding-2026-05-25.html`
- `docs/research/sprint-2-function-mobility-2026-05-25.html`
- `docs/research/sprint-3-daily-function-tracker-2026-05-25.html`
- `docs/research/daily-function-tracker-template-2026-05-25.csv`
- `docs/research/sprint-4-paper-author-discovery-2026-05-25.html`
- `docs/research/sprint-4-paper-author-discovery-2026-05-25.csv`
- `docs/research/sprint-5-qeeg-tpbm-evidence-2026-05-25.html`
- `docs/research/sprint-5-qeeg-tpbm-evidence-2026-05-25.csv`
- `docs/research/medication-neuroplasticity-doctor-brief-2026-05-26.html`
- `docs/research/medication-neuroplasticity-doctor-brief-2026-05-26.csv`
- `docs/research/research-session-wrapup-2026-05-26.html`

### Brain / wiki updates
- `.brain/index.json`
- `.brain/artifacts/research-session-wrapup-2026-05-26.html`
- `.brain/obsidian-custom.md`
- `.brain/wiki/index.md`
- `.brain/wiki/pharmacological-adjuncts.md`
- `.brain/wiki/research-papers-index.md`
- `.brain/sessions/2026-05-26-research-ops-youtube-x-medication-wrap.md`

### Obsidian vault output
- `/Users/ericlespagnon/Library/Mobile Documents/iCloud~md~obsidian/Documents/Ricopoulos/Projects/NegletFix/brain/artifacts/research-session-wrapup-2026-05-26.html`

## VPS Drift Check
- N/A. NegletFix is local-only and has no deployed VPS target for this research session.

## Branch Status
- Branch: `main`
- No unmerged remote feature/codex/claude branches found during wrap-up branch hygiene.
- Existing unrelated dirty files remain untouched: Unity generated/project asset changes, `.codex/`, `AGENTS.md`, old compass artifacts, screenshots, and `docs/Eric Files/`.

## Decisions Locked
- YouTube and X are discovery channels, not evidence.
- QEEG/tPBM remains a watchlist path only; no protocol change.
- Medication adjuncts remain clinician-only. No medication is promoted for chronic HH recovery.
- The next engineering move is still the Unity Editor smoke test of WIP-001.
- HTML remains the preferred format for long research synthesis and should be copied into Obsidian when mobile reading matters.

## Next Steps
- Run the fresh-head Unity Editor smoke test of `AudioVisualTraining`.
- Verify trial logging and basic usability before Quest controller input work.
- Use the daily-function tracker alongside contrast sensitivity during any training block.
- Bring the medication doctor brief only as a structured conversation aid, not as a request for a specific prescription.
