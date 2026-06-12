---
title: Hardware Setup
last_updated: 2026-05-30
confidence: HIGH
sources:
  - CLAUDE.md
  - NEUROFEEDBACK_PROTOCOL.md
  - PROJECT_SUMMARY.md
  - RESEARCH_SUMMARY.md
  - .brain/cross-cutting.md
  - 2026-05-14 research audit + 2026-05-16 working fix for Quest dev mode (see §6 + §9)
  - .brain/sessions/2026-05-30-quest-guided-pilot-wrap.md
---

# Hardware Setup

Actually-works setup guide for NegletFix. Three devices, one mobile bridge, one Unity-capable computer, total cost ~$500. Home-based, no clinical infrastructure required.

See [[unity-architecture]] for Unity side, [[eeg-neurofeedback]] for the signal pipeline, [[audiovisual-training-protocol]] for what the hardware runs.

---

## 1. Components

| Device | Role | Approx. cost |
|--------|------|--------------|
| **Meta Quest 2 (or Quest 3)** | VR headset — visual stimulus, head tracking, gaze proxy | ~$250 (Q2) / ~$500 (Q3) |
| **Muse 2** or **Muse S** | Consumer 4-channel EEG — TP10 right parietal is the primary target | ~$250 |
| **iOS 12+ or Android 8+ phone/tablet** | Runs Mind Monitor app, BLE bridge + OSC streamer | user-owned |
| **Mind Monitor app** | iOS/Android app — parses Muse BLE, streams OSC to Unity | ~$15 one-time |
| **Mac or PC** | Unity development + OSC receiver | user-owned, VR-capable GPU preferred |

Sources: `CLAUDE.md`, `PROJECT_SUMMARY.md:103`, `RESEARCH_SUMMARY.md:185-220`, `NEUROFEEDBACK_PROTOCOL.md:386-405`.

---

## 2. Network Topology

```
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│   Muse 2 / S    │──BLE→│  Mind Monitor   │─OSC→│   Mac (Unity)   │
│   (on head)     │      │  (phone/tablet) │  UDP │  10.0.0.152     │
└─────────────────┘      └─────────────────┘port 5000 └─────────────────┘
                                                           │
                                                           │ USB-C Link
                                                           │  (or Air Link WiFi)
                                                           ▼
                                                    ┌─────────────────┐
                                                    │  Meta Quest 2/3 │
                                                    │   (on head)     │
                                                    └─────────────────┘
```

All devices must be on the **same WiFi network** for OSC to reach Unity.

---

## 3. Muse Setup

1. Fully charge Muse (2+ hours typical battery, `NEUROFEEDBACK_PROTOCOL.md:389`).
2. Fit the headband:
   - Sensor pads centered on forehead (AF7, AF8) and behind each ear on the mastoid (TP9 left, TP10 right)
   - Damp electrode pads slightly with water or saline if skin is dry — improves contact
   - Hair moved out from under pads; beard/glasses can interfere
3. In the Muse companion app (or Mind Monitor), verify all four electrodes are connected. Mind Monitor shows a "horseshoe" indicator — all four must be solid before trusting the data.
4. Avoid jaw clenching, repeated blinks, and head movement — these contaminate the signal (`NEUROFEEDBACK_PROTOCOL.md:304-307`).

**Fit quality >60%** is the working threshold (`NEUROFEEDBACK_PROTOCOL.md:388`). Below that, reposition or rehydrate pads.

---

## 4. Mind Monitor App Configuration

Mind Monitor (`https://mind-monitor.com/`, ~$15 iOS/Android) is the BLE→OSC bridge.

