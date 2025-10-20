using UnityEngine;

namespace NeglectFix.EEG
{
    /// <summary>
    /// Simulates EEG data for testing without a Muse headband
    /// Generates realistic alpha/beta wave patterns
    /// </summary>
    public class EEGSimulator : MonoBehaviour
    {
        [Header("Simulation Settings")]
        [SerializeField] private bool enableSimulation = true;
        [SerializeField] private float baseEngagement = 0.5f;
        [SerializeField] private float variationAmount = 0.2f;
        [SerializeField] private float variationSpeed = 0.5f;

        [Header("Simulated Wave Values")]
        [SerializeField] private float simulatedAlpha = 0.5f;
        [SerializeField] private float simulatedBeta = 0.5f;
        [SerializeField] private float simulatedTheta = 0.3f;
        [SerializeField] private float simulatedGamma = 0.4f;

        private float timeOffset;
        private EngagementCalculator engagementCalculator;

        public bool IsSimulating => enableSimulation;
        public float CurrentEngagement { get; private set; }

        void Start()
        {
            engagementCalculator = GetComponent<EngagementCalculator>();
            if (engagementCalculator == null)
            {
                engagementCalculator = gameObject.AddComponent<EngagementCalculator>();
            }

            timeOffset = Random.Range(0f, 100f);
            Debug.Log("EEG Simulator started - Testing mode active");
        }

        void Update()
        {
            if (!enableSimulation) return;

            // Simulate natural variations in brain waves
            float time = Time.time * variationSpeed + timeOffset;

            // Alpha waves (8-12 Hz) - relaxation
            simulatedAlpha = Mathf.Clamp01(
                0.5f + Mathf.Sin(time * 1.2f) * 0.2f +
                Mathf.PerlinNoise(time * 0.3f, 0) * 0.1f
            );

            // Beta waves (12-30 Hz) - active thinking
            simulatedBeta = Mathf.Clamp01(
                0.5f + Mathf.Sin(time * 2.1f) * 0.15f +
                Mathf.PerlinNoise(time * 0.5f, 10) * 0.15f
            );

            // Theta waves (4-8 Hz) - drowsiness
            simulatedTheta = Mathf.Clamp01(
                0.3f + Mathf.Sin(time * 0.8f) * 0.1f +
                Mathf.PerlinNoise(time * 0.2f, 20) * 0.1f
            );

            // Gamma waves (30-100 Hz) - concentration
            simulatedGamma = Mathf.Clamp01(
                0.4f + Mathf.Sin(time * 3.5f) * 0.1f +
                Mathf.PerlinNoise(time * 0.7f, 30) * 0.2f
            );

            // Calculate engagement (beta / alpha ratio)
            CurrentEngagement = CalculateEngagement();

            // Send to EngagementCalculator if it exists
            if (engagementCalculator != null)
            {
                engagementCalculator.UpdateSimulatedValues(
                    simulatedAlpha, simulatedBeta,
                    simulatedTheta, simulatedGamma,
                    CurrentEngagement
                );
            }
        }

        private float CalculateEngagement()
        {
            // Engagement = (Beta + Gamma) / (Alpha + Theta)
            float attention = (simulatedBeta + simulatedGamma) / 2f;
            float relaxation = (simulatedAlpha + simulatedTheta) / 2f;

            if (relaxation > 0.01f)
            {
                float engagement = attention / relaxation;
                return Mathf.Clamp01(engagement);
            }

            return 0.5f;
        }

        public void SimulateHighEngagement()
        {
            simulatedBeta = 0.8f;
            simulatedGamma = 0.7f;
            simulatedAlpha = 0.3f;
            simulatedTheta = 0.2f;
        }

        public void SimulateLowEngagement()
        {
            simulatedBeta = 0.3f;
            simulatedGamma = 0.2f;
            simulatedAlpha = 0.8f;
            simulatedTheta = 0.7f;
        }

        void OnGUI()
        {
            if (!enableSimulation) return;

            // Display simulation status
            GUI.Box(new Rect(10, 10, 200, 150), "EEG Simulator");
            GUI.Label(new Rect(20, 35, 180, 20), $"Alpha: {simulatedAlpha:F2}");
            GUI.Label(new Rect(20, 55, 180, 20), $"Beta: {simulatedBeta:F2}");
            GUI.Label(new Rect(20, 75, 180, 20), $"Theta: {simulatedTheta:F2}");
            GUI.Label(new Rect(20, 95, 180, 20), $"Gamma: {simulatedGamma:F2}");
            GUI.Label(new Rect(20, 115, 180, 20), $"Engagement: {CurrentEngagement:F2}");
            GUI.Label(new Rect(20, 135, 180, 20), "Press E/R for High/Low");
        }

        void OnKeyPress()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SimulateHighEngagement();
                Debug.Log("Simulating HIGH engagement");
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                SimulateLowEngagement();
                Debug.Log("Simulating LOW engagement");
            }
        }
    }
}