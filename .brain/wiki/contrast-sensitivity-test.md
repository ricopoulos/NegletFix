---
title: Contrast Sensitivity Test (The Measurement Instrument)
last_updated: 2026-04-14
confidence: HIGH
sources:
  - Unity/NeglectFix/Assets/Scripts/Assessment/ContrastSensitivityTest.cs
  - Unity/NeglectFix/Assets/Scripts/Assessment/ContrastTestInput.cs
  - docs/research/contrast_sensitivity_module.md
  - CLAUDE.md
  - .brain/sessions/2025-12-15.md
---

# Contrast Sensitivity Test

The primary objective measurement instrument in NegletFix. A modified Pelli-Robson protocol running in Unity, producing a LogCS score for each of three visual fields (central, right hemifield, left hemifield). This is the test that established [[erics-baseline]] and will track progress through [[audiovisual-training-protocol]].

Implementation: `Unity/NeglectFix/Assets/Scripts/Assessment/ContrastSensitivityTest.cs` (868 lines).

---

## 1. Clinical Protocol — Modified Pelli-Robson

Based on Pelli, Robson & Wilkins (1988) — the gold-standard chart for peak contrast sensitivity.

| Parameter | Value | Source |
|-----------|-------|--------|
| Optotypes | Sloan letters: C D H K N O R S V Z | Pelli et al. 1988 [HIGH] |
| Letter size | 2° visual angle | Pelli-Robson standard |
| Viewing distance | 1 m (virtual) | Pelli-Robson standard |
| Background | 50% gray (RGB 128,128,128) | ~50 cd/m² |
| Target background luminance | 85 cd/m² (Pelli-Robson reference) | `ContrastSensitivityTest.cs:57` |
| Contrast steps | 0.15 log units | Standard PR increment |
| Contrast range | 0.00 to 2.25 LogCS | `ContrastSensitivityTest.cs:50` |
| Letters per level | 3 (triplet) | Standard |
| Pass criterion | 2/3 correct | `ContrastSensitivityTest.cs:66` |
| Display gamma | 2.2 (typical) | `ContrastSensitivityTest.cs:54` |
| Gamma correction | Enabled | `ContrastSensitivityTest.cs:60` |

The LogCS scale is inverted contrast threshold:
- `LogCS = log10(1 / contrast_threshold)`
- LogCS 0.00 = 100% contrast (pure black letter on gray)
- LogCS 1.00 = 10% contrast
- LogCS 2.00 = 1% contrast
- LogCS 2.25 = 0.56% contrast (near-threshold for normal vision)

The conversion is implemented in `LogCSToContrast()` at `ContrastSensitivityTest.cs:397-404`. Weber contrast is applied to compute letter luminance: `L_letter = L_background × (1 - C)` (`ContrastSensitivityTest.cs:411-441`), with inverse-gamma correction to achieve the target display luminance.

---

## 2. Clinical Reference Values

From `docs/research/contrast_sensitivity_module.md:569-575` and Elliott, Sanderson & Conkey (1990):

### Pelli-Robson Normative Data by Age

| Age | Mean LogCS | Range |
|-----|-----------|-------|
| 20-30 | 1.95 | 1.80-2.10 |
| 40-50 | 1.90 | 1.70-2.05 |
| 50-60 | 1.80 | 1.65-1.95 |
| 60-70 | 1.70 | 1.50-1.85 |
| 70+ | 1.55 | 1.35-1.75 |

### Significance Thresholds

- **Minimum detectable change**: 0.15 LogCS (one triplet) [HIGH]
- **Clinically significant improvement**: ≥0.30 LogCS (two triplets) [HIGH, Elliott et al. 1990]
- **Coefficient of repeatability**: ±0.24 LogCS (Elliott et al. 1990)

Interpretation (built into `GetScoreInterpretation()` at `ContrastSensitivityTest.cs:597-604`):
- ≥1.80 → Normal range
- 1.50-1.79 → Mild reduction
- 1.20-1.49 → Moderate reduction
- 0.80-1.19 → Significant reduction
- <0.80 → Severe reduction

---

## 3. Hemifield Testing — The Critical Design

To assess hemianopia, letters must appear in the affected peripheral field, not the preserved central vision. Three test modes (`ContrastSensitivityTest.cs:127-133`):

- **Central** — letters at screen center; tests foveal contrast sensitivity
- **LeftHemifield** — letters offset LEFT; tests Eric's affected field
- **RightHemifield** — letters offset RIGHT; tests Eric's intact field

### The Fixation Cross — Non-negotiable

During hemifield tests, a **red "+" fixation cross** appears at screen center. The user MUST keep eyes fixated on it while letters appear in peripheral vision. Looking directly at the letter would test central vision again and invalidate the hemifield measurement.

- Fixation cross is auto-created dynamically if not wired in Inspector (`ContrastSensitivityTest.cs:165-191`)
- Shown only during LeftHemifield/RightHemifield phases (`ContrastSensitivityTest.cs:359-368`)
- Rendered as sibling of letter display, cross below letter in z-order

### Hemifield Eccentricity

