---
title: Rehabilitation Roadmap
last_updated: 2026-04-14
confidence: MIXED
sources:
  - .brain/index.json
  - .brain/backlog.md
  - .brain/sessions/2025-12-15.md
  - CLAUDE.md
  - PROJECT_SUMMARY.md
  - NEUROFEEDBACK_PROTOCOL.md
---

# Rehabilitation Roadmap

Honest progress map. What has been done, what is next, and what is genuinely unknown or at risk. Confidence is intentionally mixed: what's DONE is high (measured or committed); what's NEXT is medium (planned, parameters known, untested on Eric); what's HOPED FOR is low to unknown.

See [[erics-baseline]] for the measured starting point, [[audiovisual-training-protocol]] for the main intervention, [[unity-architecture]] for code readiness.

---

## 1. DONE — Measured / Committed [HIGH]

### Baseline measurement established (2025-12-15)
- Central ~1.05+ LogCS, Right 2.25 LogCS (maxed), Left 0.00 LogCS, Asymmetry 2.25
- First validated measurement — all prior tests invalidated by hemifield bug
- Commit `cddc43c` on `main`
- See [[erics-baseline]]

### Hemifield positioning bug fixed (2025-12-15)
- Letters now correctly render in left/right peripheral positions, not always at center
- Fixation cross added for valid hemifield testing
- Console tags (`[Central]`, `[LeftHemifield]`, `[RightHemifield]`) for verification
- See [[contrast-sensitivity-test]]#the-hemifield-positioning-bug-fixed-2025-12-15

