# Crumb: Session1Pilot launch blocked by Quest controller check

**Date:** 2026-05-31 08:50 local
**Branch:** `main`

## What happened

- Installed `Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk` successfully on Quest 2 (`1WMHH831TR1047`).
- Launch command was sent successfully.
- The Quest did not enter the Unity app. Android activity state stayed on:
  `com.oculus.vrshell/.systemdialog.launchcheck.LaunchCheckControllerRequiredDialogActivity`
- This is a Meta/Horizon launch check, not a Unity crash.

## Evidence

- `adb devices -l` showed the Quest as authorized over USB after opening Meta Quest Developer Hub.
- `adb install -r Builds/AVTrainingSession1Pilot.apk` returned `Success`.
- `adb shell pidof com.UnityTechnologies.com.unity.template.urpblank` returned no app PID after launch.
- `dumpsys input` did not list Quest/Oculus controller input devices.
- `dumpsys bluetooth_manager` showed one recent BLE connection, but no HID input device.
- Direct launch with `am start -n ...UnityPlayerGameActivity` still hit the same controller-required launch dialog.
- MQDH casting connected but showed a black frame; ADB screencap was also black.

## Current blocker

The headset must see the required Quest controllers before the app will launch. Wake both controllers, check batteries, or re-pair them if needed. Once the controller-required dialog clears, rerun:

```bash
cd /Users/ericlespagnon/Dropbox/DEV-LOCAL/NegletFix/Unity/NeglectFix
../../scripts/quest-adb.sh install-run Builds/AVTrainingSession1Pilot.apk
```
