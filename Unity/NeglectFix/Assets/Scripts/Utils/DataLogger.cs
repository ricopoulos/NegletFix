using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NeglectFix.Utils
{
    /// <summary>
    /// Logs EEG, gaze, and performance data to CSV for analysis.
    /// Creates timestamped session files with all neurofeedback data.
    ///
    /// CSV Format:
    /// timestamp, tp10_alpha, tp10_beta, tp10_theta, engagement_score, threshold,
    /// head_yaw, head_pitch, left_gaze, reward_triggered, task_event
    /// </summary>
    public class DataLogger : MonoBehaviour
    {
        [Header("Dependencies")]
        public NeglectFix.EEG.MuseOSCReceiver museReceiver;
        public NeglectFix.EEG.EngagementCalculator engagementCalculator;
        public GazeDetector gazeDetector;
        public RewardController rewardController;

        [Header("Logging Settings")]
        [Tooltip("Folder name for log files (relative to project root)")]
        public string logFolderName = "Logs";

        [Tooltip("Logging frequency (samples per second)")]
        [Range(1f, 30f)]
        public float loggingFrequency = 10f; // 10 Hz recommended

        [Tooltip("Auto-start logging on scene load")]
        public bool autoStartLogging = false;

        [Header("Status")]
        public bool isLogging = false;
        public string currentSessionFile = "";
        public int samplesLogged = 0;

        // Internal state
        private StreamWriter csvWriter;
        private float lastLogTime = 0f;
        private float sessionStartTime = 0f;
        private StringBuilder logBuffer = new StringBuilder();
        private List<string> eventLog = new List<string>();

        // CSV header
        private const string CSV_HEADER = "timestamp_ms,tp10_alpha,tp10_beta,tp10_theta,engagement_score,threshold,head_yaw,head_pitch,left_gaze,reward_triggered,event";

        void Start()
        {
            // Find dependencies if not assigned
            if (museReceiver == null)
                museReceiver = FindObjectOfType<NeglectFix.EEG.MuseOSCReceiver>();

            if (engagementCalculator == null)
                engagementCalculator = FindObjectOfType<NeglectFix.EEG.EngagementCalculator>();

            if (gazeDetector == null)
                gazeDetector = FindObjectOfType<GazeDetector>();

            if (rewardController == null)
                rewardController = FindObjectOfType<RewardController>();

            // Subscribe to events
            if (rewardController != null)
            {
                rewardController.OnRewardTriggered += () => LogEvent("reward_triggered");
            }

            if (engagementCalculator != null)
            {
                engagementCalculator.OnBaselineComplete += () => LogEvent("baseline_complete");
            }

            if (autoStartLogging)
            {
                StartLogging();
            }
        }

        public void StartLogging()
        {
            if (isLogging)
            {
                Debug.LogWarning("[DataLogger] Already logging!");
                return;
            }

            // Create log folder if it doesn't exist
            string logPath = Path.Combine(Application.dataPath, "..", logFolderName);
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
                Debug.Log($"[DataLogger] Created log folder: {logPath}");
            }

            // Create session file with timestamp
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"session_{timestamp}.csv";
            currentSessionFile = Path.Combine(logPath, fileName);

            // Open CSV writer
            try
            {
                csvWriter = new StreamWriter(currentSessionFile, false);
                csvWriter.WriteLine(CSV_HEADER);
                csvWriter.WriteLine($"# Session started: {DateTime.Now}");
                csvWriter.WriteLine($"# Unity Version: {Application.unityVersion}");
                csvWriter.WriteLine($"# Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
                csvWriter.WriteLine(CSV_HEADER); // Write header again after metadata

                isLogging = true;
                sessionStartTime = Time.time;
                samplesLogged = 0;

                Debug.Log($"[DataLogger] ✓ Logging started: {currentSessionFile}");
                LogEvent("session_start");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataLogger] Failed to create log file: {e.Message}");
            }
        }

        public void StopLogging()
        {
            if (!isLogging) return;

            LogEvent("session_end");

            // Write session summary
            WriteSummary();

            // Close file
            if (csvWriter != null)
            {
                csvWriter.Flush();
                csvWriter.Close();
                csvWriter = null;
            }

            isLogging = false;
            Debug.Log($"[DataLogger] Logging stopped. {samplesLogged} samples logged to: {currentSessionFile}");
        }

        void Update()
        {
            if (!isLogging) return;

            // Log at specified frequency
            float interval = 1f / loggingFrequency;
            if (Time.time - lastLogTime >= interval)
            {
                LogDataPoint();
                lastLogTime = Time.time;
            }
        }

        private void LogDataPoint()
        {
            if (csvWriter == null) return;

            // Get current timestamp (milliseconds since session start)
            float timestamp = (Time.time - sessionStartTime) * 1000f;

            // Get EEG data
            float tp10_alpha = museReceiver != null ? museReceiver.GetTP10Alpha() : 0f;
            float tp10_beta = museReceiver != null ? museReceiver.GetTP10Beta() : 0f;
            float tp10_theta = museReceiver != null ? museReceiver.GetTP10Theta() : 0f;

            // Get engagement data
            float engagementScore = engagementCalculator != null ? engagementCalculator.GetEngagementScore() : 0f;
            float threshold = engagementCalculator != null ? engagementCalculator.GetThreshold() : 0f;

            // Get gaze data
            float headYaw = gazeDetector != null ? gazeDetector.GetYaw() : 0f;
            float headPitch = gazeDetector != null ? gazeDetector.GetPitch() : 0f;
            int leftGaze = (gazeDetector != null && gazeDetector.IsLookingLeft()) ? 1 : 0;

            // Check for reward trigger
            int rewardTriggered = (rewardController != null && rewardController.rewardActive) ? 1 : 0;

            // Get event (if any)
            string eventName = "";
            if (eventLog.Count > 0)
            {
                eventName = eventLog[0];
                eventLog.RemoveAt(0);
            }

            // Build CSV line
            logBuffer.Clear();
            logBuffer.Append($"{timestamp:F0},");
            logBuffer.Append($"{tp10_alpha:F4},{tp10_beta:F4},{tp10_theta:F4},");
            logBuffer.Append($"{engagementScore:F4},{threshold:F4},");
            logBuffer.Append($"{headYaw:F2},{headPitch:F2},");
            logBuffer.Append($"{leftGaze},{rewardTriggered},");
            logBuffer.Append(eventName);

            // Write to file
            csvWriter.WriteLine(logBuffer.ToString());
            samplesLogged++;

            // Flush every 100 samples (balance between performance and data safety)
            if (samplesLogged % 100 == 0)
            {
                csvWriter.Flush();
            }
        }

        /// <summary>
        /// Log a custom event (e.g., "baseline_complete", "task_started", "object_discovered")
        /// </summary>
        public void LogEvent(string eventName)
        {
            eventLog.Add(eventName);
            Debug.Log($"[DataLogger] Event logged: {eventName}");
        }

        private void WriteSummary()
        {
            if (csvWriter == null) return;

            float sessionDuration = Time.time - sessionStartTime;

            csvWriter.WriteLine();
            csvWriter.WriteLine("# SESSION SUMMARY");
            csvWriter.WriteLine($"# Total duration: {sessionDuration:F1} seconds ({sessionDuration / 60f:F1} minutes)");
            csvWriter.WriteLine($"# Total samples: {samplesLogged}");
            csvWriter.WriteLine($"# Sample rate: {loggingFrequency} Hz");

            if (rewardController != null)
            {
                int totalRewards = rewardController.GetTotalRewards();
                float rewardRate = totalRewards / sessionDuration;
                csvWriter.WriteLine($"# Total rewards: {totalRewards} ({rewardRate:F2} rewards/second)");
            }

            if (engagementCalculator != null && engagementCalculator.baselineComplete)
            {
                float finalSuccessRate = engagementCalculator.GetSuccessRate();
                csvWriter.WriteLine($"# Final success rate: {finalSuccessRate:P0}");
                csvWriter.WriteLine($"# Final threshold: {engagementCalculator.GetThreshold():F2}");
            }

            csvWriter.WriteLine($"# Session ended: {DateTime.Now}");
        }

        void OnApplicationQuit()
        {
            // Ensure logging stops cleanly
            StopLogging();
        }

        void OnDestroy()
        {
            StopLogging();
        }

        // Public API
        public bool IsLogging() => isLogging;
        public int GetSamplesLogged() => samplesLogged;
        public string GetCurrentSessionFile() => currentSessionFile;

        // Debug UI
        void OnGUI()
        {
            if (!isLogging) return;

            float sessionDuration = Time.time - sessionStartTime;

            GUILayout.BeginArea(new Rect(10, Screen.height - 110, 400, 100));
            GUILayout.Label("<b>Data Logging</b>");
            GUILayout.Label($"Session: {sessionDuration / 60f:F1} min | Samples: {samplesLogged}");
            GUILayout.Label($"File: {Path.GetFileName(currentSessionFile)}");
            GUILayout.EndArea();
        }
    }
}
