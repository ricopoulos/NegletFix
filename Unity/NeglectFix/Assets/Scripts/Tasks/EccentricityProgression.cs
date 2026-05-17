using UnityEngine;
using System.Collections.Generic;

namespace NeglectFix.Tasks
{
    /// <summary>
    /// Computes the eccentricity ladder for audiovisual training, personalized to the
    /// patient's contrast-sensitivity asymmetry.
    ///
    /// For SEVERE deficit (Eric's case: Left 0.00, Right 2.25 LogCS):
    ///   Start NEAR scotoma border (~5°) and expand outward over sessions.
    ///   Rationale: Yang/Cavanaugh/Saionz 2023 (n=12 chronic V1 stroke) found training
    ///   at scotoma-border locations produced the highest response rate; trying to train
    ///   deep in the blind field (e.g., 56°) wastes early sessions on locations the SC
    ///   pathway can't yet recruit.
    ///
    /// For MILD deficit (asymmetry ≤ 1.0 LogCS):
    ///   Start at moderate eccentricity (~15°) and expand to 45° over sessions.
    ///   Rationale: residual function exists across the field; cover broader range.
    ///
    /// Helper class, not a MonoBehaviour. AudioVisualTraining instantiates this with
    /// the baseline ContrastSensitivityResults at session start.
    /// </summary>
    public class EccentricityProgression
    {
        // Severity classification thresholds (in LogCS asymmetry between intact and affected hemifields)
        private const float SEVERE_THRESHOLD = 1.5f;
        private const float MILD_THRESHOLD = 0.5f;

        // Eccentricity ladders (degrees from center)
        private static readonly float[] SEVERE_LADDER = new float[] { 5f, 8f, 12f, 16f, 20f };
        private static readonly float[] MODERATE_LADDER = new float[] { 8f, 16f, 24f, 32f, 40f };
        private static readonly float[] MILD_LADDER = new float[] { 15f, 25f, 35f, 45f, 55f };

        public enum Severity { Severe, Moderate, Mild }

        public Severity severity { get; private set; }
        public float[] ladder { get; private set; }
        public int totalSessionsPlanned { get; private set; }

        // Affected hemifield — derived from baseline
        public enum Hemifield { Left, Right }
        public Hemifield affectedHemifield { get; private set; }

        public EccentricityProgression(NeglectFix.Assessment.ContrastSensitivityResults baseline, int totalSessionsPlanned = 30)
        {
            this.totalSessionsPlanned = totalSessionsPlanned;

            float left = baseline.leftHemifieldLogCS;
            float right = baseline.rightHemifieldLogCS;
            float asymmetry = Mathf.Abs(right - left);

            // Determine affected side (the lower-LogCS hemifield)
            affectedHemifield = (left < right) ? Hemifield.Left : Hemifield.Right;

            // Classify severity
            if (asymmetry >= SEVERE_THRESHOLD)
            {
                severity = Severity.Severe;
                ladder = SEVERE_LADDER;
            }
            else if (asymmetry >= MILD_THRESHOLD)
            {
                severity = Severity.Moderate;
                ladder = MODERATE_LADDER;
            }
            else
            {
                severity = Severity.Mild;
                ladder = MILD_LADDER;
            }

            Debug.Log($"[EccentricityProgression] Asymmetry={asymmetry:F2} LogCS → {severity} deficit. " +
                      $"Affected: {affectedHemifield}. Ladder: [{string.Join(", ", ladder)}]°.");
        }

        /// <summary>
        /// For the given session index (1-based, matching ProgramScheduler.sessionsCompleted+1),
        /// return the eccentricities to train in that session.
        ///
        /// Ladder progression: across the program, gradually advance from the innermost
        /// eccentricity to the outermost. Within a single session, present 2-3 adjacent
        /// eccentricities to maintain variety.
        /// </summary>
        public float[] GetEccentricitiesForSession(int sessionIndex)
        {
            // Map session index to ladder position
            // Early sessions (1-6) stay at innermost; expand outward as session count grows
            float progress = Mathf.Clamp01((float)(sessionIndex - 1) / totalSessionsPlanned);

            // Center of the active band slides from index 0 → ladder.Length - 1
            float centerFloat = progress * (ladder.Length - 1);
            int centerIdx = Mathf.RoundToInt(centerFloat);

            // Return 3 eccentricities: center +/- 1 (clamped to ladder bounds)
            int lo = Mathf.Max(0, centerIdx - 1);
            int hi = Mathf.Min(ladder.Length - 1, centerIdx + 1);

            List<float> result = new List<float>();
            for (int i = lo; i <= hi; i++)
                result.Add(ladder[i]);

            return result.ToArray();
        }

        /// <summary>
        /// World-space horizontal offset (in degrees) that should be applied to put the
        /// stimulus in the affected hemifield. Negative = left, positive = right.
        /// </summary>
        public float ApplyHemifieldDirection(float eccentricityDeg)
        {
            return affectedHemifield == Hemifield.Left ? -eccentricityDeg : eccentricityDeg;
        }
    }
}
