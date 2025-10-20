# Unity Project Setup Guide

## After Creating the Project

### 1. Import Package Dependencies
Once Unity opens, go to **Window → Package Manager**:

**Required Packages:**
- **XR Plugin Management** (com.unity.xr.management)
- **XR Interaction Toolkit** (com.unity.xr.interaction.toolkit)
- **OpenXR Plugin** (com.unity.xr.openxr)

**Optional but Useful:**
- **TextMeshPro** (for better UI text)
- **ProBuilder** (for quick 3D prototyping)

### 2. Configure XR Settings
**Edit → Project Settings → XR Plug-in Management**:
- Check "OpenXR" for PC and Android
- Under OpenXR settings:
  - Add "Oculus Touch Controller Profile"
  - Add "Meta Quest Support" feature

### 3. Import Our Scripts
The scripts are already in the right folder structure:
```
Unity/NeglectFix/Assets/Scripts/
├── EEG/
│   ├── EEGSimulator.cs (NEW - for testing)
│   ├── MuseOSCReceiver.cs
│   └── EngagementCalculator.cs
├── Tasks/
│   └── TaskManager.cs
└── Utils/
    ├── DataLogger.cs
    ├── GazeDetector.cs
    └── RewardController.cs
```

### 4. Create Test Scene Structure
In the Hierarchy, create:
```
NegletFixTestScene
├── XR Origin (from XR Interaction Toolkit)
│   └── Camera Offset
│       └── Main Camera
├── EEG System (Empty GameObject)
│   ├── EEGSimulator (attach EEGSimulator.cs)
│   └── EngagementCalculator (attach EngagementCalculator.cs)
├── Task System (Empty GameObject)
│   └── TaskManager (attach TaskManager.cs)
└── Environment
    └── Ground Plane (3D Object → Plane)
```

### 5. Quick Test Without Hardware
1. **Add EEGSimulator** to test without Muse:
   - Create empty GameObject named "EEGSimulator"
   - Add `EEGSimulator.cs` component
   - Enable "Enable Simulation" checkbox

2. **Test Controls**:
   - Press **E** = Simulate high engagement
   - Press **R** = Simulate low engagement
   - Watch the on-screen GUI for real-time values

3. **Play Mode**:
   - Hit Play button
   - You should see EEG values updating in the GUI
   - Engagement score should fluctuate naturally

### 6. Build Settings for Quest
**File → Build Settings**:
- Platform: Android
- Texture Compression: ASTC
- Click "Switch Platform"

**Player Settings** (for Android):
- Minimum API Level: 29
- Target API Level: Automatic
- Graphics APIs: OpenGLES3, Vulkan
- Package Name: com.neglectfix.rehabilitation

### 7. Testing Checklist
- [ ] Unity project created and opens
- [ ] XR packages imported
- [ ] Scripts compile without errors
- [ ] EEG Simulator shows values in Play mode
- [ ] Basic scene with XR Origin configured
- [ ] Android platform selected (for Quest)

## Troubleshooting

### If scripts have errors:
- Make sure you're using Unity 2022.3+ or Unity 6.2
- Check that namespace `NeglectFix` is consistent
- Ensure all script files are in correct folders

### If XR doesn't work:
- Reinstall XR packages from Package Manager
- Check XR Plug-in Management settings
- Ensure OpenXR is selected for your platform

### Can't see simulator GUI:
- Check Game view is active
- EEGSimulator component must be enabled
- "Enable Simulation" checkbox must be checked

## Next Steps After Setup
1. Test EEG simulator in Play mode
2. Add first rehabilitation task (e.g., Visual Search)
3. Connect Quest headset for VR testing
4. Set up Mind Monitor for real EEG data
5. Implement adaptive difficulty based on engagement

---
Ready to start? Create the project and follow these steps!