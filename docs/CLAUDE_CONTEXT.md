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
- 7 fully-implemented C# scripts (~1,729 LOC total)
- See `/Unity/NeglectFix/Assets/Scripts/`

**V0 Scripts (all production-ready)**:
| Script | LOC | Purpose |
|--------|-----|---------|
| `EEG/MuseOSCReceiver.cs` | 167 | OSC parsing, TP10 channel events |
| `EEG/EngagementCalculator.cs` | 315 | Beta/alpha ratio, adaptive threshold |
| `EEG/EEGSimulator.cs` | 145 | Testing without Muse hardware |
| `Utils/DataLogger.cs` | 281 | CSV session logging at 10Hz |
| `Utils/GazeDetector.cs` | 164 | VR head tracking, left-gaze detection |
| `Utils/RewardController.cs` | 332 | Visual/audio rewards, EEG+gaze trigger |
| `Tasks/TaskManager.cs` | 325 | Abstract base for rehabilitation tasks |

### V1 (Not Started) - Cross-Modal Audiovisual Rehabilitation
Building on V0, adding validated audiovisual training that leverages the **superior colliculus pathway**.

**Key scientific insight**: The retino-collicular pathway is preserved in hemianopia. Spatiotemporally congruent sound+visual stimuli produce supra-additive responses in SC neurons, enabling detection of stimuli that would otherwise be below threshold.

**V1 Scripts (planned, not yet implemented)**:

*Core AV Rehabilitation:*
- `AudioVisualStimulusController.cs` - Synchronized AV presentation
- `LoomingSoundGenerator.cs` - 400Hz triangular wave, exponential rise
- `StimulusPositioner.cs` - Place stimuli at validated eccentricities
- `AdaptiveStaircaseController.cs` - Up-down difficulty adjustment
- `ContrastEnhancementManager.cs` - URP post-processing control
- `SessionManager.cs` - 3-block session structure

*Assessment Module (baseline/progress tracking) - WORKING:*
- `Assessment/ContrastSensitivityTest.cs` (~620 LOC) - Pelli-Robson style hemifield contrast test **[TESTED 2025-12-13]**
- `Assessment/ContrastTestInput.cs` (~260 LOC) - VR controller + keyboard input handling **[TESTED]**
- `Assessment/ContrastResultsUI.cs` (349 LOC) - Results display, history, interpretation (not yet wired)
- `Integration/CalibrationManager.cs` - Pre-training calibration workflow (planned)

**Test Scene**: `ContrastTestScene.unity` - Working scene with Canvas UI for testing

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

- `/docs/research/scientific_foundation.md` - SC pathway, multisensory integration
- `/docs/research/contrast_sensitivity_module.md` - Assessment system design with draft code
- `/docs/decisions/v1_architecture.md` - Hardware, audio, timing decisions

## Unity Project

**Location**: `/Unity/NeglectFix/` (single folder, cleaned up Dec 2025)
**Unity Version**: 6.2 (6000.2.8f1) with VR template
**Re-add to Unity Hub**: Add → browse to `/Unity/NeglectFix/`

## Unity MCP Integration (NEW - 2025-12-13)

**Claude Code can now directly interact with Unity Editor!**

This is critical for accessibility - Eric is visually impaired and struggles with Unity's interface. Claude is now his "eyes and hands" inside the editor.

### Available Capabilities

| Tool | What It Does |
|------|--------------|
| `manage_scene` | Read hierarchy, create/load scenes, take screenshots |
| `manage_gameobject` | Create, modify, delete GameObjects and components |
| `manage_script` | Create and edit C# scripts |
| `manage_material` | Create materials, set properties |
| `read_console` | View errors, warnings, and logs |
| `execute_menu_item` | Run any Unity menu command |

### How to Start a Session

1. **Open Unity** with NegletFix project
2. **Start MCP**: Window → MCP For Unity → Toggle MCP Window → Start Session
3. **Verify green indicator** = connected
4. **Start Claude Code** - Unity tools are automatically available

### MCP Server Management

```bash
# Verify connection
claude mcp list

# Remove if needed
claude mcp remove UnityMCP

# Re-add
claude mcp add --transport stdio UnityMCP -- "/Users/ericlespagnon/.local/bin/uvx" --from "git+https://github.com/CoplayDev/unity-mcp@v8.2.3#subdirectory=Server" mcp-for-unity
```

## Communication Style

Eric is technical and understands both the neuroscience and the Unity/C# implementation. He's motivated, researching extensively, and wants evidence-based approaches. Direct technical communication works best.

## Current Status (2025-12-13)

**Unity MCP Integration**: COMPLETE
- Claude Code can now directly interact with Unity Editor
- Reads scene hierarchy, creates GameObjects, writes scripts
- Critical accessibility improvement for visually impaired developer

**Contrast Sensitivity Test**: First working version complete
- Full test sequence runs (Central → Right → Left)
- Alpha-transparency based letter fading works well
- Keyboard input functional (Sloan letters + Backspace for "can't see")
- Calibration needs refinement - test may be too easy at start

**Console Warnings** (minor, to fix later):
- 5 deprecation warnings: `FindObjectOfType` → `FindFirstObjectByType`
- Files: ContrastResultsUI.cs, ContrastTestInput.cs, EngagementCalculator.cs, RewardController.cs

**Next Priority**:
1. Calibrate contrast curve to match clinical Pelli-Robson
2. Test Left vs Right hemifield to measure asymmetry
3. Wire up results display UI
4. Build V1 audiovisual rehabilitation system (now faster with Unity MCP!)

---

*Last updated: 2025-12-13*
