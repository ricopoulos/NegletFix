# Session: BioRender Visual Evaluation

**Date**: 2026-06-07
**Status**: Complete

## Objectives

- Evaluate whether the BioRender Codex plugin/trial is useful for NegletFix.
- Keep the intended use narrow: cosmetic, medical-oriented visual clarity for
  doctor-facing briefs and a personal blog, not data storage or clinical
  evidence handling.
- Verify local plugin installation state enough to know what to try next.

## Outcomes

- Recommended BioRender as a visual style layer for NegletFix, not as an
  evidence layer.
- Defined the likely trial use cases:
  - doctor-facing one-page diagram;
  - protocol flow diagram;
  - personal blog explainer;
  - reusable visual kit for brain, visual field, headset, data/progress, and
    rehab-session concepts.
- Set the privacy boundary: feed BioRender sanitized descriptions only; do not
  upload MRI images, doctor letters, full identity details, raw clinical files,
  CSVs, or private medical artifacts.
- Verified the local Codex plugin config contains
  `biorender@openai-curated` enabled and the plugin cache exists under
  `/Users/ericlespagnon/.codex/plugins/cache/openai-curated/biorender`.
- Found that this running thread did not expose a callable BioRender tool, even
  after tool discovery. The plugin appears to be an app connector rather than a
  local coding skill/MCP tool in the current thread.
- Prepared the next prompt for a fresh Codex thread or explicit app invocation:
  ask BioRender for templates/icon sets for left homonymous hemianopia after
  right occipital stroke, VR-based visual rehabilitation, field mapping, and
  patient progress tracking.

## Files Modified

- `.brain/sessions/2026-06-07-biorender-visual-evaluation.md`

## Remote Drift Check

- Skipped: no remote server is configured for NegletFix.

## Branch Status

- Current branch: `main`
- Branch audit found no unmerged or unpushed `feature/`, `claude/`, `Codex/`,
  or `codex/` work branches.

## Dirty Tree Status

Session-owned and intended for commit in this closeout:

- `.brain/sessions/2026-06-07-biorender-visual-evaluation.md`

Pre-existing/user-owned WIP left untouched:

- Tracked Unity/headset work already dirty before this BioRender discussion:
  `Unity/NeglectFix/Assets/Editor/AvTrainingManualSmokeSceneBuilder.cs`,
  `Unity/NeglectFix/Assets/Scenes/AVTrainingSession1Pilot.unity`,
  `Unity/NeglectFix/Assets/Scripts/Tasks/AudioVisualTraining.cs`,
  `Unity/NeglectFix/Assets/Scripts/Utils/DataLogger.cs`,
  `Unity/NeglectFix/Assets/Tests/PlayMode/AudioVisualTrainingSmokeTest.cs`,
  and `scripts/generate-rehab-report.js`.

Generated/ignored or artifact candidates left untouched:

- `.codex/`
- `SmokeResults/`
- `Unity/NeglectFix/.utmp/`
- `Unity/NeglectFix/SmokeResults/`
- Unity screenshots, tutorial/readme assets, smoke logs/XML/CSVs, pulled device
  screenshots, and Compass markdown artifacts currently present in the working
  tree.

Unknown/follow-up WIP left untouched:

- New field-mapping scene/script/test files and `docs/Eric Files/` remain part
  of the existing Unity/research WIP and need their own implementation closeout
  or cleanup pass.

## Wiki / Relationships

- Wiki impact: no durable protocol, evidence, or medical wiki claim changed.
  BioRender is only a communication/tooling evaluation until actual visuals are
  selected and incorporated into a report or doctor/blog brief.
- Relationships: checked. No relationship update was needed because no real
  external contact happened.

## Next Steps

- Start a fresh Codex thread and invoke the BioRender app connector directly,
  using the prepared sanitized prompt.
- Evaluate BioRender on cosmetic/clarity value only: can it produce or locate a
  medically coherent visual style for a doctor one-pager and personal blog?
- If useful, create a small NegletFix visual kit and keep it separate from raw
  clinical evidence.