1. Install on phone/tablet, pair with Muse via BLE.
2. In Mind Monitor Settings → **OSC Streaming**:
   - Enable OSC output
   - **Target IP**: the Mac/PC running Unity — e.g., `10.0.0.152` (Eric's LAN IP; `CLAUDE.md` references this; change to match current network)
   - **OSC Port**: `5000` (matches `MuseOSCReceiver.receivePort` default, `MuseOSCReceiver.cs:29`)
   - Enable band-power output (alpha, beta, theta absolute — all 4 channels)
3. Verify WiFi: phone and Mac on the same SSID, no VPN blocking UDP, no macOS firewall blocking port 5000.

The OSC address format Mind Monitor emits (consumed by `MuseOSCReceiver.cs:75-77`):
```
/muse/elements/alpha_absolute  [TP9, AF7, AF8, TP10]
/muse/elements/beta_absolute   [TP9, AF7, AF8, TP10]
/muse/elements/theta_absolute  [TP9, AF7, AF8, TP10]
```

When Unity starts receiving, `MuseOSCReceiver.cs:139` logs `[MuseOSC] ✓ Receiving data from Mind Monitor!` — confirm this before proceeding.

---

## 5. Mac / PC Requirements

From `NEUROFEEDBACK_PROTOCOL.md:397-401`:
- **OS**: macOS 10.15+ or Windows 10/11
- **RAM**: 8 GB min, 16 GB recommended
- **GPU**: GTX 1060 equivalent or better (VR-ready) — for editor Link; Quest standalone build is lighter on the host
- **Unity**: 6.2 (6000.2.8f1) with VR template

**Find your Mac's LAN IP** (needed for Mind Monitor target):
```
ipconfig getifaddr en0   # macOS, typical WiFi interface
```

**Firewall**: System Settings → Network → Firewall → allow incoming connections on port 5000 for Unity.

---

## 6. Quest 2 Developer Mode

Required for sideloading NegletFix builds.

1. Create a Meta developer account at https://developer.oculus.com/ and create an "organization" (required by Meta).
2. In the Meta Horizon mobile app on the phone paired to your Quest → Devices → your headset → Developer Mode → ON.
3. Connect Quest to Mac via USB-C cable; inside the headset, accept the "Allow USB Debugging" prompt.
4. Verify: `adb devices` from a terminal (requires Android Platform Tools) shows the Quest.

> **⚠ "Only the owner can do it" error — RESOLVED 2026-05-16** *(superseded the April 27 org-membership theory)*
>
> **Actual cause** (verified via 7-source research pass 2026-05-14 + working fix 2026-05-16): the error refers to the headset's **primary Meta account** — the account that paired the device — not the developer org owner. To toggle dev mode, the primary account must (a) be a registered Meta developer with a verified org, (b) be 18+, (c) have org verification complete (2FA or credit card), AND (d) be the account flipping the toggle in its own Meta Horizon app. The April 27 "invite to org as Admin" theory was incorrect — community thread 1322016 confirms this.
>
> **Working fix (Eric, 2026-05-16)**:
> 1. On the previous owner's phone: Meta Horizon app → Devices → tap headset → **Remove Device / Unpair** (do this FIRST — otherwise Find My Device lock can demand previous owner's credentials after reset)
> 2. On the headset: Settings → System → Factory Reset
> 3. On the new owner's phone (signed into new Meta account with verified dev org): Meta Horizon app → Add Device → Quest → pair
> 4. Toggle Developer Mode in the new owner's Meta Horizon app → Devices → Quest → Headset Settings → Developer Mode → ON
> 5. Verify: `adb devices` from terminal shows the headset's serial
>
> **Cost of the fix**: previous owner loses access to apps/saves/boundary on this headset (their purchases remain on their Meta account and work on other devices). Acceptable for a shared family Quest being repurposed for dev work.

### In-editor VR testing
- **Meta Quest Link** (formerly Oculus Link): USB-C cable, mirror Quest to PC, play in Unity directly. Most reliable.
- **Air Link**: same but over WiFi 5 GHz — convenient, adds latency.

### Deploy to Quest
- **Unity Build Settings → Android → Build and Run** while Quest is connected and Developer Mode is on.
- Build output: APK installed on headset, launchable from "Unknown Sources" in Quest library.

See [[unity-architecture]]#quest-2-deployment-android for build settings detail.

### ADB workflow validated 2026-05-30

Source map: `.brain/sessions/2026-05-30-quest-guided-pilot-wrap.md`, `scripts/quest-adb.sh`.

Quest USB ADB became a major friction point during the guided pilot session. The practical recovery path was:

1. Open **Meta Quest Developer Hub** on the Mac.
2. Let MQDH detect the Quest 2 over USB.
3. Confirm terminal ADB sees the same headset:
   ```bash
   adb devices -l
   ```
4. If USB is stable long enough, enable cached Wi-Fi ADB:
   ```bash
   scripts/quest-adb.sh enable-wifi
   ```
5. For later loops, prefer:
   ```bash
   scripts/quest-adb.sh status
   scripts/quest-adb.sh install-run
   scripts/quest-adb.sh logs
   ```

Known current device:
- Quest 2 serial: `1WMHH831TR1047`
- Cached LAN IP used during the session: `10.0.0.136`
- Terminal ADB path: `/opt/homebrew/bin/adb`

For off-face testing, the headset may sleep or show `HEADSET_UNMOUNTED`. This command worked during the session:
```bash
adb shell am broadcast -a com.oculus.vrpowermanager.prox_close
```
Use it deliberately: it can keep the display awake and drain battery until reboot.

