# NegletFix Development Setup Checklist

## Required Software & Installation Steps

### 1. Unity Development Environment
- [ ] **Unity Hub**: Download from https://unity.com/download
  - Install Unity Hub first (it manages Unity versions)
  - Once installed, open Unity Hub

- [ ] **Unity 2022.3 LTS**: Install via Unity Hub
  - In Unity Hub, go to "Installs" tab
  - Click "Install Editor"
  - Choose Unity 2022.3 LTS (Long Term Support)
  - Add modules:
    - ✅ Android Build Support (for Quest)
    - ✅ Android SDK & NDK Tools
    - ✅ OpenJDK
    - ✅ Documentation (optional but recommended)

### 2. EEG Setup (Muse Device)
- [ ] **Mind Monitor App**:
  - iOS: https://apps.apple.com/app/mind-monitor/id988527143
  - Android: https://play.google.com/store/apps/details?id=com.sonicPenguins.museMonitor
  - Cost: ~$15 (one-time purchase)
  - Settings needed:
    - OSC Stream Target IP: Your Mac's IP address
    - OSC Stream Port: 5000
    - Enable: OSC Streaming

### 3. VR Development (Meta Quest)
- [ ] **Meta Quest Developer Mode**: Enable on your Quest
  1. Create Meta developer account: https://developer.oculus.com
  2. Enable Developer Mode in Oculus mobile app
  3. Connect Quest via USB-C cable

- [ ] **Meta Quest Link** (for PC VR development):
  - Download: https://www.meta.com/quest/setup/
  - Alternative: Use Quest in standalone mode with Build & Run

### 4. Development Tools
- [ ] **Visual Studio Code** (if not installed):
  - Download: https://code.visualstudio.com
  - Extensions to add:
    - Unity Code Snippets
    - C# Dev Kit

- [ ] **Git** (already installed ✓)

## Quick Test Sequence

### Step 1: Verify Unity Installation
```bash
# After Unity Hub installation
ls "/Applications/Unity Hub.app" && echo "✓ Unity Hub installed"
```

### Step 2: Create Test Project
1. Open Unity Hub
2. New Project → 3D (URP) or VR template
3. Name: "NegletFix"
4. Location: This directory

### Step 3: Import Our Scripts
The Unity scripts are already in `Unity/NeglectFix/Assets/Scripts/`:
- EEG/MuseOSCReceiver.cs
- EEG/EngagementCalculator.cs
- Tasks/TaskManager.cs
- Utils/DataLogger.cs
- Utils/GazeDetector.cs
- Utils/RewardController.cs

### Step 4: Test Without Hardware
We can test initially without Muse or Quest:
- **Simulated EEG**: Use random values for alpha/beta
- **Desktop VR**: Use Unity XR Device Simulator
- **Mouse Input**: Simulate gaze tracking

## Installation Status Tracker

| Component | Required | Installed | Notes |
|-----------|----------|-----------|-------|
| Unity Hub | Yes | ⏳ | Download: 150MB |
| Unity 2022.3 LTS | Yes | ⏳ | Download: ~4GB |
| Mind Monitor | Optional* | ⏳ | *Can simulate initially |
| Meta Quest Link | Optional* | ⏳ | *Can use simulator |
| VS Code | Recommended | ❓ | Check if installed |
| Git | Yes | ✅ | Already configured |

## Next Actions

1. **Start with Unity Hub** - This is the gateway to everything
2. **While Unity downloads** (~20-30 min), you can:
   - Review the research papers in RESEARCH_SUMMARY.md
   - Plan which rehabilitation task to implement first
   - Set up your Quest for developer mode (if you have one)

## Helpful Commands

```bash
# Check your IP address (for Mind Monitor OSC setup)
ipconfig getifaddr en0  # WiFi
ipconfig getifaddr en1  # Ethernet (if different)

# Open Unity Hub once installed
open "/Applications/Unity Hub.app"

# Track this setup progress
git add SETUP_CHECKLIST.md
git commit -m "Track development environment setup progress"
git push
```

## Ready Indicators
You'll know setup is complete when:
- ✅ Unity Hub opens successfully
- ✅ Unity 2022.3 LTS appears in Installs
- ✅ Can create a new VR project
- ✅ Project opens without errors
- ✅ Can enter Play mode in Unity Editor

---
*Last Updated: October 20, 2025*