# /start-session - Begin a NegletFix Working Session

Natural language triggers count: "start session", "resume", "let's continue",
or any fresh substantial task in this repo.

`AGENTS.md` is the source of truth. This command is a local shorthand for that
contract.

## Protocol

### 1. Dirty Tree Classification

Run:

```bash
git status --short --branch
```

Classify dirty paths before unrelated work:

- Session-owned.
- Pre-existing/user-owned WIP.
- Generated/ignored candidate.
- Unknown/risky.

Do not stage, revert, delete, or overwrite pre-existing/user-owned work without
explicit approval. If dirty tracked code touches the task, inspect it before
editing.

### 2. Sync From Remote

Run:

```bash
git fetch origin
```

If the worktree is clean:

```bash
git pull --ff-only origin main
```

If dirty work could be overwritten, skip pull and report why.

### 3. Read Brain Memory

Read, in order:

1. `.brain/index.json`
2. Latest `.brain/sessions/`
3. Latest `.brain/crumbs/`
4. `.brain/backlog.md`
5. `.brain/cross-cutting.md` only for broad system context
6. `.brain/decisions.md` only for architecture/product decisions

### 4. Branch Audit

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

If unmerged or unpushed work exists, get an explicit decision for each branch:
merge now, keep with reason, or abandon/delete with approval.

### 5. Remote Drift Check

NegletFix has no VPS or production server. Record:

```text
remote drift check skipped: no remote server configured for NegletFix
```

### 6. Wiki Index

If `.brain/wiki/` exists, read `.brain/wiki/index.md` only. Pull specific pages
on demand during the work.

### 7. Obsidian Inbox

```bash
/Users/ericlespagnon/Dropbox/DEV-LOCAL/My2ndBrain/scripts/obsidian-bridge.sh scan
```

Highlight any NegletFix inbox items.

### 8. Report Before Coding

Summarize project, branch, last session, module health, active issues,
backlog/WIP, branch audit, remote drift, Obsidian inbox, wiki context, and dirty
tree classification before editing.
