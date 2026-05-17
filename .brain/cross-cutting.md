# Cross-Cutting Learnings

> Issues, patterns, and knowledge that span multiple modules or the whole project.

---

## Medical Context

**Patient**: Eric Lespagnon
**Condition**: Left Homonymous Hemianopia from right PCA stroke (July 2021)
**Key Symptoms**:
- Complete left visual field blindness
- Bilateral dimness/"gray overlay" - NOT a comfort issue, a real daily struggle
- "Even with a bright day he always sees dark"

**This is a rehabilitation project, not accommodation.**

---

## Patterns

### UI Positioning for Hemifield Testing
- 3D Transform positioning (`localPosition`) is separate from UI Canvas positioning (`anchoredPosition`)
- Must apply offsets to BOTH if using mixed 3D/UI elements
- Always verify positioning by checking which hemifield shows in logs

### Contrast Sensitivity Testing
- LogCS scale: 0.0 = 100% contrast (black), 2.25 = 0.56% contrast (nearly invisible)
- Clinically significant change: ≥0.30 LogCS
- Pelli-Robson standard: 2/3 correct in triplet to pass level

---

## Known Issues

- Desktop testing has arbitrary pixel offsets (300px) - VR will use proper visual angles
- No screen calibration yet - viewing distance affects accuracy

---

## Environment

- **Unity Version**: 6.2 (6000.2.8f1) with VR template
- **Target Platform**: Meta Quest 2/3 (Android ARM64)
- **EEG Hardware**: Muse headband via Mind Monitor OSC (port 5000)
- **Development**: macOS, Unity Editor testing

---

## Key Research References

**Reframed 2026-05-14**: Daibert-Nido 2021 was previously labeled the "anchor paper" with HIGH confidence and +0.31–0.54 LogCS as the expected outcome. After the May 14 PubMed-verified audit, it's now correctly framed as an **N=2 pediatric brain-tumor pilot**, not a powered adult-stroke trial. See `wiki/research-papers-index.md` Recent Additions section for the full audit + 20+ new verified citations.

**Daibert-Nido et al. (2021)**: Foundational pilot. 3D-MOT-IVR paradigm in immersive VR.
- DOI: 10.3389/fneur.2021.680211
- **N=2 pediatric** brain tumor survivors (not adult stroke)
- ~5 hours total training time; results plausible but unreplicated at scale

**Alharshan/Alwashmi et al. (2026 NeuroImage)**: Direct adult-stroke evidence — closest to Eric's case.
- PMID 41421499
- N=15 post-stroke hemianopia, immersive AV training, 30 min/day × 5 days/wk × 6 weeks (15 hr total)
- DTI evidence of microstructural change in occipital/thalamus/MTG
- **This is the recommended dose for Eric's Phase 2 program** (Alharshan dose)

**Namgung et al. (2024 + 2025)**: Chronic VR-VPL multicenter RCTs.
- Brain Behav 2024 + JAMA Netw Open 2025, n=82 each, chronic >3-6 mo post-stroke
- Strongest published RCTs in Eric's exact population (chronic stroke VFD)
- 5 days/week × 12 weeks (~60 sessions). Personalized stimulus matters (2025 paper)

**Yang/Cavanaugh/Saionz et al. (2023)**: Chronic CS reality check.
- PMC10491352 (medRxiv preprint), Huxlin lab
- n=12 chronic V1 stroke. Only 7/12 (58%) responded at any trained location
- Blind-field CS stays ~4× lower than intact-field even after training
- Sets honest expectation: 2.25 LogCS asymmetry will not fully close

---

## Phase 1 Milestone (2026-05-16)

NegletFix went from "all wiki, no code" to "code on disk, compiles clean" in one session:
- April 27 Quest dev mode blocker resolved (factory-reset + re-pair to Eric's Meta account; old org-membership theory was wrong)
- 3 new scripts in `Tasks/`: `AudioVisualTraining.cs` (Paradigm B), `ProgramScheduler.cs` (state JSON), `EccentricityProgression.cs` (CS-driven ladder)
- `DataLogger.cs` extended with trial CSV
- `RewardController.cs` decoupled — `RewardMode.OpenLoop` default, EEG-gated mode preserved for v2
- Unity 6.2 imported clean, XR Plug-in Management configured (OpenXR + Meta Quest Support)
- 6 commits pushed to `main`
- Build plan: `docs/build-plan-2026-05-16.html`

Open: smoke test in Editor, Quest controller input binding, SSAO disable, Quest 3 acquisition.

---

*Last updated: 2026-05-16*
