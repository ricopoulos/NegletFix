# Architectural Decisions

> Document the "WHY" behind major technical choices.

---

## Why Contrast Sensitivity Testing First?

**Decision**: Build contrast sensitivity assessment before rehabilitation training

**Rationale**:
- Need objective baseline to measure progress
- Validates the visual deficit quantitatively
- Daibert-Nido study used contrast sensitivity as primary outcome measure

**Trade-offs**: Delays the "fun" rehabilitation tasks, but ensures we can track real progress

**Revisit If**: Baseline is established and we need to prioritize training features

---

## Why UI-Based Letter Display (Not 3D World)?

**Decision**: Use TextMeshPro on Canvas for letter display, not 3D text in world space

**Rationale**:
- Easier to control exact screen positioning for hemifield testing
- Font rendering is cleaner and more consistent
- Works identically in Editor and VR

**Trade-offs**: Need to convert visual angles to pixel offsets (300px currently hardcoded)

**Revisit If**: Moving to VR where we can use proper visual angle calculations with known FOV

---

## Why Fixation Cross for Hemifield Tests?

**Decision**: Display red "+" at center during left/right hemifield tests

**Rationale**:
- User must keep eyes fixed on center for valid peripheral vision testing
- Without fixation, user might look directly at the letter (defeats purpose)
- Standard practice in clinical visual field testing

**Trade-offs**: Adds complexity, user must understand not to chase letters

**Revisit If**: Adding eye tracking to enforce/validate fixation

---

*Last updated: 2025-12-15*
