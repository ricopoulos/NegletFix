# /branch-audit - Check for Unmerged or Unpushed Work

Use at session start, before new features, when investigating missing code, and
during wrap-up.

## Protocol

### 1. Fetch

```bash
git fetch origin
```

### 2. Check Unmerged Remote Branches

```bash
git branch -r --no-merged main | rg "feature/|claude/|Codex/|codex/" || true
```

If `rg` is unavailable, use `grep -E`.

### 3. Check Local vs Remote Differences

```bash
for branch in $(git branch --format='%(refname:short)' | rg "feature/|claude/|Codex/|codex/"); do
  LOCAL=$(git rev-parse "$branch" 2>/dev/null)
  REMOTE=$(git rev-parse "origin/$branch" 2>/dev/null)
  if [ "$LOCAL" != "$REMOTE" ] 2>/dev/null; then
    AHEAD=$(git rev-list --count "origin/$branch..$branch" 2>/dev/null || echo "no remote")
    echo "Warning: $branch has $AHEAD unpushed commits"
  fi
done
```

### 4. Report Findings

If issues are found, report:

- Unmerged remote branches.
- Local branches with unpushed commits.
- Branches whose recent commits do not appear to have a matching
  `.brain/sessions/` summary.

Require an explicit decision for each branch before unrelated work:

- Merge now.
- Keep working, with reason documented for wrap-up.
- Abandoned/delete, only with approval.

If clean, say:

```text
Branch audit complete: no unmerged or unpushed work branches found.
```
