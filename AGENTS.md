# NegletFix - Codex Project Contract

This file tells Codex how to work in this repository. It is the canonical
project-level operating contract. `CLAUDE.md` and local command files should
defer to this file when there is any conflict.

## Project Summary

NegletFix is a VR rehabilitation system for Eric's left homonymous hemianopia
and contrast/brightness deficits after a right PCA stroke in July 2021. The
current product track is Quest-based audiovisual rehabilitation with objective
CSV evidence, headset validation, and research sidecar outputs.

Key project paths:

- Project root: `/Users/ericlespagnon/Dropbox/DEV-LOCAL/NegletFix`
- Unity project: `Unity/NeglectFix`
- Brain memory: `.brain/`
- Wiki: `.brain/wiki/`
- Obsidian bridge:
  `/Users/ericlespagnon/Dropbox/DEV-LOCAL/My2ndBrain/scripts/obsidian-bridge.sh`

Medical context:

- Diagnosis: left homonymous hemianopia from right PCA stroke.
- MRI finding: large encephalomalacia in right occipital lobe.
- Main symptoms: complete left visual field blindness plus bilateral dimness /
  gray overlay. This is a real daily rehabilitation target, not a comfort issue.
- Baseline contrast sensitivity from 2025-12-15:
  - Right/intact: 2.25 LogCS.
  - Left/affected: 0.00 LogCS.
  - Asymmetry: 2.25 LogCS.

Do not provide medical prescriptions. Research outputs may identify clinician
questions, evidence quality, and safety boundaries.

## Session Start Ritual

When the user says "start session", starts a new task, asks to resume work, or
opens a fresh Codex session in this repo, run this context ritual before coding.
Natural-language triggers count; the user does not need a slash command.

Goal: reconstruct project state from git, `.brain/`, wiki, branch status,
remote drift, and Obsidian inbox, then report the state back to the user before
making task changes.

### Step 1: Dirty Worktree Classification

Run this before editing:

```bash
git status --short --branch
```

If the worktree is dirty, classify the dirty paths before starting unrelated
work:

- Session-owned: changes from the current task/session.
- Pre-existing/user-owned WIP: changes already present before this task or
  clearly outside the current scope.
- Generated/ignored candidate: build output, cache, scratch, exports, APKs,
  Unity logs, screenshots, smoke CSVs, or tool artifacts.
- Unknown/risky: changes that need inspection before any edit, deploy, stage,
  or cleanup.

Rules:

- Do not stage, revert, delete, move, overwrite, or deploy pre-existing/user
  WIP without explicit approval.
- If dirty tracked code is related to the user task, inspect it and work with
  it instead of assuming it can be discarded.
- If substantial unclassified dirty work exists, report it in the session-start
  summary and ask before broad cleanup or unrelated implementation.

### Step 2: Sync From Remote

Fetch first:

```bash
git fetch origin
```

If the worktree is clean, update main:

```bash
git pull --ff-only origin main
```

If local dirty work could be overwritten, do not pull. Report that pull was
skipped because the dirty tree must be classified or cleaned first.

### Step 3: Read Brain Memory

Read these in order:

1. `.brain/index.json` - module health, active issues, current phase.
2. Latest files in `.brain/sessions/` - what happened last session.
3. Latest files in `.brain/crumbs/` - mid-session checkpoints.
4. `.brain/backlog.md` - blocked/deferred WIP.
5. `.brain/cross-cutting.md` only if the task touches broad system behavior.
6. `.brain/decisions.md` only if the task touches architecture or product
   direction.

Do not bulk-load every memory file. Pull targeted files as needed.

If `.brain/` is missing, say so and propose creating it only if the user wants
this project enrolled in the brain system.

### Step 4: Branch Audit

Run:

```bash
git branch -r --no-merged main | rg "feature/|claude/|Codex/|codex/" || true

for branch in $(git branch --format='%(refname:short)' | rg "feature/|claude/|Codex/|codex/"); do
  LOCAL=$(git rev-parse "$branch" 2>/dev/null)
  REMOTE=$(git rev-parse "origin/$branch" 2>/dev/null)
  if [ "$LOCAL" != "$REMOTE" ] 2>/dev/null; then
    AHEAD=$(git rev-list --count "origin/$branch..$branch" 2>/dev/null || echo "no remote")
    echo "Warning: $branch has $AHEAD unpushed commits"
  fi
done
```

If `rg` is unavailable, use `grep -E`.

If unmerged or unpushed work is found, require an explicit decision for each
branch before starting unrelated new work:

- Merge now.
- Keep working, with reason documented for wrap-up.
- Abandoned/delete, only with explicit approval.

Ritual skip detection: for each unmerged branch with recent commits, check
whether a corresponding session summary exists in `.brain/sessions/`. If not,
flag that the previous session may have skipped wrap-up and propose a catch-up
summary, push, and drift check.

### Step 5: Remote Drift Check

NegletFix currently has no VPS or production server. Remote drift check is
therefore skipped by default:

```text
remote drift check skipped: no remote server configured for NegletFix
```

