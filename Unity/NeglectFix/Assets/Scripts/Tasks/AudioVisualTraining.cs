using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NeglectFix.Tasks
{
    /// <summary>
    /// Paradigm B — Congruent audiovisual pair detection in the affected hemifield.
    /// Implementation of the Wake Forest / Rowland 2023 protocol family, adapted for chronic
    /// adult stroke at the Alharshan/Alwashmi 2026 dose (30 min × 5×/week × 6 weeks).
    ///
    /// Trial structure (each trial ~3-7 sec):
    /// 1. Random inter-stimulus interval (2-5 sec)
    /// 2. Pick eccentricity from EccentricityProgression (personalized to baseline CS asymmetry)
    /// 3. Spawn visual disk at hemifield-correct position, simultaneously play co-localized audio tone
    /// 4. Wait for user "I saw it" input (controller button / key / gaze) within response window
    /// 5. Log trial (RT, hit/miss, eccentricity, contrast)
    /// 6. Adapt contrast via 2-up/1-down weighted staircase (converges on ~70.7% correct)
    ///
    /// Session structure: 3 × 10-min blocks with 30-sec rests between (= 30 min training phase).
    /// Baseline + cooldown phases inherit from TaskManager (baseline 120s, cooldown 60s).
    ///
    /// EEG decoupled (v1): the reward fires on detection, not on EEG engagement.
    /// EngagementCalculator can still log passively for retrospective analysis.
    ///
    /// See [[audiovisual-training-protocol]] in .brain/wiki/ for the protocol spec.
    /// </summary>
    public class AudioVisualTraining : TaskManager
    {
        [Header("AV Training — Stimulus")]
        [Tooltip("Prefab to instantiate as the visual stimulus (typically a Sphere or disk). Will be cloned per trial.")]
        public GameObject visualStimulusPrefab;

        [Tooltip("Audio clip for the auditory component. If null, a procedural tone is generated.")]
        public AudioClip stimulusTone;

        [Tooltip("Visual stimulus on-duration in seconds.")]
        [Range(0.05f, 1.0f)]
        public float stimulusDurationSec = 0.25f;

        [Tooltip("Distance from camera at which stimuli are placed (meters).")]
        [Range(0.5f, 5f)]
        public float stimulusDistanceMeters = 1.5f;

        [Header("AV Training — Trial Timing")]
        [Tooltip("Minimum gap between stimuli (seconds).")]
        public float minInterStimulusIntervalSec = 2.0f;

        [Tooltip("Maximum gap between stimuli (seconds).")]
        public float maxInterStimulusIntervalSec = 5.0f;

        [Tooltip("How long after stimulus onset to wait for a response before counting as miss.")]
        [Range(0.5f, 3.0f)]
        public float responseWindowSec = 1.5f;

        [Header("AV Training — Block Structure (Alharshan dose)")]
        [Tooltip("Number of blocks per session.")]
        public int blocksPerSession = 3;

        [Tooltip("Duration of each training block in seconds. 3 × 600s = 30 min training phase.")]
        public float blockDurationSec = 600f;

        [Tooltip("Rest duration between blocks in seconds.")]
        public float interBlockRestSec = 30f;

        [Header("AV Training — Adaptive Staircase")]
        [Tooltip("2-up/1-down weighted staircase: this many correct in a row to make stimulus harder.")]
        [Range(1, 4)]
        public int correctsToStepDownContrast = 2;

        [Tooltip("LogCS step size when adapting (0.15 = standard Pelli-Robson triplet).")]
        public float staircaseStepLogCS = 0.15f;

        [Header("AV Training — Personalization")]
        [Tooltip("Baseline contrast sensitivity results (drives eccentricity ladder + starting contrast). Auto-loaded if null.")]
        public NeglectFix.Assessment.ContrastSensitivityResults baselineResults;

        [Header("AV Training — Dependencies")]
        public ProgramScheduler programScheduler;
        public NeglectFix.Utils.DataLogger trialLogger;

        // Internal state
        private EccentricityProgression progression;
        private float currentLogCS;             // Current contrast on the staircase
        private int consecutiveCorrect;         // For 2-up/1-down logic
        private int currentSessionIndex;        // 1-based, matches ProgramScheduler
        private bool responseReceived;
        private float trialStimulusOnsetTime;
        private float trialAudioOnsetTime;
        private float trialResponseTime;
        private bool trialHitFlag;
        private int totalTrialsThisSession;
        private int totalHitsThisSession;

        // Trial data record — persisted via DataLogger
        public struct TrainingTrial
        {
            public int sessionIndex;
            public int blockIndex;
            public int trialIndex;
            public float eccentricityDeg;
            public string hemifield;
            public float contrastLogCS;
            public float stimulusOnsetMs;
            public float audioOnsetMs;
            public float responseOnsetMs;
            public float rtMs;
            public bool hit;
            public float avDeltaMs;     // audio onset relative to visual onset (sub-50ms target)
        }

        protected override void Start()
        {
            // Override default phase durations for Alharshan dose
            // Baseline 120s (unchanged), Training 1800s (30 min), Cooldown 60s (shorter than default)
            trainingDuration = blocksPerSession * blockDurationSec + (blocksPerSession - 1) * interBlockRestSec;
            cooldownDuration = 60f;
            taskName = "AudioVisualTraining_ParadigmB";

            // Find personalization dependencies
            if (programScheduler == null)
                programScheduler = FindObjectOfType<ProgramScheduler>();

            if (trialLogger == null)
                trialLogger = dataLogger;

            // Load baseline if not provided
            if (baselineResults == null)
            {
                Debug.LogWarning("[AVTraining] No baselineResults assigned. Eccentricity progression will use defaults; " +
                                 "contrast will start at a generic level. Run contrast sensitivity test first for personalization.");
            }
            else
            {
                progression = new EccentricityProgression(baselineResults, programScheduler?.totalSessionsPlanned ?? 30);
                // Start at affected-hemifield baseline + 0.30 LogCS (easier than threshold to build engagement)
                float baselineAffected = (progression.affectedHemifield == EccentricityProgression.Hemifield.Left)
                    ? baselineResults.leftHemifieldLogCS
                    : baselineResults.rightHemifieldLogCS;
                currentLogCS = Mathf.Max(0f, baselineAffected + 0.30f);
                Debug.Log($"[AVTraining] Starting contrast: {currentLogCS:F2} LogCS (baseline affected = {baselineAffected:F2}, +0.30 offset).");
            }

            // Determine session index
            currentSessionIndex = (programScheduler != null) ? programScheduler.sessionsCompleted + 1 : 1;

            base.Start();
        }

        protected override void OnTrainingPhaseStart()
        {
            base.OnTrainingPhaseStart();

            // Record session start with scheduler
            programScheduler?.RecordSessionStart();

            totalTrialsThisSession = 0;
            totalHitsThisSession = 0;

            // Begin block-by-block trial loop
            StartCoroutine(RunBlocks());
        }

        protected override void OnTrainingPhaseEnd()
        {
            base.OnTrainingPhaseEnd();
            programScheduler?.RecordSessionComplete();

            float hitRate = totalTrialsThisSession > 0 ? (float)totalHitsThisSession / totalTrialsThisSession : 0f;
            Debug.Log($"[AVTraining] Session complete. {totalTrialsThisSession} trials, {totalHitsThisSession} hits " +
                      $"({hitRate:P0}). Final staircase: {currentLogCS:F2} LogCS.");
        }

        private IEnumerator RunBlocks()
        {
            for (int blockIdx = 1; blockIdx <= blocksPerSession; blockIdx++)
            {
                dataLogger?.LogEvent($"block_{blockIdx}_start");
                Debug.Log($"[AVTraining] === BLOCK {blockIdx}/{blocksPerSession} ===");

                float blockEndTime = Time.time + blockDurationSec;
                int trialIndex = 0;
                while (Time.time < blockEndTime && currentPhase == SessionPhase.Training)
                {
                    trialIndex++;
                    yield return RunSingleTrial(blockIdx, trialIndex);
                }

                dataLogger?.LogEvent($"block_{blockIdx}_end");

                if (blockIdx < blocksPerSession)
                {
                    Debug.Log($"[AVTraining] Rest for {interBlockRestSec}s before block {blockIdx + 1}.");
                    dataLogger?.LogEvent("block_rest_start");
                    yield return new WaitForSeconds(interBlockRestSec);
                    dataLogger?.LogEvent("block_rest_end");
                }
            }
        }

        private IEnumerator RunSingleTrial(int blockIdx, int trialIdx)
        {
            // Inter-stimulus interval
            float isi = UnityEngine.Random.Range(minInterStimulusIntervalSec, maxInterStimulusIntervalSec);
            yield return new WaitForSeconds(isi);

            // Eccentricity from progression
            float eccentricityDeg = 8f;  // fallback
            string hemifield = "left";
            if (progression != null)
            {
                float[] eccentricities = progression.GetEccentricitiesForSession(currentSessionIndex);
                eccentricityDeg = eccentricities[UnityEngine.Random.Range(0, eccentricities.Length)];
                hemifield = progression.affectedHemifield.ToString().ToLowerInvariant();
                // Apply directionality (negative for left hemifield, positive for right)
                eccentricityDeg = progression.ApplyHemifieldDirection(eccentricityDeg);
            }

            // Compute world-space position
            Vector3 stimulusPosition = ComputeStimulusPosition(eccentricityDeg, stimulusDistanceMeters);

            // Fire AV pair (visual + audio onset within sub-50ms — Bean/Stein/Rowland 2023 says
            // strict perceptual binding is not required; SC integration window is 100-500ms anyway)
            responseReceived = false;
            trialHitFlag = false;
            trialResponseTime = -1f;

            GameObject stimObj = SpawnVisualStimulus(stimulusPosition);
            trialStimulusOnsetTime = Time.time;
            AudioSource audioSrc = PlayCoLocalizedAudio(stimObj, stimulusPosition);
            trialAudioOnsetTime = Time.time;

            // Active stimulus window: wait for response OR stimulus duration to elapse
            float stimulusEndTime = trialStimulusOnsetTime + stimulusDurationSec;
            float responseDeadline = trialStimulusOnsetTime + responseWindowSec;

            while (Time.time < responseDeadline)
            {
                // Check for response input
                if (!responseReceived && DetectResponse())
                {
                    responseReceived = true;
                    trialResponseTime = Time.time;
                    trialHitFlag = true;
                    break;
                }

                // Disable visual stimulus after stimulusDurationSec, but keep listening for response
                if (Time.time >= stimulusEndTime && stimObj != null && stimObj.activeSelf)
                {
                    stimObj.SetActive(false);
                }

                yield return null;
            }

            // Trial bookkeeping
            totalTrialsThisSession++;
            if (trialHitFlag) totalHitsThisSession++;

            float stimulusOnsetMs = (trialStimulusOnsetTime - sessionStartTime) * 1000f;
            float audioOnsetMs = (trialAudioOnsetTime - sessionStartTime) * 1000f;
            float responseOnsetMs = trialResponseTime > 0 ? (trialResponseTime - sessionStartTime) * 1000f : -1f;
            float rtMs = trialResponseTime > 0 ? (trialResponseTime - trialStimulusOnsetTime) * 1000f : -1f;
            float avDeltaMs = (trialAudioOnsetTime - trialStimulusOnsetTime) * 1000f;

            var trial = new TrainingTrial
            {
                sessionIndex = currentSessionIndex,
                blockIndex = blockIdx,
                trialIndex = trialIdx,
                eccentricityDeg = eccentricityDeg,
                hemifield = hemifield,
                contrastLogCS = currentLogCS,
                stimulusOnsetMs = stimulusOnsetMs,
                audioOnsetMs = audioOnsetMs,
                responseOnsetMs = responseOnsetMs,
                rtMs = rtMs,
                hit = trialHitFlag,
                avDeltaMs = avDeltaMs,
            };
            trialLogger?.LogTrainingTrial(trial);

            // Reward on hit (open-loop — not gated on EEG)
            if (trialHitFlag && rewardController != null)
            {
                rewardController.TriggerReward();
            }

            // Adapt staircase
            UpdateStaircase(trialHitFlag);

            // Cleanup
            if (stimObj != null) Destroy(stimObj);
        }

        private void UpdateStaircase(bool hit)
        {
            if (hit)
            {
                consecutiveCorrect++;
                if (consecutiveCorrect >= correctsToStepDownContrast)
                {
                    // Reduce contrast (make harder) — increase LogCS
                    currentLogCS += staircaseStepLogCS;
                    consecutiveCorrect = 0;
                }
            }
            else
            {
                // 1-down: any miss makes stimulus easier (reduce LogCS)
                currentLogCS = Mathf.Max(0f, currentLogCS - staircaseStepLogCS);
                consecutiveCorrect = 0;
            }
        }

        private Vector3 ComputeStimulusPosition(float eccentricityDeg, float distance)
        {
            // Convert eccentricity (deg) + distance (m) to a world-space horizontal offset.
            // Camera assumed at origin looking down +Z. Stimulus placed at angle in front of camera.
            Camera cam = Camera.main;
            Vector3 origin = cam != null ? cam.transform.position : Vector3.zero;
            Quaternion forward = cam != null ? cam.transform.rotation : Quaternion.identity;

            float radians = eccentricityDeg * Mathf.Deg2Rad;
            // Offset in camera-local space: horizontal X
            Vector3 localOffset = new Vector3(Mathf.Sin(radians) * distance, 0f, Mathf.Cos(radians) * distance);
            return origin + forward * localOffset;
        }

        private GameObject SpawnVisualStimulus(Vector3 position)
        {
            GameObject stim;
            if (visualStimulusPrefab != null)
            {
                stim = Instantiate(visualStimulusPrefab, position, Quaternion.identity);
            }
            else
            {
                // Fallback: bright sphere
                stim = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                stim.transform.position = position;
                stim.transform.localScale = Vector3.one * 0.1f;
                Renderer r = stim.GetComponent<Renderer>();
                // Compute contrast from currentLogCS → grayscale luminance offset from background
                float contrastFraction = LogCSToContrast(currentLogCS);
                float backgroundLuminance = 0.5f;  // gray background assumption
                float letterLuminance = Mathf.Clamp01(backgroundLuminance * (1f - contrastFraction));
                if (r != null) r.material.color = new Color(letterLuminance, letterLuminance, letterLuminance);
            }
            return stim;
        }

        private AudioSource PlayCoLocalizedAudio(GameObject parent, Vector3 position)
        {
            AudioSource src = parent != null ? parent.AddComponent<AudioSource>() : gameObject.AddComponent<AudioSource>();
            src.spatialBlend = 1f;     // fully 3D
            src.transform.position = position;
            src.volume = 0.7f;
            src.playOnAwake = false;

            if (stimulusTone != null)
            {
                src.clip = stimulusTone;
                src.Play();
            }
            else
            {
                // Procedural fallback: short tone using PlayOneShot with a generated clip
                src.clip = GenerateToneClip(frequencyHz: 400f, durationSec: stimulusDurationSec, sampleRate: 44100);
                src.Play();
            }
            return src;
        }

        private static AudioClip GenerateToneClip(float frequencyHz, float durationSec, int sampleRate)
        {
            int sampleCount = Mathf.RoundToInt(durationSec * sampleRate);
            float[] samples = new float[sampleCount];
            float twoPiF = 2f * Mathf.PI * frequencyHz;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                // Hann window to avoid click on onset/offset
                float window = 0.5f * (1f - Mathf.Cos(2f * Mathf.PI * i / (sampleCount - 1)));
                samples[i] = Mathf.Sin(twoPiF * t) * window * 0.6f;
            }
            var clip = AudioClip.Create("AVTrainingTone", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private bool DetectResponse()
        {
            // Quest controller: trigger button on either hand
            // (Wired via Input System binding in production; falls back to keyboard for editor testing)
            if (Input.GetKeyDown(KeyCode.Space)) return true;
            if (Input.GetKeyDown(KeyCode.Return)) return true;

            // Quest controller trigger axis fallback (Unity legacy input)
            if (Input.GetAxis("Submit") > 0.5f) return true;

            return false;
        }

        // Static helper: LogCS to fractional contrast (Weber)
        private static float LogCSToContrast(float logCS)
        {
            return Mathf.Clamp01(1f / Mathf.Pow(10f, logCS));
        }

        // Debug HUD
        protected override void OnGUI()
        {
            base.OnGUI();

            if (currentPhase != SessionPhase.Training) return;

            GUILayout.BeginArea(new Rect(10, Screen.height - 230, 350, 110));
            GUILayout.Label("<b>AV Training (Paradigm B)</b>");
            GUILayout.Label($"Session: {currentSessionIndex} | LogCS: {currentLogCS:F2}");
            GUILayout.Label($"Trials: {totalTrialsThisSession} | Hits: {totalHitsThisSession}" +
                            (totalTrialsThisSession > 0 ? $" ({(float)totalHitsThisSession / totalTrialsThisSession:P0})" : ""));
            if (progression != null)
                GUILayout.Label($"Severity: {progression.severity} | Affected: {progression.affectedHemifield}");
            GUILayout.Label("<i>Press SPACE on stimulus detection.</i>");
            GUILayout.EndArea();
        }
    }
}