2026-06-11 live-run note:
- Wi-Fi ADB at `10.0.0.136:5555` was reachable by ping but refused ADB until a short USB handshake re-enabled `adb tcpip 5555`.
- `scripts/quest-adb.sh enable-wifi` restored Wi-Fi ADB immediately once USB was authorized.
- Horizon intercepted the first Unity launch with a controllers-required dialog; both controllers must be awake and the dialog dismissed before Unity enters foreground.
- Before monitoring logs, confirm foreground with:
  ```bash
  adb -s 10.0.0.136:5555 shell pidof com.UnityTechnologies.com.unity.template.urpblank
  adb -s 10.0.0.136:5555 shell dumpsys activity activities | rg "mResumedActivity|UnityPlayerGameActivity"
  ```

---

## 7. Physical Wearing Setup

Eric needs to wear both Muse AND Quest simultaneously. Order matters:

1. Put **Muse on first** — electrodes must contact skin cleanly, headband tightened
2. Put **Quest over Muse** — the Quest head strap sits above/around the Muse; make sure neither pushes the other out of place
3. Verify in Mind Monitor that all four electrodes still show good contact after Quest is fitted

Some users find the Muse S (softer band) more compatible with Quest than the Muse 2 (harder headband). The project materials don't specify which Eric uses.

---

## 8. Test Environment

From `.brain/cross-cutting.md:41-46`:
- Quiet room (audio cues must be audible, external sound interferes with 400 Hz stimulus)
- Consistent lighting (ambient light leaks into Quest via nose gap — affects baseline)
- Comfortable seated position (VR motion sickness concerns minimized when stationary)
- Same time of day across sessions where possible (alertness is an EEG baseline variable)

---

## 9. Troubleshooting

| Symptom | Likely cause | Fix |
|---------|-------------|-----|
| Unity shows "No data received for 3 seconds" | OSC not reaching Unity | Check phone/Mac on same WiFi, firewall port 5000 open, Mind Monitor target IP matches Mac's current LAN IP (can change on DHCP) |
| Mind Monitor horseshoe has 1-2 bad electrodes | Pad contact | Dampen pad, reposition, check hair under electrodes |
| Muse disconnects repeatedly | BLE interference or low battery | Charge Muse, move phone closer, disable other BLE devices |
| Unity compiles but no OSC even with extOSC installed | `#define EXTOSC_INSTALLED` not uncommented | Open `MuseOSCReceiver.cs:5`, remove the `//` before the define |
| Quest not detected by `adb` | Driver (Windows) / USB-C cable | Use the official Quest cable or known-good USB-C 3.0 data cable (not charge-only) |
| Quest intermittently disappears from terminal ADB but MQDH sees it | Competing/flaky ADB daemon or USB handshake | Open MQDH, confirm device is connected, then restart terminal ADB. Prefer `scripts/quest-adb.sh enable-wifi` once USB works long enough. Keep MQDH and terminal pointed at the same ADB path if possible. |
| Quest shows `unauthorized` and no prompt is visible | Debug prompt missed, headset asleep, or stale host key | Wake headset, replug USB, watch inside headset for the prompt, choose **Always allow**. If still stuck, restart ADB; only delete `~/.android/adbkey*` as a last resort because it forces reauthorization. |
| Both Muse and Quest slip during session | Fit conflict | Try Muse S instead of Muse 2; Quest elite strap improves stability |
| VR motion sickness during AV training | Fatigue or new user | Start 5-10 min sessions, stationary only; no locomotion; 90 Hz refresh minimum |
| Meta Horizon app: *"only the owner can do it"* when toggling Developer Mode | Headset is primary-paired to a different Meta account than the one with the verified dev org — "owner" means primary device-paired account, NOT org owner | **Verified-working fix (Eric 2026-05-16)**: previous owner unpairs in their Meta app first → factory-reset Quest → re-pair to dev-org owner's account → toggle dev mode in their Meta Horizon app. See §6 for full procedure. The earlier "invite to org as Admin" workaround was based on a wrong diagnosis and is not the fix. |
| Markers hard to see because Quest camera/passthrough appears behind app | App background not visually controlled | Use the current AV training builds with `AVTrainingControlledBackdrop`: opaque gray camera-locked quad behind prompts/stimuli. Eric confirmed gray background + visible markers in-headset on 2026-05-30. |

---

## 10. OSC Port Reference

| Port | Protocol | Use |
|------|----------|-----|
| 5000 | UDP | Mind Monitor → Unity OSC (Muse band powers) |

Only one port needed. Don't change it unless something else on the network is already using it (uncommon for 5000). If you do change it, update both Mind Monitor target-port AND `MuseOSCReceiver.receivePort` in Unity.

---

## 11. Cross-References

- Unity codebase the hardware feeds: [[unity-architecture]]
- EEG signal processing: [[eeg-neurofeedback]]
- What the hardware runs: [[audiovisual-training-protocol]]
- Current readiness: [[rehabilitation-roadmap]]
