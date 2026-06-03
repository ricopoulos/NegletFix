# /end-session - Close a NegletFix Working Session

Natural language triggers count: "let's wrap up", "end session", "that's it for
today", "we're done here", or similar.

This is not just git cleanup. It preserves `.brain/`, durable wiki knowledge,
relationship context, Obsidian mobile access, and dirty-tree accountability.

`AGENTS.md` is the source of truth. This command is a local shorthand for that
contract.

## Mandatory Final State

Do not call the session wrapped until:

- A session summary exists in `.brain/sessions/`.
- Wiki impact was checked if `.brain/wiki/` exists.
- Relationships were checked if `.brain/relationships.md` exists.
- Session-owned brain/docs/code changes were committed and pushed, unless the
  user explicitly skips recording.
- Obsidian `populate` and `done` completed if the bridge is installed.
- Every dirty path is committed, approved to remain, or classified.

## Protocol

### 1. Remote Drift Check

NegletFix has no VPS or production server. Record:

```text
remote drift check skipped: no remote server configured for NegletFix
```

If a future remote server is added, compare local and remote key files at both
session start and end.

### 2. Branch Hygiene

Run the start-session branch audit again:

```bash
git branch -r --no-merged main | rg "feature/|claude/|Codex/|codex/" || true
git branch --show-current
```

Document kept, merged, or deleted branches and reasons.

### 3. Dirty Tree Closeout Gate

Run:

```bash
git status --short --branch
```

Classify remaining dirty paths:

- Committed/pushed in this closeout.
- Pre-existing/user-owned WIP.
- Generated/ignored candidate.
- Documented follow-up cleanup.
- Blocked/unclassified.

If session-owned or release/clinical/protocol-relevant files remain dirty,
closeout is blocked until they are committed/pushed or the user explicitly
approves the exception and it is recorded in the session summary.

### 4. Gather Context

- Read crumbs created this session.
- Review work accomplished, validation evidence, generated reports, user
  decisions, and remaining dirty paths.
- Separate current-session work from unrelated WIP.

### 5. Write Session Summary

Create `.brain/sessions/YYYY-MM-DD-{topic}.md` with:

- Objectives.
- Outcomes.
- Files modified.
- Remote drift check.
- Branch status.
- Dirty tree status.
- Wiki / relationships.
- Next steps.

### 6. Wiki Ingest

If `.brain/wiki/` exists, update only pages affected by durable project
knowledge. Run both:

- Content match scan.
- Claim invalidation scan for status-changing sessions such as headset pass,
  first dose block, protocol shift, launch, pause, resume, deploy, or wrap.

Every new claim needs a source map to the session, file/line, report artifact,
CSV/log artifact, or decision ID. If no wiki impact, write "No wiki impact" in
the session summary.

### 7. Relationship Check

If `.brain/relationships.md` exists, scan tracked people. Change `last_contact`
only for actual contact, change `temperature` only for real relationship warmth
changes, and get explicit approval before writing relationship updates.

### 8. Propose Memory Updates

Tell the user which files will be updated before committing.

### 9. Commit and Push

Stage selectively. Do not stage unrelated dirty files.

```bash
git add <approved files>
git commit -m "{type}({scope}): {description}"
git push origin main
```

Push is mandatory unless the user explicitly skips recording.

### 10. Sync to Obsidian

Use `populate`, not `sync`.

```bash
PROJECT_NAME="NegletFix"
/Users/ericlespagnon/Dropbox/DEV-LOCAL/My2ndBrain/scripts/obsidian-bridge.sh populate .brain "$PROJECT_NAME"
/Users/ericlespagnon/Dropbox/DEV-LOCAL/My2ndBrain/scripts/obsidian-bridge.sh done "$PROJECT_NAME" "{1-line summary}"
```

Successful closeout should show project note write, brain copy, briefing
generation, and a `Logged: NegletFix - ...` line.

Do not skip `done`; the daily note log is separate from `populate`.
