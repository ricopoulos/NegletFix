using UnityEngine;
using System;
using System.Collections;

namespace NeglectFix.Tasks
{
    /// <summary>
    /// Base class for all VR rehabilitation tasks.
    /// Handles session structure (baseline, training, cooldown), timing, and data logging.
    ///
    /// Inherit from this class to create specific tasks (e.g., KitchenDiscovery, BirdWatching, etc.)
    /// </summary>
    public abstract class TaskManager : MonoBehaviour
    {
        [Header("Task Settings")]
        [Tooltip("Task name for logging")]
        public string taskName = "Task";

        [Tooltip("Task difficulty level (1-3)")]
        [Range(1, 3)]
        public int taskLevel = 1;

        [Header("Session Structure")]
        [Tooltip("Baseline phase duration (seconds)")]
        public float baselineDuration = 120f; // 2 minutes

        [Tooltip("Training phase duration (seconds)")]
        public float trainingDuration = 900f; // 15 minutes

        [Tooltip("Cool-down phase duration (seconds)")]
        public float cooldownDuration = 180f; // 3 minutes

        [Header("Dependencies")]
        public NeglectFix.Utils.DataLogger dataLogger;
        public NeglectFix.EEG.EngagementCalculator engagementCalculator;
        public NeglectFix.Utils.RewardController rewardController;

        [Header("Status")]
        public SessionPhase currentPhase = SessionPhase.NotStarted;
        public float phaseStartTime = 0f;
        public float sessionStartTime = 0f;

        // Session phases
        public enum SessionPhase
        {
            NotStarted,
            Baseline,
            Training,
            Cooldown,
            Completed
        }

        // Events
        public event Action OnSessionStarted;
        public event Action OnBaselineStarted;
        public event Action OnTrainingStarted;
        public event Action OnCooldownStarted;
        public event Action OnSessionCompleted;

        protected virtual void Start()
        {
            // Find dependencies
            if (dataLogger == null)
                dataLogger = FindObjectOfType<NeglectFix.Utils.DataLogger>();

            if (engagementCalculator == null)
                engagementCalculator = FindObjectOfType<NeglectFix.EEG.EngagementCalculator>();

            if (rewardController == null)
                rewardController = FindObjectOfType<NeglectFix.Utils.RewardController>();

            // Auto-start session
            StartCoroutine(RunSessionSequence());
        }

        private IEnumerator RunSessionSequence()
        {
            // Wait a moment for all systems to initialize
            yield return new WaitForSeconds(2f);

            sessionStartTime = Time.time;
            OnSessionStarted?.Invoke();

            // Phase 1: Baseline
            yield return StartCoroutine(BaselinePhase());

            // Phase 2: Training
            yield return StartCoroutine(TrainingPhase());

            // Phase 3: Cooldown
            yield return StartCoroutine(CooldownPhase());

            // Session complete
            currentPhase = SessionPhase.Completed;
            OnSessionCompleted?.Invoke();
            OnTaskCompleted();

            Debug.Log($"[{taskName}] Session completed! Total time: {GetSessionDuration() / 60f:F1} minutes");
        }

        private IEnumerator BaselinePhase()
        {
            currentPhase = SessionPhase.Baseline;
            phaseStartTime = Time.time;
            OnBaselineStarted?.Invoke();

            Debug.Log($"[{taskName}] === BASELINE PHASE ===");
            Debug.Log("Sit comfortably, relax, and look around naturally.");
            Debug.Log($"Duration: {baselineDuration} seconds ({baselineDuration / 60f:F1} minutes)");

            // Start EEG baseline measurement
            if (engagementCalculator != null)
            {
                engagementCalculator.StartBaseline();
            }

            // Start data logging
            if (dataLogger != null && !dataLogger.IsLogging())
            {
                dataLogger.StartLogging();
            }

            dataLogger?.LogEvent($"baseline_start_{taskName}");

            // Call task-specific baseline setup
            OnBaselinePhaseStart();

            // Wait for baseline duration
            yield return new WaitForSeconds(baselineDuration);

            dataLogger?.LogEvent("baseline_end");
            OnBaselinePhaseEnd();

            Debug.Log($"[{taskName}] Baseline complete!");
        }

        private IEnumerator TrainingPhase()
        {
            currentPhase = SessionPhase.Training;
            phaseStartTime = Time.time;
            OnTrainingStarted?.Invoke();

            Debug.Log($"[{taskName}] === TRAINING PHASE ===");
            Debug.Log("Neurofeedback is now active. Focus on looking left!");
            Debug.Log($"Duration: {trainingDuration} seconds ({trainingDuration / 60f:F1} minutes)");

            dataLogger?.LogEvent($"training_start_{taskName}");

            // Call task-specific training setup
            OnTrainingPhaseStart();

            // Wait for training duration
            yield return new WaitForSeconds(trainingDuration);

            dataLogger?.LogEvent("training_end");
            OnTrainingPhaseEnd();

            Debug.Log($"[{taskName}] Training complete!");
        }