Quest devices, APKs, pulled logs, and SmokeResults are validation artifacts, not
remote production drift. If a future remote server or cloud dashboard is added,
define key drift files here and compare them at session start and end.

### Step 6: Load Wiki Catalog

If `.brain/wiki/` exists:

1. Read `.brain/wiki/index.md`.
2. Do not load all wiki pages upfront.
3. During the task, load relevant wiki pages on demand before changing areas
   they describe.

### Step 7: Obsidian Inbox Scan

If the bridge exists, scan for user notes captured from phone or Obsidian:

```bash
/Users/ericlespagnon/Dropbox/DEV-LOCAL/My2ndBrain/scripts/obsidian-bridge.sh scan
```

Highlight inbox items for NegletFix. If the bridge is missing or fails, skip
gracefully and say so; `.brain/` remains the source of truth.

### Step 8: Report Before Coding

Before changing task files, summarize:

```markdown
Session initialized.

Project: NegletFix
Current branch: {branch}
Last session: {date/topic}
Phase: {phase}
Module health: {short summary}
Active issues: {count + key blockers}
Backlog/WIP: {relevant items}
Branch audit: {clean or decisions needed}
Remote drift: skipped, no remote configured / mismatches if later configured
Obsidian inbox: {NegletFix items or none}
Wiki context: index loaded / not present
Dirty tree: {clean or classified summary}

Ready to work on: {user task}
```

Only proceed to implementation after this ritual unless the user explicitly asks
for an emergency quick action.

## Crumb Ritual

When the user says "drop a crumb", or when the session is long, context-heavy,
or about to run a long operation, write a checkpoint:

Path: `.brain/crumbs/YYYY-MM-DD-HHMM.md`

Format:

```markdown
# Crumb: YYYY-MM-DD HH:MM

## Progress So Far
- What has been completed
- Counts/metrics
- Decisions made

## Currently
- What's running or next

## Key Findings
- Notable discoveries, blockers, or risks
```

Do not stop after writing a crumb unless the user asks to pause.

## Session End Ritual

When the user says "let's wrap up", "end session", "that's it for today",
"we're done here", or similar, run the full closeout ritual. This is not just
git cleanup: it preserves `.brain/`, updates durable wiki knowledge, syncs
Obsidian, and keeps phone/web/Mac continuation working.

Mandatory final state: do not consider the session closed until:

- A session summary exists in `.brain/sessions/`.
- Wiki impact was checked if `.brain/wiki/` exists.
- Relationships were checked if `.brain/relationships.md` exists.
- Brain updates were committed and pushed, unless the user explicitly skips
  recording.
- Obsidian `populate` and `done` completed, if the bridge is installed.
- Every dirty worktree path is either committed, intentionally ignored/removed
  with approval, or explicitly classified as pre-existing/user-owned WIP,
  generated/ignored candidate, or documented follow-up cleanup.
- No deployed, production-relevant, or session-owned code remains uncommitted
  unless the user explicitly approves the exception and it is recorded in the
  session summary.

### Step 1: Remote Drift Check

Run the same remote drift check from session start. For the current NegletFix
setup, record:

```text
remote drift check skipped: no remote server configured for NegletFix
```

### Step 2: Branch Hygiene

Run the same branch audit from session start. Document kept, merged, or deleted
branches and reasons.

### Step 3: Dirty Tree Closeout Gate

Run:

```bash
git status --short --branch
```

Classify every remaining dirty path before calling the session wrapped:

- Committed/pushed in this closeout: completed session work.
- Pre-existing/user-owned WIP: unrelated work that must not be staged or
  reverted.
- Generated/ignored candidate: build output, cache, scratch, exports, logs,
  screenshots, APKs, smoke CSVs, or tool artifacts.
- Documented follow-up cleanup: known worktree debt that needs its own cleanup
  session.
- Blocked/unclassified: anything whose owner or safety is unclear.

Closeout rules:

- If any session-owned work remains unstaged/uncommitted, closeout is blocked
  until it is committed and pushed or the user explicitly approves leaving it
  uncommitted.
- If any deployed, release, or clinical/protocol-relevant file is dirty,
  closeout is blocked until it is committed, intentionally reverted with
  approval, or explicitly recorded as an approved exception.
- If unrelated dirty work remains, list it by category in the session summary
  and final response. Do not describe the repo as clean.
- If the dirty tree is large, provide a concise top-level classification
  instead of dumping every file path, and recommend a dedicated cleanup session.

### Step 4: Gather Session Context

- Read crumbs created this session.
- Review `git status --short`.
- Separate Codex changes from unrelated dirty work.
- Apply the Dirty Tree Closeout Gate before staging files.
- Review command results, headset validations, generated reports, research
  findings, and user decisions.

### Step 5: Draft Session Summary

Create `.brain/sessions/YYYY-MM-DD-{topic}.md`:

