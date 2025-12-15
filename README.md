# Rebuilding My Brain

**A stroke survivor's open-source journey to rehabilitate his own vision using VR and neurofeedback.**

![Status](https://img.shields.io/badge/Status-Active%20Development-blue)
![Unity](https://img.shields.io/badge/Unity-6.2-black)
![License](https://img.shields.io/badge/License-Personal%20Research-orange)

---

## The Story

In July 2021, I had a stroke. Right posterior cerebral artery. Hemorrhagic transformation. The MRIs show a large area of encephalomalacia in my right occipital lobe - the part of your brain that processes vision.

The result: **left homonymous hemianopia**. My entire left visual field is gone. Not dark, not blurry - just *absent*. Plus this persistent "gray overlay" that makes everything feel dimmer than it should be.

I'm 52. I run a windsurfing business. I'm not ready to just accept this.

So I'm building my own rehabilitation system.

**NeglectFix** combines consumer VR (Meta Quest), EEG neurofeedback (Muse headband), and evidence-based audiovisual training protocols to try something the medical system doesn't offer: targeted, daily, closed-loop brain training designed specifically for my deficit.

I'm both the developer and the patient. Every line of code is personal.

---

## What's Actually Built

This isn't vaporware. Here's what exists and works:

### V0: EEG Neurofeedback System (Complete)
Real-time brain monitoring that tracks engagement via beta/alpha ratios at TP10 (right parietal cortex).

| Script | Lines | Status |
|--------|-------|--------|
| `EEG/MuseOSCReceiver.cs` | 167 | Working |
| `EEG/EngagementCalculator.cs` | 315 | Working |
| `EEG/EEGSimulator.cs` | 145 | Working (press E/R to test) |
| `Utils/DataLogger.cs` | 281 | Ready |
| `Utils/GazeDetector.cs` | 164 | Ready |
| `Utils/RewardController.cs` | 332 | Ready |
| `Tasks/TaskManager.cs` | 325 | Ready |

**The closed-loop concept:**
```
Muse EEG → Mind Monitor app → OSC → Unity → Quest VR
     ↓
If (brain engaged + looking left) → Reward
     ↓
Neuroplastic reinforcement of left-awareness
```

### V1: Contrast Sensitivity Assessment (Baseline Complete ✓)
Before training, you need to measure. This module implements a modified Pelli-Robson test with hemifield-specific measurements.

| Script | Lines | Status |
|--------|-------|--------|
| `Assessment/ContrastSensitivityTest.cs` | ~650 | **Baseline recorded Dec 15, 2025** |
| `Assessment/ContrastTestInput.cs` | ~260 | Working |
| `Assessment/ContrastResultsUI.cs` | 349 | Ready to wire |

**First Validated Baseline (Dec 15, 2025):**

| Visual Field | LogCS Score | Contrast | Result |
|--------------|-------------|----------|--------|
| Central | ~1.05+ | ~9% | Good |
| Right (intact) | **2.25** | 0.56% | Maxed the test |
| Left (affected) | **0.00** | 100% | Total failure |

**Asymmetry: 2.25 LogCS** - Clinically significant threshold is ≥0.30

This confirms the left homonymous hemianopia and establishes a measurable baseline for tracking rehabilitation progress. Target improvement: +0.31 to +0.54 LogCS based on Daibert-Nido 2021 study.

### Coming Next: Cross-Modal Audiovisual Training
The science is clear: spatiotemporally synchronized sound + visual stimuli can activate the preserved retino-collicular pathway (superior colliculus) in hemianopia patients. The brain can learn to "see" in the blind field when given the right training.

**Key parameters from the research:**
- AV sync: ≤16ms (superior colliculus binding window)
- Looming sound: 400Hz, 250ms, 55→75dB exponential rise
- Training eccentricities: 8°, 24°, 40°, 56°
- Session length: 15 min (3×5min blocks)

---

## The Science

This isn't wishful thinking. Each component is based on validated research:

**VR Therapy for Neglect** - Meta-analysis of 29 studies, 1,561 patients showed significant improvements
> "VR-based visual exploration therapy demonstrates efficacy in hemispatial neglect rehabilitation following stroke."
> — Frontiers in Neuroscience (2023)

**EEG Neurofeedback** - 20 min/day for 6 days showed measurable alpha reduction in neglect patients
> "Patients demonstrated successful neurofeedback learning, shown by improved regulation of alpha oscillations from right posterior parietal cortex."
> — Ros et al., Neural Plasticity (2017)

**Closed-Loop BCI** - Real-time feedback more effective than open-loop for stroke rehabilitation
> "Patients with severe motor impairments benefited most from EEG-based neurofeedback in VR."
> — REINVENT Platform, Frontiers (2019)

**Cross-Modal Plasticity** - The superior colliculus pathway is preserved in cortical blindness
> "Multisensory integration in SC produces supra-additive responses, enabling detection of stimuli below threshold."
> — Multiple studies (Bolognini, Dundon, Tinelli)

See [RESEARCH_SUMMARY.md](RESEARCH_SUMMARY.md) for complete bibliography.

---

## Hardware (~$500 total)

- **Meta Quest 2** - VR headset ($250)
- **Muse 2 or Muse S** - EEG headband ($250)
- **Mobile device** - For Mind Monitor app (iOS/Android, ~$15)
- **VR-capable computer** - Mac or PC for Unity development

---

## Project Structure

```
NeglectFix/
├── README.md                 ← You are here
├── CLAUDE.md                 ← Project context for AI assistance
├── .brain/                   ← Agent Brain memory system
│   ├── index.json            ← Module status, baseline results
│   ├── cross-cutting.md      ← Patterns, medical context
│   ├── decisions.md          ← Architecture decisions (the "why")
│   └── sessions/             ← Session summaries with blog notes
│
├── docs/
│   └── research/             ← Scientific foundation
│
├── Unity/NeglectFix/         ← Unity 6.2 project
│   └── Assets/Scripts/
│       ├── EEG/              ← Muse integration
│       ├── Assessment/       ← Contrast sensitivity testing
│       ├── Utils/            ← Logging, rewards, gaze
│       └── Tasks/            ← Rehabilitation exercises
│
├── QUICK_START.md            ← 30-min OSC test guide
├── NEUROFEEDBACK_PROTOCOL.md ← Training parameters
└── VR_REHABILITATION_TASKS.md ← Task designs
```

---

## Development Log

| Date | Milestone |
|------|-----------|
| Oct 2025 | V0 complete: 7 scripts, full EEG pipeline |
| Dec 5 | Claude Code onboarding, project cleanup |
| Dec 12 | Contrast Sensitivity module implemented (1,190 LOC) |
| Dec 13 | First test sequence runs in Unity |
| Dec 14 | Bug hunting - discovered hemifield positioning issue |
| Dec 15 | **Critical fix:** Letters now appear in correct visual fields |
| Dec 15 | **First validated baseline!** Right: 2.25, Left: 0.00 LogCS |
| Dec 15 | Agent Brain memory system for progress tracking |

---

## Current Status (December 2025)

**Working:**
- Unity 6.2 project compiles and runs
- EEG simulator (no hardware needed to test)
- Contrast sensitivity test with hemifield positioning
- Red fixation cross for valid peripheral vision testing
- Keyboard input for all Sloan letters
- **First baseline recorded** - tracking starts now

**Completed:**
- Hemifield positioning bug fixed (letters appear at correct screen positions)
- Left vs right hemifield asymmetry measured: 2.25 LogCS difference
- Agent Brain memory system (`.brain/`) for session tracking

**Next:**
- Audiovisual training module (cross-modal rehabilitation)
- Weekly contrast sensitivity re-assessments to track progress
- Quest VR deployment for immersive testing
- Real Muse EEG integration

---

## Disclaimer

This is personal research, not a medical device.

- Not FDA approved
- Not a replacement for professional care
- Not suitable for clinical use without validation

But it's based on validated individual components, it's fully documented, and if it helps even a little, it's worth building.

**If you're a hemianopia patient, a researcher, or just curious - follow along. Ask questions. Maybe contribute.**

---

## Follow the Journey

I'm documenting this project as I build it. The technical parts, the frustrations, the breakthroughs, and the personal side of rehabilitating your own brain.

**Blog: [Rebuilding My Brain](https://ricopoulos.com/rebuilding-my-brain)** *(coming soon)*

---

## Contact

This is an open research project. Feel free to:
- Replicate the system
- Adapt for your own rehabilitation needs
- Share findings with the neurorehab community
- Contribute improvements

**GitHub**: [ricopoulos/NegletFix](https://github.com/ricopoulos/NegletFix)

---

## Acknowledgments

**Research pioneers:** Ros et al., REINVENT team, Bolognini, Dundon, Tinelli, and all VR neurorehabilitation researchers.

**Tools:** InteraXon (Muse), Meta (Quest), Unity Technologies, extOSC, Mind Monitor.

**AI assistance:** Claude (Anthropic) for research synthesis, code review, and documentation.

---

*Last updated: December 15, 2025*
