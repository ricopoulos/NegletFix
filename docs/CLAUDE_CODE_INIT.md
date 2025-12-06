# Claude Code Initialization Prompt

**Copy and paste this into your first Claude Code session after committing the `/docs` folder:**

---

## Prompt:

I've set up a new documentation workflow for this project. Please read and acknowledge:

1. **Read `/docs/CLAUDE_CONTEXT.md`** - This is the master brief about the project, who I am (I'm the patient AND developer), and critical implementation parameters.

2. **Check `/docs/sessions/`** - These are summaries from my brainstorming sessions with Claude chat. The most recent one will have context about what we discussed and decided.

3. **Going forward**:
   - At the start of significant sessions, glance at recent session notes for context
   - When we make important architectural decisions, note them in `/docs/decisions/`
   - If you discover something important during implementation that should inform future brainstorming, mention it so I can add it to the docs

4. **About this project**: 
   - NeglectFix is a VR neurorehabilitation system for my own left hemianopia from stroke
   - V0 (EEG neurofeedback) is complete
   - V1 (cross-modal audiovisual rehabilitation) is in development
   - The parameters in CLAUDE_CONTEXT.md come from validated clinical research - don't modify them without discussion

Please confirm you've read the context files and understand the workflow. Then let me know what you see as the current state and next steps based on the documentation.

---

## What This Does

When Claude Code reads `CLAUDE_CONTEXT.md`, it will understand:
- You're both developer AND patient
- The scientific foundation (SC pathway, why AV sync matters)
- Critical parameters that shouldn't be changed casually
- Where to find additional context (session notes, research docs)

This creates continuity between your brainstorming sessions and implementation work.
