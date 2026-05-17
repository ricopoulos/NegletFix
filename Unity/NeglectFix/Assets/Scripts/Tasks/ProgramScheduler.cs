using UnityEngine;
using System;
using System.IO;

namespace NeglectFix.Tasks
{
    /// <summary>
    /// Tracks the overall rehabilitation program state across sessions.
    ///
    /// Default program: Paradigm B (congruent-pair detection), Alharshan/Alwashmi 2026 dose
    /// — 30 sessions × 30 min × 5 days/week × 6 weeks = ~15 hours.
    ///
    /// Persists session count, last-session timestamp, paradigm choice, and re-measurement
    /// trigger flags to a JSON file in Application.persistentDataPath. Survives app restarts.
    ///
    /// Re-measurement is triggered after sessions configured in `reMeasurementSessions`
    /// (default: every 5 sessions for the quick-check, full reassessment at session count).
    /// </summary>
    public class ProgramScheduler : MonoBehaviour
    {
        public enum Paradigm
        {
            CongruentPair_WakeForest,    // Paradigm B — Rowland 2023 / Wake Forest style
            ThreeDMOT_IVR_DaibertNido    // Paradigm A — Daibert-Nido / Misawa / Alharshan style
        }

        [Header("Program Configuration")]
        [Tooltip("Which paradigm this phase is running. v1 default: Paradigm B.")]
        public Paradigm currentParadigm = Paradigm.CongruentPair_WakeForest;

        [Tooltip("Total sessions planned for this phase (Alharshan dose default).")]
        public int totalSessionsPlanned = 30;

        [Tooltip("Target frequency in days/week. Reminder logic uses this.")]
        [Range(1, 7)]
        public int sessionsPerWeek = 5;

        [Tooltip("Session numbers at which to prompt a quick contrast-sensitivity re-measurement.")]
        public int[] reMeasurementSessions = new int[] { 5, 10, 15, 20, 25 };

        [Tooltip("Storage filename for program state (in Application.persistentDataPath).")]
        public string stateFileName = "program_state.json";

        [Header("Status (read-only)")]
        public int sessionsCompleted = 0;
        public DateTime lastSessionTimestamp = DateTime.MinValue;
        public DateTime programStartedAt = DateTime.MinValue;
        public bool reMeasurementDue = false;

        // Events
        public event Action OnSessionStarted;
        public event Action OnSessionCompleted;
        public event Action OnReMeasurementDue;
        public event Action OnProgramCompleted;

        [Serializable]
        private class ProgramState
        {
            public string paradigm;
            public int totalSessionsPlanned;
            public int sessionsPerWeek;
            public int[] reMeasurementSessions;
            public int sessionsCompleted;
            public string lastSessionTimestampIso;
            public string programStartedAtIso;
        }

        void Awake()
        {
            LoadState();
        }

        public void RecordSessionStart()
        {
            if (sessionsCompleted == 0 && programStartedAt == DateTime.MinValue)
            {
                programStartedAt = DateTime.UtcNow;
            }

            OnSessionStarted?.Invoke();
            Debug.Log($"[ProgramScheduler] Session {sessionsCompleted + 1} of {totalSessionsPlanned} started ({currentParadigm}).");
        }

        public void RecordSessionComplete()
        {
            sessionsCompleted++;
            lastSessionTimestamp = DateTime.UtcNow;

            // Check re-measurement trigger
            reMeasurementDue = false;
            foreach (int triggerSession in reMeasurementSessions)
            {
                if (sessionsCompleted == triggerSession)
                {
                    reMeasurementDue = true;
                    OnReMeasurementDue?.Invoke();
                    Debug.Log($"[ProgramScheduler] Re-measurement due after session {sessionsCompleted}.");
                    break;
                }
            }

            // Check program completion
            if (sessionsCompleted >= totalSessionsPlanned)
            {
                OnProgramCompleted?.Invoke();
                Debug.Log($"[ProgramScheduler] ✓ Program phase complete ({sessionsCompleted}/{totalSessionsPlanned}).");
            }

            OnSessionCompleted?.Invoke();
            SaveState();

            Debug.Log($"[ProgramScheduler] Session {sessionsCompleted} of {totalSessionsPlanned} complete. " +
                      $"Next session due: {GetNextDueDate():yyyy-MM-dd HH:mm}.");
        }

