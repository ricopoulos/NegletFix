# NeglectFix V0 - Quick Start Guide
## Get Up and Running in 30 Minutes

**Goal**: Test OSC connection between Muse EEG and Unity

---

## Prerequisites Checklist

Hardware you need:
- [ ] **Muse headband** (Muse 2 or Muse S) - charged
- [ ] **Meta Quest 2** - charged
- [ ] **Mobile device** (iOS/Android) - for Mind Monitor
- [ ] **Computer** (Mac/PC) - for Unity, VR-capable GPU
- [ ] **WiFi network** - all devices on same network

Software to install:
- [ ] **Unity Hub** - https://unity.com/download
- [ ] **Unity 2021.3 LTS** - via Unity Hub
- [ ] **Mind Monitor** - https://apps.apple.com/us/app/mind-monitor/id988527143

---

## Step 1: Test Muse Headband (5 min)

### Fit the Headband

1. Put on Muse headband:
   - Forehead sensors (AF7, AF8) above eyebrows
   - Ear sensors (TP9, TP10) behind ears
   - Adjust for snug but comfortable fit

2. Open Mind Monitor app on mobile:
   - Pair with Muse via Bluetooth
   - Check "Horseshoe" connection quality
   - All 4 electrodes should show green (good) or yellow (ok)
   - If red: Adjust fit, moisten electrodes, move hair

3. Verify EEG signals:
   - You should see wavy lines for each channel
   - Blink your eyes → You'll see spike in frontal channels (AF7/AF8)
   - Clench jaw → You'll see artifact (this is normal)

**Success**: All 4 electrodes green/yellow, EEG waves visible

---

## Step 2: Configure OSC Streaming (5 min)

### Get Your Unity Computer's IP Address

**Mac**:
```bash
ifconfig | grep "inet " | grep -v 127.0.0.1
```
Look for something like `192.168.1.100`

**Windows**:
```cmd
ipconfig
```
Look for "IPv4 Address" like `192.168.1.100`

### Configure Mind Monitor

1. In Mind Monitor app:
   - Tap **Settings** (gear icon)
   - Scroll to **OSC Stream Output**

2. Configure OSC:
   - **IP Address**: Enter your Unity computer's IP (e.g., `192.168.1.100`)
   - **Port**: `5000`
   - **Stream Type**: Select **Band Powers** (alpha, beta, theta, etc.)
   - **Enable**: Toggle ON

3. Start streaming:
   - Go back to main screen
   - Tap **OSC** button (should turn green)
   - Leave app running in foreground

**Success**: OSC button is green, "Streaming to 192.168.1.100:5000" message

---

## Step 3: Setup Unity Project (10 min)

### Open Project in Unity

1. Launch **Unity Hub**
2. Click **Add** → **Add project from disk**
3. Navigate to: `/Users/ericlespagnon/Dropbox/DEV-LOCAL/NegletFix/Unity/NeglectFix`
4. Select folder and click **Open**
5. Unity will open (may take 2-3 minutes first time)

### Install extOSC Package

1. In Unity: **Window** → **Package Manager**
2. Click **+** (top-left) → **Add package from git URL**
3. Enter: `https://github.com/Iam1337/extOSC.git`
4. Click **Add**
5. Wait for package to install

**Alternative** (if git URL fails):
1. Download extOSC: https://github.com/Iam1337/extOSC/releases
2. Extract ZIP
3. Copy `Assets/extOSC` folder into your Unity project's `Assets/` folder

### Install XR Plugin Management

1. **Window** → **Package Manager**
2. Search for **"XR Plugin Management"**
3. Click **Install**
4. After install: **Edit** → **Project Settings** → **XR Plug-in Management**
5. Check **"Oculus"** under Android tab (for Quest 2)

---

## Step 4: Create OSC Test Scene (10 min)

### Create New Scene

1. **File** → **New Scene** → **3D (Built-in Render Pipeline)**
2. **File** → **Save As** → `Assets/Scenes/OSC_Test.unity`

### Add OSC Receiver

