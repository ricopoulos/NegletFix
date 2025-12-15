using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NeglectFix.Assessment
{
    /// <summary>
    /// Modified Pelli-Robson contrast sensitivity test for VR.
    /// Tests central, left (affected), and right (intact) hemifields separately.
    ///
    /// Clinical parameters:
    /// - Letter size: 2° visual angle (Pelli-Robson standard)
    /// - Contrast steps: 0.15 LogCS
    /// - Triplet scoring: 2/3 correct to pass
    /// - Normal range (age 50-60): 1.65-1.95 LogCS
    /// - Clinically significant change: ≥0.30 LogCS
    /// </summary>
    public class ContrastSensitivityTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private TestMode testMode = TestMode.PelliRobson;

        [Header("Display Settings")]
        [Tooltip("Letter size in degrees of visual angle")]
        [SerializeField] private float letterSizeDegrees = 2.0f;

        [Tooltip("Virtual viewing distance in meters")]
        [SerializeField] private float viewingDistanceMeters = 1.0f;

        [Tooltip("Background luminance (50% gray = standard)")]
        [SerializeField] private Color backgroundColor = new Color(0.5f, 0.5f, 0.5f);

        [Tooltip("Eccentricity for hemifield testing (degrees from center)")]
        [SerializeField] private float hemifieldEccentricity = 10f;

        [Tooltip("UI offset in pixels for hemifield testing (based on typical viewing distance)")]
        [SerializeField] private float hemifieldUIOffset = 300f;

        [Header("Pelli-Robson Parameters")]
        [Tooltip("Starting contrast (0.0 = 100% contrast, higher = harder)")]
        [SerializeField] private float startingLogCS = 0.0f; // Start at 100% contrast (easiest)

        [Tooltip("Contrast step size (standard = 0.15)")]
        [SerializeField] private float logCSStep = 0.15f;

        [Tooltip("Maximum testable LogCS (2.25 = 0.56% contrast, near threshold)")]
        [SerializeField] private float maxLogCS = 2.25f;

        [Header("Display Calibration")]
        [Tooltip("Monitor gamma (typical: 2.2 for most displays)")]
        [SerializeField] private float displayGamma = 2.2f;

        [Tooltip("Background luminance in cd/m² (Pelli-Robson standard: 85)")]
        [SerializeField] private float targetBackgroundLuminance = 85f;

        [Tooltip("Enable gamma correction for accurate contrast")]
        [SerializeField] private bool useGammaCorrection = true;

        [Tooltip("Letters per contrast level")]
        [SerializeField] private int lettersPerTriplet = 3;

        [Tooltip("Correct responses needed to pass triplet")]
        [SerializeField] private int correctToPass = 2;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI letterDisplay;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Image backgroundPanel;
        [SerializeField] private Transform letterTransform;

        [Header("Fixation Cross (for hemifield testing)")]
        [Tooltip("Fixation cross shown during hemifield tests - user must keep eyes on this")]
        [SerializeField] private TextMeshProUGUI fixationCross;
        [SerializeField] private Color fixationColor = Color.red;
        [SerializeField] private int fixationFontSize = 80;

        [Header("Audio Feedback")]
        [SerializeField] private AudioClip correctSound;
        [SerializeField] private AudioClip incorrectSound;
        [SerializeField] private AudioClip levelCompleteSound;
        [SerializeField] private AudioClip testCompleteSound;

        [Header("Timing")]
        [Tooltip("Time to display each letter (0 = until response)")]
        [SerializeField] private float letterDisplayTime = 0f;

        [Tooltip("Delay between letters")]
        [SerializeField] private float interLetterDelay = 0.5f;

        // Sloan letters - designed for equal legibility at all sizes
        private readonly char[] sloanLetters = { 'C', 'D', 'H', 'K', 'N', 'O', 'R', 'S', 'V', 'Z' };

        // Test state
        private float currentLogCS;
        private int currentTripletIndex;
        private int correctInTriplet;
        private char currentLetter;
        private char[] currentTriplet;
        private bool testInProgress;
        private bool awaitingResponse;
        private float letterOnsetTime;
        private HemifieldMode currentHemifield;

        // Results storage
        private Dictionary<HemifieldMode, float> hemifieldResults = new Dictionary<HemifieldMode, float>();
        private List<TrialResult> trialHistory = new List<TrialResult>();

        // Audio
        private AudioSource audioSource;

        // Events
        public event Action<HemifieldMode, float> OnHemifieldTestComplete;
        public event Action<ContrastSensitivityResults> OnAllTestsComplete;
        public event Action<TrialResult> OnTrialComplete;

        public enum TestMode
        {
            PelliRobson,        // Standard contrast sensitivity
            QuickScreen,        // Fast 3-level check (for progress monitoring)
            SpatialFrequency    // CSF curve (advanced, future)
        }

        public enum HemifieldMode
        {
            Central,            // Fixation point (0°)
            LeftHemifield,      // Affected field in left hemianopia
            RightHemifield,     // Intact field
            Binocular           // Both eyes, central
        }

        [Serializable]
        public class TrialResult
        {
            public float logCS;
            public char presentedLetter;
            public char responseLetter;
            public bool correct;
            public float reactionTimeMs;
            public HemifieldMode hemifield;
            public DateTime timestamp;
            public int tripletNumber;
            public int letterInTriplet;
        }

        void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        void Start()
        {
            CreateFixationCrossIfNeeded();
            InitializeTest();
        }

        /// <summary>
        /// Creates a fixation cross dynamically if not assigned in the inspector.
        /// The fixation cross is essential for valid hemifield testing.
        /// </summary>
        private void CreateFixationCrossIfNeeded()
        {
            if (fixationCross == null && letterDisplay != null)
            {
                // Create fixation cross as sibling of letter display
                GameObject fixationObj = new GameObject("FixationCross");
                fixationObj.transform.SetParent(letterDisplay.transform.parent, false);

                fixationCross = fixationObj.AddComponent<TextMeshProUGUI>();
                fixationCross.text = "+";
                fixationCross.fontSize = fixationFontSize;
                fixationCross.color = fixationColor;
                fixationCross.alignment = TextAlignmentOptions.Center;

                var rectTransform = fixationCross.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(100, 100);

                Debug.Log("[ContrastTest] Created fixation cross for hemifield testing");
            }

            // Hide initially
            if (fixationCross != null)
                fixationCross.gameObject.SetActive(false);
        }

        public void InitializeTest()
        {
            currentLogCS = startingLogCS;
            currentTripletIndex = 0;
            correctInTriplet = 0;
            testInProgress = false;
            awaitingResponse = false;
            hemifieldResults.Clear();
            trialHistory.Clear();

            if (backgroundPanel != null)
                backgroundPanel.color = backgroundColor;

            if (letterDisplay != null)
                letterDisplay.text = "";

            // Hide fixation cross during initialization
            if (fixationCross != null)
                fixationCross.gameObject.SetActive(false);

            UpdateInstructions("CONTRAST SENSITIVITY TEST\n\n" +
                "Type the letter you see.\n" +
                "Press BACKSPACE if you can't see it.\n\n" +
                "Press SPACE to start");
        }

        /// <summary>
        /// Start testing a specific hemifield
        /// </summary>
        public void StartTest(HemifieldMode hemifield)
        {
            currentHemifield = hemifield;
            currentLogCS = startingLogCS;
            currentTripletIndex = 0;
            correctInTriplet = 0;
            testInProgress = true;

            UpdateInstructions($"Testing: {GetHemifieldName(hemifield)}\n" +
                "Focus on the center and identify letters.");

            GenerateNewTriplet();
            StartCoroutine(ShowNextLetterWithDelay(interLetterDelay));
        }

        /// <summary>
        /// Run full test sequence: Central → Right → Left
        /// </summary>
        public void StartFullTestSequence()
        {
            StartCoroutine(RunFullSequence());
        }

        private IEnumerator RunFullSequence()
        {
            // Test order: Central first (establishes fixation), then intact, then affected
            HemifieldMode[] sequence = {
                HemifieldMode.Central,
                HemifieldMode.RightHemifield,
                HemifieldMode.LeftHemifield
            };

            foreach (var hemifield in sequence)
            {
                UpdateInstructions($"Preparing to test: {GetHemifieldName(hemifield)}\n\n" +
                    "Press SPACE when ready.");

                // Wait a frame to clear any pending input
                yield return null;

                // Wait for user to release Space first (if held)
                yield return new WaitWhile(() => Input.GetKey(KeyCode.Space));

                // Now wait for new Space press
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

                yield return new WaitForSeconds(0.5f);

                StartTest(hemifield);

                // Wait for this hemifield test to complete
                yield return new WaitUntil(() => !testInProgress);

                yield return new WaitForSeconds(1f);
            }

            // All tests complete
            var results = GetResults();
            PlaySound(testCompleteSound);
            OnAllTestsComplete?.Invoke(results);

            DisplayFinalResults(results);
        }

        private void GenerateNewTriplet()
        {
            currentTriplet = new char[lettersPerTriplet];
            List<char> available = new List<char>(sloanLetters);

            for (int i = 0; i < lettersPerTriplet; i++)
            {
                int idx = UnityEngine.Random.Range(0, available.Count);
                currentTriplet[i] = available[idx];
                available.RemoveAt(idx); // No repeats within triplet
            }

            currentTripletIndex = 0;
            correctInTriplet = 0;
        }

        private IEnumerator ShowNextLetterWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ShowNextLetter();
        }

        private void ShowNextLetter()
        {
            if (!testInProgress) return;

            currentLetter = currentTriplet[currentTripletIndex];

            // Calculate letter color from LogCS
            float contrast = LogCSToContrast(currentLogCS);
            Color letterColor = GetLetterColor(contrast);

            // Position based on hemifield
            if (letterTransform != null)
            {
                letterTransform.localPosition = GetHemifieldPosition(currentHemifield);
            }

            // Display letter - BIG and CENTERED
            if (letterDisplay != null)
            {
                letterDisplay.text = currentLetter.ToString();
                letterDisplay.color = letterColor;
                letterDisplay.fontSize = 400; // Very large for visibility
                letterDisplay.alignment = TMPro.TextAlignmentOptions.Center;

                // Position letter based on hemifield and ensure it renders on top
                var rectTransform = letterDisplay.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                    // Apply hemifield offset for visual field testing
                    float xOffset = GetHemifieldUIOffset(currentHemifield);
                    rectTransform.anchoredPosition = new Vector2(xOffset, 0);

                    rectTransform.sizeDelta = new Vector2(500, 500);
                    rectTransform.SetAsLastSibling(); // Render on top of other UI
                }

                Debug.Log($"[ContrastTest] [{currentHemifield}] Letter: {currentLetter}, LogCS: {currentLogCS:F2}, Contrast: {contrast:P1}, Gray: {letterColor.r:F3}");
            }

            // Hide instructions completely during letter display
            if (instructionText != null)
            {
                instructionText.gameObject.SetActive(false);
            }

            // Show fixation cross during hemifield tests (user must keep eyes on center)
            if (fixationCross != null)
            {
                bool showFixation = (currentHemifield == HemifieldMode.LeftHemifield ||
                                     currentHemifield == HemifieldMode.RightHemifield);
                fixationCross.gameObject.SetActive(showFixation);

                if (showFixation)
                {
                    // Ensure fixation cross renders below the letter
                    fixationCross.transform.SetAsFirstSibling();
                }
            }

            awaitingResponse = true;
            letterOnsetTime = Time.time;

            // Auto-timeout if configured
            if (letterDisplayTime > 0)
            {
                StartCoroutine(LetterTimeout(letterDisplayTime));
            }
        }

        private IEnumerator LetterTimeout(float timeout)
        {
            yield return new WaitForSeconds(timeout);
            if (awaitingResponse)
            {
                OnUserResponse(' '); // No response = wrong
            }
        }

        /// <summary>
        /// Convert LogCS to Weber contrast (0-1 range)
        /// LogCS = log10(1/contrast_threshold)
        /// LogCS 0.0 = 100% contrast (black on gray)
        /// LogCS 1.0 = 10% contrast
        /// LogCS 2.0 = 1% contrast
        /// LogCS 2.25 = 0.56% contrast (near threshold for normal vision)
        /// </summary>
        private float LogCSToContrast(float logCS)
        {
            // Convert log contrast sensitivity to linear contrast
            // LogCS = log10(1/C) where C is the Weber contrast threshold
            // So C = 10^(-logCS)
            float contrast = Mathf.Pow(10f, -logCS);
            return Mathf.Clamp01(contrast);
        }

        /// <summary>
        /// Calculate letter luminance using Weber contrast formula.
        /// Weber contrast: C = (L_background - L_letter) / L_background
        /// Solving for L_letter: L_letter = L_background * (1 - C)
        /// </summary>
        private Color GetLetterColor(float contrast)
        {
            // Background is 50% gray (linear luminance 0.5 before gamma)
            float backgroundLinear = 0.5f;

            // Weber contrast formula: C = (Lb - Ll) / Lb
            // Solve for letter luminance: Ll = Lb * (1 - C)
            float letterLinear = backgroundLinear * (1f - contrast);

            // Clamp to valid range
            letterLinear = Mathf.Clamp01(letterLinear);

            // Apply gamma correction if enabled
            // To display a target linear luminance, we need to apply inverse gamma
            // displayValue = linearValue^(1/gamma)
            float letterValue;
            if (useGammaCorrection)
            {
                letterValue = Mathf.Pow(letterLinear, 1f / displayGamma);
            }
            else
            {
                letterValue = letterLinear;
            }

            Debug.Log($"[ContrastTest] LogCS: {currentLogCS:F2}, Contrast: {contrast:P1}, " +
                      $"Linear: {letterLinear:F4}, Display: {letterValue:F4}");

            // Return solid color (no alpha) - luminance-based contrast
            return new Color(letterValue, letterValue, letterValue, 1f);
        }

        private Vector3 GetHemifieldPosition(HemifieldMode hemifield)
        {
            float eccentricityRad = hemifieldEccentricity * Mathf.Deg2Rad;
            float offset = viewingDistanceMeters * Mathf.Tan(eccentricityRad);

            switch (hemifield)
            {
                case HemifieldMode.LeftHemifield:
                    return new Vector3(-offset, 0, 0);
                case HemifieldMode.RightHemifield:
                    return new Vector3(offset, 0, 0);
                case HemifieldMode.Central:
                case HemifieldMode.Binocular:
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// Get UI pixel offset for hemifield testing.
        /// Left hemifield = negative X (letter appears on left side of screen)
        /// Right hemifield = positive X (letter appears on right side of screen)
        /// User must keep eyes fixated on center while letters appear in peripheral vision.
        /// </summary>
        private float GetHemifieldUIOffset(HemifieldMode hemifield)
        {
            switch (hemifield)
            {
                case HemifieldMode.LeftHemifield:
                    return -hemifieldUIOffset; // Letter appears on LEFT side of screen
                case HemifieldMode.RightHemifield:
                    return hemifieldUIOffset;  // Letter appears on RIGHT side of screen
                case HemifieldMode.Central:
                case HemifieldMode.Binocular:
                default:
                    return 0f; // Letter appears at CENTER
            }
        }

        private float CalculateFontSize()
        {
            // 2° visual angle at 1m ≈ 35mm letter height
            // Convert to Unity font size based on canvas settings
            float letterHeightMeters = 2f * viewingDistanceMeters *
                Mathf.Tan(letterSizeDegrees * 0.5f * Mathf.Deg2Rad);

            // Assuming world-space canvas at 100 units per meter
            return letterHeightMeters * 100f * 2f;
        }

        /// <summary>
        /// Called when user responds (from voice recognition or controller)
        /// </summary>
        public void OnUserResponse(char responseLetter)
        {
            if (!awaitingResponse) return;
            awaitingResponse = false;

            float reactionTime = (Time.time - letterOnsetTime) * 1000f;
            bool correct = (char.ToUpper(responseLetter) == currentLetter);

            Debug.Log($"[ContrastTest] Response: '{responseLetter}' vs Expected: '{currentLetter}' = {(correct ? "CORRECT" : "WRONG")} (triplet {currentTripletIndex+1}/{lettersPerTriplet}, score {correctInTriplet + (correct ? 1 : 0)}/{currentTripletIndex+1})");

            // Record trial
            var trial = new TrialResult
            {
                logCS = currentLogCS,
                presentedLetter = currentLetter,
                responseLetter = char.ToUpper(responseLetter),
                correct = correct,
                reactionTimeMs = reactionTime,
                hemifield = currentHemifield,
                timestamp = DateTime.Now,
                tripletNumber = (int)(currentLogCS / logCSStep),
                letterInTriplet = currentTripletIndex
            };
            trialHistory.Add(trial);
            OnTrialComplete?.Invoke(trial);

            // Feedback
            if (correct)
            {
                correctInTriplet++;
                PlaySound(correctSound);
            }
            else
            {
                PlaySound(incorrectSound);
            }

            // Clear letter briefly
            if (letterDisplay != null)
                letterDisplay.text = "";

            // Advance
            currentTripletIndex++;

            if (currentTripletIndex >= lettersPerTriplet)
            {
                EvaluateTriplet();
            }
            else
            {
                StartCoroutine(ShowNextLetterWithDelay(interLetterDelay));
            }
        }

        private void EvaluateTriplet()
        {
            Debug.Log($"[ContrastTest] Evaluating triplet: {correctInTriplet}/{lettersPerTriplet} correct, need {correctToPass} to pass");

            if (correctInTriplet >= correctToPass)
            {
                // Passed - increase difficulty
                PlaySound(levelCompleteSound);
                currentLogCS += logCSStep;

                if (currentLogCS > maxLogCS)
                {
                    EndTest(maxLogCS);
                }
                else
                {
                    GenerateNewTriplet();
                    StartCoroutine(ShowNextLetterWithDelay(interLetterDelay * 2));
                }
            }
            else
            {
                // Failed - test complete at previous level
                float finalScore = Mathf.Max(0, currentLogCS - logCSStep);
                EndTest(finalScore);
            }
        }

        private void EndTest(float finalLogCS)
        {
            testInProgress = false;
            hemifieldResults[currentHemifield] = finalLogCS;

            if (letterDisplay != null)
                letterDisplay.text = "";

            string interpretation = GetScoreInterpretation(finalLogCS);

            UpdateInstructions($"{GetHemifieldName(currentHemifield)} Complete\n\n" +
                $"Contrast Sensitivity: {finalLogCS:F2} LogCS\n" +
                $"{interpretation}\n\n" +
                "Press TRIGGER to continue.");

            Debug.Log($"[ContrastTest] {currentHemifield}: {finalLogCS:F2} LogCS");
            OnHemifieldTestComplete?.Invoke(currentHemifield, finalLogCS);
        }

        private string GetScoreInterpretation(float logCS)
        {
            if (logCS >= 1.80f) return "Normal range";
            if (logCS >= 1.50f) return "Mild reduction";
            if (logCS >= 1.20f) return "Moderate reduction";
            if (logCS >= 0.80f) return "Significant reduction";
            return "Severe reduction";
        }

        private string GetHemifieldName(HemifieldMode hemifield)
        {
            switch (hemifield)
            {
                case HemifieldMode.Central: return "Central Vision";
                case HemifieldMode.LeftHemifield: return "Left Hemifield (Affected)";
                case HemifieldMode.RightHemifield: return "Right Hemifield (Intact)";
                case HemifieldMode.Binocular: return "Binocular";
                default: return hemifield.ToString();
            }
        }

        private void DisplayFinalResults(ContrastSensitivityResults results)
        {
            UpdateInstructions(results.GetSummary());
        }

        private void UpdateInstructions(string text)
        {
            if (instructionText != null)
            {
                instructionText.gameObject.SetActive(true); // Re-enable if hidden
                instructionText.text = text;
            }
            // Clear letter display when showing instructions
            if (letterDisplay != null)
            {
                letterDisplay.text = "";
            }
        }

        private void UpdateStatus(string text)
        {
            if (statusText != null)
                statusText.text = text;
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Get comprehensive results for all tested hemifields
        /// </summary>
        public ContrastSensitivityResults GetResults()
        {
            return new ContrastSensitivityResults
            {
                centralLogCS = hemifieldResults.ContainsKey(HemifieldMode.Central) ?
                    hemifieldResults[HemifieldMode.Central] : -1f,
                leftHemifieldLogCS = hemifieldResults.ContainsKey(HemifieldMode.LeftHemifield) ?
                    hemifieldResults[HemifieldMode.LeftHemifield] : -1f,
                rightHemifieldLogCS = hemifieldResults.ContainsKey(HemifieldMode.RightHemifield) ?
                    hemifieldResults[HemifieldMode.RightHemifield] : -1f,
                asymmetry = CalculateAsymmetry(),
                trialHistory = new List<TrialResult>(trialHistory),
                testDate = DateTime.Now
            };
        }

        private float CalculateAsymmetry()
        {
            if (hemifieldResults.ContainsKey(HemifieldMode.LeftHemifield) &&
                hemifieldResults.ContainsKey(HemifieldMode.RightHemifield))
            {
                // Positive asymmetry = left deficit (expected in left hemianopia)
                return hemifieldResults[HemifieldMode.RightHemifield] -
                       hemifieldResults[HemifieldMode.LeftHemifield];
            }
            return 0f;
        }

        // Public accessors
        public bool IsTestInProgress => testInProgress;
        public bool IsAwaitingResponse => awaitingResponse;
        public float CurrentLogCS => currentLogCS;
        public HemifieldMode CurrentHemifield => currentHemifield;

        #region Calibration & Debug Methods

        /// <summary>
        /// Preview a specific LogCS level (for calibration)
        /// </summary>
        public void PreviewContrastLevel(float logCS)
        {
            float contrast = LogCSToContrast(logCS);
            Color letterColor = GetLetterColor(contrast);

            if (letterDisplay != null)
            {
                letterDisplay.text = "H"; // Standard test letter
                letterDisplay.color = letterColor;
            }

            UpdateStatus($"Preview: LogCS {logCS:F2} = {contrast:P1} contrast");
            Debug.Log($"[Calibration] LogCS {logCS:F2}: Contrast={contrast:P2}, RGB=({letterColor.r:F3}, {letterColor.g:F3}, {letterColor.b:F3})");
        }

        /// <summary>
        /// Show calibration strip with all standard Pelli-Robson levels
        /// Call this to verify your display is properly calibrated
        /// </summary>
        public void ShowCalibrationInfo()
        {
            string info = "Pelli-Robson Contrast Levels:\n";
            info += "══════════════════════════════════════\n";
            info += "LogCS  | Contrast | Letter Gray Value\n";
            info += "──────────────────────────────────────\n";

            float[] standardLevels = { 0.0f, 0.15f, 0.30f, 0.45f, 0.60f, 0.75f, 0.90f, 1.05f, 1.20f, 1.35f, 1.50f, 1.65f, 1.80f, 1.95f, 2.10f, 2.25f };

            foreach (float logCS in standardLevels)
            {
                float contrast = LogCSToContrast(logCS);
                float letterLinear = 0.5f * (1f - contrast);
                float displayValue = useGammaCorrection ? Mathf.Pow(letterLinear, 1f / displayGamma) : letterLinear;

                info += $"{logCS:F2}   | {contrast,7:P1} | {displayValue:F3} ({(int)(displayValue * 255)})\n";
            }

            info += "══════════════════════════════════════\n";
            info += $"Gamma correction: {(useGammaCorrection ? $"ON (γ={displayGamma})" : "OFF")}\n";
            info += "Normal threshold: 1.65-1.95 LogCS";

            Debug.Log(info);
            UpdateInstructions(info);
        }

        /// <summary>
        /// Cycle through contrast levels for visual verification (press keys 0-9)
        /// </summary>
        public void StartCalibrationMode()
        {
            testInProgress = false;
            StartCoroutine(CalibrationModeLoop());
        }

        private IEnumerator CalibrationModeLoop()
        {
            UpdateInstructions("CALIBRATION MODE\n\n" +
                "Press UP/DOWN arrows to adjust contrast\n" +
                "Press G to toggle gamma correction\n" +
                "Press C to show all levels\n" +
                "Press ESCAPE to exit\n\n" +
                "Verify letters disappear at ~1.8-2.0 LogCS for normal vision");

            float previewLogCS = 0f;
            bool calibrating = true;

            while (calibrating)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    previewLogCS = Mathf.Min(previewLogCS + 0.15f, maxLogCS);
                    PreviewContrastLevel(previewLogCS);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    previewLogCS = Mathf.Max(previewLogCS - 0.15f, 0f);
                    PreviewContrastLevel(previewLogCS);
                }
                else if (Input.GetKeyDown(KeyCode.G))
                {
                    useGammaCorrection = !useGammaCorrection;
                    PreviewContrastLevel(previewLogCS);
                    Debug.Log($"[Calibration] Gamma correction: {(useGammaCorrection ? "ON" : "OFF")}");
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    ShowCalibrationInfo();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    calibrating = false;
                }

                yield return null;
            }

            InitializeTest();
        }

        /// <summary>
        /// Get expected contrast values for a specific LogCS
        /// Useful for debugging and verification
        /// </summary>
        public (float weberContrast, float letterLuminance, float displayGray) GetContrastDetails(float logCS)
        {
            float contrast = LogCSToContrast(logCS);
            float letterLinear = 0.5f * (1f - contrast);
            float displayGray = useGammaCorrection ? Mathf.Pow(letterLinear, 1f / displayGamma) : letterLinear;
            return (contrast, letterLinear, displayGray);
        }

        #endregion
    }

    /// <summary>
    /// Container for contrast sensitivity test results
    /// </summary>
    [Serializable]
    public class ContrastSensitivityResults
    {
        public float centralLogCS;
        public float leftHemifieldLogCS;
        public float rightHemifieldLogCS;
        public float asymmetry;
        public List<ContrastSensitivityTest.TrialResult> trialHistory;
        public DateTime testDate;

        public string GetSummary()
        {
            string summary = $"Contrast Sensitivity Results\n" +
                $"Date: {testDate:yyyy-MM-dd HH:mm}\n" +
                $"═══════════════════════════════\n";

            if (centralLogCS >= 0)
                summary += $"Central:         {centralLogCS:F2} LogCS\n";
            if (rightHemifieldLogCS >= 0)
                summary += $"Right (intact):  {rightHemifieldLogCS:F2} LogCS\n";
            if (leftHemifieldLogCS >= 0)
                summary += $"Left (affected): {leftHemifieldLogCS:F2} LogCS\n";
            if (asymmetry != 0)
                summary += $"Asymmetry:       {asymmetry:F2} LogCS\n";

            summary += $"═══════════════════════════════\n" +
                $"Normal range (age 50-60): 1.65-1.95\n" +
                $"Significant change: ≥0.30 LogCS";

            return summary;
        }

        /// <summary>
        /// Check if results show clinically significant asymmetry
        /// </summary>
        public bool HasSignificantAsymmetry()
        {
            return asymmetry >= 0.30f;
        }

        /// <summary>
        /// Get average reaction time for correct responses
        /// </summary>
        public float GetAverageReactionTime()
        {
            float sum = 0f;
            int count = 0;
            foreach (var trial in trialHistory)
            {
                if (trial.correct)
                {
                    sum += trial.reactionTimeMs;
                    count++;
                }
            }
            return count > 0 ? sum / count : 0f;
        }
    }
}
