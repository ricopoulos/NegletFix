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

## Key Research Reference

**Daibert-Nido et al. (2021)**: "Home-Based Visual Rehabilitation in Patients With Hemianopia"
- DOI: 10.3389/fneur.2021.680211
- Protocol: 15 min sessions, every 2 days, 6-7 weeks
- Results: +0.31 to +0.54 LogCS improvement
- Total training time: Under 5 hours

---

*Last updated: 2025-12-15*
