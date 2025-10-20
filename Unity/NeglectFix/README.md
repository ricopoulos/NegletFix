# NeglectFix V0 - Unity Project

Closed-loop VR neurorehabilitation system for hemispatial neglect + hemianopia.

## Project Overview

**Hardware**: Muse EEG + Meta Quest 2
**Target**: Left-space awareness training using real-time neurofeedback
**Evidence base**: See `/RESEARCH_SUMMARY.md` and `/NEUROFEEDBACK_PROTOCOL.md`

## Setup Instructions

### Prerequisites

1. **Unity Hub** + **Unity 2021.3 LTS** (or newer)
   - Download: https://unity.com/download

2. **Meta Quest 2 Development Setup**:
   - Enable Developer Mode on Quest 2
   - Install Meta Quest Link or use Air Link
   - Quest 2 connected via USB-C cable or WiFi

3. **Muse EEG Headband** (Muse 2 or Muse S)
   - Download Mind Monitor app (iOS/Android)
   - Configure OSC streaming (see below)

4. **Mind Monitor OSC Configuration**:
   - Open Mind Monitor app
   - Settings → OSC Stream Output
   - Enter your Unity computer's IP address
   - Port: `5000` (default)
   - Enable: `Band Powers` + `Raw EEG` streams

### Unity Package Dependencies

This project requires the following Unity packages:

1. **XR Plugin Management** (Meta Quest support)
   - Window → Package Manager → XR Plugin Management → Install
   - Settings → Oculus → Enable

2. **OSC Receiver** (extOSC or OscCore):
   - **Option A**: extOSC (recommended)
     - Download: https://github.com/Iam1337/extOSC
     - Or via Package Manager: Add from git URL
   - **Option B**: OscCore
     - Download: https://github.com/stella3d/OscCore

3. **TextMeshPro** (UI text):
   - Window → Package Manager → TextMeshPro → Install

### Project Setup

1. **Open project in Unity Hub**:
   ```
   Add → Select: /Unity/NeglectFix
   ```

2. **Configure Build Settings**:
   - File → Build Settings
   - Platform: Android (for Quest 2)
   - Switch Platform
   - Texture Compression: ASTC

3. **Configure Player Settings**:
   - Edit → Project Settings → Player
   - Company Name: Your name
   - Product Name: NeglectFix
   - Minimum API Level: Android 10.0 (API 29)
   - Install Location: Auto

4. **Configure XR Settings**:
   - Edit → Project Settings → XR Plug-in Management
   - Android Tab → Check "Oculus"
   - Stereo Rendering Mode: Multiview

5. **Test OSC Connection**:
   - Open Scene: `Scenes/OSC_Test.unity`
   - Enter Play Mode
   - Check Console for "OSC Receiver initialized on port 5000"
   - Start Mind Monitor OSC streaming
   - Verify EEG data packets in Console

## Project Structure

```
Assets/
├── Scenes/                    # Unity scenes
│   ├── OSC_Test.unity        # OSC connection test scene
│   ├── MainMenu.unity        # Task selection menu
│   └── Level1_KitchenDiscovery.unity
├── Scripts/
│   ├── EEG/                  # EEG processing
│   │   ├── MuseOSCReceiver.cs       # OSC packet receiver
│   │   ├── EEGProcessor.cs          # Band power extraction
│   │   └── EngagementCalculator.cs  # Alpha/beta scoring
│   ├── Tasks/                # VR rehabilitation tasks
│   │   ├── TaskManager.cs           # Base class for all tasks
│   │   ├── KitchenDiscovery.cs      # Task 1.1
│   │   └── ...
│   ├── UI/                   # User interface
│   │   ├── SessionDashboard.cs
│   │   └── ProgressTracker.cs
│   └── Utils/                # Utilities
│       ├── DataLogger.cs            # CSV logging
│       ├── GazeDetector.cs          # Head yaw tracking
│       └── RewardController.cs      # Visual/audio rewards
├── Prefabs/                  # Reusable objects
│   ├── RewardZone.prefab
│   ├── HighlightableObject.prefab
│   └── AudioCue.prefab
└── Materials/                # Visual materials
```

## Quick Start Guide

### 1. Test OSC Connection

1. Open `OSC_Test.unity` scene
2. Start Mind Monitor on mobile device
3. Enable OSC streaming (your Unity computer IP, port 5000)
4. Put on Muse headband
5. Enter Play Mode in Unity
6. Console should show real-time EEG data

### 2. Run First Task

1. Open `Level1_KitchenDiscovery.unity`
2. Put on Quest 2 headset (Link cable connected)
3. Start Mind Monitor OSC streaming
4. Enter Play Mode
5. Look around the kitchen, focus on left-side objects
6. When EEG engagement + left-gaze → Objects should brighten/glow

### 3. View Session Data

After session ends:
- Data saved to: `Logs/session_YYYY-MM-DD_HH-MM-SS.csv`
- Open in Excel/Numbers or Python for analysis

## Core Scripts Overview

### EEG Pipeline

**`MuseOSCReceiver.cs`**:
- Receives OSC packets from Mind Monitor
- Parses band power data (alpha, beta, theta)
- Exposes events for other scripts

**`EEGProcessor.cs`**:
- Processes TP10 channel (right parietal)
- Smooths data (moving average)
- Calculates relative power

