# NeglectFix - Claude Code Context

> **Read this file first when working on this project.**

## What Is This Project?

NeglectFix is a personal VR neurorehabilitation system for **left homonymous hemianopia** following right PCA stroke. The developer (Eric) is both the creator AND the patient - he's building this to treat his own visual deficits.

## The Person Behind This

- **Condition**: Right PCA stroke (July 2021) with hemorrhagic transformation
- **Damage**: Large encephalomalacia in right occipital lobe
- **Visual deficit**: Left homonymous hemianopia (blind left visual field)
- **Subjective symptom**: "Gray overlay" - bilateral dimness affecting entire visual experience
- **Phase**: Chronic (3+ years post-stroke)
- **Profession**: Self-employed windsurfing business owner

## Current Architecture

### V0 (Complete) - EEG Neurofeedback System
- Muse EEG headband → Mind Monitor → Unity via OSC
- Tracks beta/alpha engagement ratio at TP10 (right parietal)
- 7 C# scripts for closed-loop neurofeedback
- See `/Unity/NeglectFix/Assets/Scripts/`

### V1 (In Development) - Cross-Modal Audiovisual Rehabilitation
Building on V0, adding validated audiovisual training that leverages the **superior colliculus pathway**.

**Key scientific insight**: The retino-collicular pathway is preserved in hemianopia. Spatiotemporally congruent sound+visual stimuli produce supra-additive responses in SC neurons, enabling detection of stimuli that would otherwise be below threshold.

## Critical Implementation Parameters

These values come from validated clinical research - don't change without discussion:

| Parameter | Value | Source |
|-----------|-------|--------|
| AV Synchronization | ≤16ms | SC binding window research |
| Stimulus Duration | 100-500ms | Bolognini, Wake Forest |
| Looming Sound | 400Hz, 250ms, 55→75dB exponential | Romei attention capture |
| Cue-Target Delay | 250ms (sound precedes visual) | Validated protocol |
| Training Eccentricities | 8°, 24°, 40°, 56° | Dundon, Tinelli |
| Session Length | 15 min (3×5min blocks) | Frontiers 2021 |

## Key Unity Requirements

- **Audio timing**: Use `AudioSettings.dspTime` (sample-accurate ~0.02ms), NOT `Time.time` (frame-dependent ~8-16ms)
- **Spatial audio**: Meta XR Audio SDK with Universal HRTF
- **Quest target**: Android ARM64, Vulkan, ASTC textures
- **Phase Sync**: Enable in OVRManager to reduce latency

## File Structure for Context

```
/docs
├── CLAUDE_CONTEXT.md        ← You are here
├── research/                ← Scientific findings, clinical protocols
├── decisions/               ← Architecture and design decisions
└── sessions/                ← Brainstorming session summaries
```

**Workflow**: Eric brainstorms with Claude (chat), produces session notes, commits to `/docs/sessions/`. Claude Code reads these for implementation context.

## Key References

- `/docs/research/` - Detailed scientific foundation
- `NeglectFix_V1_Knowledge_Document.docx` - Comprehensive V1 specs
- `NeglectFix_V1_QuickRef.md` - Quick reference with code snippets

## Communication Style

Eric is technical and understands both the neuroscience and the Unity/C# implementation. He's motivated, researching extensively, and wants evidence-based approaches. Direct technical communication works best.

---

*Last updated: 2024-12-05*
