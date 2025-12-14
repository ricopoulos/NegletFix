# Unity MCP Setup Guide for NeglectFix Development

## Context for Claude Code

You are helping Eric Ricopoulos set up Unity MCP (Model Context Protocol) integration to accelerate development of **NeglectFix**, a VR neurorehabilitation system for left homonymous hemianopia following a right PCA stroke.

### Project Overview
- **Target Platform**: Meta Quest 2/3 standalone VR
- **Unity Version**: 2022.3 LTS (or Unity 6)
- **Render Pipeline**: URP (Universal Render Pipeline)
- **Additional Hardware**: Muse EEG headband for neurofeedback
- **Key Features Being Built**:
  - Cross-modal audiovisual rehabilitation (synchronized sound + visual stimuli)
  - EEG neurofeedback integration
  - Contrast sensitivity testing
  - Spatial audio with HRTF via Meta XR Audio SDK

### Why MCP Integration is Needed
Eric has comprehensive V0 code and V1 specifications but struggles with the Unity Editor workflow. MCP will allow you (Claude Code) to directly manipulate Unity scenes, create scripts, manage assets, and automate tasks through natural language.

---

## Recommended MCP Implementation Options

### Option 1: CoplayDev/unity-mcp (Recommended for Claude Code)
**Repository**: https://github.com/CoplayDev/unity-mcp

**Why This One**:
- HTTP-first transport (simpler setup)
- Specific Claude Code integration documentation
- MIT licensed, actively maintained
- Supports multiple Unity Editor instances

**Capabilities**:
- `manage_asset`: Create, import, delete assets
- `manage_scene`: Open, save, modify scenes, create/manipulate GameObjects
- `manage_material`: Create materials, set properties, assign to renderers
- `manage_script`: Create, view, update C# scripts
- `execute_menu_item`: Run any Unity menu command
- Natural language task automation

### Option 2: CoderGamester/mcp-unity (Most Comprehensive)
**Repository**: https://github.com/CoderGamester/mcp-unity

**Why Consider This**:
- 38+ tools available
- Automatic IDE integration (adds Unity Library/PackedCache to workspace)
- ProBuilder integration for 3D modeling
- Cross-platform build automation

---

## Installation Steps

### Prerequisites
1. **Node.js 18+** - Check with `node --version`
2. **Unity 2021.3+** (Eric has 2022.3 LTS)
3. **Python 3.10+** (for CoplayDev version)
4. **Git**

### Step 1: Install Unity Package

#### For CoplayDev/unity-mcp:
```bash
# Clone the repository
git clone https://github.com/CoplayDev/unity-mcp.git
cd unity-mcp
```

In Unity:
1. Open Package Manager (Window → Package Manager)
2. Click "+" → "Add package from disk..."
3. Navigate to the cloned `unity-mcp` folder and select `package.json`

#### Alternative - CoderGamester/mcp-unity:
In Unity Package Manager:
1. Click "+" → "Add package from git URL..."
2. Enter: `https://github.com/CoderGamester/mcp-unity.git`

### Step 2: Install Dependencies

#### For CoplayDev (Python-based):
```bash
cd unity-mcp
pip install -r requirements.txt
# or with uv:
uv pip install -r requirements.txt
```

#### For CoderGamester (Node.js-based):
```bash
cd mcp-unity/Server~
npm install
npm run build
```

### Step 3: Configure Claude Code MCP

Create or edit the MCP configuration file:

**Location**: 
- Project-level: `.claude/mcp.json` in your Unity project root
- Global: `~/.claude/settings.json`

#### For CoplayDev/unity-mcp:
```json
{
  "mcpServers": {
    "unity-mcp": {
      "command": "python",
      "args": ["/ABSOLUTE/PATH/TO/unity-mcp/server/main.py"],
      "env": {
        "UNITY_MCP_HOST": "localhost",
        "UNITY_MCP_PORT": "5100"
      }
    }
  }
}
```

#### For CoderGamester/mcp-unity:
```json
{
  "mcpServers": {
    "mcp-unity": {
      "command": "node",
      "args": ["/ABSOLUTE/PATH/TO/mcp-unity/Server~/build/index.js"]
    }
  }
}
```

**IMPORTANT**: Replace `/ABSOLUTE/PATH/TO/` with the actual path. The path MUST NOT contain spaces.

### Step 4: Start the Unity MCP Server

#### In Unity Editor:
1. Go to **Window → MCP for Unity → Start Local HTTP Server**
2. Verify the indicator shows "Session Active"

#### Or from terminal (CoderGamester):
```bash
cd mcp-unity/Server~
node build/index.js
```

### Step 5: Verify Connection

In Claude Code, try:
```
"List all GameObjects in the current scene"
```

If connected, Claude should be able to query and manipulate Unity.

---

## NeglectFix-Specific Tasks for Claude Code

Once MCP is connected, here are the key development tasks:

### V1 Cross-Modal Audiovisual System

