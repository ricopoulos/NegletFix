using UnityEngine;
using System.Collections.Generic;
using System;

namespace NeglectFix.EEG
{
    /// <summary>
    /// Calculates engagement score from EEG band powers (TP10 right parietal cortex).
    /// Implements adaptive threshold based on baseline measurement.
    ///
    /// Based on research:
    /// - Ros et al. (2017): Alpha reduction = cortical activation
    /// - Beta increase = enhanced alertness
    /// - Beta/theta ratio = composite alertness index
    /// </summary>
    public class EngagementCalculator : MonoBehaviour
    {
        [Header("Dependencies")]
        public MuseOSCReceiver museReceiver;

        [Header("Neurofeedback Parameters")]
        [Tooltip("Weight for beta power (higher = more important)")]
        [Range(0f, 2f)]
        public float betaWeight = 1.0f;

        [Tooltip("Weight for alpha power reduction (higher = penalize more)")]
        [Range(0f, 2f)]
        public float alphaWeight = 1.0f;

        [Tooltip("Penalty factor for high theta (0 = no penalty, 1 = full penalty)")]
        [Range(0f, 1f)]
        public float thetaPenalty = 0.3f;

        [Header("Baseline Settings")]
        [Tooltip("Duration of baseline measurement in seconds")]
        public float baselineDuration = 120f; // 2 minutes

        [Tooltip("Standard deviation multiplier for threshold")]
        public float thresholdStdMultiplier = 0.5f;

        [Header("Adaptive Threshold")]
        [Tooltip("Target success rate (40-60% optimal)")]
        [Range(0.2f, 0.8f)]
        public float targetSuccessRate = 0.5f;

        [Tooltip("How often to check success rate (seconds)")]
        public float adaptationInterval = 120f; // 2 minutes

        [Header("Smoothing")]
        [Tooltip("Moving average window size (samples)")]
        public int smoothingWindow = 10;

        [Header("Status")]
        public bool baselineComplete = false;
        public float currentEngagement = 0f;
        public float engagementThreshold = 0f;
        public bool isEngaged = false;

        // Baseline data
        private List<float> baselineEngagementScores = new List<float>();
        private float baselineMean = 0f;
        private float baselineStd = 0f;
        private float baselineStartTime = 0f;

        // Real-time data
        private Queue<float> alphaHistory = new Queue<float>();
        private Queue<float> betaHistory = new Queue<float>();
        private Queue<float> thetaHistory = new Queue<float>();

        // Adaptive threshold
        private float lastAdaptationTime = 0f;
        private int successCount = 0;
        private int totalSamples = 0;

        // Events
        public event Action<float> OnEngagementCalculated;
        public event Action OnEngagementThresholdExceeded;
        public event Action OnBaselineComplete;

        void Start()
        {
            if (museReceiver == null)
            {
                museReceiver = FindObjectOfType<MuseOSCReceiver>();
            }

            if (museReceiver != null)
            {
                museReceiver.OnTP10AlphaReceived += OnAlphaUpdate;
                museReceiver.OnTP10BetaReceived += OnBetaUpdate;
                museReceiver.OnTP10ThetaReceived += OnThetaUpdate;
            }
            else
            {
                Debug.LogError("[Engagement] MuseOSCReceiver not found!");
            }
        }

        private void OnAlphaUpdate(float alpha)
        {
            alphaHistory.Enqueue(alpha);
            if (alphaHistory.Count > smoothingWindow)
                alphaHistory.Dequeue();
        }

        private void OnBetaUpdate(float beta)
        {
            betaHistory.Enqueue(beta);
            if (betaHistory.Count > smoothingWindow)
                betaHistory.Dequeue();
        }

        private void OnThetaUpdate(float theta)
        {
            thetaHistory.Enqueue(theta);
            if (thetaHistory.Count > smoothingWindow)
                thetaHistory.Dequeue();

            // Calculate engagement after receiving all bands
            CalculateEngagement();
        }

        public void StartBaseline()
        {
            Debug.Log("[Engagement] Starting baseline measurement...");
            baselineStartTime = Time.time;
            baselineEngagementScores.Clear();
            baselineComplete = false;
        }

        private void CalculateEngagement()
        {
            if (alphaHistory.Count == 0 || betaHistory.Count == 0 || thetaHistory.Count == 0)
                return;

            // Get smoothed values
            float alpha = GetAverage(alphaHistory);
            float beta = GetAverage(betaHistory);
            float theta = GetAverage(thetaHistory);

            // Prevent division by zero
            if (alpha < 0.01f) alpha = 0.01f;

            // Calculate engagement score
            // Higher beta = good (alertness)
            // Lower alpha = good (cortical activation)
            // Lower theta = good (not drowsy)
            float betaAlphaRatio = beta / alpha;
            float thetaNormalized = Mathf.Clamp01(theta / 10f); // Normalize theta (typical range 0-10)
            float thetaFactor = 1f - (thetaNormalized * thetaPenalty);

            currentEngagement = (betaAlphaRatio * betaWeight) * thetaFactor;

            // During baseline, collect scores
            if (!baselineComplete && Time.time - baselineStartTime < baselineDuration)
            {
                baselineEngagementScores.Add(currentEngagement);
            }
            else if (!baselineComplete && baselineEngagementScores.Count > 0)
            {
                CompleteBaseline();
            }

            // After baseline, check threshold
            if (baselineComplete)
            {
                CheckEngagementThreshold();
                AdaptThreshold();
            }

            OnEngagementCalculated?.Invoke(currentEngagement);
        }

