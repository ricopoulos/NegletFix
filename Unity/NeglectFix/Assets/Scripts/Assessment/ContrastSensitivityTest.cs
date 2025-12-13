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

        [Header("Pelli-Robson Parameters")]
        [Tooltip("Starting contrast (0.0 = 100% contrast, higher = harder)")]
        [SerializeField] private float startingLogCS = 0.5f; // Start at 31% contrast (easier to see)

        [Tooltip("Contrast step size (standard = 0.15)")]
        [SerializeField] private float logCSStep = 0.15f;

        [Tooltip("Maximum testable LogCS (2.0 = 1% contrast, very hard)")]
        [SerializeField] private float maxLogCS = 2.0f;

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
            InitializeTest();
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

            UpdateInstructions("Contrast Sensitivity Test\n\n" +
                "You will see letters at decreasing contrast.\n" +
                "Say or select the letter you see.\n" +
                "Press B if you cannot see the letter.\n\n" +
                "Press TRIGGER to begin.");
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

            // Display letter
            if (letterDisplay != null)
            {
                letterDisplay.text = currentLetter.ToString();
                letterDisplay.color = letterColor; // Use calculated contrast color
                letterDisplay.fontSize = 300; // Large font for visibility

                // Center the letter display
                var rectTransform = letterDisplay.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(0, 100); // Center-ish, slightly above middle
                }

                Debug.Log($"[ContrastTest] Showing letter: {currentLetter} at LogCS {currentLogCS:F2}, contrast: {contrast:F3}");
            }

            // Move instructions below the letter
            if (instructionText != null)
            {
                instructionText.text = $"Type the letter you see (or Backspace if you can't)";

                var rectTransform = instructionText.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(0, -200); // Below center
                }
            }

            // Update status
            UpdateStatus($"LogCS: {currentLogCS:F2} | Letter {currentTripletIndex + 1}/{lettersPerTriplet}");

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
        /// LogCS = log10(1/threshold)
        /// LogCS 0.0 = 100% contrast (black on gray)
        /// LogCS 2.25 = 0.56% contrast (barely visible)
        /// </summary>
        private float LogCSToContrast(float logCS)
        {
            // More aggressive contrast reduction for testing
            // LogCS 0.0 = 100% contrast
            // LogCS 1.0 = 10% contrast
            // LogCS 2.0 = 1% contrast
            float threshold = Mathf.Pow(10f, -logCS);
            return Mathf.Clamp01(threshold); // Changed: threshold IS the contrast now
        }

        private Color GetLetterColor(float contrast)
        {
            // Use alpha channel for contrast instead of luminance
            // This works regardless of background color
            // contrast = 1.0 → fully opaque (black)
            // contrast = 0.0 → fully transparent (invisible)

            float alpha = Mathf.Clamp01(contrast * 3f); // Multiply by 3 to make low contrast disappear faster

            Debug.Log($"[ContrastTest] Contrast: {contrast:F4}, Alpha: {alpha:F4}");

            return new Color(0f, 0f, 0f, alpha); // Black letter with varying transparency
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
                instructionText.text = text;
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
