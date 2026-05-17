---
title: Rehabilitation Roadmap
last_updated: 2026-05-14
confidence: MIXED
sources:
  - .brain/index.json
  - .brain/backlog.md
  - .brain/sessions/2025-12-15.md
  - CLAUDE.md
  - PROJECT_SUMMARY.md
  - NEUROFEEDBACK_PROTOCOL.md
  - 2026-05-14 PubMed-verified audit — see [[research-papers-index]] Recent Additions
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
- Daibert-Nido 2021 protocol parameters extracted and documented (since reframed — see below)
- Ros et al. 2017 neurofeedback target identified (TP10, ↓α ↑β)
- See [[research-papers-index]], [[scientific-foundation]]

### Brain v2.0 + Obsidian bridge sync (2026-03-21)
- Memory system upgraded to v2 methodology
- Session rituals (`/start-session`, `/end-session`, `/crumb`, `/branch-audit`) operational

### 2026-05-14 — Research audit + literature expansion
- 4-agent parallel audit + PubMed-verified literature pass identified 20+ new papers and corrected 3 citation errors + 1 misframed anchor
- Daibert-Nido 2021 downgraded from HIGH "anchor" to LOW–MEDIUM (N=2 pediatric pilot, not adult stroke trial)
- Strong new evidence for chronic responsiveness: **Namgung 2024/2025** (n=82 RCTs each), **Alharshan/Alwashmi 2026** (n=15 stroke HH, DTI mechanism), **Raffin 2025** (cf-tACS chronic, n=16), **Diana 2025** (AV+tDCS chronic, n=18)
- Critical reality-check evidence: **Yang/Cavanaugh/Saionz 2023** (only 58% of chronic patients respond, blind-field CS stays ~4× lower than intact), **Saionz 2025 TVST** natural history (chronic VF stable), **Treves 2025 JMIR** consumer-NF meta (null under sham)
- Hemianopia-vs-neglect category mismatch flagged in [[eeg-neurofeedback]] — EEG-NF layer is now framed as exploratory adjunct
- See [[research-papers-index]] Recent Additions section for full citations

### 2026-05-16 — Quest dev mode resolved + Phase 1 Unity scaffold built
**Quest 2 dev mode** (April 27 blocker, finally closed):
- Verified the "only the owner can do it" error refers to the headset's primary Meta account, not the dev org owner (April 27 misdiagnosis retired)
- Working fix: previous owner unpaired from her Meta Horizon app → factory reset Quest 2 → re-paired to Eric's Meta account → dev mode toggled → adb authorized
- Quest 2 serial `1WMHH831TR1047`, Android 14, current firmware

**Phase 1 Unity scaffold built** (commits `ecf327f` + `7ea389c` + `24a3075` on `main`):
- New scripts in `Tasks/`: `AudioVisualTraining.cs` (Paradigm B main task), `ProgramScheduler.cs` (session state JSON), `EccentricityProgression.cs` (baseline-driven ladder)
- `DataLogger.cs` extended with `LogTrainingTrial()` + per-session AV trial CSV
- `RewardController.cs` decoupled from EEG — `RewardMode` enum with `OpenLoop` (v1 default) and `EegGated` (v2 reserved)
- `Packages/manifest.json` — added XR Management 4.5.0, OpenXR 1.14.0, Interaction Toolkit 3.0.7
- XR Plug-in Management configured: OpenXR enabled for Android, Oculus Touch Controller Profile, Meta Quest Support feature enabled
- Project Validation: 14/15 checks passing (SSAO is a deferred polish item)
- Unity 6.2 imported the project on first open with **zero errors, zero warnings**

**Build plan deliverable**: `docs/build-plan-2026-05-16.html` — styled HTML build plan capturing the paradigm decision (Both A+B sequential), the EEG deferral, the 5-phase build sequence, and honest expectations.

**Open from 2026-05-16**:
- Smoke test the AV training in Editor (fresh-head task — deferred to next session)
- Quest controller input binding via InputSystem (replaces keyboard SPACE fallback in `DetectResponse()`)
- Quest 3 acquisition (budget-dependent)

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
Three dose options, picked based on Eric's chronic profile (see [[audiovisual-training-protocol]] §1):
- **Option A — Daibert-Nido 2021 dose** (~5 hr total over 6-7 weeks): the original pediatric-pilot dose. Probably under-dosed for chronic adult.
- **Option B — Misawa/Daibert-Nido 2024 dose** (~5.25 hr, Meta Quest at home): same total budget, pediatric Quest validation.
- **Option C — Alharshan/Alwashmi 2026 dose** (~15 hr, 30 min × 5 days/wk × 6 weeks): **the only direct adult-stroke evidence**. Recommended for Eric.

