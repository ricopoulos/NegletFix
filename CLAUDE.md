# NegletFix - Claude Code Context

## **CRITICAL: Read Memory Before Starting**

**STOP. Before doing ANY work, you MUST read these files first:**

```
.brain/index.json          # Module health, baseline results, active issues
.brain/sessions/           # What happened in previous sessions
.brain/crumbs/             # Any mid-session checkpoints
```

This project uses **Agent Brain** - a persistent memory system. Reading `.brain/` gives you:
- Current module/component status
- What was done in previous sessions (including blog-friendly summaries)
- Known patterns and learnings
- Unfinished work or blockers

**Only after reading `.brain/` should you proceed with the user's request.**

### Session End Ritual

When user says "let's wrap up", "end session", or similar:
1. Check `.brain/crumbs/` for any mid-session checkpoints
2. Generate session summary combining crumbs + final work
3. Write summary to `.brain/sessions/YYYY-MM-DD.md` (include Blog Notes section!)
4. Update `.brain/index.json` if module status changed
5. Commit and push to GitHub
6. Get user approval

### Available Commands
- `/crumb` - Save progress checkpoint before context compaction

---

## Project Overview
VR-based rehabilitation system for left homonymous hemianopia with contrast/brightness deficits following right PCA stroke. Combines audiovisual stimulation training with EEG neurofeedback using consumer hardware (Meta Quest 2/3 + Muse headband).

## Eric's Medical Condition
- **Diagnosis**: Left Homonymous Hemianopia from right PCA stroke (July 2021)
- **MRI Finding**: Large area of encephalomalacia in right occipital lobe
- **Symptoms**:
  - Complete left visual field blindness
  - Bilateral dimness/"gray overlay" - "even with a bright day he always sees dark"
  - This is NOT a comfort issue - it's a real daily struggle and rehabilitation target

## Baseline Contrast Sensitivity Results (December 15, 2025)

First validated measurement after fixing hemifield positioning bug:

| Hemifield | LogCS Score | Contrast Threshold | Interpretation |
|-----------|-------------|-------------------|----------------|
| **Central** | ~1.05+ | ~9% | Good central vision |
| **Right (Intact)** | **2.25 LogCS** | 0.6% | Excellent - maxed out test |
| **Left (Affected)** | **0.00 LogCS** | 100% | Severe deficit - can't see ANY letters |

**Asymmetry: 2.25 LogCS** (clinically significant threshold is 0.30 LogCS)

### What This Means
- Right visual field has near-perfect contrast sensitivity
- Left visual field couldn't detect letters even at 100% contrast (pure black on gray)
- This objectively confirms the left homonymous hemianopia
- This is the baseline for tracking rehabilitation progress

### Rehabilitation Target
Based on Daibert-Nido 2021 study, audiovisual training can achieve +0.31 to +0.54 LogCS improvement. Goal is to improve the left hemifield score from 0.00.

## Contrast Sensitivity Test - Key Information

### Test Controls
- **Letter keys** (C, D, H, K, N, O, R, S, V, Z): Respond with the letter you see
- **Delete** (Mac) / **Backspace** (Windows): "Can't see" response
- **Space**: Start test / advance to next phase

### Test Sequence
1. Central (letters at center) - baseline
2. Right Hemifield (letters 300px right) - intact vision test
3. Left Hemifield (letters 300px left) - affected vision test

### Important: Hemifield Testing
- Red fixation cross (+) appears in center during hemifield tests
- User MUST keep eyes on fixation cross
- Letters appear in peripheral vision - don't chase them with eyes

## Code Fixes Applied (December 15, 2025)

### Critical Bug Fixed: Hemifield Positioning
**Problem**: Letters were always displayed at center regardless of hemifield mode.
- Line 275 set `letterTransform.localPosition` for 3D
- Line 292 ALWAYS set UI `anchoredPosition = Vector2.zero`

**Solution**: Added `GetHemifieldUIOffset()` function and proper UI positioning:
- Central: 0px offset (center)
- Right Hemifield: +300px (right side)
- Left Hemifield: -300px (left side)

### Files Modified
- `Assets/Scripts/Assessment/ContrastSensitivityTest.cs`:
  - Added `hemifieldUIOffset` parameter (300px default)
  - Added `GetHemifieldUIOffset()` function
  - Fixed `ShowNextLetter()` to apply hemifield offset to RectTransform
  - Added fixation cross creation and display logic
  - Added hemifield indicator to console logs: `[LeftHemifield]`, `[RightHemifield]`

## Unity Project Location
`/Users/ericlespagnon/Dropbox/DEV-LOCAL/NegletFix/Unity/NeglectFix/`

## Key Research Reference
- **Daibert-Nido et al. (2021)**: "Home-Based Visual Rehabilitation in Patients With Hemianopia"
- DOI: 10.3389/fneur.2021.680211
- Total training time: Under 5 hours
- Results: +0.31 to +0.54 LogCS improvement in contrast sensitivity

## Next Steps
1. Implement audiovisual rehabilitation training module
2. Track progress with periodic contrast sensitivity assessments
3. Compare results to baseline (Left: 0.00, Right: 2.25)