**`EngagementCalculator.cs`**:
- Computes engagement score: `(beta/alpha) × (1-theta_factor)`
- Adaptive threshold management
- Triggers reward events when threshold exceeded

### Task System

**`TaskManager.cs`** (base class):
- Session timing (baseline, training, cooldown)
- Success/failure tracking
- Data logging coordination

**`KitchenDiscovery.cs`** (example task):
- Inherits from TaskManager
- Manages object discovery mechanics
- Integrates with RewardController

### Utilities

**`GazeDetector.cs`**:
- Tracks Quest 2 head rotation
- Detects left-gaze (yaw > 15°)
- Provides boolean for combined trigger

**`RewardController.cs`**:
- Visual effects (brightness, glow)
- Audio cues (spatial sound)
- Reward timing and cooldown

**`DataLogger.cs`**:
- CSV export of all session data
- Timestamped EEG + gaze + events
- Session summary generation

## Network Configuration

### Finding Your Unity Computer IP Address

**macOS**:
```bash
ifconfig | grep "inet " | grep -v 127.0.0.1
```

**Windows**:
```cmd
ipconfig
```
Look for "IPv4 Address" under your active network adapter.

### Firewall Settings

Ensure port `5000` (or your chosen OSC port) is open:

**macOS**:
- System Preferences → Security & Privacy → Firewall
- Firewall Options → Add Unity (allow incoming connections)

**Windows**:
- Control Panel → Windows Defender Firewall
- Advanced Settings → Inbound Rules → New Rule → Port 5000

## Troubleshooting

### OSC Connection Issues

**Problem**: "No OSC packets received"

**Solutions**:
1. Verify Unity computer IP address correct in Mind Monitor
2. Check both devices on same WiFi network
3. Disable VPN on Unity computer
4. Check firewall allows port 5000
5. Try different OSC port (e.g., 7000)

### Muse Headband Issues

**Problem**: "Poor electrode contact"

**Solutions**:
1. Adjust headband fit (snug but comfortable)
2. Moisten electrode contacts (tap water or saline)
3. Move hair away from electrode pads
4. Check battery level > 20%

### Quest 2 Issues

**Problem**: "Low frame rate in VR"

**Solutions**:
1. Reduce graphics quality (Edit → Project Settings → Quality)
2. Use Link cable instead of Air Link (more stable)
3. Close background apps on Unity computer
4. Lower Quest 2 refresh rate (72 Hz vs. 90 Hz)

### EEG Signal Quality

**Problem**: "Noisy or erratic EEG signals"

**Solutions**:
1. Minimize jaw clenching (relax face muscles)
2. Reduce eye blinking frequency
3. Sit still (avoid head movement during baseline)
4. Check electrode contacts (TP10 especially critical)

## Development Workflow

### Adding a New Task

1. Duplicate existing task script (e.g., `KitchenDiscovery.cs`)
2. Rename and modify task-specific logic
3. Create new Unity scene
4. Add TaskManager component to scene controller object
5. Design environment in Scene view
6. Test in Play Mode with OSC streaming

### Modifying Neurofeedback Parameters

Edit `EngagementCalculator.cs`:
- `alphaWeight`: How much alpha reduction matters (default: 1.0)
- `betaWeight`: How much beta increase matters (default: 1.0)
- `thetaFactor`: Penalty for high theta (default: 0.3)
- `baselineThreshold`: Multiplier on baseline (default: 0.5 std dev)

### Adjusting Reward Sensitivity

Edit `RewardController.cs`:
- `cooldownDuration`: Time between rewards (default: 1.0 sec)
- `glowIntensity`: Brightness of object glow (default: 1.5)
- `audioVolume`: Reward sound volume (default: 0.7)

## Data Analysis

Session CSV files contain:
- `timestamp`: Milliseconds since session start
- `tp10_alpha`: Alpha power from TP10 (μV²)
- `tp10_beta`: Beta power from TP10 (μV²)
- `tp10_theta`: Theta power from TP10 (μV²)
- `engagement_score`: Calculated score
- `threshold`: Current adaptive threshold
- `head_yaw`: Head rotation in degrees
- `left_gaze`: Boolean (1 = looking left, 0 = not)
- `reward_triggered`: Boolean (1 = reward event, 0 = no reward)

### Python Analysis Example

```python
import pandas as pd
import matplotlib.pyplot as plt

# Load session data
df = pd.read_csv('Logs/session_2025-10-19_14-30-00.csv')

# Plot engagement over time
plt.figure(figsize=(12, 6))
plt.plot(df['timestamp']/1000, df['engagement_score'], label='Engagement')
plt.plot(df['timestamp']/1000, df['threshold'], label='Threshold', linestyle='--')
plt.xlabel('Time (seconds)')
plt.ylabel('Engagement Score')
plt.legend()
plt.title('Neurofeedback Training Session')
plt.show()

# Calculate success rate
success_rate = df['reward_triggered'].sum() / len(df) * 100
print(f"Success rate: {success_rate:.1f}%")

# Left-gaze percentage
left_gaze_pct = df['left_gaze'].sum() / len(df) * 100
print(f"Time spent gazing left: {left_gaze_pct:.1f}%")
```

## Contributing

This is a personal research project. Feel free to adapt for your own use!

## Research References

See `/RESEARCH_SUMMARY.md` for full citations.

## License

Personal use only. Not for clinical/commercial distribution.

---

**Project version**: V0.1
**Unity version**: 2021.3 LTS
**Last updated**: October 19, 2025