Expected outcomes (revised 2026-05-14):
- Primary: ≥+0.30 LogCS at scotoma-border locations in ~50-60% of responders (Yang/Cavanaugh/Saionz 2023, n=12 chronic)
- Secondary: detection-RT improvements, scanning efficiency, ADL transfer
- **Not realistic**: closing the left-hemifield 0.00 → right-hemifield 2.25 asymmetry — chronic blind-field CS stays ~4× lower than intact even after training

### EEG integration (optional, Phase 2)
Currently `status: planned`, `health: gray` in `.brain/index.json:16-20`:
- Wire Muse → Mind Monitor → Unity via real (not simulated) OSC
- Uncomment `#define EXTOSC_INSTALLED` after installing extOSC
- First 2-3 sessions: baseline EEG without closed-loop, to validate signal quality on Eric's head
- Then enable closed-loop gating of audiovisual rewards

---

## 3. UNKNOWN / AT RISK [LOW — be honest]

### Potential adjuncts (not currently in the build, worth tracking)
Added 2026-05-14:
- **cf-tACS V1↔MT** (Raffin 2025 Brain, n=16 chronic): forward cf-tACS + motion training expanded kinetic field borders faster than typical VPL. **Not actionable for home setup** — tACS hardware separate, expert placement required, no consumer-grade equivalent. But a strong "if we ever get clinic time" option.
- **Anodal tDCS over ipsilesional occipital cortex** (Diana 2025 Eur J Neurol, n=18 chronic): added value over AV training alone, 2 h/day × 10 days. tDCS hardware ~€500–2000 for consumer/research-grade kits (HeadCap, Soterix). Discuss with Eric's neurologist before any DIY adoption — current is small but non-zero risk.
- **Pharmacological adjuncts**: see [[pharmacological-adjuncts]] (already separate). Levodopa, fluoxetine, citicoline — all have weak/conflicting evidence for chronic visual rehab; neurologist conversation territory only.

### Muse signal quality on Eric specifically [DOWNGRADED to HIGH risk, 2026-05-14]
Ros et al. 2017 used medical-grade EEG. Muse is consumer-grade dry-electrode. **Sokolova 2024 + Pavlov 2024 device comparisons rank Muse S last among consumer EEG devices on signal quality** — log-SNR significantly lower than research-grade, weak alpha-spindle detection even eyes-closed. Treves 2025 JMIR meta (16 RCTs, 11 used Muse) showed null cognitive effects under sham control. The combination — weak signal + weak meta-analytic effects — argues for treating EEG-NF as exploratory only. **First session priority**: validate signal quality on Eric's head (visible eyes-open vs eyes-closed alpha modulation) before counting Muse data.

### Whether Eric is a "neurofeedback responder"
Not everyone learns EEG self-regulation. The literature on neurofeedback responders vs. non-responders is imperfect. Even for responders, learning curve is 3-10+ sessions (Ros et al. 2017; `NEUROFEEDBACK_PROTOCOL.md:321-326`). If Eric is a non-responder, the project's closed-loop layer adds no value — but the open-loop audiovisual layer (which worked in Daibert-Nido without EEG) still should. [MEDIUM risk — failure mode has a fallback]

### Whether Daibert-Nido gains reproduce in a chronic adult [revised 2026-05-14]
The published +0.31 to +0.54 LogCS range was the **cohort mean of N=2 pediatric brain-tumor survivors**, not adult stroke. Yang/Cavanaugh/Saionz 2023 (n=12 chronic adult stroke) found only 58% of patients responded at any trained location and blind-field CS stayed ~4× lower than intact-field after training. Realistic expectation for Eric: **~50-60% chance of any measurable gain; magnitude likely 0.20-0.40 LogCS at scotoma-border locations if responding, not full restoration**. Clinically significant change is still ≥0.30 LogCS (Elliott/Bullimore/Bailey 1991), so a responder outcome still counts as success. [HIGH risk for magnitude, MEDIUM risk for direction (was LOW)]

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
