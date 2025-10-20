using UnityEngine;
using UnityEngine.XR;
using System;

namespace NeglectFix.Utils
{
    /// <summary>
    /// Detects when user is gazing left in VR using Quest 2 head tracking.
    /// Provides events for left-gaze detection to integrate with neurofeedback.
    ///
    /// Left-gaze defined as: Head yaw > threshold degrees (default: 15°)
    /// Positive yaw = looking left, negative yaw = looking right
    /// </summary>
    public class GazeDetector : MonoBehaviour
    {
        [Header("Gaze Settings")]
        [Tooltip("Minimum yaw angle (degrees) to count as looking left")]
        [Range(5f, 45f)]
        public float leftGazeThreshold = 15f;

        [Tooltip("Optional: Pitch threshold for up/down gaze filtering")]
        [Range(-30f, 30f)]
        public float maxPitchDeviation = 30f;

        [Header("Smoothing")]
        [Tooltip("Smooth head rotation over frames to reduce jitter")]
        public bool useSmoothing = true;

        [Tooltip("Smoothing factor (higher = smoother but more lag)")]
        [Range(0.1f, 0.9f)]
        public float smoothingFactor = 0.3f;

        [Header("Status")]
        public bool isLookingLeft = false;
        public float currentYaw = 0f;
        public float currentPitch = 0f;
        public float currentRoll = 0f;

        // Tracking references
        private Camera vrCamera;
        private Vector3 smoothedEulerAngles;

        // Events
        public event Action OnStartLookingLeft;
        public event Action OnStopLookingLeft;
        public event Action<float> OnYawChanged;

        void Start()
        {
            // Get VR camera (usually Main Camera with XR Rig)
            vrCamera = Camera.main;
            if (vrCamera == null)
            {
                Debug.LogError("[GazeDetector] Main Camera not found! Make sure your XR Rig is set up.");
                enabled = false;
                return;
            }

            smoothedEulerAngles = vrCamera.transform.eulerAngles;
            Debug.Log("[GazeDetector] Initialized. Left-gaze threshold: " + leftGazeThreshold + "°");
        }

        void Update()
        {
            UpdateGazeDirection();
        }

        private void UpdateGazeDirection()
        {
            // Get current head rotation
            Vector3 currentEuler = vrCamera.transform.eulerAngles;

            // Apply smoothing if enabled
            if (useSmoothing)
            {
                smoothedEulerAngles = Vector3.Lerp(smoothedEulerAngles, currentEuler, smoothingFactor);
                currentEuler = smoothedEulerAngles;
            }

            // Convert to -180 to +180 range for intuitive understanding
            currentYaw = NormalizeAngle(currentEuler.y);
            currentPitch = NormalizeAngle(currentEuler.x);
            currentRoll = NormalizeAngle(currentEuler.z);

            // Check if looking left
            bool wasLookingLeft = isLookingLeft;
            isLookingLeft = IsGazingLeft();

            // Trigger events on state changes
            if (isLookingLeft && !wasLookingLeft)
            {
                OnStartLookingLeft?.Invoke();
            }
            else if (!isLookingLeft && wasLookingLeft)
            {
                OnStopLookingLeft?.Invoke();
            }

            // Emit yaw change event
            OnYawChanged?.Invoke(currentYaw);
        }

        private bool IsGazingLeft()
        {
            // Left-gaze = positive yaw > threshold
            // Also check pitch isn't too extreme (looking up/down)
            return currentYaw > leftGazeThreshold &&
                   Mathf.Abs(currentPitch) < maxPitchDeviation;
        }

        /// <summary>
        /// Converts angle from 0-360 range to -180 to +180 range
        /// </summary>
        private float NormalizeAngle(float angle)
        {
            if (angle > 180f)
                return angle - 360f;
            return angle;
        }

        // Public accessors
        public bool IsLookingLeft() => isLookingLeft;
        public float GetYaw() => currentYaw;
        public float GetPitch() => currentPitch;
        public float GetRoll() => currentRoll;

        // Get intensity of left-gaze (0 = threshold, 1 = 90° left)
        public float GetLeftGazeIntensity()
        {
            if (!isLookingLeft) return 0f;
            return Mathf.Clamp01((currentYaw - leftGazeThreshold) / (90f - leftGazeThreshold));
        }

        // Debug visualization
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 310, 10, 300, 120));
            GUILayout.Label("<b>Head Tracking</b>");
            GUILayout.Label($"Yaw:   {currentYaw:F1}° {(currentYaw > 0 ? "← LEFT" : "RIGHT →")}");
            GUILayout.Label($"Pitch: {currentPitch:F1}°");
            GUILayout.Label($"Status: {(isLookingLeft ? "<color=green>Looking LEFT ✓</color>" : "<color=grey>Not looking left</color>")}");
            GUILayout.EndArea();
        }

        // Gizmo visualization in Scene view
        void OnDrawGizmos()
        {
            if (!Application.isPlaying || vrCamera == null) return;

            // Draw gaze direction
            Vector3 gazeDirection = vrCamera.transform.forward;
            Vector3 startPos = vrCamera.transform.position;

            // Color based on left-gaze state
            Gizmos.color = isLookingLeft ? Color.green : Color.red;
            Gizmos.DrawRay(startPos, gazeDirection * 2f);

            // Draw left-threshold indicator
            Gizmos.color = Color.yellow;
            Vector3 thresholdDir = Quaternion.Euler(0, leftGazeThreshold, 0) * Vector3.forward;
            Gizmos.DrawRay(startPos, thresholdDir * 1.5f);
        }
    }
}