- 3D world offset: ±10° visual angle from center at 1m viewing distance (`GetHemifieldPosition()` at `ContrastSensitivityTest.cs:443-459`)
- UI screen offset: ±300 px default (`hemifieldUIOffset`, `ContrastSensitivityTest.cs:40`)
- Note from `.brain/cross-cutting.md:36-38`: "Desktop testing has arbitrary pixel offsets (300px) — VR will use proper visual angles." The 300px value is a desktop-testing approximation, not a calibrated eccentricity.

---

## 4. The Hemifield Positioning Bug (Fixed 2025-12-15)

**This bug invalidated all contrast testing before December 15, 2025.**

### Root cause
Two positioning systems were in use simultaneously — 3D Transform (`localPosition`) and UI Canvas (`anchoredPosition`) — but only the 3D system received the hemifield offset. The UI system was hardcoded to `Vector2.zero`, which **won**: letters always rendered at screen center regardless of `hemifieldMode` (`CLAUDE.md:87-98`; `.brain/sessions/2025-12-15.md:10-15`).

### Impact
Before the fix, Eric scored highly on "left hemifield" tests because the letters were actually at center, where his vision is intact. The test was lying.

### Fix
Added `GetHemifieldUIOffset()` function (`ContrastSensitivityTest.cs:461-480`) and updated `ShowNextLetter()` to apply the offset to the letter's RectTransform anchoredPosition (`ContrastSensitivityTest.cs:333-345`):

```csharp
float xOffset = GetHemifieldUIOffset(currentHemifield);
rectTransform.anchoredPosition = new Vector2(xOffset, 0);
```

Plus fixation cross creation/display (`ContrastSensitivityTest.cs:165-191`, 357-368) and hemifield tag in console logs (`[LeftHemifield]`, `[RightHemifield]`, `[Central]`) for verification (`ContrastSensitivityTest.cs:347`).

**Verification rule** (`.brain/cross-cutting.md:24-25`): Always check console logs for the hemifield indicator before trusting a score.

---

## 5. How to Run a Valid Test

### Controls (`ContrastSensitivityTest.cs`, `ContrastTestInput.cs`)

| Key | Action |
|-----|--------|
| Letter keys C D H K N O R S V Z | Respond with the letter you see |
| Delete (Mac) / Backspace (Win) | "Can't see" response |
| Space | Start test / advance to next phase |

### Test Sequence (`RunFullSequence()` at `ContrastSensitivityTest.cs:245-284`)

1. **Central** — baseline foveal measurement. Press Space to begin.
2. **Right Hemifield (intact)** — fixate on red "+", letters appear on right.
3. **Left Hemifield (affected)** — fixate on red "+", letters appear on left.

Order rationale: Central first establishes the fixation habit; intact second sets the personal ceiling; affected last (when fixation discipline is best).

### Valid-test checklist

- Console shows the correct `[Hemifield]` tag for each phase
- Eyes remain on the red "+" during peripheral phases — do not chase letters
- Room lighting is consistent between sessions
- Same viewing distance between sessions (currently uncalibrated — see known issues)

---

## 6. Scoring Algorithm

Per-triplet (`EvaluateTriplet()` at `ContrastSensitivityTest.cs:550-576`):
1. Start at `startingLogCS` (default 0.00 = 100% contrast)
2. Show 3 random Sloan letters at current LogCS
3. If ≥2/3 correct → advance LogCS by +0.15 step (harder)
4. If <2/3 → test ends for this hemifield; final score = **last passed level** (`currentLogCS - logCSStep`)
5. If user maxes out at LogCS 2.25 → record 2.25 as final score

Final `ContrastSensitivityResults` object (`ContrastSensitivityTest.cs:810-867`) includes per-hemifield scores, asymmetry (right − left), full trial history, and reaction times.

---

## 7. Known Issues & Calibration Gaps

From `.brain/cross-cutting.md:34-38`:
- **No screen calibration** — viewing distance is assumed 1 m but not enforced; head position affects angular eccentricity
- **Arbitrary 300px hemifield offset** in desktop mode — represents an approximation, not a calibrated visual angle
- **Luminance uncalibrated** — the 85 cd/m² target assumes a properly calibrated display; uncalibrated screens introduce unknown error

When the project moves to Quest VR, these become fixable via known FOV and fixed headset distance (`.brain/decisions.md:30-34`).

---

## 8. Calibration / Debug Mode

`StartCalibrationMode()` (`ContrastSensitivityTest.cs:741-790`) provides interactive verification:
- Up/Down arrows — step through LogCS levels
- G — toggle gamma correction
- C — dump all 16 standard Pelli-Robson levels with gray values
- Escape — exit

For a normal-vision observer, letters should become imperceptible around LogCS 1.8-2.0. If they disappear much earlier, the display is over-dimmed; much later, under-dimmed.

---

## 9. Cross-References

- Baseline results produced by this instrument: [[erics-baseline]]
- Why this measurement matters theoretically: [[scientific-foundation]]
- The intervention whose effect this test will measure: [[audiovisual-training-protocol]]
- Code architecture context: [[unity-architecture]]
- Full paper citations: [[research-papers-index]]
