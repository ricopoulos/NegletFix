using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

namespace NeglectFix.Assessment
{
    /// <summary>
    /// Displays contrast sensitivity test results and progress history.
    /// Shows current results, historical trend, and comparison to baseline.
    /// </summary>
    public class ContrastResultsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ContrastSensitivityTest contrastTest;
        [SerializeField] private NeglectFix.Utils.DataLogger dataLogger;

        [Header("UI Elements - Current Results")]
        [SerializeField] private TextMeshProUGUI summaryText;
        [SerializeField] private TextMeshProUGUI centralScoreText;
        [SerializeField] private TextMeshProUGUI leftScoreText;
        [SerializeField] private TextMeshProUGUI rightScoreText;
        [SerializeField] private TextMeshProUGUI asymmetryText;

        [Header("UI Elements - Visual Bars")]
        [SerializeField] private Image centralBar;
        [SerializeField] private Image leftBar;
        [SerializeField] private Image rightBar;
        [SerializeField] private float maxBarLogCS = 2.25f;

        [Header("UI Elements - History")]
        [SerializeField] private TextMeshProUGUI historyText;
        [SerializeField] private GameObject historyPanel;

        [Header("UI Elements - Interpretation")]
        [SerializeField] private TextMeshProUGUI interpretationText;
        [SerializeField] private Image interpretationBackground;

        [Header("Colors")]
        [SerializeField] private Color normalColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color mildColor = new Color(0.8f, 0.8f, 0.2f);
        [SerializeField] private Color moderateColor = new Color(0.8f, 0.5f, 0.2f);
        [SerializeField] private Color severeColor = new Color(0.8f, 0.2f, 0.2f);

        [Header("Settings")]
        [SerializeField] private bool autoSaveResults = true;
        [SerializeField] private bool showHistoryOnComplete = true;

        // Cached results
        private ContrastSensitivityResults currentResults;
        private List<ContrastSensitivityResults> historyCache = new List<ContrastSensitivityResults>();

        void Start()
        {
            if (contrastTest == null)
                contrastTest = FindObjectOfType<ContrastSensitivityTest>();

            if (dataLogger == null)
                dataLogger = FindObjectOfType<NeglectFix.Utils.DataLogger>();

            // Subscribe to test completion
            if (contrastTest != null)
            {
                contrastTest.OnAllTestsComplete += OnTestComplete;
                contrastTest.OnHemifieldTestComplete += OnHemifieldComplete;
            }

            // Hide panels initially
            if (historyPanel != null)
                historyPanel.SetActive(false);
        }

        private void OnHemifieldComplete(ContrastSensitivityTest.HemifieldMode hemifield, float logCS)
        {
            // Update individual score display as each hemifield completes
            UpdateSingleScore(hemifield, logCS);
        }

        private void OnTestComplete(ContrastSensitivityResults results)
        {
            currentResults = results;

            // Display full results
            DisplayResults(results);

            // Save to file
            if (autoSaveResults && dataLogger != null)
            {
                dataLogger.LogContrastSensitivityResults(results);
            }

            // Show history if enabled
            if (showHistoryOnComplete && historyPanel != null)
            {
                LoadAndDisplayHistory();
                historyPanel.SetActive(true);
            }
        }

        public void DisplayResults(ContrastSensitivityResults results)
        {
            // Summary text
            if (summaryText != null)
            {
                summaryText.text = results.GetSummary();
            }

            // Individual scores
            UpdateScoreDisplay(centralScoreText, centralBar, results.centralLogCS, "Central");
            UpdateScoreDisplay(leftScoreText, leftBar, results.leftHemifieldLogCS, "Left (Affected)");
            UpdateScoreDisplay(rightScoreText, rightBar, results.rightHemifieldLogCS, "Right (Intact)");

            // Asymmetry
            if (asymmetryText != null)
            {
                string asymSign = results.asymmetry >= 0 ? "+" : "";
                asymmetryText.text = $"Asymmetry: {asymSign}{results.asymmetry:F2} LogCS";
                asymmetryText.color = GetAsymmetryColor(results.asymmetry);
            }

            // Interpretation
            DisplayInterpretation(results);
        }

        private void UpdateSingleScore(ContrastSensitivityTest.HemifieldMode hemifield, float logCS)
        {
            switch (hemifield)
            {
                case ContrastSensitivityTest.HemifieldMode.Central:
                    UpdateScoreDisplay(centralScoreText, centralBar, logCS, "Central");
                    break;
                case ContrastSensitivityTest.HemifieldMode.LeftHemifield:
                    UpdateScoreDisplay(leftScoreText, leftBar, logCS, "Left");
                    break;
                case ContrastSensitivityTest.HemifieldMode.RightHemifield:
                    UpdateScoreDisplay(rightScoreText, rightBar, logCS, "Right");
                    break;
            }
        }

        private void UpdateScoreDisplay(TextMeshProUGUI text, Image bar, float logCS, string label)
        {
            if (logCS < 0) return; // Not tested

            if (text != null)
            {
                text.text = $"{label}: {logCS:F2} LogCS";
                text.color = GetScoreColor(logCS);
            }

            if (bar != null)
            {
                float fillAmount = Mathf.Clamp01(logCS / maxBarLogCS);
                bar.fillAmount = fillAmount;
                bar.color = GetScoreColor(logCS);
            }
        }

