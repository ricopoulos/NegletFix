#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ADB="${ADB:-adb}"
PACKAGE_ID="${PACKAGE_ID:-com.UnityTechnologies.com.unity.template.urpblank}"
APK_PATH="${APK_PATH:-$ROOT_DIR/Unity/NeglectFix/Builds/AVTrainingSession1Pilot.apk}"
IP_FILE="${QUEST_IP_FILE:-$ROOT_DIR/.quest-adb-ip}"

usage() {
  cat <<USAGE
Usage:
  scripts/quest-adb.sh status
  scripts/quest-adb.sh enable-wifi [quest-ip]
  scripts/quest-adb.sh connect [quest-ip]
  scripts/quest-adb.sh install-run [apk-path]
  scripts/quest-adb.sh run
  scripts/quest-adb.sh logs

Typical flow:
  1. Connect USB once and accept "Always allow".
  2. scripts/quest-adb.sh enable-wifi
  3. Unplug USB.
  4. scripts/quest-adb.sh install-run

Environment overrides:
  ADB=/path/to/adb
  PACKAGE_ID=com.example.package
  APK_PATH=/path/to/app.apk
  QUEST_IP_FILE=/path/to/ip-cache
USAGE
}

adb_device_ids() {
  "$ADB" devices | awk 'NR > 1 && $2 == "device" { print $1 }'
}

first_device() {
  adb_device_ids | head -n 1
}

cached_wifi_device() {
  if [[ ! -f "$IP_FILE" ]]; then
    return 0
  fi

  local cached_ip
  cached_ip="$(<"$IP_FILE")"
  if [[ -z "$cached_ip" ]]; then
    return 0
  fi

  adb_device_ids | awk -v target="$(normalize_ip "$cached_ip")" '$0 == target { print; exit }'
}

first_usb_device() {
  adb_device_ids | awk '$0 !~ /:/{ print; exit }'
}

normalize_ip() {
  local target="$1"
  if [[ "$target" == *:* ]]; then
    printf '%s\n' "$target"
  else
    printf '%s:5555\n' "$target"
  fi
}

discover_usb_ip() {
  local serial="$1"
  "$ADB" -s "$serial" shell ip route get 8.8.8.8 2>/dev/null \
    | tr ' ' '\n' \
    | awk 'prev == "src" { print; exit } { prev = $0 }'
}

ensure_any_device() {
  local serial
  serial="$(cached_wifi_device || true)"
  if [[ -n "$serial" ]]; then
    printf '%s\n' "$serial"
    return 0
  fi

  serial="$(first_device || true)"
  if [[ -n "$serial" ]]; then
    printf '%s\n' "$serial"
    return 0
  fi

  if [[ -f "$IP_FILE" ]]; then
    local cached_ip
    cached_ip="$(<"$IP_FILE")"
    if [[ -n "$cached_ip" ]]; then
      "$ADB" connect "$(normalize_ip "$cached_ip")" >/dev/null || true
      serial="$(first_device || true)"
      if [[ -n "$serial" ]]; then
        printf '%s\n' "$serial"
        return 0
      fi
    fi
  fi

  echo "No authorized ADB device found." >&2
  echo "If USB is connected, wake the headset and accept the debug prompt." >&2
  echo "If Wi-Fi ADB was enabled earlier, run: scripts/quest-adb.sh connect <quest-ip>" >&2
  return 1
}

cmd_status() {
  "$ADB" devices -l
  if [[ -f "$IP_FILE" ]]; then
    echo "Cached Quest Wi-Fi IP: $(<"$IP_FILE")"
  fi
}

cmd_enable_wifi() {
  local serial ip
  serial="$(first_usb_device || true)"
  if [[ -z "$serial" ]]; then
    echo "No USB-authorized Quest found. Need one successful USB ADB connection to enable adb tcpip 5555." >&2
    "$ADB" devices -l >&2
    return 1
  fi

  ip="${1:-}"
  if [[ -z "$ip" ]]; then
    ip="$(discover_usb_ip "$serial" || true)"
  fi

  if [[ -z "$ip" ]]; then
    echo "Could not discover Quest Wi-Fi IP. Pass it explicitly:" >&2
    echo "  scripts/quest-adb.sh enable-wifi 10.0.0.xxx" >&2
    return 1
  fi

  echo "Enabling Wi-Fi ADB on $serial at $ip:5555"
  "$ADB" -s "$serial" tcpip 5555
  sleep 2
  "$ADB" connect "$ip:5555"
  printf '%s\n' "$ip" > "$IP_FILE"
  "$ADB" devices -l
}

cmd_connect() {
  local ip="${1:-}"
  if [[ -z "$ip" && -f "$IP_FILE" ]]; then
    ip="$(<"$IP_FILE")"
  fi

  if [[ -z "$ip" ]]; then
    echo "No Quest IP provided and no cached IP found." >&2
    echo "Usage: scripts/quest-adb.sh connect 10.0.0.xxx" >&2
    return 1
  fi

  "$ADB" connect "$(normalize_ip "$ip")"
  printf '%s\n' "${ip%%:*}" > "$IP_FILE"
  "$ADB" devices -l
}

cmd_install_run() {
  local apk="${1:-$APK_PATH}"
  local serial
  if [[ ! -f "$apk" ]]; then
    echo "APK not found: $apk" >&2
    return 1
  fi

  serial="$(ensure_any_device)"
  "$ADB" -s "$serial" install -r "$apk"
  cmd_run
}

cmd_run() {
  local serial
  serial="$(ensure_any_device)"
  "$ADB" -s "$serial" shell am force-stop "$PACKAGE_ID" || true
  "$ADB" -s "$serial" shell monkey -p "$PACKAGE_ID" -c android.intent.category.LAUNCHER 1
}

cmd_logs() {
  local serial
  serial="$(ensure_any_device)"
  "$ADB" -s "$serial" logcat -v time Unity:I AndroidRuntime:E '*:S'
}

case "${1:-}" in
  status)
    cmd_status
    ;;
  enable-wifi)
    shift
    cmd_enable_wifi "${1:-}"
    ;;
  connect)
    shift
    cmd_connect "${1:-}"
    ;;
  install-run)
    shift
    cmd_install_run "${1:-}"
    ;;
  run)
    cmd_run
    ;;
  logs)
    cmd_logs
    ;;
  -h|--help|help|"")
    usage
    ;;
  *)
    echo "Unknown command: $1" >&2
    usage >&2
    exit 1
    ;;
esac