1. **Create Empty GameObject**: Right-click in Hierarchy → **Create Empty**
2. Rename to `MuseOSCReceiver`
3. In Inspector: **Add Component** → Search for `MuseOSCReceiver`
4. Script will show in Inspector with settings:
   - **Receive Port**: `5000` (matches Mind Monitor)

### Test Connection

1. Click **Play** button in Unity
2. Check **Console** (Window → General → Console)
3. You should see:
   ```
   [MuseOSC] Receiver initialized on port 5000
   [MuseOSC] Waiting for Mind Monitor to stream data...
   ```

4. With Mind Monitor streaming (OSC green), within a few seconds:
   ```
   [MuseOSC] ✓ Receiving data from Mind Monitor!
   ```

5. In **Game view**, you'll see live EEG data:
   ```
   Muse EEG - TP10 (Right Parietal)
   Alpha: 3.24 μV²
   Beta:  1.87 μV²
   Theta: 2.15 μV²
   ```

**Success**: Console says "Receiving data" and Game view shows live numbers

---

## Troubleshooting

### "No OSC packets received"

**Check**:
- [ ] Unity computer and mobile device on **same WiFi** network
- [ ] IP address in Mind Monitor is **correct** (recheck Step 2)
- [ ] OSC button in Mind Monitor is **green** (streaming on)
- [ ] Firewall allows port 5000 (Mac: System Preferences → Security & Privacy)

**Try**:
- Change OSC port to `7000` in both Mind Monitor and Unity
- Restart Mind Monitor app
- Restart Unity
- Turn off VPN if running

### "Poor electrode contact"

**Check**:
- [ ] Headband snug but comfortable
- [ ] Forehead sensors centered above eyebrows
- [ ] Ear sensors touching skin behind ears (not on hair)

**Try**:
- Moisten electrode pads (tap water on fingertip, wipe pads)
- Adjust headband position
- Move hair away from sensors
- Clean sensors with alcohol wipe (if very dirty)

### extOSC package not found

**Try**:
- Download manually: https://github.com/Iam1337/extOSC/archive/refs/heads/master.zip
- Extract and copy `extOSC/Assets/extOSC` into your Unity project's `Assets/` folder
- Unity will auto-import

---

## Next Steps

### ✅ You've completed OSC test!

**What you have now**:
- Muse EEG streaming to Unity via OSC
- Real-time alpha, beta, theta data visible
- Foundation for neurofeedback system

### Next: Add Engagement Calculator

1. **Create another Empty GameObject** in Hierarchy
2. Rename to `EngagementCalculator`
3. **Add Component** → `EngagementCalculator`
4. In Inspector:
   - **Muse Receiver**: Drag the `MuseOSCReceiver` GameObject here
5. Click **Play**
6. After baseline starts, you'll see engagement score calculated in Game view

### Then: Build Kitchen Discovery Task

Follow instructions in `Unity/NeglectFix/README.md` to:
- Create kitchen environment
- Add reward objects
- Implement closed-loop feedback

---

## Full Documentation

- **Research**: `/RESEARCH_SUMMARY.md`
- **Protocol**: `/NEUROFEEDBACK_PROTOCOL.md`
- **Tasks**: `/VR_REHABILITATION_TASKS.md`
- **Unity Guide**: `/Unity/NeglectFix/README.md`
- **Project Summary**: `/PROJECT_SUMMARY.md`

---

## Common Questions

**Q: Do I need Quest 2 for OSC testing?**
A: No! You can test Muse → Unity OSC connection without VR. Quest 2 only needed for actual rehabilitation tasks.

**Q: Can I use wireless/Air Link for Quest 2?**
A: Yes, but Link cable is more stable for development. Test both.

**Q: How do I know if my computer is VR-capable?**
A: Check Meta's compatibility tool: https://support.oculus.com/. Minimum: GTX 1060 or equivalent GPU.

**Q: Mind Monitor costs money?**
A: Yes, around $5-10 (varies by region). It's the easiest way to stream Muse data via OSC.

---

**You're ready to start!** 🧠✨

Put on your Muse headband, open Mind Monitor, hit Play in Unity, and watch your brainwaves flow! 🌊
