# NegletFix - Claude Code Context

`AGENTS.md` is the canonical project contract for this repository. Read and
follow it first. If this file conflicts with `AGENTS.md`, `AGENTS.md` wins.

## Required Start Context

Before doing work, reconstruct the project state:

1. Run `git status --short --branch` and classify dirty paths.
2. Fetch/pull only when safe for the current dirty tree.
3. Read `.brain/index.json`, latest `.brain/sessions/`, latest `.brain/crumbs/`,
   and `.brain/backlog.md`.
4. Run the branch audit for `feature/`, `claude/`, `Codex/`, and `codex/`
   branches.
5. Load `.brain/wiki/index.md` and pull wiki pages on demand.
6. Scan Obsidian with:
   `/Users/ericlespagnon/Dropbox/DEV-LOCAL/My2ndBrain/scripts/obsidian-bridge.sh scan`
7. Summarize the state before coding.

## Required End Context

When the user asks to wrap up, run the full end-session ritual from `AGENTS.md`:

- Session summary in `.brain/sessions/`.
- Wiki impact check.
- Relationship check if `.brain/relationships.md` exists.
- Dirty-tree closeout gate.
- Selective commit and push unless the user explicitly skips recording.
- Obsidian `populate` and `done`.

Do not describe the session as wrapped while session-owned work is uncommitted,
or while dirty paths are unclassified.

## Project Overview

VR-based rehabilitation system for left homonymous hemianopia with
contrast/brightness deficits following right PCA stroke. Combines Quest-based
audiovisual stimulation training with an EEG/neurofeedback track that remains
decoupled from the v1 critical path.

Unity project:

```text
/Users/ericlespagnon/Dropbox/DEV-LOCAL/NegletFix/Unity/NeglectFix/
```

## Eric's Medical Condition

- Diagnosis: left homonymous hemianopia from right PCA stroke in July 2021.
- MRI finding: large area of encephalomalacia in right occipital lobe.
- Symptoms:
  - Complete left visual field blindness.
  - Bilateral dimness / gray overlay.
  - "Even with a bright day he always sees dark."

This is a rehabilitation project, not accommodation.

## Baseline Contrast Sensitivity Results

First validated measurement after fixing the hemifield positioning bug:

| Hemifield | LogCS Score | Contrast Threshold | Interpretation |
| --- | --- | --- | --- |
| Central | about 1.05+ | about 9% | Good central vision |
| Right/intact | 2.25 LogCS | 0.6% | Excellent, maxed out test |
| Left/affected | 0.00 LogCS | 100% | Severe deficit |

Asymmetry: 2.25 LogCS. Clinically significant threshold: 0.30 LogCS.

## Evidence Rules

- Quest rehab changes need real headset validation when headset behavior is the
  proof gate.
- Preserve validation artifacts: build, install/run method, live log, pulled
  CSVs, and subjective report when available.
- Keep research claims source-graded and separate from protocol changes.
- Do not provide medical prescriptions.
