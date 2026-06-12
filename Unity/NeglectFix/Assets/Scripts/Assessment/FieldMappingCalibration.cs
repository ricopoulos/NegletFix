using System;
using System.Collections;
using System.Collections.Generic;
using NeglectFix.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace NeglectFix.Assessment
{
    /// <summary>
    /// Separate fixed-cross spatial field map for choosing defensible rehab target locations.
    /// This is not Humphrey perimetry and does not estimate retinal position without eye tracking.
    /// It records controlled world/camera/head-pose evidence before the AV rehab dose ramp resumes.
    /// </summary>
    public class FieldMappingCalibration : MonoBehaviour
    {
        public enum CalibrationPhase
        {
            NotStarted,
            Ready,
            Mapping,
            Completed
        }

        public enum Hemifield
        {
            Left,
            Right
        }

        [Header("Calibration Scope")]
        [Tooltip("Affected side used for first-pass rehab location recommendations.")]
        public Hemifield affectedHemifield = Hemifield.Left;

        [Tooltip("Horizontal/vertical target angles in degrees. X = horizontal, Y = vertical.")]
        public Vector2[] testAnglesDeg =
        {
            new Vector2(-5f, 0f),
            new Vector2(-8f, 0f),
            new Vector2(-12f, 0f),
            new Vector2(-16f, 0f),
            new Vector2(5f, 0f),
            new Vector2(10f, 0f),
            new Vector2(15f, 0f),
            new Vector2(0f, 5f),
            new Vector2(0f, 10f),
            new Vector2(0f, 15f),
            new Vector2(0f, -5f),
            new Vector2(0f, -10f),
            new Vector2(0f, -15f)
        };

        [Tooltip("Repeats per target point. Two repeats keeps the scene quick while catching obvious misses.")]
        [Range(1, 5)]
        public int repetitionsPerPoint = 2;

        [Tooltip("Shuffle the target sequence so locations are not predictable.")]
        public bool shuffleTrialOrder = true;

        [Header("Timing")]
        [Range(0f, 3f)]
        public float initialDelaySec = 0.75f;

        [Range(0f, 5f)]
        public float readyCountdownDurationSec = 1f;

        public bool requireReadyConfirmation = true;

        [Range(0.1f, 3f)]
        public float minInterTrialIntervalSec = 0.6f;

        [Range(0.1f, 3f)]
        public float maxInterTrialIntervalSec = 1.0f;

        [Range(0.05f, 1.0f)]
        public float stimulusDurationSec = 0.35f;

        [Range(0.5f, 3.0f)]
        public float responseWindowSec = 1.5f;

        [Header("Stimulus")]
        [Range(0.5f, 5f)]
        public float stimulusDistanceMeters = 1.5f;

        [Range(0.5f, 12f)]
        public float stimulusAngularSizeDeg = 6f;

        [Range(0f, 1f)]
        public float stimulusBackgroundLuminance = 0.5f;

        [Range(0f, 1f)]
        public float stimulusContrast = 1f;

        [Range(32, 512)]
        public int stimulusTextureSize = 128;

        [Range(0f, 0.5f)]
        public float stimulusSoftEdgeFraction = 0.12f;

        [Header("Controlled Background")]
        public bool ensureOpaqueBackdrop = true;

        [Range(1.6f, 5f)]
        public float backdropDistanceMeters = 2.4f;

        [Range(2f, 8f)]
        public float backdropWidthMeters = 5.2f;

        [Range(1.5f, 5f)]
        public float backdropHeightMeters = 3.4f;

        [Range(0f, 1f)]
        public float backdropLuminance = 0.5f;

        [Header("Fixation Cross")]
        public bool showCenterFixationCross = true;

        [Range(0.75f, 3f)]
        public float fixationCrossDistanceMeters = 1.15f;

        [Range(0.01f, 0.2f)]
        public float fixationCrossSizeMeters = 0.04f;

        [Range(0.002f, 0.03f)]
        public float fixationCrossThicknessMeters = 0.004f;

        [Range(0f, 1f)]
        public float fixationCrossLuminance = 0.92f;

        [Header("Prompt")]
        public bool showPromptInHeadset = true;

        [Range(0.75f, 3f)]
        public float promptDistanceMeters = 1.25f;

        [TextArea(2, 5)]
        public string readyInstructions =
            "This is a field-mapping calibration, not rehab.\nKeep your gaze on the center cross. Press when you detect a point.\nKeep your head still.";

        [TextArea(2, 4)]
        public string completionInstructions = "Field map complete.\nYou can remove the headset now.";

        [Header("Input")]
        public InputActionProperty responseAction;
        public bool enableLegacyKeyboardFallback = true;

        [Header("Dependencies")]
        public DataLogger dataLogger;

        [Header("Status")]
        public CalibrationPhase currentPhase = CalibrationPhase.NotStarted;
        public int totalTrialsCompleted = 0;
        public int totalHits = 0;
        public bool autoStartOnSceneLoad = true;
        public bool autoStartSessionLog = true;

        private struct PlannedTrial
        {
            public Vector2 anglesDeg;
            public int repeatIndex;
            public string axis;
        }

        public struct CalibrationTrial
        {
            public int trialIndex;
            public int repeatIndex;
            public string axis;
            public float horizontalAngleDeg;
            public float verticalAngleDeg;
            public float stimulusDistanceMeters;
            public Vector3 stimulusWorldPosition;
            public Vector3 cameraWorldPosition;
            public Vector3 cameraRelativeDirectionAtOnset;
            public float headYawOnsetDeg;
            public float headPitchOnsetDeg;
            public float headRollOnsetDeg;
            public float stimulusOnsetMs;
            public float responseOnsetMs;
            public float rtMs;
            public bool hit;
            public float headYawResponseDeg;
            public float headPitchResponseDeg;
            public float headRollResponseDeg;
            public Vector3 cameraRelativeDirectionAtResponse;
        }

        private InputAction defaultResponseAction;
        private bool responseActionEnabledByTask;
        private readonly List<UnityEngine.XR.InputDevice> xrControllerDevices = new List<UnityEngine.XR.InputDevice>();
        private bool leftXRResponsePressed;
        private bool rightXRResponsePressed;
        private Material stimulusMaterial;
        private Texture2D stimulusTexture;
        private GameObject backdrop;
        private Material backdropMaterial;
        private GameObject fixationCrossRoot;
        private Material fixationCrossMaterial;
        private GameObject promptRoot;
        private TextMeshProUGUI promptText;
        private float mapStartTime;

        public event Action<CalibrationTrial> OnCalibrationTrialLogged;
        public event Action OnCalibrationCompleted;

        private void OnEnable()
        {
            EnableResponseAction();
        }

        private void OnDisable()
        {
            DisableResponseAction();
        }

        private void Start()
        {
            if (dataLogger == null)
                dataLogger = FindObjectOfType<DataLogger>();

            dataLogger?.SetFieldMappingAffectedAxis(GetAffectedAxisName());

            EnableResponseAction();
            EnsureOpaqueBackdrop();
            EnsureFixationCrossDisplay();
            SetFixationCrossVisible(false);

            if (autoStartOnSceneLoad)
                StartCoroutine(RunCalibration());
        }

        private void OnDestroy()
        {
            dataLogger?.CloseFieldMappingLog();

            if (defaultResponseAction != null)
            {
                defaultResponseAction.Dispose();
                defaultResponseAction = null;
            }

            DestroyStimulusResources();
            DestroyBackdrop();
            DestroyFixationCrossDisplay();
            DestroyPromptDisplay();
        }

        public void StartCalibration()
        {
            if (currentPhase == CalibrationPhase.NotStarted)
                StartCoroutine(RunCalibration());
        }

        private IEnumerator RunCalibration()
        {
            yield return new WaitForSeconds(Mathf.Max(0f, initialDelaySec));

            currentPhase = CalibrationPhase.Ready;
            EnsurePromptDisplay();

            if (requireReadyConfirmation)
            {
                while (!DetectResponse())
                {
                    UpdateReadyPrompt(waitingForConfirmation: true, readyCountdownDurationSec);
                    yield return null;
                }
            }

            float countdownEndTime = Time.time + Mathf.Max(0f, readyCountdownDurationSec);
            while (Time.time < countdownEndTime)
            {
                UpdateReadyPrompt(waitingForConfirmation: false, countdownEndTime - Time.time);
                yield return null;
            }

            DestroyPromptDisplay();
            SetFixationCrossVisible(true);

            if (autoStartSessionLog && dataLogger != null && !dataLogger.IsLogging())
                dataLogger.StartLogging();

            dataLogger?.LogEvent("field_mapping_start");
            mapStartTime = Time.time;
            currentPhase = CalibrationPhase.Mapping;
            totalTrialsCompleted = 0;
            totalHits = 0;

            yield return RunMappingTrials();

            currentPhase = CalibrationPhase.Completed;
            SetFixationCrossVisible(false);
            dataLogger?.LogEvent("field_mapping_complete");
            dataLogger?.CloseFieldMappingLog();

            if (autoStartSessionLog)
                dataLogger?.StopLogging();

            ShowCompletionPrompt();
            OnCalibrationCompleted?.Invoke();
            Debug.Log($"[FieldMap] Calibration complete. {totalHits}/{totalTrialsCompleted} hits. File: {dataLogger?.GetCurrentFieldMappingFile()}");
        }

        private IEnumerator RunMappingTrials()
        {
            List<PlannedTrial> plannedTrials = BuildTrialSequence();

            for (int i = 0; i < plannedTrials.Count; i++)
            {
                PlannedTrial planned = plannedTrials[i];
                float isi = UnityEngine.Random.Range(minInterTrialIntervalSec, maxInterTrialIntervalSec);
                yield return new WaitForSeconds(isi);

                CalibrationTrial trial = default(CalibrationTrial);
                yield return RunSingleTrial(planned, i + 1, result => trial = result);

                dataLogger?.LogFieldMappingTrial(trial);
                OnCalibrationTrialLogged?.Invoke(trial);
            }
        }

        private IEnumerator RunSingleTrial(PlannedTrial planned, int trialIndex, Action<CalibrationTrial> onComplete)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("[FieldMap] Cannot run calibration trial without a Main Camera.");
                yield break;
            }

            Vector3 stimulusPosition = ComputeStimulusPosition(planned.anglesDeg, stimulusDistanceMeters);
            Vector3 cameraPositionAtOnset = cam.transform.position;
            Quaternion cameraRotationAtOnset = cam.transform.rotation;
            Vector3 cameraRelativeDirectionAtOnset = cam.transform.InverseTransformDirection(
                (stimulusPosition - cameraPositionAtOnset).normalized);
            Vector3 onsetEuler = NormalizeEulerAngles(cameraRotationAtOnset.eulerAngles);

            GameObject stimulus = SpawnStimulus(stimulusPosition);
            float stimulusOnsetTime = Time.time;
            float responseTime = -1f;
            bool hit = false;

            float stimulusEndTime = stimulusOnsetTime + stimulusDurationSec;
            float responseDeadline = stimulusOnsetTime + responseWindowSec;

            while (Time.time < responseDeadline && currentPhase == CalibrationPhase.Mapping)
            {
                if (!hit && DetectResponse())
                {
                    hit = true;
                    responseTime = Time.time;
                    break;
                }

                if (Time.time >= stimulusEndTime && stimulus != null && stimulus.activeSelf)
                    stimulus.SetActive(false);

                yield return null;
            }

            if (stimulus != null)
                Destroy(stimulus);

            Vector3 responseEuler = NormalizeEulerAngles(cam.transform.rotation.eulerAngles);
            Vector3 cameraRelativeDirectionAtResponse = cam.transform.InverseTransformDirection(
                (stimulusPosition - cam.transform.position).normalized);

            totalTrialsCompleted++;
            if (hit)
                totalHits++;

            var trial = new CalibrationTrial
            {
                trialIndex = trialIndex,
                repeatIndex = planned.repeatIndex,
                axis = planned.axis,
                horizontalAngleDeg = planned.anglesDeg.x,
                verticalAngleDeg = planned.anglesDeg.y,
                stimulusDistanceMeters = stimulusDistanceMeters,
                stimulusWorldPosition = stimulusPosition,
                cameraWorldPosition = cameraPositionAtOnset,
                cameraRelativeDirectionAtOnset = cameraRelativeDirectionAtOnset,
                headYawOnsetDeg = onsetEuler.y,
                headPitchOnsetDeg = onsetEuler.x,
                headRollOnsetDeg = onsetEuler.z,
                stimulusOnsetMs = (stimulusOnsetTime - mapStartTime) * 1000f,
                responseOnsetMs = responseTime > 0f ? (responseTime - mapStartTime) * 1000f : -1f,
                rtMs = responseTime > 0f ? (responseTime - stimulusOnsetTime) * 1000f : -1f,
                hit = hit,
                headYawResponseDeg = responseEuler.y,
                headPitchResponseDeg = responseEuler.x,
                headRollResponseDeg = responseEuler.z,
                cameraRelativeDirectionAtResponse = cameraRelativeDirectionAtResponse
            };

            Debug.Log($"[FieldMap] Trial {trialIndex}: {planned.axis} " +
                      $"h={planned.anglesDeg.x:F1} v={planned.anglesDeg.y:F1} " +
                      $"{(hit ? "hit" : "miss")} rt={trial.rtMs:F0}ms.");

            onComplete?.Invoke(trial);
        }

        private List<PlannedTrial> BuildTrialSequence()
        {
            var plannedTrials = new List<PlannedTrial>();
            int repeats = Mathf.Max(1, repetitionsPerPoint);

            foreach (Vector2 angles in testAnglesDeg)
            {
                for (int repeat = 1; repeat <= repeats; repeat++)
                {
                    plannedTrials.Add(new PlannedTrial
                    {
                        anglesDeg = angles,
                        repeatIndex = repeat,
                        axis = GetAxisLabel(angles)
                    });
                }
            }

            if (shuffleTrialOrder)
                Shuffle(plannedTrials);

            return plannedTrials;
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        private string GetAffectedAxisName()
        {
            return affectedHemifield == Hemifield.Right ? "right" : "left";
        }

        private static string GetAxisLabel(Vector2 anglesDeg)
        {
            float absHorizontal = Mathf.Abs(anglesDeg.x);
            float absVertical = Mathf.Abs(anglesDeg.y);

            if (absHorizontal >= absVertical && absHorizontal > 0.01f)
                return anglesDeg.x < 0f ? "left" : "right";

            if (absVertical > 0.01f)
                return anglesDeg.y > 0f ? "up" : "down";

            return "center";
        }

        private Vector3 ComputeStimulusPosition(Vector2 anglesDeg, float distance)
        {
            Camera cam = Camera.main;
            Vector3 origin = cam != null ? cam.transform.position : Vector3.zero;
            Quaternion rotation = cam != null ? cam.transform.rotation : Quaternion.identity;

            float yawRad = anglesDeg.x * Mathf.Deg2Rad;
            float pitchRad = anglesDeg.y * Mathf.Deg2Rad;
            Vector3 localDirection = new Vector3(
                Mathf.Sin(yawRad) * Mathf.Cos(pitchRad),
                Mathf.Sin(pitchRad),
                Mathf.Cos(yawRad) * Mathf.Cos(pitchRad)).normalized;

            return origin + rotation * localDirection * distance;
        }

        private GameObject SpawnStimulus(Vector3 position)
        {
            GameObject stimulus = GameObject.CreatePrimitive(PrimitiveType.Quad);
            stimulus.name = "FieldMappingStimulus";
            stimulus.transform.position = position;
            FaceToCamera(stimulus);
            ScaleStimulusToAngularSize(stimulus, position);

            Collider collider = stimulus.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            Renderer renderer = stimulus.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = GetStimulusMaterial();
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }

            return stimulus;
        }

        private void FaceToCamera(GameObject target)
        {
            Camera cam = Camera.main;
            if (cam == null || target == null)
                return;

            target.transform.rotation = cam.transform.rotation;
        }

        private void ScaleStimulusToAngularSize(GameObject stimulus, Vector3 position)
        {
            Camera cam = Camera.main;
            float distance = cam != null ? Vector3.Distance(cam.transform.position, position) : stimulusDistanceMeters;
            float diameterMeters = 2f * distance * Mathf.Tan(stimulusAngularSizeDeg * Mathf.Deg2Rad * 0.5f);
            stimulus.transform.localScale = new Vector3(diameterMeters, diameterMeters, 1f);
        }

        private Material GetStimulusMaterial()
        {
            if (stimulusMaterial != null)
                return stimulusMaterial;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Texture");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            stimulusMaterial = new Material(shader)
            {
                name = "FieldMappingStimulusMaterial",
                color = Color.white
            };

            UpdateStimulusTexture();

            if (stimulusMaterial.HasProperty("_BaseMap"))
                stimulusMaterial.SetTexture("_BaseMap", stimulusTexture);
            if (stimulusMaterial.HasProperty("_MainTex"))
                stimulusMaterial.SetTexture("_MainTex", stimulusTexture);
            if (stimulusMaterial.HasProperty("_BaseColor"))
                stimulusMaterial.SetColor("_BaseColor", Color.white);
            if (stimulusMaterial.HasProperty("_Color"))
                stimulusMaterial.SetColor("_Color", Color.white);
            if (stimulusMaterial.HasProperty("_Cull"))
                stimulusMaterial.SetFloat("_Cull", 0f);

            return stimulusMaterial;
        }

        private void UpdateStimulusTexture()
        {
            int size = Mathf.ClosestPowerOfTwo(Mathf.Clamp(stimulusTextureSize, 32, 512));
            if (stimulusTexture == null || stimulusTexture.width != size || stimulusTexture.height != size)
            {
                if (stimulusTexture != null)
                    Destroy(stimulusTexture);

                stimulusTexture = new Texture2D(size, size, TextureFormat.RGBA32, false, true)
                {
                    name = "FieldMappingStimulusTexture",
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
            }

            Color[] pixels = new Color[size * size];
            float background = Mathf.Clamp01(stimulusBackgroundLuminance);
            float contrast = Mathf.Clamp01(stimulusContrast);
            float targetLuminance = Mathf.Clamp01(background * (1f - contrast));
            float edgeStart = 0.5f * (1f - Mathf.Clamp01(stimulusSoftEdgeFraction));

            for (int y = 0; y < size; y++)
            {
                float v = ((y + 0.5f) / size - 0.5f) * 2f;
                for (int x = 0; x < size; x++)
                {
                    float u = ((x + 0.5f) / size - 0.5f) * 2f;
                    float radius = Mathf.Sqrt(u * u + v * v) * 0.5f;
                    float mask = 1f - Mathf.SmoothStep(edgeStart, 0.5f, radius);
                    float luminance = Mathf.Lerp(background, targetLuminance, mask);
                    pixels[y * size + x] = new Color(luminance, luminance, luminance, mask);
                }
            }

            stimulusTexture.SetPixels(pixels);
            stimulusTexture.Apply(false, false);
        }

        private void EnsureOpaqueBackdrop()
        {
            if (!ensureOpaqueBackdrop || backdrop != null)
                return;

            Camera cam = Camera.main;
            if (cam == null)
                return;

            backdrop = GameObject.CreatePrimitive(PrimitiveType.Quad);
            backdrop.name = "FieldMappingControlledBackdrop";
            backdrop.transform.SetParent(cam.transform, false);
            backdrop.transform.localPosition = new Vector3(0f, 0f, Mathf.Max(backdropDistanceMeters, stimulusDistanceMeters + 0.35f));
            backdrop.transform.localRotation = Quaternion.identity;
            backdrop.transform.localScale = new Vector3(backdropWidthMeters, backdropHeightMeters, 1f);

            Collider collider = backdrop.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            float luminance = Mathf.Clamp01(backdropLuminance);
            Color color = new Color(luminance, luminance, luminance, 1f);
            backdropMaterial = new Material(shader)
            {
                name = "FieldMappingBackdropMaterial",
                color = color,
                renderQueue = 2000
            };

            if (backdropMaterial.HasProperty("_BaseColor"))
                backdropMaterial.SetColor("_BaseColor", color);
            if (backdropMaterial.HasProperty("_Color"))
                backdropMaterial.SetColor("_Color", color);
            if (backdropMaterial.HasProperty("_Cull"))
                backdropMaterial.SetFloat("_Cull", 0f);

            Renderer renderer = backdrop.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = backdropMaterial;
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
                return;

            fixationCrossRoot = new GameObject("FieldMappingCenterFixationCross");
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
            Color color = new Color(luminance, luminance, luminance, 1f);
            fixationCrossMaterial = new Material(shader)
            {
                name = "FieldMappingFixationCrossMaterial",
                color = color,
                renderQueue = 3000
            };

            if (fixationCrossMaterial.HasProperty("_BaseColor"))
                fixationCrossMaterial.SetColor("_BaseColor", color);
            if (fixationCrossMaterial.HasProperty("_Color"))
                fixationCrossMaterial.SetColor("_Color", color);
            if (fixationCrossMaterial.HasProperty("_Cull"))
                fixationCrossMaterial.SetFloat("_Cull", 0f);

            CreateFixationCrossBar("Horizontal", new Vector3(fixationCrossSizeMeters, fixationCrossThicknessMeters, 1f));
            CreateFixationCrossBar("Vertical", new Vector3(fixationCrossThicknessMeters, fixationCrossSizeMeters, 1f));
            fixationCrossRoot.SetActive(false);
        }

        private void CreateFixationCrossBar(string name, Vector3 localScale)
        {
            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bar.name = $"FieldMappingFixationCross_{name}";
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

        private void EnsurePromptDisplay()
        {
            if (!showPromptInHeadset || promptRoot != null)
                return;

            Camera cam = Camera.main;
            if (cam == null)
                return;

            promptRoot = new GameObject("FieldMappingPrompt");
            promptRoot.transform.SetParent(cam.transform, false);
            promptRoot.transform.localPosition = new Vector3(0f, -0.02f, promptDistanceMeters);
            promptRoot.transform.localRotation = Quaternion.identity;
            promptRoot.transform.localScale = Vector3.one * 0.0015f;

            Canvas canvas = promptRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = cam;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(900f, 520f);

            var scaler = promptRoot.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 24f;

            promptRoot.AddComponent<GraphicRaycaster>();

            GameObject panelObject = new GameObject("Panel");
            panelObject.transform.SetParent(promptRoot.transform, false);
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

            promptText = textObject.AddComponent<TextMeshProUGUI>();
            promptText.alignment = TextAlignmentOptions.Center;
            promptText.color = Color.white;
            promptText.enableAutoSizing = true;
            promptText.fontSizeMin = 24f;
            promptText.fontSizeMax = 54f;
            promptText.raycastTarget = false;
        }

        private void UpdateReadyPrompt(bool waitingForConfirmation, float countdownRemainingSec)
        {
            if (promptText == null)
                return;

            if (waitingForConfirmation)
            {
                promptText.text =
                    "<b>FIELD MAP</b>\n\n" +
                    readyInstructions +
                    "\n\n<size=70%>No data collection starts until you confirm.</size>";
                return;
            }

            int seconds = Mathf.Max(0, Mathf.CeilToInt(countdownRemainingSec));
            promptText.text =
                $"<b>STARTING IN {seconds}</b>\n\n" +
                "Keep your gaze on the center cross.";
        }

        private void ShowCompletionPrompt()
        {
            EnsurePromptDisplay();

            if (promptText != null)
            {
                promptText.text =
                    "<b>FIELD MAP COMPLETE</b>\n\n" +
                    completionInstructions;
            }
        }

        private bool DetectResponse()
        {
            InputAction action = responseAction.action;
            if (action != null && action.enabled && action.WasPressedThisFrame())
                return true;

            if (DetectXRControllerResponse())
                return true;

            if (!enableLegacyKeyboardFallback)
                return false;

            if (Input.GetKeyDown(KeyCode.Space)) return true;
            if (Input.GetKeyDown(KeyCode.Return)) return true;

            try
            {
                if (Input.GetAxis("Submit") > 0.5f) return true;
            }
            catch (ArgumentException)
            {
                // Some input configurations do not define the legacy Submit axis.
            }

            return false;
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
                Debug.Log($"[FieldMap] XR {handLabel} response detected via {source}.");
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
            if (action == null || action.enabled)
                return;

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
            if (action != null)
                return action;

            defaultResponseAction = new InputAction(
                name: "FieldMappingResponse",
                type: InputActionType.Button,
                expectedControlType: "Button");

            AddDefaultResponseBindings(defaultResponseAction);
            responseAction = new InputActionProperty(defaultResponseAction);
            return defaultResponseAction;
        }

        private static void AddDefaultResponseBindings(InputAction action)
        {
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

        private static Vector3 NormalizeEulerAngles(Vector3 euler)
        {
            return new Vector3(
                NormalizeAngle(euler.x),
                NormalizeAngle(euler.y),
                NormalizeAngle(euler.z));
        }

        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f)
                angle -= 360f;
            return angle;
        }

        private void DestroyStimulusResources()
        {
            if (stimulusMaterial != null)
            {
                Destroy(stimulusMaterial);
                stimulusMaterial = null;
            }

            if (stimulusTexture != null)
            {
                Destroy(stimulusTexture);
                stimulusTexture = null;
            }
        }

        private void DestroyBackdrop()
        {
            if (backdrop != null)
            {
                Destroy(backdrop);
                backdrop = null;
            }

            if (backdropMaterial != null)
            {
                Destroy(backdropMaterial);
                backdropMaterial = null;
            }
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

        private void DestroyPromptDisplay()
        {
            if (promptRoot != null)
            {
                Destroy(promptRoot);
                promptRoot = null;
                promptText = null;
            }
        }

        private void OnGUI()
        {
            if (currentPhase == CalibrationPhase.NotStarted || currentPhase == CalibrationPhase.Completed)
                return;

            GUILayout.BeginArea(new Rect(10, Screen.height - 150, 360, 120));
            GUILayout.Label("<b>Field Mapping Calibration</b>");
            GUILayout.Label($"Phase: {currentPhase}");
            GUILayout.Label($"Trials: {totalTrialsCompleted}/{Mathf.Max(0, testAnglesDeg.Length * repetitionsPerPoint)}");
            GUILayout.Label($"Hits: {totalHits}/{totalTrialsCompleted}");
            GUILayout.Label("<i>Fixate center cross. Press when detected.</i>");
            GUILayout.EndArea();
        }
    }
}