        public DateTime GetNextDueDate()
        {
            if (lastSessionTimestamp == DateTime.MinValue)
                return DateTime.UtcNow;

            // sessionsPerWeek=5 → roughly every 1.4 days. Round to 1 day for simplicity.
            int daysGap = Mathf.Max(1, 7 / sessionsPerWeek);
            return lastSessionTimestamp.AddDays(daysGap);
        }

        public bool IsSessionDueNow()
        {
            return DateTime.UtcNow >= GetNextDueDate();
        }

        public bool IsProgramComplete() => sessionsCompleted >= totalSessionsPlanned;

        public bool IsReMeasurementDue() => reMeasurementDue;

        public void AcknowledgeReMeasurement()
        {
            reMeasurementDue = false;
            SaveState();
        }

        public int GetSessionsCompleted() => sessionsCompleted;

        public int GetSessionsRemaining() => Mathf.Max(0, totalSessionsPlanned - sessionsCompleted);

        /// <summary>
        /// Reset the program state — for starting a new paradigm phase (e.g., after Phase 2 finishes,
        /// switching from Paradigm B to Paradigm A). Saves the change immediately.
        /// </summary>
        public void ResetForNewPhase(Paradigm newParadigm, int newTotalSessions = 30)
        {
            currentParadigm = newParadigm;
            totalSessionsPlanned = newTotalSessions;
            sessionsCompleted = 0;
            lastSessionTimestamp = DateTime.MinValue;
            programStartedAt = DateTime.MinValue;
            reMeasurementDue = false;
            SaveState();
            Debug.Log($"[ProgramScheduler] Reset for new phase: {newParadigm}, {newTotalSessions} sessions.");
        }

        private string GetStatePath() => Path.Combine(Application.persistentDataPath, stateFileName);

        private void SaveState()
        {
            try
            {
                var state = new ProgramState
                {
                    paradigm = currentParadigm.ToString(),
                    totalSessionsPlanned = totalSessionsPlanned,
                    sessionsPerWeek = sessionsPerWeek,
                    reMeasurementSessions = reMeasurementSessions,
                    sessionsCompleted = sessionsCompleted,
                    lastSessionTimestampIso = lastSessionTimestamp == DateTime.MinValue
                        ? "" : lastSessionTimestamp.ToString("O"),
                    programStartedAtIso = programStartedAt == DateTime.MinValue
                        ? "" : programStartedAt.ToString("O"),
                };
                string json = JsonUtility.ToJson(state, true);
                File.WriteAllText(GetStatePath(), json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ProgramScheduler] Failed to save state: {e.Message}");
            }
        }

        private void LoadState()
        {
            string path = GetStatePath();
            if (!File.Exists(path))
            {
                Debug.Log("[ProgramScheduler] No saved state found. Starting fresh.");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                var state = JsonUtility.FromJson<ProgramState>(json);

                if (Enum.TryParse(state.paradigm, out Paradigm parsed))
                    currentParadigm = parsed;

                totalSessionsPlanned = state.totalSessionsPlanned;
                sessionsPerWeek = state.sessionsPerWeek;
                reMeasurementSessions = state.reMeasurementSessions ?? reMeasurementSessions;
                sessionsCompleted = state.sessionsCompleted;

                if (!string.IsNullOrEmpty(state.lastSessionTimestampIso))
                    lastSessionTimestamp = DateTime.Parse(state.lastSessionTimestampIso);
                if (!string.IsNullOrEmpty(state.programStartedAtIso))
                    programStartedAt = DateTime.Parse(state.programStartedAtIso);

                Debug.Log($"[ProgramScheduler] Loaded state: {sessionsCompleted}/{totalSessionsPlanned} " +
                          $"({currentParadigm}), last session {lastSessionTimestamp:yyyy-MM-dd HH:mm}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ProgramScheduler] Failed to load state: {e.Message}. Starting fresh.");
            }
        }

        // Debug UI — top-right corner status
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 310, 10, 300, 120));
            GUILayout.Label("<b>Program Status</b>");
            GUILayout.Label($"Paradigm: {currentParadigm}");
            GUILayout.Label($"Session: {sessionsCompleted} / {totalSessionsPlanned}");
            if (lastSessionTimestamp != DateTime.MinValue)
                GUILayout.Label($"Last: {lastSessionTimestamp:yyyy-MM-dd HH:mm}");
            GUILayout.Label($"Next due: {(IsSessionDueNow() ? "NOW" : GetNextDueDate().ToString("MM-dd HH:mm"))}");
            if (reMeasurementDue)
                GUILayout.Label("<color=yellow>⚠ Re-measurement due</color>");
            GUILayout.EndArea();
        }
    }
}