**Create these scripts**:

1. **AudioVisualStimulusController.cs**
   - Synchronized audiovisual stimulus presentation
   - Uses AudioSettings.dspTime for ≤16ms precision
   - Positions stimuli at 8°, 24°, 40°, 56° eccentricities in left hemifield

2. **LoomingSoundGenerator.cs**
   - 400Hz triangular wave
   - 250ms duration
   - Exponential amplitude rise from 55→75 dB
   - HRTF spatialization via Meta XR Audio

3. **StimulusPositioner.cs**
   - Helper for visual angle calculations
   - Converts degrees to world positions relative to camera

4. **AdaptiveStaircaseController.cs**
   - Up-down staircase for difficulty adaptation
   - Speed range: 3°/s to 240°/s
   - Log-unit step sizes

5. **ContrastEnhancementManager.cs**
   - URP Volume Override control
   - User-adjustable brightness/contrast sliders

6. **SessionManager.cs**
   - 3 blocks × 5 minutes structure
   - 15 trials per block
   - 2-4 second randomized ISI

### Scene Setup Tasks

```
"Create a new scene called 'AVRehabilitation' with:
- Main Camera with OVRManager and OVRCameraRig
- Audio Source with Meta XR Audio spatializer
- URP Volume with Color Adjustments override
- Empty GameObject 'RehabilitationController' for scripts"
```

### Meta XR SDK Configuration

```
"Configure the project for Quest standalone:
- Set Android ARM64 build target
- Enable Vulkan graphics API
- Import Meta XR SDK packages
- Enable Meta XR Audio spatializer in Audio settings"
```

### Integration with Existing V0 Scripts

The V0 codebase has these scripts that need integration:
- MuseOSCReceiver.cs (EEG data via OSC)
- EngagementCalculator.cs (beta/alpha ratio at TP10)
- GazeDetector.cs (head orientation tracking)
- RewardController.cs (feedback thresholds)
- DataLogger.cs (CSV export)
- TaskManager.cs (task orchestration)
- RewardGlow.cs (visual feedback)

**Integration task**:
```
"Connect EngagementCalculator output to AudioVisualStimulusController 
to modulate training difficulty based on EEG engagement signal"
```

---

## Key Parameters Reference

Copy these validated parameters into the scripts:

| Parameter | Value | Source |
|-----------|-------|--------|
| AV Synchronization | ≤16ms | SC binding window |
| Stimulus Duration | 100-500ms | Bolognini, Wake Forest |
| Training Eccentricities | 8°, 24°, 40°, 56° | Dundon, Tinelli |
| Looming Sound Frequency | 400Hz | Romei |
| Looming Duration | 250ms | Romei |
| Looming Amplitude | 55→75 dB exponential | Romei |
| Cue-Target Delay | 250ms (sound first) | Validated |
| Session Length | 15 min (3×5min blocks) | Frontiers 2021 |
| Total Program | 20 sessions (~5 hours) | Clinically validated |

---

## Troubleshooting

### "Claude Code can't find Unity tools"
1. Ensure Unity Editor is open with the project
2. Verify MCP server is running (check Unity window)
3. Check the MCP config path is absolute and has no spaces
4. Restart Claude Code after config changes

### "Connection refused"
- Check firewall isn't blocking localhost ports
- Verify the port matches between Unity and config (default: 5100 or 27182)

### "Node.js not found"
```bash
# macOS
brew install node

# Windows
# Download from https://nodejs.org/
```

### Unity launched from Finder doesn't inherit PATH
Either:
1. Launch Unity Hub from Terminal: `open -a "Unity Hub"`
2. Use the Unity MCP window's "Choose Claude Install Location" to set absolute path

---

## Quick Start Commands for Claude Code

Once connected, try these:

```
"Show me the current scene hierarchy"

"Create a C# script called AudioVisualStimulusController with 
sample-accurate audio scheduling using AudioSettings.dspTime"

"Add a URP Volume to the scene with Color Adjustments enabled"

"Create a prefab for the visual stimulus - a yellow sphere 
at 1.57° visual angle"

"Set up the Meta XR Audio SDK with HRTF spatialization"
```

---

## Resources

- **CoplayDev/unity-mcp**: https://github.com/CoplayDev/unity-mcp
- **CoderGamester/mcp-unity**: https://github.com/CoderGamester/mcp-unity
- **Claude Code MCP Docs**: https://docs.anthropic.com/claude-code/mcp
- **Meta XR Audio SDK**: https://developer.oculus.com/documentation/unity/audio-overview/

---

## Next Steps After Setup

1. **Verify connection** with a simple scene query
2. **Create the core V1 scripts** listed above
3. **Set up the rehabilitation scene** with VR camera rig
4. **Test audiovisual synchronization** with oscilloscope or timing validation
5. **Integrate with V0 EEG neurofeedback** pipeline
6. **Run first rehabilitation session** for testing

Good luck! This integration should dramatically speed up your NeglectFix development.
