using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

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

            string logPath = GetSessionLogDirectory();

            try
            {
                // Create log folder if it doesn't exist
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

        private string GetSessionLogDirectory()
        {
#if UNITY_EDITOR
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", logFolderName));
#else
            return Path.Combine(Application.persistentDataPath, logFolderName);
#endif
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
            CloseTrialFile();
            CloseFieldMappingFile();
        }

        void OnDestroy()
        {
            StopLogging();
            CloseTrialFile();
            CloseFieldMappingFile();
        }

        // Public API
        public bool IsLogging() => isLogging;
        public int GetSamplesLogged() => samplesLogged;
        public string GetCurrentSessionFile() => currentSessionFile;
        public void CloseTrainingTrialLog() => CloseTrialFile();

        #region Contrast Sensitivity Logging

        /// <summary>
        /// Log contrast sensitivity test results to dedicated assessment file
        /// </summary>
        public void LogContrastSensitivityResults(NeglectFix.Assessment.ContrastSensitivityResults results)
        {
            // Create assessments folder
            string assessmentsPath = Path.Combine(Application.persistentDataPath, "assessments");
            if (!Directory.Exists(assessmentsPath))
            {
                Directory.CreateDirectory(assessmentsPath);
            }

            // Trial-level data file
            string trialFilename = $"contrast_trials_{results.testDate:yyyy-MM-dd_HH-mm}.csv";
            string trialPath = Path.Combine(assessmentsPath, trialFilename);

            try
            {
                using (StreamWriter writer = new StreamWriter(trialPath, false))
                {
                    // Header
                    writer.WriteLine("timestamp,logCS,letter_presented,letter_response,correct,hemifield,reaction_ms,triplet,letter_index");

                    // Trial data
                    foreach (var trial in results.trialHistory)
                    {
                        writer.WriteLine($"{trial.timestamp:O},{trial.logCS:F2},{trial.presentedLetter}," +
                            $"{trial.responseLetter},{trial.correct},{trial.hemifield},{trial.reactionTimeMs:F0}," +
                            $"{trial.tripletNumber},{trial.letterInTriplet}");
                    }
                }
                Debug.Log($"[DataLogger] Contrast trials saved: {trialPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataLogger] Failed to save contrast trials: {e.Message}");
            }

            // Append to summary file (tracks progress over time)
            string summaryPath = Path.Combine(assessmentsPath, "contrast_summary.csv");
            bool writeHeader = !File.Exists(summaryPath);

            try
            {
                using (StreamWriter writer = new StreamWriter(summaryPath, append: true))
                {
                    if (writeHeader)
                    {
                        writer.WriteLine("date,time,central_logcs,left_logcs,right_logcs,asymmetry,avg_rt_ms,total_trials");
                    }

                    float avgRT = results.GetAverageReactionTime();
                    writer.WriteLine($"{results.testDate:yyyy-MM-dd},{results.testDate:HH:mm}," +
                        $"{results.centralLogCS:F2},{results.leftHemifieldLogCS:F2}," +
                        $"{results.rightHemifieldLogCS:F2},{results.asymmetry:F2}," +
                        $"{avgRT:F0},{results.trialHistory.Count}");
                }
                Debug.Log($"[DataLogger] Contrast summary updated: {summaryPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataLogger] Failed to update contrast summary: {e.Message}");
            }

            // Also log event if session logging is active
            if (isLogging)
            {
                LogEvent($"contrast_test_complete_L{results.leftHemifieldLogCS:F2}_R{results.rightHemifieldLogCS:F2}");
            }
        }

        /// <summary>
        /// Get path to assessments folder
        /// </summary>
        public string GetAssessmentsPath()
        {
            return Path.Combine(Application.persistentDataPath, "assessments");
        }

        #endregion

        #region AV Training Trial Logging

        // Per-session trial CSV writer (separate from the 10Hz neurofeedback CSV)
        private StreamWriter trialWriter;
        private string currentTrialFile = "";
        private int trainingTrialsLogged = 0;
        private int trainingRehabTrialsLogged = 0;
        private int trainingControlTrialsLogged = 0;

        private const string TRIAL_CSV_HEADER =
            "timestamp_ms,session_index,block_index,trial_index,eccentricity_deg,hemifield," +
            "contrast_logcs,stimulus_onset_ms,audio_onset_ms,response_onset_ms,rt_ms,hit,av_delta_ms," +
            "trial_type,is_control_trial,counts_for_rehab_dose,horizontal_angle_deg,vertical_angle_deg";

        /// <summary>
        /// Log a single AV training trial. Lazily opens a per-program-session trial file the first time
        /// it's called per session. Trial files live in Application.persistentDataPath/training_trials/.
        ///
        /// Schema: see TRIAL_CSV_HEADER. RT in ms (negative if miss). av_delta_ms is audio onset
        /// relative to visual onset (target sub-50ms per Bean/Stein/Rowland 2023).
        /// </summary>
        public void LogTrainingTrial(NeglectFix.Tasks.AudioVisualTraining.TrainingTrial trial)
        {
            if (trialWriter == null)
            {
                OpenTrialFile();
            }

            if (trialWriter == null) return;

            try
            {
                float timestampMs = (Time.time - sessionStartTime) * 1000f;
                string line =
                    $"{timestampMs:F0}," +
                    $"{trial.sessionIndex},{trial.blockIndex},{trial.trialIndex}," +
                    $"{trial.eccentricityDeg:F2},{trial.hemifield}," +
                    $"{trial.contrastLogCS:F3}," +
                    $"{trial.stimulusOnsetMs:F0},{trial.audioOnsetMs:F0}," +
                    $"{trial.responseOnsetMs:F0},{trial.rtMs:F0}," +
                    $"{(trial.hit ? 1 : 0)}," +
                    $"{trial.avDeltaMs:F2}," +
                    $"{trial.trialType}," +
                    $"{(trial.isControlTrial ? 1 : 0)}," +
                    $"{(trial.countsForRehabDose ? 1 : 0)}," +
                    $"{trial.horizontalAngleDeg:F2},{trial.verticalAngleDeg:F2}";

                trialWriter.WriteLine(line);
                trainingTrialsLogged++;
                if (trial.isControlTrial)
                    trainingControlTrialsLogged++;
                else if (trial.countsForRehabDose)
                    trainingRehabTrialsLogged++;

                if (trainingTrialsLogged % 20 == 0)
                    trialWriter.Flush();
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataLogger] LogTrainingTrial failed: {e.Message}");
            }
        }

        private void OpenTrialFile()
        {
            string trialsPath = Path.Combine(Application.persistentDataPath, "training_trials");
            if (!Directory.Exists(trialsPath))
                Directory.CreateDirectory(trialsPath);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            currentTrialFile = Path.Combine(trialsPath, $"av_training_{timestamp}.csv");
            trainingTrialsLogged = 0;
            trainingRehabTrialsLogged = 0;
            trainingControlTrialsLogged = 0;

            try
            {
                trialWriter = new StreamWriter(currentTrialFile, false);
                // Metadata header
                trialWriter.WriteLine($"# AV Training Trials");
                trialWriter.WriteLine($"# Session started: {DateTime.Now:O}");
                trialWriter.WriteLine($"# Unity: {Application.unityVersion}");
                trialWriter.WriteLine($"# Device: {SystemInfo.deviceModel} / {SystemInfo.operatingSystem}");
                trialWriter.WriteLine($"# Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
                trialWriter.WriteLine(TRIAL_CSV_HEADER);
                Debug.Log($"[DataLogger] Trial logging opened: {currentTrialFile}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataLogger] Failed to open trial file: {e.Message}");
                trialWriter = null;
            }
        }

        private void CloseTrialFile()
        {
            if (trialWriter != null)
            {
                try
                {
                    trialWriter.WriteLine();
                    trialWriter.WriteLine("# TRIAL SUMMARY");
                    trialWriter.WriteLine($"# Total recorded trials: {trainingTrialsLogged}");
                    trialWriter.WriteLine($"# Rehab-dose trials: {trainingRehabTrialsLogged}");
                    trialWriter.WriteLine($"# Control trials: {trainingControlTrialsLogged}");
                    trialWriter.Flush();
                    trialWriter.Close();
                }
                catch { }
                trialWriter = null;
                Debug.Log($"[DataLogger] Trial log closed: {currentTrialFile} ({trainingTrialsLogged} trials).");
            }
        }

        public string GetCurrentTrialFile() => currentTrialFile;
        public int GetTrainingTrialsLogged() => trainingTrialsLogged;
        public int GetTrainingRehabTrialsLogged() => trainingRehabTrialsLogged;
        public int GetTrainingControlTrialsLogged() => trainingControlTrialsLogged;

        #endregion

        #region Field Mapping Calibration Logging

        private sealed class FieldMapPointStats
        {
            public string axis;
            public float horizontalAngleDeg;
            public float verticalAngleDeg;
            public int total;
            public int hits;
            public float hitRtSumMs;

            public float HitRate => total > 0 ? (float)hits / total : 0f;
            public float MeanHitRtMs => hits > 0 ? hitRtSumMs / hits : -1f;
        }

        private StreamWriter fieldMapWriter;
        private string currentFieldMappingFile = "";
        private int fieldMappingTrialsLogged = 0;
        private string fieldMappingAffectedAxis = "left";
        private readonly Dictionary<string, FieldMapPointStats> fieldMapPointStats = new Dictionary<string, FieldMapPointStats>();
        private readonly List<string> fieldMapRecommendationLabels = new List<string>();

        private const string FIELD_MAPPING_CSV_HEADER =
            "timestamp_ms,trial_index,repeat_index,axis,horizontal_angle_deg,vertical_angle_deg,stimulus_distance_m," +
            "stimulus_world_x,stimulus_world_y,stimulus_world_z,camera_world_x,camera_world_y,camera_world_z," +
            "camera_relative_dir_x,camera_relative_dir_y,camera_relative_dir_z,head_yaw_onset_deg,head_pitch_onset_deg,head_roll_onset_deg," +
            "stimulus_onset_ms,response_onset_ms,rt_ms,hit,head_yaw_response_deg,head_pitch_response_deg,head_roll_response_deg," +
            "camera_relative_dir_response_x,camera_relative_dir_response_y,camera_relative_dir_response_z";

        public void SetFieldMappingAffectedAxis(string axis)
        {
            if (string.IsNullOrWhiteSpace(axis))
                return;

            fieldMappingAffectedAxis = axis.Trim().ToLowerInvariant();
        }

        public void LogFieldMappingTrial(NeglectFix.Assessment.FieldMappingCalibration.CalibrationTrial trial)
        {
            if (fieldMapWriter == null)
                OpenFieldMappingFile();

            if (fieldMapWriter == null) return;

            try
            {
                float timestampMs = isLogging
                    ? (Time.time - sessionStartTime) * 1000f
                    : Time.time * 1000f;

                string line = string.Join(",",
                    FloatCsv(timestampMs, 0),
                    trial.trialIndex.ToString(CultureInfo.InvariantCulture),
                    trial.repeatIndex.ToString(CultureInfo.InvariantCulture),
                    trial.axis,
                    FloatCsv(trial.horizontalAngleDeg, 2),
                    FloatCsv(trial.verticalAngleDeg, 2),
                    FloatCsv(trial.stimulusDistanceMeters, 3),
                    FloatCsv(trial.stimulusWorldPosition.x, 4),
                    FloatCsv(trial.stimulusWorldPosition.y, 4),
                    FloatCsv(trial.stimulusWorldPosition.z, 4),
                    FloatCsv(trial.cameraWorldPosition.x, 4),
                    FloatCsv(trial.cameraWorldPosition.y, 4),
                    FloatCsv(trial.cameraWorldPosition.z, 4),
                    FloatCsv(trial.cameraRelativeDirectionAtOnset.x, 5),
                    FloatCsv(trial.cameraRelativeDirectionAtOnset.y, 5),
                    FloatCsv(trial.cameraRelativeDirectionAtOnset.z, 5),
                    FloatCsv(trial.headYawOnsetDeg, 2),
                    FloatCsv(trial.headPitchOnsetDeg, 2),
                    FloatCsv(trial.headRollOnsetDeg, 2),
                    FloatCsv(trial.stimulusOnsetMs, 0),
                    FloatCsv(trial.responseOnsetMs, 0),
                    FloatCsv(trial.rtMs, 0),
                    trial.hit ? "1" : "0",
                    FloatCsv(trial.headYawResponseDeg, 2),
                    FloatCsv(trial.headPitchResponseDeg, 2),
                    FloatCsv(trial.headRollResponseDeg, 2),
                    FloatCsv(trial.cameraRelativeDirectionAtResponse.x, 5),
                    FloatCsv(trial.cameraRelativeDirectionAtResponse.y, 5),
                    FloatCsv(trial.cameraRelativeDirectionAtResponse.z, 5));

                fieldMapWriter.WriteLine(line);
                fieldMappingTrialsLogged++;
                UpdateFieldMapStats(trial);

                if (fieldMappingTrialsLogged % 20 == 0)
                    fieldMapWriter.Flush();
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataLogger] LogFieldMappingTrial failed: {e.Message}");
            }
        }

        public void CloseFieldMappingLog() => CloseFieldMappingFile();

        private void OpenFieldMappingFile()
        {
            string fieldMapPath = Path.Combine(Application.persistentDataPath, "field_mapping");
            if (!Directory.Exists(fieldMapPath))
                Directory.CreateDirectory(fieldMapPath);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            currentFieldMappingFile = Path.Combine(fieldMapPath, $"field_mapping_{timestamp}.csv");
            fieldMappingTrialsLogged = 0;
            fieldMapPointStats.Clear();
            fieldMapRecommendationLabels.Clear();

            try
            {
                fieldMapWriter = new StreamWriter(currentFieldMappingFile, false);
                fieldMapWriter.WriteLine("# Field Mapping Calibration Trials");
                fieldMapWriter.WriteLine($"# Session started: {DateTime.Now:O}");
                fieldMapWriter.WriteLine($"# Unity: {Application.unityVersion}");
                fieldMapWriter.WriteLine($"# Device: {SystemInfo.deviceModel} / {SystemInfo.operatingSystem}");
                fieldMapWriter.WriteLine($"# Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
                fieldMapWriter.WriteLine($"# Affected axis for rehab recommendation: {fieldMappingAffectedAxis}");
                fieldMapWriter.WriteLine(FIELD_MAPPING_CSV_HEADER);
                Debug.Log($"[DataLogger] Field mapping logging opened: {currentFieldMappingFile}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataLogger] Failed to open field mapping file: {e.Message}");
                fieldMapWriter = null;
            }
        }

        private void CloseFieldMappingFile()
        {
            if (fieldMapWriter == null)
                return;

            try
            {
                WriteFieldMappingSummary();
                fieldMapWriter.Flush();
                fieldMapWriter.Close();
            }
            catch { }

            fieldMapWriter = null;
            Debug.Log($"[DataLogger] Field mapping log closed: {currentFieldMappingFile} ({fieldMappingTrialsLogged} trials).");
        }

        private void UpdateFieldMapStats(NeglectFix.Assessment.FieldMappingCalibration.CalibrationTrial trial)
        {
            string key = $"{trial.axis}|{FloatCsv(trial.horizontalAngleDeg, 2)}|{FloatCsv(trial.verticalAngleDeg, 2)}";

            if (!fieldMapPointStats.TryGetValue(key, out FieldMapPointStats stats))
            {
                stats = new FieldMapPointStats
                {
                    axis = trial.axis,
                    horizontalAngleDeg = trial.horizontalAngleDeg,
                    verticalAngleDeg = trial.verticalAngleDeg
                };
                fieldMapPointStats[key] = stats;
            }

            stats.total++;

            if (trial.hit)
            {
                stats.hits++;
                if (trial.rtMs >= 0f)
                    stats.hitRtSumMs += trial.rtMs;
            }
        }

        private void WriteFieldMappingSummary()
        {
            fieldMapWriter.WriteLine();
            fieldMapWriter.WriteLine("# FIELD MAP SUMMARY");
            fieldMapWriter.WriteLine($"# Total calibration trials: {fieldMappingTrialsLogged}");

            foreach (FieldMapPointStats stats in GetSortedFieldMapStats())
            {
                fieldMapWriter.WriteLine(
                    $"# Point {stats.axis} h={stats.horizontalAngleDeg:F2} v={stats.verticalAngleDeg:F2}: " +
                    $"{stats.hits}/{stats.total} hits ({stats.HitRate:P0}), mean_hit_rt_ms={stats.MeanHitRtMs:F0}");
            }

            fieldMapRecommendationLabels.Clear();
            fieldMapRecommendationLabels.AddRange(BuildFieldMapRecommendations());

            string recommendation = fieldMapRecommendationLabels.Count > 0
                ? string.Join(" | ", fieldMapRecommendationLabels)
                : "none";

            fieldMapWriter.WriteLine($"# Recommended rehab locations: {recommendation}");
        }

        private List<FieldMapPointStats> GetSortedFieldMapStats()
        {
            var stats = new List<FieldMapPointStats>(fieldMapPointStats.Values);
            stats.Sort((a, b) =>
            {
                int axisCompare = string.Compare(a.axis, b.axis, StringComparison.Ordinal);
                if (axisCompare != 0)
                    return axisCompare;

                int horizontalCompare = Mathf.Abs(a.horizontalAngleDeg).CompareTo(Mathf.Abs(b.horizontalAngleDeg));
                if (horizontalCompare != 0)
                    return horizontalCompare;

                return Mathf.Abs(a.verticalAngleDeg).CompareTo(Mathf.Abs(b.verticalAngleDeg));
            });
            return stats;
        }

        private List<string> BuildFieldMapRecommendations()
        {
            var affected = new List<FieldMapPointStats>();
            foreach (FieldMapPointStats stats in fieldMapPointStats.Values)
            {
                if (string.Equals(stats.axis, fieldMappingAffectedAxis, StringComparison.OrdinalIgnoreCase))
                    affected.Add(stats);
            }

            affected.Sort((a, b) => Mathf.Abs(a.horizontalAngleDeg + a.verticalAngleDeg)
                .CompareTo(Mathf.Abs(b.horizontalAngleDeg + b.verticalAngleDeg)));

            var partial = affected.FindAll(stats => stats.HitRate >= 0.25f && stats.HitRate <= 0.85f);
            if (partial.Count > 0)
                return FormatFieldMapRecommendations(partial, "boundary");

            var visible = affected.FindAll(stats => stats.hits > 0);
            visible.Sort((a, b) => Mathf.Abs(b.horizontalAngleDeg + b.verticalAngleDeg)
                .CompareTo(Mathf.Abs(a.horizontalAngleDeg + a.verticalAngleDeg)));

            if (visible.Count > 0)
                return FormatFieldMapRecommendations(visible, "hardest_visible");

            return FormatFieldMapRecommendations(affected, "no_hit_use_high_contrast_validation");
        }

        private static List<string> FormatFieldMapRecommendations(List<FieldMapPointStats> stats, string reason)
        {
            var labels = new List<string>();
            int count = Mathf.Min(3, stats.Count);

            for (int i = 0; i < count; i++)
            {
                FieldMapPointStats point = stats[i];
                labels.Add(
                    $"{point.axis}(h={point.horizontalAngleDeg:F1},v={point.verticalAngleDeg:F1}," +
                    $"hit_rate={point.HitRate:P0},n={point.total},reason={reason})");
            }

            return labels;
        }

        private static string FloatCsv(float value, int decimals)
        {
            return value.ToString($"F{decimals}", CultureInfo.InvariantCulture);
        }

        public string GetCurrentFieldMappingFile() => currentFieldMappingFile;
        public int GetFieldMappingTrialsLogged() => fieldMappingTrialsLogged;
        public IReadOnlyList<string> GetFieldMappingRecommendationLabels() => fieldMapRecommendationLabels;

        #endregion

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
