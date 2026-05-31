using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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

        public enum StimulusPattern
        {
            SolidDisk,
            GaborPatch
        }

        [Tooltip("Generated fallback target when no prefab is assigned.")]
        public StimulusPattern generatedStimulusPattern = StimulusPattern.SolidDisk;

        [Tooltip("Generated fallback target diameter in degrees of visual angle.")]
        [Range(0.5f, 10f)]
        public float stimulusAngularSizeDeg = 3f;

        [Tooltip("Linear gray background luminance used by generated fallback stimuli.")]
        [Range(0f, 1f)]
        public float stimulusBackgroundLuminance = 0.5f;

        [Tooltip("Texture resolution for generated disk/Gabor fallback stimuli.")]
        [Range(32, 512)]
        public int generatedStimulusTextureSize = 128;

        [Tooltip("Cycles across the generated Gabor patch diameter.")]
        [Range(1f, 12f)]
        public float gaborCycles = 4f;

        [Tooltip("Soft edge width as a fraction of generated stimulus radius.")]
        [Range(0f, 0.5f)]
        public float stimulusSoftEdgeFraction = 0.12f;

        [Tooltip("Minimum generated stimulus Weber contrast. Keep at 0 for clinical runs; use >0 for visibility validation builds.")]
        [Range(0f, 1f)]
        public float minimumGeneratedStimulusContrast = 0f;

        [Header("AV Training — Controlled Background")]
        [Tooltip("Render an opaque gray field behind all prompts and stimuli so Quest passthrough/home video cannot reduce contrast.")]
        public bool ensureOpaqueTrainingBackdrop = true;

        [Tooltip("Distance from the headset/camera where the opaque visual-field backdrop appears.")]
        [Range(1.6f, 5f)]
        public float opaqueBackdropDistanceMeters = 2.4f;

        [Tooltip("Backdrop width in meters. Keep large enough to cover the left/right eccentricity ladder.")]
        [Range(2f, 8f)]
        public float opaqueBackdropWidthMeters = 5.2f;

        [Tooltip("Backdrop height in meters.")]
        [Range(1.5f, 5f)]
        public float opaqueBackdropHeightMeters = 3.4f;

        [Tooltip("Linear gray luminance of the controlled backdrop.")]
        [Range(0f, 1f)]
        public float opaqueBackdropLuminance = 0.5f;

        [Header("AV Training — Fixation Guidance")]
        [Tooltip("Render a small central cross so the patient has a stable starting point before each marker appears.")]
        public bool showCenterFixationCross = true;

        [Tooltip("Distance from the headset/camera where the fixation cross appears.")]
        [Range(0.75f, 3f)]
        public float fixationCrossDistanceMeters = 1.15f;

        [Tooltip("Total cross length in meters. Keep small so it anchors gaze without becoming the task.")]
        [Range(0.01f, 0.2f)]
        public float fixationCrossSizeMeters = 0.04f;

        [Tooltip("Cross stroke thickness in meters.")]
        [Range(0.002f, 0.03f)]
        public float fixationCrossThicknessMeters = 0.004f;

        [Tooltip("Linear luminance of the fixation cross.")]
        [Range(0f, 1f)]
        public float fixationCrossLuminance = 0.92f;

        [Header("AV Training — Trial Timing")]
        [Tooltip("Minimum gap between stimuli (seconds).")]
        public float minInterStimulusIntervalSec = 2.0f;

        [Tooltip("Maximum gap between stimuli (seconds).")]
        public float maxInterStimulusIntervalSec = 5.0f;

        [Tooltip("How long after stimulus onset to wait for a response before counting as miss.")]
        [Range(0.5f, 3.0f)]
        public float responseWindowSec = 1.5f;

        [Header("AV Training — Sparse Control Trials")]
        [Tooltip("Insert sparse intact-hemifield controls during recorded blocks. For Eric's baseline this means occasional right-side controls.")]
        public bool enableIntactHemifieldControlTrials = true;

        [Tooltip("Chance that an eligible recorded trial becomes an intact-hemifield control.")]
        [Range(0f, 0.3f)]
        public float intactControlTrialProbability = 0.15f;

        [Tooltip("Minimum rehab-dose trials required between intact-hemifield controls.")]
        [Range(0, 20)]
        public int minimumRehabTrialsBetweenControlTrials = 3;

        [Header("AV Training — Block Structure (Alharshan dose)")]
        [Tooltip("Number of blocks per session.")]
        public int blocksPerSession = 3;

        [Tooltip("Duration of each training block in seconds. 3 × 600s = 30 min training phase.")]
        public float blockDurationSec = 600f;

        [Tooltip("Rest duration between blocks in seconds.")]
        public float interBlockRestSec = 30f;

        [Tooltip("Cooldown phase duration after the training blocks complete.")]
        public float cooldownDurationSec = 60f;

        [Header("AV Training — Practice")]
        [Tooltip("Run a short unlogged practice block before recorded training trials.")]
        public bool enablePracticeBlock = false;

        [Tooltip("How long to show the practice instructions after calibration and before practice trials.")]
        [Range(0f, 20f)]
        public float practiceIntroDurationSec = 4f;

        [Tooltip("Practice duration in seconds. Practice trials do not write to the training-trial CSV or staircase.")]
        [Range(0f, 120f)]
        public float practiceDurationSec = 20f;

        [Tooltip("Trigger normal reward feedback during practice hits, while keeping practice out of the recorded pilot results.")]
        public bool rewardPracticeHits = true;

        [TextArea(2, 5)]
        public string practicePromptInstructions = "Practice first.\nStart on the center cross. When a marker appears, move only your eyes to it and press.\nKeep your head still. These practice trials are not counted.";

        [Header("AV Training — Adaptive Staircase")]
        [Tooltip("2-up/1-down weighted staircase: this many correct in a row to make stimulus harder.")]
        [Range(1, 4)]
        public int correctsToStepDownContrast = 2;

        [Tooltip("LogCS step size when adapting (0.15 = standard Pelli-Robson triplet).")]
        public float staircaseStepLogCS = 0.15f;

        [Header("AV Training — Personalization")]
        [Tooltip("Baseline contrast sensitivity results (drives eccentricity ladder + starting contrast). Auto-loaded if null.")]
        public NeglectFix.Assessment.ContrastSensitivityResults baselineResults;

        [Header("AV Training — Input")]
        [Tooltip("Input System action for 'I saw it'. If empty, Quest trigger + Space/Enter bindings are created at runtime.")]
        public InputActionProperty responseAction;

        [Tooltip("Keep legacy keyboard/Submit fallback enabled for quick Editor testing.")]
        public bool enableLegacyKeyboardFallback = true;

        [Header("AV Training — Ready Prompt")]
        [Tooltip("Enable the pre-baseline ready prompt and countdown for AV training sessions.")]
        public bool enableAvReadyPrompt = true;

        [Tooltip("Render a world-space readiness prompt in front of the headset.")]
        public bool showReadyPromptInHeadset = true;

        [Tooltip("Distance from the headset/camera where the ready prompt appears.")]
        [Range(0.75f, 3f)]
        public float readyPromptDistanceMeters = 1.25f;

        [TextArea(2, 4)]
        public string readyPromptInstructions = "This is rehab training, not a contrast test.\nStart on the center cross. When a marker appears, move your eyes to it and press.\nKeep your head still.";

        [TextArea(1, 3)]
        public string readyCountdownInstructions = "Find the center cross.\nKeep your head still. Move only your eyes after each marker appears.";

        [TextArea(2, 4)]
        public string baselinePromptInstructions = "Calibrating before training starts.\nKeep the headset still and look at the center cross.";

        [Header("AV Training — Completion Prompt")]
        [Tooltip("Render a clear end-of-run message in the headset after cooldown completes.")]
        public bool showCompletionPromptInHeadset = true;

        [TextArea(2, 4)]
        public string completionPromptInstructions = "You can remove the headset now.\nThank you.";

        [Header("AV Training — Dependencies")]
        public ProgramScheduler programScheduler;
        public NeglectFix.Utils.DataLogger trialLogger;

        // Internal state
        private InputAction defaultResponseAction;
        private bool responseActionEnabledByTask;
        private readonly List<UnityEngine.XR.InputDevice> xrControllerDevices = new List<UnityEngine.XR.InputDevice>();
        private readonly List<UnityEngine.XR.InputFeatureUsage> xrFeatureUsages = new List<UnityEngine.XR.InputFeatureUsage>();
        private bool leftXRResponsePressed;
        private bool rightXRResponsePressed;
        private bool xrControllerDevicesLogged;
        private Material generatedStimulusMaterial;
        private Texture2D generatedStimulusTexture;
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
        private int totalRehabTrialsThisSession;
        private int totalRehabHitsThisSession;
        private int totalControlTrialsThisSession;
        private int totalControlHitsThisSession;
        private int rehabTrialsSinceLastControl;
        private bool trainingLoopActive;
        private GameObject readyPromptRoot;
        private TextMeshProUGUI readyPromptText;
        private GameObject visualFieldBackdrop;
        private Material visualFieldBackdropMaterial;
        private GameObject fixationCrossRoot;
        private Material fixationCrossMaterial;

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
            public string trialType;    // rehab, right_control, left_control
            public bool isControlTrial;
            public bool countsForRehabDose;
        }

        private void OnEnable()
        {
            EnableResponseAction();
        }

        private void OnDisable()
        {
            DisableResponseAction();
        }

        private void OnDestroy()
        {
            if (defaultResponseAction != null)
            {
                defaultResponseAction.Dispose();
                defaultResponseAction = null;
            }

            if (generatedStimulusMaterial != null)
            {
                Destroy(generatedStimulusMaterial);
                generatedStimulusMaterial = null;
            }

            if (generatedStimulusTexture != null)
            {
                Destroy(generatedStimulusTexture);
                generatedStimulusTexture = null;
            }

            if (visualFieldBackdrop != null)
            {
                Destroy(visualFieldBackdrop);
                visualFieldBackdrop = null;
            }

            if (visualFieldBackdropMaterial != null)
            {
                Destroy(visualFieldBackdropMaterial);
                visualFieldBackdropMaterial = null;
            }

            DestroyFixationCrossDisplay();
            DestroyReadyPromptDisplay();
        }

        protected override void Start()
        {
            if (enableAvReadyPrompt)
            {
                readyPromptEnabled = true;
                requireReadyConfirmation = true;
            }

            // Override default phase durations from the configured block/practice structure.
            float recordedTrainingDuration = blocksPerSession * blockDurationSec + (blocksPerSession - 1) * interBlockRestSec;
            float practiceTotalDuration = enablePracticeBlock
                ? Mathf.Max(0f, practiceIntroDurationSec) + Mathf.Max(0f, practiceDurationSec)
                : 0f;
            trainingDuration = recordedTrainingDuration + practiceTotalDuration;
            cooldownDuration = cooldownDurationSec;
            taskName = "AudioVisualTraining_ParadigmB";
            EnableResponseAction();
            LogXRControllerDevices();
            EnsureOpaqueTrainingBackdrop();
            EnsureFixationCrossDisplay();
            SetFixationCrossVisible(false);

            // Find personalization dependencies
            if (programScheduler == null)
                programScheduler = FindObjectOfType<ProgramScheduler>();

            if (dataLogger == null)
                dataLogger = FindObjectOfType<NeglectFix.Utils.DataLogger>();

            if (trialLogger == null)
                trialLogger = dataLogger;

            // Load baseline if provided and non-empty. A serialized all-zero object should not
            // be treated as a valid clinical baseline because it flips tie cases to the right side.
            if (!HasUsableBaselineResults(baselineResults))
            {
                Debug.LogWarning("[AVTraining] No baselineResults assigned. Eccentricity progression will use defaults; " +
                                 "contrast will start at a generic level. Run contrast sensitivity test first for personalization.");
                baselineResults = null;
            }
            else
            {
                progression = new EccentricityProgression(baselineResults, programScheduler?.totalSessionsPlanned ?? 30);
                // Start below the affected-hemifield threshold so early sessions are visible enough
                // to build engagement. Lower LogCS means higher contrast.
                float baselineAffected = (progression.affectedHemifield == EccentricityProgression.Hemifield.Left)
                    ? baselineResults.leftHemifieldLogCS
                    : baselineResults.rightHemifieldLogCS;
                currentLogCS = Mathf.Max(0f, baselineAffected - 0.30f);
                Debug.Log($"[AVTraining] Starting contrast: {currentLogCS:F2} LogCS (baseline affected = {baselineAffected:F2}, -0.30 offset).");
            }

            // Determine session index
            int plannedSessions = Mathf.Max(1, programScheduler?.totalSessionsPlanned ?? 30);
            int completedSessions = Mathf.Max(0, programScheduler?.sessionsCompleted ?? 0);
            currentSessionIndex = Mathf.Clamp(completedSessions + 1, 1, plannedSessions);

            base.Start();
        }

        protected override void OnReadyPhaseStart()
        {
            SetFixationCrossVisible(false);

            if (!showReadyPromptInHeadset)
                return;

            EnsureReadyPromptDisplay();
            UpdateReadyPromptDisplay(readyCountdownDuration, waitingForConfirmation: true);
        }

        protected override void OnReadyPhaseUpdate(float countdownRemainingSec, bool waitingForConfirmation)
        {
            if (!showReadyPromptInHeadset)
                return;

            EnsureReadyPromptDisplay();
            UpdateReadyPromptDisplay(countdownRemainingSec, waitingForConfirmation);
        }

        protected override void OnReadyPhaseEnd()
        {
            DestroyReadyPromptDisplay();
        }

        protected override bool IsReadyConfirmationPressed()
        {
            return DetectResponse() || base.IsReadyConfirmationPressed();
        }

        protected override void OnTaskCompleted()
        {
            SetFixationCrossVisible(false);
            base.OnTaskCompleted();

            if (showCompletionPromptInHeadset)
                ShowCompletionPromptDisplay();
        }

        protected override void OnBaselinePhaseStart()
        {
            SetFixationCrossVisible(true);

            if (!showReadyPromptInHeadset)
                return;

            EnsureReadyPromptDisplay();
            StartCoroutine(UpdateBaselinePrompt());
        }

        protected override void OnBaselinePhaseEnd()
        {
            DestroyReadyPromptDisplay();
        }

        protected override void OnTrainingPhaseStart()
        {
            DestroyReadyPromptDisplay();
            SetFixationCrossVisible(true);

            base.OnTrainingPhaseStart();

            // Record session start with scheduler
            programScheduler?.RecordSessionStart();

            totalTrialsThisSession = 0;
            totalHitsThisSession = 0;
            totalRehabTrialsThisSession = 0;
            totalRehabHitsThisSession = 0;
            totalControlTrialsThisSession = 0;
            totalControlHitsThisSession = 0;
            rehabTrialsSinceLastControl = 0;
            trainingLoopActive = true;

            // Begin optional practice, then the recorded block-by-block trial loop.
            StartCoroutine(RunPracticeThenBlocks());
        }

        protected override void OnTrainingPhaseEnd()
        {
            trainingLoopActive = false;
            SetFixationCrossVisible(false);
            base.OnTrainingPhaseEnd();
            trialLogger?.CloseTrainingTrialLog();
            programScheduler?.RecordSessionComplete();

            float rehabHitRate = totalRehabTrialsThisSession > 0
                ? (float)totalRehabHitsThisSession / totalRehabTrialsThisSession
                : 0f;
            float controlHitRate = totalControlTrialsThisSession > 0
                ? (float)totalControlHitsThisSession / totalControlTrialsThisSession
                : 0f;

            Debug.Log($"[AVTraining] Session complete. {totalTrialsThisSession} recorded trials " +
                      $"({totalRehabTrialsThisSession} rehab, {totalControlTrialsThisSession} controls). " +
                      $"Rehab: {totalRehabHitsThisSession}/{totalRehabTrialsThisSession} ({rehabHitRate:P0}). " +
                      $"Controls: {totalControlHitsThisSession}/{totalControlTrialsThisSession} ({controlHitRate:P0}). " +
                      $"Final staircase: {currentLogCS:F2} LogCS.");
        }

        private IEnumerator RunPracticeThenBlocks()
        {
            if (enablePracticeBlock && practiceDurationSec > 0f)
            {
                yield return RunPracticeBlock();

                if (!IsTrainingLoopActive())
                    yield break;
            }

            yield return RunBlocks();
        }

        private IEnumerator RunPracticeBlock()
        {
            dataLogger?.LogEvent("practice_start");
            Debug.Log("[AVTraining] === PRACTICE BLOCK (unlogged trials) ===");

            if (showReadyPromptInHeadset && practiceIntroDurationSec > 0f)
            {
                EnsureReadyPromptDisplay();
                UpdatePracticePromptDisplay();
                yield return WaitForTrainingSeconds(practiceIntroDurationSec);
                DestroyReadyPromptDisplay();
            }

            if (!IsTrainingLoopActive())
                yield break;

            float practiceEndTime = Time.time + practiceDurationSec;
            int practiceTrialIndex = 0;
            while (Time.time < practiceEndTime && IsTrainingLoopActive())
            {
                practiceTrialIndex++;
                yield return RunSingleTrial(0, practiceTrialIndex, recordTrial: false);
            }

            dataLogger?.LogEvent("practice_end");
        }

        private IEnumerator RunBlocks()
        {
            for (int blockIdx = 1; blockIdx <= blocksPerSession; blockIdx++)
            {
                dataLogger?.LogEvent($"block_{blockIdx}_start");
                Debug.Log($"[AVTraining] === BLOCK {blockIdx}/{blocksPerSession} ===");

                float blockEndTime = Time.time + blockDurationSec;
                int trialIndex = 0;
                while (Time.time < blockEndTime && IsTrainingLoopActive())
                {
                    trialIndex++;
                    yield return RunSingleTrial(blockIdx, trialIndex);
                }

                if (!IsTrainingLoopActive())
                    yield break;

                dataLogger?.LogEvent($"block_{blockIdx}_end");

                if (blockIdx < blocksPerSession)
                {
                    Debug.Log($"[AVTraining] Rest for {interBlockRestSec}s before block {blockIdx + 1}.");
                    dataLogger?.LogEvent("block_rest_start");
                    yield return WaitForTrainingSeconds(interBlockRestSec);
                    if (!IsTrainingLoopActive())
                        yield break;

                    dataLogger?.LogEvent("block_rest_end");
                }
            }
        }

        private IEnumerator RunSingleTrial(int blockIdx, int trialIdx, bool recordTrial = true)
        {
            // Inter-stimulus interval
            float isi = UnityEngine.Random.Range(minInterStimulusIntervalSec, maxInterStimulusIntervalSec);
            yield return WaitForTrainingSeconds(isi);

            if (!IsTrainingLoopActive())
                yield break;

            bool isControlTrial = recordTrial && ShouldRunIntactHemifieldControlTrial();
            bool countsForRehabDose = recordTrial && !isControlTrial;
            string hemifield = isControlTrial ? GetIntactHemifieldName() : GetAffectedHemifieldName();
            string trialType = isControlTrial ? $"{hemifield}_control" : "rehab";

            float unsignedEccentricityDeg = PickEccentricityForCurrentSession();
            float eccentricityDeg = ApplyHemifieldDirection(unsignedEccentricityDeg, hemifield);

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

            while (Time.time < responseDeadline && IsTrainingLoopActive())
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

            if (!IsTrainingLoopActive())
            {
                if (stimObj != null) Destroy(stimObj);
                yield break;
            }

            // Trial bookkeeping. Practice trials are deliberately excluded from the pilot CSV
            // and staircase so the recorded block remains interpretable.
            if (recordTrial)
            {
                totalTrialsThisSession++;
                if (trialHitFlag) totalHitsThisSession++;

                if (isControlTrial)
                {
                    totalControlTrialsThisSession++;
                    if (trialHitFlag) totalControlHitsThisSession++;
                    rehabTrialsSinceLastControl = 0;
                }
                else
                {
                    totalRehabTrialsThisSession++;
                    if (trialHitFlag) totalRehabHitsThisSession++;
                    rehabTrialsSinceLastControl++;
                }
            }

            float stimulusOnsetMs = (trialStimulusOnsetTime - sessionStartTime) * 1000f;
            float audioOnsetMs = (trialAudioOnsetTime - sessionStartTime) * 1000f;
            float responseOnsetMs = trialResponseTime > 0 ? (trialResponseTime - sessionStartTime) * 1000f : -1f;
            float rtMs = trialResponseTime > 0 ? (trialResponseTime - trialStimulusOnsetTime) * 1000f : -1f;
            float avDeltaMs = (trialAudioOnsetTime - trialStimulusOnsetTime) * 1000f;

            if (recordTrial)
            {
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
                    trialType = trialType,
                    isControlTrial = isControlTrial,
                    countsForRehabDose = countsForRehabDose,
                };
                trialLogger?.LogTrainingTrial(trial);
            }
            else
            {
                Debug.Log($"[AVTraining] Practice trial {trialIdx}: {(trialHitFlag ? "hit" : "miss")} " +
                          $"at {eccentricityDeg:F1} deg {hemifield}, RT={rtMs:F0}ms.");
            }

            // Reward on hit (open-loop — not gated on EEG)
            if (trialHitFlag && rewardController != null && (recordTrial || rewardPracticeHits))
            {
                rewardController.TriggerReward();
            }

            // Adapt staircase only from affected-hemifield rehab-dose trials.
            if (countsForRehabDose)
                UpdateStaircase(trialHitFlag);

            // Cleanup
            if (stimObj != null) Destroy(stimObj);
        }

        private bool ShouldRunIntactHemifieldControlTrial()
        {
            if (!enableIntactHemifieldControlTrials)
                return false;

            if (intactControlTrialProbability <= 0f)
                return false;

            // The first recorded trial should always be a rehab-dose trial, so the run starts
            // anchored to the affected hemifield before any control checks appear.
            if (totalRehabTrialsThisSession == 0)
                return false;

            if (rehabTrialsSinceLastControl < Mathf.Max(0, minimumRehabTrialsBetweenControlTrials))
                return false;

            return UnityEngine.Random.value < Mathf.Clamp01(intactControlTrialProbability);
        }

        private float PickEccentricityForCurrentSession()
        {
            if (progression == null)
                return 8f;

            float[] eccentricities = progression.GetEccentricitiesForSession(currentSessionIndex);
            return eccentricities[UnityEngine.Random.Range(0, eccentricities.Length)];
        }

        private string GetAffectedHemifieldName()
        {
            return progression != null
                ? progression.affectedHemifield.ToString().ToLowerInvariant()
                : "left";
        }

        private string GetIntactHemifieldName()
        {
            return GetOppositeHemifieldName(GetAffectedHemifieldName());
        }

        private static string GetOppositeHemifieldName(string hemifield)
        {
            return string.Equals(hemifield, "right", StringComparison.OrdinalIgnoreCase)
                ? "left"
                : "right";
        }

        private static float ApplyHemifieldDirection(float eccentricityDeg, string hemifield)
        {
            float unsignedEccentricity = Mathf.Abs(eccentricityDeg);
            return string.Equals(hemifield, "left", StringComparison.OrdinalIgnoreCase)
                ? -unsignedEccentricity
                : unsignedEccentricity;
        }

        private bool IsTrainingLoopActive()
        {
            return trainingLoopActive && currentPhase == SessionPhase.Training;
        }

        private IEnumerator WaitForTrainingSeconds(float durationSec)
        {
            float endTime = Time.time + durationSec;
            while (Time.time < endTime && IsTrainingLoopActive())
                yield return null;
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
            if (visualStimulusPrefab != null)
            {
                GameObject prefabStimulus = Instantiate(visualStimulusPrefab, position, Quaternion.identity);
                FaceStimulusToCamera(prefabStimulus);
                return prefabStimulus;
            }

            GameObject stim = GameObject.CreatePrimitive(PrimitiveType.Quad);
            stim.name = $"AVTrainingStimulus_{generatedStimulusPattern}";
            stim.transform.position = position;
            FaceStimulusToCamera(stim);
            ScaleStimulusToAngularSize(stim, position);

            Collider collider = stim.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            float contrastFraction = Mathf.Max(LogCSToContrast(currentLogCS), minimumGeneratedStimulusContrast);
            UpdateGeneratedStimulusTexture(contrastFraction);

            Renderer renderer = stim.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = GetGeneratedStimulusMaterial();

            return stim;
        }

        private static bool HasUsableBaselineResults(NeglectFix.Assessment.ContrastSensitivityResults results)
        {
            if (results == null)
                return false;

            bool hasHemifields = results.leftHemifieldLogCS >= 0f && results.rightHemifieldLogCS >= 0f;
            bool hasSignal = Mathf.Abs(results.leftHemifieldLogCS) > 0.01f ||
                             Mathf.Abs(results.rightHemifieldLogCS) > 0.01f ||
                             Mathf.Abs(results.centralLogCS) > 0.01f ||
                             Mathf.Abs(results.asymmetry) > 0.01f;

            return hasHemifields && hasSignal;
        }

        private void FaceStimulusToCamera(GameObject stim)
        {
            Camera cam = Camera.main;
            if (cam == null || stim == null) return;

            stim.transform.rotation = cam.transform.rotation;
        }

        private void ScaleStimulusToAngularSize(GameObject stim, Vector3 position)
        {
            Camera cam = Camera.main;
            float distance = cam != null ? Vector3.Distance(cam.transform.position, position) : stimulusDistanceMeters;
            float diameterMeters = 2f * distance * Mathf.Tan(stimulusAngularSizeDeg * Mathf.Deg2Rad * 0.5f);
            stim.transform.localScale = new Vector3(diameterMeters, diameterMeters, 1f);
        }

        private void EnsureOpaqueTrainingBackdrop()
        {
            if (!ensureOpaqueTrainingBackdrop || visualFieldBackdrop != null)
                return;

            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("[AVTraining] Cannot create opaque backdrop because no Main Camera was found.");
                return;
            }

            visualFieldBackdrop = GameObject.CreatePrimitive(PrimitiveType.Quad);
            visualFieldBackdrop.name = "AVTrainingControlledBackdrop";
            visualFieldBackdrop.transform.SetParent(cam.transform, false);
            visualFieldBackdrop.transform.localPosition = new Vector3(
                0f,
                0f,
                Mathf.Max(opaqueBackdropDistanceMeters, stimulusDistanceMeters + 0.35f));
            visualFieldBackdrop.transform.localRotation = Quaternion.identity;
            visualFieldBackdrop.transform.localScale = new Vector3(
                opaqueBackdropWidthMeters,
                opaqueBackdropHeightMeters,
                1f);

            Collider collider = visualFieldBackdrop.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            float luminance = Mathf.Clamp01(opaqueBackdropLuminance);
            Color backdropColor = new Color(luminance, luminance, luminance, 1f);

            visualFieldBackdropMaterial = new Material(shader)
            {
                name = "AVTrainingControlledBackdropMaterial",
                color = backdropColor,
                renderQueue = 2000
            };

            if (visualFieldBackdropMaterial.HasProperty("_BaseColor"))
                visualFieldBackdropMaterial.SetColor("_BaseColor", backdropColor);
            if (visualFieldBackdropMaterial.HasProperty("_Color"))
                visualFieldBackdropMaterial.SetColor("_Color", backdropColor);
            if (visualFieldBackdropMaterial.HasProperty("_Cull"))
                visualFieldBackdropMaterial.SetFloat("_Cull", 0f);

            Renderer renderer = visualFieldBackdrop.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = visualFieldBackdropMaterial;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }

        private void EnsureFixationCrossDisplay()
        {
            if (!showCenterFixationCross || fixationCrossRoot != null)
                return;

            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("[AVTraining] Cannot create fixation cross because no Main Camera was found.");
                return;
            }

            fixationCrossRoot = new GameObject("AVTrainingCenterFixationCross");
            fixationCrossRoot.transform.SetParent(cam.transform, false);
            fixationCrossRoot.transform.localPosition = new Vector3(0f, 0f, fixationCrossDistanceMeters);
            fixationCrossRoot.transform.localRotation = Quaternion.identity;
            fixationCrossRoot.transform.localScale = Vector3.one;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            float luminance = Mathf.Clamp01(fixationCrossLuminance);
            Color crossColor = new Color(luminance, luminance, luminance, 1f);
            fixationCrossMaterial = new Material(shader)
            {
                name = "AVTrainingFixationCrossMaterial",
                color = crossColor,
                renderQueue = 3000
            };

            if (fixationCrossMaterial.HasProperty("_BaseColor"))
                fixationCrossMaterial.SetColor("_BaseColor", crossColor);
            if (fixationCrossMaterial.HasProperty("_Color"))
                fixationCrossMaterial.SetColor("_Color", crossColor);
            if (fixationCrossMaterial.HasProperty("_Cull"))
                fixationCrossMaterial.SetFloat("_Cull", 0f);

            CreateFixationCrossBar(
                "Horizontal",
                new Vector3(fixationCrossSizeMeters, fixationCrossThicknessMeters, 1f));
            CreateFixationCrossBar(
                "Vertical",
                new Vector3(fixationCrossThicknessMeters, fixationCrossSizeMeters, 1f));

            fixationCrossRoot.SetActive(false);
        }

        private void CreateFixationCrossBar(string name, Vector3 localScale)
        {
            if (fixationCrossRoot == null)
                return;

            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bar.name = $"AVTrainingFixationCross_{name}";
            bar.transform.SetParent(fixationCrossRoot.transform, false);
            bar.transform.localPosition = Vector3.zero;
            bar.transform.localRotation = Quaternion.identity;
            bar.transform.localScale = localScale;

            Collider collider = bar.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            Renderer renderer = bar.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = fixationCrossMaterial;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }

        private void SetFixationCrossVisible(bool visible)
        {
            if (!showCenterFixationCross)
            {
                if (fixationCrossRoot != null)
                    fixationCrossRoot.SetActive(false);
                return;
            }

            EnsureFixationCrossDisplay();

            if (fixationCrossRoot != null)
                fixationCrossRoot.SetActive(visible);
        }

        private void DestroyFixationCrossDisplay()
        {
            if (fixationCrossRoot != null)
            {
                Destroy(fixationCrossRoot);
                fixationCrossRoot = null;
            }

            if (fixationCrossMaterial != null)
            {
                Destroy(fixationCrossMaterial);
                fixationCrossMaterial = null;
            }
        }

        private Material GetGeneratedStimulusMaterial()
        {
            if (generatedStimulusMaterial != null)
                return generatedStimulusMaterial;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Texture");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            generatedStimulusMaterial = new Material(shader)
            {
                name = "AVTrainingGeneratedStimulus",
                color = Color.white
            };

            if (generatedStimulusMaterial.HasProperty("_BaseMap"))
                generatedStimulusMaterial.SetTexture("_BaseMap", generatedStimulusTexture);
            if (generatedStimulusMaterial.HasProperty("_MainTex"))
                generatedStimulusMaterial.SetTexture("_MainTex", generatedStimulusTexture);
            if (generatedStimulusMaterial.HasProperty("_BaseColor"))
                generatedStimulusMaterial.SetColor("_BaseColor", Color.white);
            if (generatedStimulusMaterial.HasProperty("_Color"))
                generatedStimulusMaterial.SetColor("_Color", Color.white);
            if (generatedStimulusMaterial.HasProperty("_Cull"))
                generatedStimulusMaterial.SetFloat("_Cull", 0f);

            return generatedStimulusMaterial;
        }

        private void UpdateGeneratedStimulusTexture(float contrastFraction)
        {
            int textureSize = Mathf.ClosestPowerOfTwo(Mathf.Clamp(generatedStimulusTextureSize, 32, 512));
            if (generatedStimulusTexture == null ||
                generatedStimulusTexture.width != textureSize ||
                generatedStimulusTexture.height != textureSize)
            {
                if (generatedStimulusTexture != null)
                    Destroy(generatedStimulusTexture);

                generatedStimulusTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false, true)
                {
                    name = "AVTrainingGeneratedStimulusTexture",
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
            }

            Color[] pixels = new Color[textureSize * textureSize];
            float background = Mathf.Clamp01(stimulusBackgroundLuminance);
            float contrast = Mathf.Clamp01(contrastFraction);
            float radius = 0.5f;
            float edgeStart = radius * (1f - Mathf.Clamp01(stimulusSoftEdgeFraction));
            float cycles = Mathf.Max(1f, gaborCycles);

            for (int y = 0; y < textureSize; y++)
            {
                float v = ((y + 0.5f) / textureSize - 0.5f) * 2f;
                for (int x = 0; x < textureSize; x++)
                {
                    float u = ((x + 0.5f) / textureSize - 0.5f) * 2f;
                    float normalizedRadius = Mathf.Sqrt(u * u + v * v) * 0.5f;
                    float mask = 1f - Mathf.SmoothStep(edgeStart, radius, normalizedRadius);
                    float luminance = background;

                    if (mask > 0f)
                    {
                        if (generatedStimulusPattern == StimulusPattern.GaborPatch)
                        {
                            float gaussian = Mathf.Exp(-(u * u + v * v) * 2.5f);
                            float carrier = Mathf.Cos(u * Mathf.PI * cycles);
                            luminance = Mathf.Clamp01(background + carrier * contrast * 0.5f * gaussian * mask);
                        }
                        else
                        {
                            float diskLuminance = Mathf.Clamp01(background * (1f - contrast));
                            luminance = Mathf.Lerp(background, diskLuminance, mask);
                        }
                    }

                    pixels[y * textureSize + x] = new Color(luminance, luminance, luminance, 1f);
                }
            }

            generatedStimulusTexture.SetPixels(pixels);
            generatedStimulusTexture.Apply(false, false);

            if (generatedStimulusMaterial != null)
            {
                if (generatedStimulusMaterial.HasProperty("_BaseMap"))
                    generatedStimulusMaterial.SetTexture("_BaseMap", generatedStimulusTexture);
                if (generatedStimulusMaterial.HasProperty("_MainTex"))
                    generatedStimulusMaterial.SetTexture("_MainTex", generatedStimulusTexture);
            }
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
            InputAction action = responseAction.action;
            if (action != null && action.enabled && action.WasPressedThisFrame())
                return true;

            if (DetectXRControllerResponse())
                return true;

            if (!enableLegacyKeyboardFallback) return false;

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) return true;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return)) return true;

            if (UnityEngine.Input.GetAxis("Submit") > 0.5f) return true;

            return false;
        }

        private void EnsureReadyPromptDisplay()
        {
            if (readyPromptRoot != null && readyPromptText != null)
                return;

            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("[AVTraining] Cannot create ready prompt because no Main Camera was found.");
                return;
            }

            readyPromptRoot = new GameObject("AVTrainingReadyPrompt");
            readyPromptRoot.transform.SetParent(cam.transform, false);
            readyPromptRoot.transform.localPosition = new Vector3(0f, -0.02f, readyPromptDistanceMeters);
            readyPromptRoot.transform.localRotation = Quaternion.identity;
            readyPromptRoot.transform.localScale = Vector3.one * 0.0015f;

            Canvas canvas = readyPromptRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = cam;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(900f, 520f);

            var scaler = readyPromptRoot.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 24f;

            readyPromptRoot.AddComponent<GraphicRaycaster>();

            GameObject panelObject = new GameObject("Panel");
            panelObject.transform.SetParent(readyPromptRoot.transform, false);

            RectTransform panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = panelObject.AddComponent<Image>();
            panelImage.color = new Color(0.02f, 0.02f, 0.02f, 0.96f);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(panelObject.transform, false);

            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(48f, 36f);
            textRect.offsetMax = new Vector2(-48f, -36f);

            readyPromptText = textObject.AddComponent<TextMeshProUGUI>();
            readyPromptText.alignment = TextAlignmentOptions.Center;
            readyPromptText.color = Color.white;
            readyPromptText.enableAutoSizing = true;
            readyPromptText.fontSizeMin = 24f;
            readyPromptText.fontSizeMax = 54f;
            readyPromptText.raycastTarget = false;
        }

        private void UpdateReadyPromptDisplay(float countdownRemainingSec, bool waitingForConfirmation)
        {
            if (readyPromptText == null)
                return;

            if (waitingForConfirmation)
            {
                readyPromptText.text =
                    "<b>GET READY</b>\n\n" +
                    readyPromptInstructions +
                    "\n\n<size=70%>No data collection starts until you confirm.</size>";
                return;
            }

            int seconds = Mathf.Max(0, Mathf.CeilToInt(countdownRemainingSec));
            readyPromptText.text =
                $"<b>STARTING IN {seconds}</b>\n\n" +
                readyCountdownInstructions;
        }

        private void UpdatePracticePromptDisplay()
        {
            if (readyPromptText == null)
                return;

            readyPromptText.text =
                "<b>PRACTICE</b>\n\n" +
                practicePromptInstructions +
                "\n\n<size=70%>Recorded pilot trials start after practice.</size>";
        }

        private IEnumerator UpdateBaselinePrompt()
        {
            while (currentPhase == SessionPhase.Baseline)
            {
                if (readyPromptText != null)
                {
                    int seconds = Mathf.Max(0, Mathf.CeilToInt(GetPhaseRemainingTime()));
                    readyPromptText.text =
                        "<b>CALIBRATING</b>\n\n" +
                        baselinePromptInstructions +
                        $"\n\n<size=70%>Training starts in {seconds} seconds.</size>";
                }

                yield return null;
            }
        }

        private void ShowCompletionPromptDisplay()
        {
            EnsureReadyPromptDisplay();

            if (readyPromptText == null)
                return;

            readyPromptText.text =
                "<b>SESSION COMPLETE</b>\n\n" +
                completionPromptInstructions;
        }

        private void DestroyReadyPromptDisplay()
        {
            if (readyPromptRoot == null)
                return;

            Destroy(readyPromptRoot);
            readyPromptRoot = null;
            readyPromptText = null;
        }

        private bool DetectXRControllerResponse()
        {
            bool left = DetectXRControllerResponse(
                UnityEngine.XR.InputDeviceCharacteristics.Left,
                ref leftXRResponsePressed,
                "left");

            bool right = DetectXRControllerResponse(
                UnityEngine.XR.InputDeviceCharacteristics.Right,
                ref rightXRResponsePressed,
                "right");

            return left || right;
        }

        private bool DetectXRControllerResponse(
            UnityEngine.XR.InputDeviceCharacteristics hand,
            ref bool wasPressed,
            string handLabel)
        {
            xrControllerDevices.Clear();
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
                UnityEngine.XR.InputDeviceCharacteristics.Controller | hand,
                xrControllerDevices);

            bool isPressed = false;
            string source = "";

            foreach (UnityEngine.XR.InputDevice device in xrControllerDevices)
            {
                if (TryReadXRResponsePressed(device, out source))
                {
                    isPressed = true;
                    break;
                }
            }

            if (isPressed && !wasPressed)
            {
                Debug.Log($"[AVTraining] XR {handLabel} response detected via {source}.");
                wasPressed = true;
                return true;
            }

            wasPressed = isPressed;
            return false;
        }

        private static bool TryReadXRResponsePressed(UnityEngine.XR.InputDevice device, out string source)
        {
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool triggerButton) &&
                triggerButton)
            {
                source = $"{device.name}/triggerButton";
                return true;
            }

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out float trigger) &&
                trigger >= 0.65f)
            {
                source = $"{device.name}/trigger={trigger:F2}";
                return true;
            }

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool gripButton) &&
                gripButton)
            {
                source = $"{device.name}/gripButton";
                return true;
            }

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out float grip) &&
                grip >= 0.65f)
            {
                source = $"{device.name}/grip={grip:F2}";
                return true;
            }

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool primaryButton) &&
                primaryButton)
            {
                source = $"{device.name}/primaryButton";
                return true;
            }

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool secondaryButton) &&
                secondaryButton)
            {
                source = $"{device.name}/secondaryButton";
                return true;
            }

            source = "";
            return false;
        }

        private void EnableResponseAction()
        {
            InputAction action = GetOrCreateResponseAction();
            if (action == null || action.enabled) return;

            action.Enable();
            responseActionEnabledByTask = true;
        }

        private void DisableResponseAction()
        {
            InputAction action = responseAction.action;
            if (action != null && responseActionEnabledByTask && action.enabled)
                action.Disable();

            responseActionEnabledByTask = false;
        }

        private InputAction GetOrCreateResponseAction()
        {
            InputAction action = responseAction.action;
            if (action != null) return action;

            defaultResponseAction = new InputAction(
                name: "AVTrainingResponse",
                type: InputActionType.Button,
                expectedControlType: "Button");

            AddDefaultResponseBindings(defaultResponseAction);
            responseAction = new InputActionProperty(defaultResponseAction);
            return defaultResponseAction;
        }

        private static void AddDefaultResponseBindings(InputAction action)
        {
            // Generic XR covers OpenXR/Quest controller profiles; explicit Meta/Oculus bindings cover
            // editor/device layouts that surface Quest controllers by profile-specific layout names.
            action.AddBinding("<XRController>{LeftHand}/triggerPressed");
            action.AddBinding("<XRController>{RightHand}/triggerPressed");
            action.AddBinding("<XRController>{LeftHand}/trigger").WithInteraction("press");
            action.AddBinding("<XRController>{RightHand}/trigger").WithInteraction("press");
            action.AddBinding("<XRController>{LeftHand}/gripPressed");
            action.AddBinding("<XRController>{RightHand}/gripPressed");
            action.AddBinding("<XRController>{LeftHand}/primaryButton");
            action.AddBinding("<XRController>{RightHand}/primaryButton");
            action.AddBinding("<XRController>{LeftHand}/secondaryButton");
            action.AddBinding("<XRController>{RightHand}/secondaryButton");
            action.AddBinding("<OculusTouchController>{LeftHand}/triggerPressed");
            action.AddBinding("<OculusTouchController>{RightHand}/triggerPressed");
            action.AddBinding("<OculusTouchController>{LeftHand}/trigger").WithInteraction("press");
            action.AddBinding("<OculusTouchController>{RightHand}/trigger").WithInteraction("press");
            action.AddBinding("<OculusTouchController>{LeftHand}/gripPressed");
            action.AddBinding("<OculusTouchController>{RightHand}/gripPressed");
            action.AddBinding("<OculusTouchController>{LeftHand}/primaryButton");
            action.AddBinding("<OculusTouchController>{RightHand}/primaryButton");
            action.AddBinding("<OculusTouchController>{LeftHand}/secondaryButton");
            action.AddBinding("<OculusTouchController>{RightHand}/secondaryButton");
            action.AddBinding("<QuestTouchPlusController>{LeftHand}/triggerPressed");
            action.AddBinding("<QuestTouchPlusController>{RightHand}/triggerPressed");
            action.AddBinding("<QuestTouchPlusController>{LeftHand}/trigger").WithInteraction("press");
            action.AddBinding("<QuestTouchPlusController>{RightHand}/trigger").WithInteraction("press");
            action.AddBinding("<QuestProTouchController>{LeftHand}/triggerPressed");
            action.AddBinding("<QuestProTouchController>{RightHand}/triggerPressed");
            action.AddBinding("<QuestProTouchController>{LeftHand}/trigger").WithInteraction("press");
            action.AddBinding("<QuestProTouchController>{RightHand}/trigger").WithInteraction("press");
            action.AddBinding("<Keyboard>/space");
            action.AddBinding("<Keyboard>/enter");
        }

        private void LogXRControllerDevices()
        {
            if (xrControllerDevicesLogged) return;
            xrControllerDevicesLogged = true;

            xrControllerDevices.Clear();
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
                UnityEngine.XR.InputDeviceCharacteristics.Controller,
                xrControllerDevices);

            foreach (UnityEngine.XR.InputDevice device in xrControllerDevices)
            {
                xrFeatureUsages.Clear();
                device.TryGetFeatureUsages(xrFeatureUsages);
                Debug.Log($"[AVTraining] XR controller device: name={device.name}, " +
                          $"characteristics={device.characteristics}, features={string.Join(", ", xrFeatureUsages)}");
            }
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
            GUILayout.Label($"Rehab: {totalRehabHitsThisSession}/{totalRehabTrialsThisSession}" +
                            (totalRehabTrialsThisSession > 0 ? $" ({(float)totalRehabHitsThisSession / totalRehabTrialsThisSession:P0})" : ""));
            GUILayout.Label($"Controls: {totalControlHitsThisSession}/{totalControlTrialsThisSession}" +
                            (totalControlTrialsThisSession > 0 ? $" ({(float)totalControlHitsThisSession / totalControlTrialsThisSession:P0})" : ""));
            if (progression != null)
                GUILayout.Label($"Severity: {progression.severity} | Affected: {progression.affectedHemifield}");
            GUILayout.Label("<i>Press Quest trigger or SPACE on detection.</i>");
            GUILayout.EndArea();
        }
    }
}