        private Color GetScoreColor(float logCS)
        {
            if (logCS >= 1.65f) return normalColor;
            if (logCS >= 1.35f) return mildColor;
            if (logCS >= 1.00f) return moderateColor;
            return severeColor;
        }

        private Color GetAsymmetryColor(float asymmetry)
        {
            float absAsym = Mathf.Abs(asymmetry);
            if (absAsym < 0.15f) return normalColor;
            if (absAsym < 0.30f) return mildColor;
            if (absAsym < 0.60f) return moderateColor;
            return severeColor;
        }

        private void DisplayInterpretation(ContrastSensitivityResults results)
        {
            if (interpretationText == null) return;

            string interpretation = "";
            Color bgColor = normalColor;

            // Check for significant asymmetry
            if (results.HasSignificantAsymmetry())
            {
                interpretation = "Significant hemifield asymmetry detected.\n";
                interpretation += "Left visual field shows reduced contrast sensitivity.\n";
                interpretation += "This is consistent with left hemianopia.";
                bgColor = moderateColor;
            }
            else if (results.leftHemifieldLogCS >= 0 && results.leftHemifieldLogCS < 1.50f)
            {
                interpretation = "Left hemifield shows moderate to severe reduction.\n";
                interpretation += "Audiovisual training may help improve detection.";
                bgColor = moderateColor;
            }
            else if (results.centralLogCS >= 1.65f && results.rightHemifieldLogCS >= 1.65f)
            {
                interpretation = "Central and intact hemifield within normal range.\n";
                interpretation += "Good baseline for tracking rehabilitation progress.";
                bgColor = normalColor;
            }
            else
            {
                interpretation = "Results recorded.\n";
                interpretation += "Compare with future assessments to track progress.";
                bgColor = mildColor;
            }

            // Add change from baseline if available
            if (historyCache.Count > 0)
            {
                var baseline = historyCache[0];
                float leftChange = results.leftHemifieldLogCS - baseline.leftHemifieldLogCS;

                if (Mathf.Abs(leftChange) >= 0.30f)
                {
                    interpretation += $"\n\nChange from baseline: {(leftChange > 0 ? "+" : "")}{leftChange:F2} LogCS";
                    if (leftChange > 0)
                    {
                        interpretation += " (Improvement!)";
                        bgColor = normalColor;
                    }
                }
            }

            interpretationText.text = interpretation;

            if (interpretationBackground != null)
            {
                bgColor.a = 0.3f;
                interpretationBackground.color = bgColor;
            }
        }

        private void LoadAndDisplayHistory()
        {
            if (dataLogger == null) return;

            string summaryPath = Path.Combine(dataLogger.GetAssessmentsPath(), "contrast_summary.csv");

            if (!File.Exists(summaryPath))
            {
                if (historyText != null)
                    historyText.text = "No previous assessments found.";
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(summaryPath);
                string historyDisplay = "Assessment History\n══════════════════\n";

                // Skip header, show last 5 entries
                int startLine = Mathf.Max(1, lines.Length - 5);
                for (int i = startLine; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(',');
                    if (parts.Length >= 6)
                    {
                        string date = parts[0];
                        string left = parts[3];
                        string right = parts[4];
                        string asym = parts[5];

                        historyDisplay += $"{date}: L={left} R={right} (Asym={asym})\n";
                    }
                }

                if (historyText != null)
                    historyText.text = historyDisplay;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ContrastResultsUI] Failed to load history: {e.Message}");
                if (historyText != null)
                    historyText.text = "Error loading history.";
            }
        }

        /// <summary>
        /// Manually trigger results display (for testing)
        /// </summary>
        public void ShowCurrentResults()
        {
            if (contrastTest != null)
            {
                var results = contrastTest.GetResults();
                DisplayResults(results);
            }
        }

        /// <summary>
        /// Toggle history panel visibility
        /// </summary>
        public void ToggleHistory()
        {
            if (historyPanel != null)
            {
                bool newState = !historyPanel.activeSelf;
                historyPanel.SetActive(newState);

                if (newState)
                {
                    LoadAndDisplayHistory();
                }
            }
        }

        /// <summary>
        /// Get comparison text between current and baseline
        /// </summary>
        public string GetBaselineComparison()
        {
            if (currentResults == null || historyCache.Count == 0)
                return "No baseline available for comparison.";

            var baseline = historyCache[0];

            float centralChange = currentResults.centralLogCS - baseline.centralLogCS;
            float leftChange = currentResults.leftHemifieldLogCS - baseline.leftHemifieldLogCS;
            float rightChange = currentResults.rightHemifieldLogCS - baseline.rightHemifieldLogCS;
            float asymChange = currentResults.asymmetry - baseline.asymmetry;

            return $"Change from Baseline ({baseline.testDate:yyyy-MM-dd})\n" +
                $"Central:  {FormatChange(centralChange)}\n" +
                $"Left:     {FormatChange(leftChange)}\n" +
                $"Right:    {FormatChange(rightChange)}\n" +
                $"Asymmetry: {FormatChange(-asymChange)} (negative = improvement)";
        }

        private string FormatChange(float change)
        {
            string sign = change >= 0 ? "+" : "";
            string significance = Mathf.Abs(change) >= 0.30f ? " *" : "";
            return $"{sign}{change:F2} LogCS{significance}";
        }

        void OnDestroy()
        {
            if (contrastTest != null)
            {
                contrastTest.OnAllTestsComplete -= OnTestComplete;
                contrastTest.OnHemifieldTestComplete -= OnHemifieldComplete;
            }
        }
    }
}
