using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NeglectFix.Utils
{
    /// <summary>
    /// Controls visual and audio rewards.
    ///
    /// Two modes:
    /// - OpenLoop (v1 default): rewards are triggered directly by the task (e.g.,
    ///   <see cref="NeglectFix.Tasks.AudioVisualTraining"/> calls <see cref="TriggerReward"/>
    ///   on a hit). EEG engagement is NOT used as a gate.
    /// - EegGated (v2 / future): reward fires when EEG engagement crosses threshold
    ///   AND gaze is in the affected hemifield. Requires real Muse signal validation first;
    ///   per Treves 2025 (JMIR meta-analysis, 16 RCTs, 11/16 used Muse) consumer-NF shows
    ///   no benefit on cognition under sham. Keep this mode off until you have evidence
    ///   the gate is helping on Eric's data.
    ///
    /// Reward types:
    /// - Visual: Brighten, glow, highlight objects
    /// - Audio: Pleasant sounds, spatial audio cues
    /// - Environmental: Lighting changes, particle effects
    /// </summary>
    public class RewardController : MonoBehaviour
    {
        public enum RewardMode
        {
            OpenLoop,    // v1 default: task triggers rewards directly on detection
            EegGated     // v2: rewards gated by EngagementCalculator + GazeDetector
        }

        [Header("Mode")]
        [Tooltip("OpenLoop: task calls TriggerReward() directly on detection (v1 default). EegGated: requires EEG engagement + left-gaze (v2 only).")]
        public RewardMode mode = RewardMode.OpenLoop;

        [Header("Dependencies (only used in EegGated mode)")]
        public NeglectFix.EEG.EngagementCalculator engagementCalculator;
        public GazeDetector gazeDetector;

        [Header("Reward Settings")]
        [Tooltip("Cooldown time between rewards (seconds)")]
        public float cooldownDuration = 1.0f;

        [Tooltip("Duration of visual reward effect (seconds)")]
        public float rewardDuration = 2.0f;

        [Header("Visual Effects")]
        [Tooltip("Brightness multiplier during reward")]
        [Range(1f, 3f)]
        public float glowIntensity = 1.5f;

        [Tooltip("Color tint for reward glow")]
        public Color rewardColor = new Color(1f, 1f, 0.8f); // Warm yellow

        [Header("Audio Settings")]
        [Tooltip("Reward sound clip")]
        public AudioClip rewardSound;

        [Tooltip("Volume of reward sound")]
        [Range(0f, 1f)]
        public float audioVolume = 0.7f;

        [Tooltip("Use spatial audio (sound from left)")]
        public bool useSpatialAudio = true;

        [Header("Status")]
        public bool rewardActive = false;
        public int totalRewardsTriggered = 0;

        // Internal state
        private float lastRewardTime = -999f;
        private AudioSource audioSource;
        private List<GameObject> rewardObjects = new List<GameObject>();

        // Events
        public event Action OnRewardTriggered;
        public event Action OnRewardEnded;

        void Start()
        {
            // Setup audio (used in both modes)
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = audioVolume;
            audioSource.spatialBlend = useSpatialAudio ? 1f : 0f;

            if (mode == RewardMode.EegGated)
            {
                // Find EEG dependencies if not assigned
                if (engagementCalculator == null)
                    engagementCalculator = FindObjectOfType<NeglectFix.EEG.EngagementCalculator>();

                if (gazeDetector == null)
                    gazeDetector = FindObjectOfType<GazeDetector>();

                // Subscribe to engagement events
                if (engagementCalculator != null)
                {
                    engagementCalculator.OnEngagementThresholdExceeded += CheckRewardTrigger;
                    Debug.Log("[RewardController] Initialized in EegGated mode. Waiting for EEG engagement + left-gaze...");
                }
                else
                {
                    Debug.LogWarning("[RewardController] EegGated mode requested but EngagementCalculator not found. Reverting to OpenLoop.");
                    mode = RewardMode.OpenLoop;
                }
            }
            else
            {
                Debug.Log("[RewardController] Initialized in OpenLoop mode. Tasks must call TriggerReward() directly on detection.");
            }
        }

        private void CheckRewardTrigger()
        {
            // Combined trigger: EEG engagement AND left-gaze
            bool eegEngaged = engagementCalculator.IsEngaged();
            bool lookingLeft = gazeDetector != null ? gazeDetector.IsLookingLeft() : true;

            if (eegEngaged && lookingLeft && CanTriggerReward())
            {
                TriggerReward();
            }
        }

        private bool CanTriggerReward()
        {
            return Time.time - lastRewardTime >= cooldownDuration;
        }

        public void TriggerReward()
        {
            if (rewardActive) return;

            lastRewardTime = Time.time;
            totalRewardsTriggered++;
            rewardActive = true;

            // Visual reward
            StartVisualReward();

            // Audio reward
            PlayRewardSound();

            // Emit event
            OnRewardTriggered?.Invoke();

            Debug.Log($"[RewardController] ✨ REWARD #{totalRewardsTriggered} triggered!");

            // Schedule reward end
            StartCoroutine(EndRewardAfterDuration());
        }

        private void StartVisualReward()
        {
            // Apply glow effect to all registered reward objects
            foreach (GameObject obj in rewardObjects)
            {
                if (obj == null) continue;

                // Get or add glow component
                RewardGlow glow = obj.GetComponent<RewardGlow>();
                if (glow == null)
                    glow = obj.AddComponent<RewardGlow>();

                glow.StartGlow(glowIntensity, rewardColor, rewardDuration);
            }

            // Global lighting boost (optional)
            StartCoroutine(BrightenEnvironment());
        }

        private IEnumerator BrightenEnvironment()
        {
            // Find directional light (sun)
            Light directionalLight = FindDirectionalLight();
            if (directionalLight == null) yield break;

            float originalIntensity = directionalLight.intensity;
            float targetIntensity = originalIntensity * 1.3f;
            float elapsed = 0f;

            // Fade in
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                directionalLight.intensity = Mathf.Lerp(originalIntensity, targetIntensity, elapsed / 0.5f);
                yield return null;
            }

            // Hold
            yield return new WaitForSeconds(rewardDuration - 1f);

            // Fade out
            elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                directionalLight.intensity = Mathf.Lerp(targetIntensity, originalIntensity, elapsed / 0.5f);
                yield return null;
            }

            directionalLight.intensity = originalIntensity;
        }

        private Light FindDirectionalLight()
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                    return light;
            }
            return null;
        }

        private void PlayRewardSound()
        {
            if (rewardSound == null) return;

            audioSource.clip = rewardSound;
            audioSource.Play();
        }

        private IEnumerator EndRewardAfterDuration()
        {
            yield return new WaitForSeconds(rewardDuration);

            rewardActive = false;
            OnRewardEnded?.Invoke();
        }

        // Public methods for task scripts

        /// <summary>
        /// Register a GameObject to receive visual rewards (glow effect)
        /// </summary>
        public void RegisterRewardObject(GameObject obj)
        {
            if (!rewardObjects.Contains(obj))
            {
                rewardObjects.Add(obj);
                Debug.Log($"[RewardController] Registered reward object: {obj.name}");
            }
        }

        /// <summary>
        /// Unregister a reward object
        /// </summary>
        public void UnregisterRewardObject(GameObject obj)
        {
            rewardObjects.Remove(obj);
        }

        /// <summary>
        /// Manually trigger reward (for testing)
        /// </summary>
        public void ManualTrigger()
        {
            TriggerReward();
        }

        public int GetTotalRewards() => totalRewardsTriggered;

        void OnDestroy()
        {
            if (mode == RewardMode.EegGated && engagementCalculator != null)
            {
                engagementCalculator.OnEngagementThresholdExceeded -= CheckRewardTrigger;
            }
        }

        // Debug visualization
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 310, 140, 300, 100));
            GUILayout.Label("<b>Reward System</b>");
            GUILayout.Label($"Total Rewards: {totalRewardsTriggered}");
            GUILayout.Label($"Cooldown: {(CanTriggerReward() ? "Ready ✓" : $"{(cooldownDuration - (Time.time - lastRewardTime)):F1}s")}");
            GUILayout.Label($"Registered Objects: {rewardObjects.Count}");
            GUILayout.EndArea();
        }
    }

    /// <summary>
    /// Component that handles glow effect on individual objects
    /// Automatically added by RewardController
    /// </summary>
    public class RewardGlow : MonoBehaviour
    {
        private Renderer objectRenderer;
        private Material glowMaterial;
        private Color originalEmission;
        private bool isGlowing = false;
        private Coroutine glowCoroutine;

        void Awake()
        {
            objectRenderer = GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                // Create instance of material to avoid affecting all objects
                glowMaterial = objectRenderer.material;
            }
        }

        public void StartGlow(float intensity, Color glowColor, float duration)
        {
            if (objectRenderer == null || glowMaterial == null) return;

            if (glowCoroutine != null)
                StopCoroutine(glowCoroutine);

            glowCoroutine = StartCoroutine(GlowEffect(intensity, glowColor, duration));
        }

        private IEnumerator GlowEffect(float intensity, Color glowColor, float duration)
        {
            isGlowing = true;

            // Enable emission
            glowMaterial.EnableKeyword("_EMISSION");

            // Store original
            if (glowMaterial.HasProperty("_EmissionColor"))
                originalEmission = glowMaterial.GetColor("_EmissionColor");

            // Fade in
            float elapsed = 0f;
            Color targetEmission = glowColor * intensity;
            while (elapsed < 0.3f)
            {
                elapsed += Time.deltaTime;
                Color currentEmission = Color.Lerp(originalEmission, targetEmission, elapsed / 0.3f);
                glowMaterial.SetColor("_EmissionColor", currentEmission);
                yield return null;
            }

            // Hold
            yield return new WaitForSeconds(duration - 0.6f);

            // Fade out
            elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.deltaTime;
                Color currentEmission = Color.Lerp(targetEmission, originalEmission, elapsed / 0.3f);
                glowMaterial.SetColor("_EmissionColor", currentEmission);
                yield return null;
            }

            glowMaterial.SetColor("_EmissionColor", originalEmission);
            isGlowing = false;
        }
    }
}
