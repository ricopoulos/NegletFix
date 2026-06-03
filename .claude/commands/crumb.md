# /crumb - Drop a Breadcrumb Checkpoint

Use when:

- Context is getting high.
- A significant milestone has been completed.
- A long-running operation is about to start.
- The user says "drop a crumb".

## Write Checkpoint

Path:

```text
.brain/crumbs/YYYY-MM-DD-HHMM.md
```

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

Confirm the path to the user, then continue working unless the user asks to
pause.

At session end, read crumbs created during the session and use them to build the
session summary. Keep crumbs permanently; they capture granular evidence that
does not always fit in the summary.