        private void CompleteBaseline()
        {
            baselineMean = CalculateMean(baselineEngagementScores);
            baselineStd = CalculateStd(baselineEngagementScores, baselineMean);

            // Set initial threshold: mean + (stddev * multiplier)
            // Higher than baseline = requires increased engagement
            engagementThreshold = baselineMean + (baselineStd * thresholdStdMultiplier);

            baselineComplete = true;
            Debug.Log($"[Engagement] Baseline complete! Mean: {baselineMean:F2}, Std: {baselineStd:F2}, Threshold: {engagementThreshold:F2}");

            OnBaselineComplete?.Invoke();
        }

        private void CheckEngagementThreshold()
        {
            bool wasEngaged = isEngaged;
            isEngaged = currentEngagement > engagementThreshold;

            // Track for adaptive threshold
            totalSamples++;
            if (isEngaged) successCount++;

            // Trigger event on rising edge (engagement started)
            if (isEngaged && !wasEngaged)
            {
                OnEngagementThresholdExceeded?.Invoke();
            }
        }

        private void AdaptThreshold()
        {
            if (Time.time - lastAdaptationTime < adaptationInterval)
                return;

            if (totalSamples == 0) return;

            float currentSuccessRate = (float)successCount / totalSamples;

            // Adjust threshold to maintain target success rate
            if (currentSuccessRate < targetSuccessRate - 0.1f)
            {
                // Too difficult - lower threshold by 10%
                engagementThreshold *= 0.9f;
                Debug.Log($"[Engagement] Lowering threshold to {engagementThreshold:F2} (success rate: {currentSuccessRate:P0})");
            }
            else if (currentSuccessRate > targetSuccessRate + 0.1f)
            {
                // Too easy - raise threshold by 10%
                engagementThreshold *= 1.1f;
                Debug.Log($"[Engagement] Raising threshold to {engagementThreshold:F2} (success rate: {currentSuccessRate:P0})");
            }

            // Reset counters
            successCount = 0;
            totalSamples = 0;
            lastAdaptationTime = Time.time;
        }

        // Utility functions
        private float GetAverage(Queue<float> queue)
        {
            float sum = 0f;
            foreach (float val in queue)
                sum += val;
            return sum / queue.Count;
        }

        private float CalculateMean(List<float> values)
        {
            float sum = 0f;
            foreach (float val in values)
                sum += val;
            return sum / values.Count;
        }

        private float CalculateStd(List<float> values, float mean)
        {
            float sumSquaredDiff = 0f;
            foreach (float val in values)
            {
                float diff = val - mean;
                sumSquaredDiff += diff * diff;
            }
            return Mathf.Sqrt(sumSquaredDiff / values.Count);
        }

        // Public accessors
        public bool IsEngaged() => isEngaged && baselineComplete;
        public float GetEngagementScore() => currentEngagement;
        public float GetThreshold() => engagementThreshold;
        public float GetSuccessRate() => totalSamples > 0 ? (float)successCount / totalSamples : 0f;

        // Simulator support
        public void UpdateSimulatedValues(float alpha, float beta, float theta, float gamma, float engagement)
        {
            // Update histories
            alphaHistory.Clear();
            betaHistory.Clear();
            thetaHistory.Clear();

            alphaHistory.Enqueue(alpha);
            betaHistory.Enqueue(beta);
            thetaHistory.Enqueue(theta);

            // Use provided engagement or calculate
            if (engagement > 0)
            {
                currentEngagement = engagement;
            }
            else
            {
                CalculateEngagement();
            }
        }

        void OnDestroy()
        {
            if (museReceiver != null)
            {
                museReceiver.OnTP10AlphaReceived -= OnAlphaUpdate;
                museReceiver.OnTP10BetaReceived -= OnBetaUpdate;
                museReceiver.OnTP10ThetaReceived -= OnThetaUpdate;
            }
        }

        // Debug visualization
        void OnGUI()
        {
            if (!baselineComplete) return;

            GUILayout.BeginArea(new Rect(10, 220, 300, 150));
            GUILayout.Label("<b>Engagement Score</b>");
            GUILayout.Label($"Current: {currentEngagement:F2}");
            GUILayout.Label($"Threshold: {engagementThreshold:F2}");
            GUILayout.Label($"Status: {(isEngaged ? "<color=green>ENGAGED ✓</color>" : "<color=red>Not engaged</color>")}");
            GUILayout.Label($"Success Rate: {GetSuccessRate():P0}");
            GUILayout.EndArea();
        }
    }
}