### Unity project scaffold
- 10 C# scripts across Assessment, EEG, Tasks, Utils (3,339 LOC)
- MuseOSCReceiver + EngagementCalculator + RewardController + GazeDetector + DataLogger wired
- EEG simulator works for development without hardware (with the `OnKeyPress()` caveat noted in [[unity-architecture]]#known-code-issues)
- Unity 6.2 VR template + GitHub repo operational
- See [[unity-architecture]]

### Research synthesis
- 15+ papers catalogued, evidence graded
- Daibert-Nido 2021 protocol parameters extracted and documented
- Ros et al. 2017 neurofeedback target identified (TP10, ↓α ↑β)
- See [[research-papers-index]], [[scientific-foundation]]

### Brain v2.0 + Obsidian bridge sync (2026-03-21)
- Memory system upgraded to v2 methodology
- Session rituals (`/start-session`, `/end-session`, `/crumb`, `/branch-audit`) operational

---

## 2. NEXT — Planned, Ready to Build [MEDIUM]

### WIP-001: Audiovisual training module — PAUSED, ready to resume

From `.brain/backlog.md:12-28`:
- **Status**: PAUSED (not blocked)
- **Context**: Main rehabilitation module based on Daibert-Nido 2021
- **Unblocked by**: baseline measurement completed 2025-12-15
- **Resume path**:
  1. Review baseline (Left 0.00, Right 2.25 LogCS) — done
  2. Design audiovisual task using parameters in [[audiovisual-training-protocol]]
  3. Implement in Unity as `Tasks/AudioVisualTraining.cs` extending `TaskManager`
  4. Integrate `ConfigureFromContrastResults()` personalization hook

See [[audiovisual-training-protocol]]#what-to-build-in-unity for build spec.

### Periodic re-assessment cadence
Per [[contrast-sensitivity-test]] and [[audiovisual-training-protocol]]:
- **Every ~5 training sessions**: quick-check (Central + Left only) — monitor affected-field trajectory
- **Post session 20**: full 3-hemifield reassessment — compare to baseline
- Append each to [[erics-baseline]]#progress-log, never overwrite

### Quest VR deployment (next major)
- Test the contrast sensitivity test in actual Quest headset
- Replace ±300px UI offset with proper visual-angle positioning (known FOV enables calibrated eccentricity)
- Verify fixation cross and letter rendering are visible at expected contrasts on Quest OLED (~100 cd/m²)
- See `.brain/index.json:35` — "Quest VR deployment for immersive testing"

### Audiovisual training program
Per Daibert-Nido protocol:
- ~20 sessions over 6-7 weeks
- 15 min each (3 × 5 min blocks), every 2 days
- <5 hours total training time
- Expected outcome: +0.31 to +0.54 LogCS improvement on left hemifield

### EEG integration (optional, Phase 2)
Currently `status: planned`, `health: gray` in `.brain/index.json:16-20`:
- Wire Muse → Mind Monitor → Unity via real (not simulated) OSC
- Uncomment `#define EXTOSC_INSTALLED` after installing extOSC
- First 2-3 sessions: baseline EEG without closed-loop, to validate signal quality on Eric's head
- Then enable closed-loop gating of audiovisual rewards

---

## 3. UNKNOWN / AT RISK [LOW — be honest]

### Muse signal quality on Eric specifically
Ros et al. 2017 used medical-grade EEG. Muse is consumer-grade dry-electrode. The band-power patterns are robust in principle, but Eric's specific head shape, hair, skin, and fit tolerance are unknown. First real Muse session could reveal unusable signal — or could work fine. Need to try. [MEDIUM risk]

### Whether Eric is a "neurofeedback responder"
Not everyone learns EEG self-regulation. The literature on neurofeedback responders vs. non-responders is imperfect. Even for responders, learning curve is 3-10+ sessions (Ros et al. 2017; `NEUROFEEDBACK_PROTOCOL.md:321-326`). If Eric is a non-responder, the project's closed-loop layer adds no value — but the open-loop audiovisual layer (which worked in Daibert-Nido without EEG) still should. [MEDIUM risk — failure mode has a fallback]

### Whether Daibert-Nido gains reproduce in a single patient
The published range (+0.31 to +0.54 LogCS) is a cohort mean. Individual responders varied. Eric might gain more, less, or not at all. Clinically significant change is ≥0.30 LogCS (Elliott et al. 1990), so even modest gains count. [HIGH risk for magnitude, LOW risk for direction]

### Whether gains transfer to daily-life function
Contrast sensitivity is an eccentricity-specific, luminance-specific lab number. It may improve on the test without Eric subjectively noticing at dinner. Literature is mixed on transfer; Topics in Stroke Rehab 2020 argues ecological VR helps transfer, but that's indirect evidence. This is the biggest unknown — and the one that matters most to Eric. [HIGH uncertainty]

### The "gray overlay" hypothesis
Whether audiovisual training improves Eric's bilateral dimness is completely theoretical — see [[scientific-foundation]]#relevance-to-erics-specific-case. The Central LogCS in the progress log is the only way to test it. [THEORETICAL, unvalidated]

### Calibration drift
Display brightness, viewing distance, ambient light all affect contrast measurements. Without a calibrated test rig, small month-over-month changes could be noise not signal. Mitigation: re-run Central test frequently as a calibration check — if Central drifts without Left/Right changing, it's likely display-side. [MEDIUM risk, detectable]

---

## 4. Honest Summary

| Layer | Status | Evidence | Confidence |
|-------|--------|----------|------------|
| Instrument (contrast sensitivity test) | Working | Bug fixed, produces valid per-hemifield LogCS | HIGH |
| Baseline | Measured | Central ~1.05, Right 2.25, Left 0.00 | HIGH |
| Code for audiovisual training | Not yet written | Parameters fully specified | — |
| Code for EEG pipeline | Written, untested with real Muse | Simulator path works | MEDIUM |
| Audiovisual training effect on Eric's left hemifield | Not yet attempted | Daibert-Nido cohort +0.31-0.54 LogCS | MEDIUM predict |
| Effect on central dimness ("gray overlay") | Not attempted | Theoretical only | UNKNOWN |
| Transfer to daily function | Not attempted | Mixed literature | UNKNOWN |

The project has the hardware, the research base, the instrument, and the baseline. What's missing is the intervention code and 6-7 weeks of disciplined Eric-time.

---

## 5. Critical Path

1. **Build `Tasks/AudioVisualTraining.cs`** — see [[audiovisual-training-protocol]]#what-to-build-in-unity
2. **Pilot 2-3 open-loop sessions** in Unity Editor (no Muse required yet)
3. **Mid-pilot re-test** — run full contrast sensitivity test, append to [[erics-baseline]]#progress-log, check signal in left-hemifield score
4. **Wire Muse for real** — install extOSC, confirm signal quality on Eric
5. **Enable closed-loop** — add engagement gating to the training task
6. **Complete the 20-session Daibert-Nido program** with mid-course quick-checks every 5 sessions
7. **Post-program full re-assessment** — compare to 2025-12-15 baseline
8. **Document outcome honestly** — whatever the result is, log it

---

## 6. Cross-References

- Where we start: [[erics-baseline]]
- The instrument that tracks us: [[contrast-sensitivity-test]]
- The main intervention: [[audiovisual-training-protocol]]
- The optional closed-loop layer: [[eeg-neurofeedback]]
- Theoretical backbone: [[scientific-foundation]]
- Build details: [[unity-architecture]]
- Hardware readiness: [[hardware-setup]]