```markdown
# Session: {Brief Title}

**Date**: YYYY-MM-DD
**Status**: Complete | In Progress

## Objectives
- What we set out to do

## Outcomes
- What was achieved
- Key changes made
- Quest/headset, research, or report status if relevant

## Files Modified
- Key files changed
- Generated artifacts worth preserving

## Remote Drift Check
- Skipped: no remote server configured, or mismatches if later configured

## Branch Status
- Branches merged, kept with reason, or deleted

## Dirty Tree Status
- Clean, or classified remaining dirty work with owner/reason

## Wiki / Relationships
- Wiki pages updated, or "no wiki impact"
- Relationship updates proposed/applied, or "no relationship update"

## Next Steps
- What to do next session
```

### Step 6: Wiki Ingest

If `.brain/wiki/` exists, update wiki pages only when this session changed
durable project knowledge.

Use two scans:

1. Content match: pages whose topic overlaps the session.
2. Claim invalidation: pages with status claims this session may have changed,
   especially after launch, deploy, ship, pause, resume, first headset pass,
   first dose block, clinical/protocol decision, or decommissioned events.

Rules:

- Preserve existing content; append or refine instead of rewriting whole pages.
- Every new claim needs a source map pointing to the session file, file/line
  reference, report artifact, CSV/log artifact, or decision ID.
- If the session contradicts an existing wiki claim, flag it explicitly.
- If there is no durable wiki impact, write "No wiki impact" in the session
  summary.
- If a finding clearly applies across projects, ask before propagating it to
  shared Obsidian wiki.

### Step 7: Relationship Check

If `.brain/relationships.md` exists, scan the session for tracked people.

Rules:

- `last_contact` changes only when actual contact happened, not when a person
  was merely mentioned.
- `temperature` changes only when relationship warmth changed, not for routine
  blockers.
- Append dated context bullets; do not rewrite relationship history.
- Get explicit approval before writing relationship changes.

### Step 8: Propose Memory Updates

Tell the user what will be updated before committing:

- `.brain/sessions/YYYY-MM-DD-{topic}.md`
- `.brain/index.json` if module status or active issues changed
- `.brain/backlog.md` if WAITING/WIP/completed items changed
- `.brain/cross-cutting.md` for cross-module learnings
- `.brain/decisions.md` for architectural/product decisions
- `.brain/wiki/*` if durable knowledge changed
- `.brain/relationships.md` if approved relationship changes exist
- `.brain/obsidian-custom.md` if the user asked to expose custom dashboard data

### Step 9: Commit and Push

Stage selectively. Do not stage unrelated user changes, generated build output,
local scratch files, or untracked files unless they are part of the approved
work.

Prefer one implementation commit and one brain/docs commit when that makes
review cleaner.

```bash
git add <approved files>
git commit -m "{type}({scope}): {description}"
git push origin main
```

Push is mandatory unless the user explicitly skips recording.

### Step 10: Sync to Obsidian

After the push, run:

```bash
PROJECT_NAME="NegletFix"
/Users/ericlespagnon/Dropbox/DEV-LOCAL/My2ndBrain/scripts/obsidian-bridge.sh populate .brain "$PROJECT_NAME"
/Users/ericlespagnon/Dropbox/DEV-LOCAL/My2ndBrain/scripts/obsidian-bridge.sh done "$PROJECT_NAME" "{1-line summary}"
```

Use `populate`, not `sync`: every session creates a new session file, and
`populate` refreshes the session list, module table, raw `.brain/` copy, and
dashboard.

Successful closeout should show:

- `Written to ...`
- `Brain copied ...`
- `Briefing generated ...`
- `Logged: NegletFix - summary`

Do not skip `done`: the daily note log is separate from `populate`.

Known fallback: `populate` may hang during the raw `.brain` copy because the
bridge can run `rsync --checksum` into the iCloud Obsidian vault. If `populate`
has already printed the project note write but does not finish the copy/dashboard
step within about two minutes:

1. Inspect the running bridge/rsync process.
2. Stop only the stuck `obsidian-bridge.sh populate` / `copy-brain` /
   `rsync --checksum` process.
3. Complete the raw brain copy without checksum:

   ```bash
   mkdir -p "$HOME/Library/Mobile Documents/iCloud~md~obsidian/Documents/Ricopoulos/Projects/NegletFix/brain"
   rsync -a --delete --exclude .DS_Store .brain/ "$HOME/Library/Mobile Documents/iCloud~md~obsidian/Documents/Ricopoulos/Projects/NegletFix/brain/"
   /Users/ericlespagnon/Dropbox/DEV-LOCAL/My2ndBrain/scripts/obsidian-bridge.sh dashboard
   ```

4. Then run the normal `done` command.

## NegletFix-Specific Evidence Rules

- Do not call Quest rehab changes complete from editor/headless evidence alone
  when on-device behavior is the real proof gate.
- For headset work, preserve the chain: build artifact, install/launch method,
  live log, pulled trial/session CSVs, and Eric's subjective report when
  available.
- Keep assessment separate from rehab. Field mapping/calibration is not the same
  scene as therapeutic AV training.
- Treat research findings as source-graded leads until verified against primary
  medical literature or official clinical-trial records.
- Do not mix new adjuncts such as medications, QEEG, or photobiomodulation into
  the active protocol without an explicit doctor/research decision and a clean
  measurement plan.
