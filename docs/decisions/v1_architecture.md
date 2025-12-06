# V1 Architecture Decisions

## Overview

V1 extends V0 (EEG neurofeedback) with cross-modal audiovisual rehabilitation based on superior colliculus pathway activation.

## Hardware Decision: Quest 3 over Quest 2

**Decision**: Quest 3 required for AR-based rehabilitation

**Rationale**:
- Quest 2 passthrough is grayscale only - unsuitable for contrast/brightness rehabilitation
- Quest 3 has full-color passthrough with 110° FOV
- Quest 3's 19.8 PPD vs Quest 2's 17.6 PPD provides better precision

**Note**: Quest 2 remains usable for fully virtual VR tasks; Quest 3 required for AR overlay work.

## Audio System

**Decision**: Meta XR Audio SDK with Universal HRTF

**Rationale**:
- 81% improvement in elevation detection accuracy over legacy
- Required for precise spatial localization of looming sounds
- Native Quest integration

**Configuration**:
- Sample rate: 48kHz
- Spatializer: Meta XR Audio (Project Settings)
- Enable Phase Sync in OVRManager

## Timing Architecture

**Decision**: Use `AudioSettings.dspTime` for all timing-critical operations

**Rationale**:
- `AudioSettings.dspTime`: Sample-accurate (~0.02ms at 48kHz)
- `Time.time` / `Time.unscaledTime`: Frame-dependent (~8-16ms variance)
- SC binding window is ±100ms, but tighter sync = stronger response

**Pattern**:
```csharp
double playTime = AudioSettings.dspTime + delay;
audioSource.PlayScheduled(playTime);
```

## New Scripts for V1

| Script | Purpose |
|--------|---------|
| `AudioVisualStimulusController.cs` | Synchronized AV presentation |
| `LoomingSoundGenerator.cs` | 400Hz triangular wave, exponential rise |
| `StimulusPositioner.cs` | Place stimuli at validated eccentricities |
| `AdaptiveStaircaseController.cs` | Up-down difficulty adjustment |
| `ContrastEnhancementManager.cs` | URP post-processing control |
| `SessionManager.cs` | 3-block session structure |

## Integration with V0

**Key integration**: EEG engagement signal modulates AV training intensity

```
High engagement (beta > threshold) → Train peripheral blind field (40°, 56°)
Low engagement → Easier stimuli closer to fixation (8°, 24°)
```

Both EEG and behavioral data logged for correlation analysis.

---

*Decisions documented December 2024*