        private IEnumerator CooldownPhase()
        {
            currentPhase = SessionPhase.Cooldown;
            phaseStartTime = Time.time;
            OnCooldownStarted?.Invoke();

            Debug.Log($"[{taskName}] === COOL-DOWN PHASE ===");
            Debug.Log("Relax and let your brain consolidate what you learned.");
            Debug.Log($"Duration: {cooldownDuration} seconds ({cooldownDuration / 60f:F1} minutes)");

            dataLogger?.LogEvent($"cooldown_start_{taskName}");

            // Call task-specific cooldown setup
            OnCooldownPhaseStart();

            // Wait for cooldown duration
            yield return new WaitForSeconds(cooldownDuration);

            dataLogger?.LogEvent("cooldown_end");
            OnCooldownPhaseEnd();

            Debug.Log($"[{taskName}] Cool-down complete!");
        }

        // Abstract methods - override in child classes

        /// <summary>
        /// Called when baseline phase starts. Set up resting environment.
        /// </summary>
        protected virtual void OnBaselinePhaseStart()
        {
            // Override in child task
        }

        /// <summary>
        /// Called when baseline phase ends.
        /// </summary>
        protected virtual void OnBaselinePhaseEnd()
        {
            // Override in child task
        }

        /// <summary>
        /// Called when training phase starts. Activate task mechanics and neurofeedback.
        /// </summary>
        protected virtual void OnTrainingPhaseStart()
        {
            // Override in child task
        }

        /// <summary>
        /// Called when training phase ends.
        /// </summary>
        protected virtual void OnTrainingPhaseEnd()
        {
            // Override in child task
        }

        /// <summary>
        /// Called when cooldown phase starts. Reduce task difficulty or disable mechanics.
        /// </summary>
        protected virtual void OnCooldownPhaseStart()
        {
            // Override in child task
        }

        /// <summary>
        /// Called when cooldown phase ends.
        /// </summary>
        protected virtual void OnCooldownPhaseEnd()
        {
            // Override in child task
        }

        /// <summary>
        /// Called when entire session is complete.
        /// </summary>
        protected virtual void OnTaskCompleted()
        {
            // Stop logging
            if (dataLogger != null)
            {
                dataLogger.StopLogging();
            }

            // Show summary
            ShowSessionSummary();
        }

        // Utility methods

        public float GetPhaseElapsedTime()
        {
            return Time.time - phaseStartTime;
        }

        public float GetPhaseRemainingTime()
        {
            float duration = currentPhase switch
            {
                SessionPhase.Baseline => baselineDuration,
                SessionPhase.Training => trainingDuration,
                SessionPhase.Cooldown => cooldownDuration,
                _ => 0f
            };
            return Mathf.Max(0f, duration - GetPhaseElapsedTime());
        }

        public float GetSessionDuration()
        {
            return Time.time - sessionStartTime;
        }

        public float GetPhaseProgress()
        {
            float duration = currentPhase switch
            {
                SessionPhase.Baseline => baselineDuration,
                SessionPhase.Training => trainingDuration,
                SessionPhase.Cooldown => cooldownDuration,
                _ => 1f
            };
            return Mathf.Clamp01(GetPhaseElapsedTime() / duration);
        }

        private void ShowSessionSummary()
        {
            Debug.Log("=== SESSION SUMMARY ===");
            Debug.Log($"Task: {taskName} (Level {taskLevel})");
            Debug.Log($"Total duration: {GetSessionDuration() / 60f:F1} minutes");

            if (rewardController != null)
            {
                Debug.Log($"Total rewards: {rewardController.GetTotalRewards()}");
            }

            if (engagementCalculator != null && engagementCalculator.baselineComplete)
            {
                Debug.Log($"Final success rate: {engagementCalculator.GetSuccessRate():P0}");
            }

            if (dataLogger != null)
            {
                Debug.Log($"Data logged to: {dataLogger.GetCurrentSessionFile()}");
            }

            Debug.Log("=======================");
        }

        // Debug UI
        protected virtual void OnGUI()
        {
            if (currentPhase == SessionPhase.NotStarted || currentPhase == SessionPhase.Completed)
                return;

            // Phase timer
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, 10, 400, 80));
            GUILayout.Label($"<b><size=18>{taskName}</size></b>");
            GUILayout.Label($"<b>Phase:</b> {currentPhase}");
            GUILayout.Label($"<b>Time remaining:</b> {GetPhaseRemainingTime() / 60f:F1} min");
            GUILayout.Label($"<b>Progress:</b> {GetPhaseProgress():P0}");
            GUILayout.EndArea();
        }
    }
}
