# NeglectFix Contrast Sensitivity Module
## Baseline Assessment & Calibration System

### Overview

This module provides self-administered contrast sensitivity testing to:
1. Establish baseline measurements before rehabilitation
2. Track progress during the 20-session audiovisual training protocol
3. Personalize stimulus parameters based on individual contrast thresholds

---

## Integration with Zenni Vision VR

### Recommended Calibration Workflow

```
┌─────────────────────────────────────────────────────────────────┐
│                    PRE-TRAINING CALIBRATION                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  1. ZENNI VISION VR (5 min)                                    │
│     ├── Visual acuity: each eye + binocular                    │
│     ├── Landolt C at multiple sizes                            │
│     └── Output: Acuity score (e.g., 20/50)                     │
│                                                                 │
│  2. NEGLECTFIX CONTRAST MODULE (10-15 min)                     │
│     ├── Pelli-Robson style letter recognition                  │
│     ├── Spatial frequency sweep (optional)                     │
│     ├── Hemifield comparison (affected vs intact)              │
│     └── Output: LogCS score + hemifield asymmetry              │
│                                                                 │
│  3. NEGLECTFIX VISUAL FIELD QUICK-CHECK (5 min)                │
│     ├── Detection at 8°, 24°, 40°, 56° eccentricities          │
│     ├── Maps to Humphrey 24-2 positions                        │
│     └── Output: Detection thresholds per location              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│              PERSONALIZED TRAINING PARAMETERS                   │
├─────────────────────────────────────────────────────────────────┤
│  • Starting contrast level for AV stimuli                      │
│  • Brightness enhancement settings                              │
│  • Eccentricity progression based on detection map              │
│  • EEG engagement thresholds adjusted for baseline              │
└─────────────────────────────────────────────────────────────────┘
```

### Why Zenni First?

Your prescription from the eye care records (Reina Eye Care, 11/29/2021) may be outdated:
- **Documented VA:** 20/50 both eyes with spectacles
- **Current status:** 4+ years post-measurement

Zenni Vision VR can verify if your current correction is adequate before contrast testing, since uncorrected refractive error artificially reduces contrast sensitivity.

---

## Contrast Sensitivity Test Design

### Test Type: Modified Pelli-Robson

The Pelli-Robson chart is the clinical gold standard for peak contrast sensitivity. We implement a VR version with advantages:
- Controlled luminance (Quest OLED: ~100 cd/m² max)
- Consistent viewing distance (virtual 1m)
- Randomized letter sequences (prevents memorization)
- Separate hemifield testing (critical for hemianopia)

### Parameters

| Parameter | Value | Rationale |
|-----------|-------|-----------|
| Letter size | 2° visual angle | Pelli-Robson standard at 1m |
| Background | 50% gray (RGB 128,128,128) | ~50 cd/m² |
| Contrast steps | 0.15 log units | Standard PR increment |
| Contrast range | 0.00 to 2.25 LogCS | Full clinical range |
| Letters per level | 3 (triplet) | 2/3 correct = pass |
| Font | Sloan letters (C D H K N O R S V Z) | Standardized optotypes |

### Hemifield-Specific Testing

For hemianopia assessment, we present stimuli in:
- **Central** (0°): Fixation verification
- **Intact hemifield** (right): Baseline comparison  
- **Affected hemifield** (left): Rehabilitation target

This reveals the **contrast sensitivity asymmetry** between hemifields.

---

## Unity Implementation

### Core Script: ContrastSensitivityTest.cs

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ContrastSensitivityTest : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private TestMode testMode = TestMode.PelliRobson;
    [SerializeField] private HemifieldMode hemifieldMode = HemifieldMode.Central;
    
    [Header("Display Settings")]
    [SerializeField] private float letterSizeDegrees = 2.0f;
    [SerializeField] private float viewingDistanceMeters = 1.0f;
    [SerializeField] private Color backgroundColor = new Color(0.5f, 0.5f, 0.5f); // 50% gray
    
    [Header("Pelli-Robson Parameters")]
    [SerializeField] private float startingLogCS = 0.0f;
    [SerializeField] private float logCSStep = 0.15f;
    [SerializeField] private float maxLogCS = 2.25f;
    [SerializeField] private int lettersPerTriplet = 3;
    [SerializeField] private int correctToPass = 2;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI letterDisplay;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Image backgroundPanel;
    
    [Header("Audio Feedback")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip levelCompleteSound;
    
    // Sloan letters - designed for equal legibility
    private readonly char[] sloanLetters = { 'C', 'D', 'H', 'K', 'N', 'O', 'R', 'S', 'V', 'Z' };
    
    // Test state
    private float currentLogCS;
    private int currentTripletIndex;
    private int correctInTriplet;
    private char currentLetter;
    private char[] currentTriplet;
    private bool testInProgress;
    private bool awaitingResponse;
    
    // Results
    private Dictionary<HemifieldMode, float> hemifieldResults = new Dictionary<HemifieldMode, float>();
    private List<TrialResult> trialHistory = new List<TrialResult>();
    
    public enum TestMode
    {
        PelliRobson,        // Standard contrast sensitivity
        SpatialFrequency,   // CSF curve (advanced)
        QuickScreen         // Fast 3-level check
    }
    
    public enum HemifieldMode
    {
        Central,            // Fixation point
        LeftHemifield,      // Affected (blind) field
        RightHemifield,     // Intact field
        Binocular           // Both eyes, central
    }
    
    [System.Serializable]
    public class TrialResult
    {
        public float logCS;
        public char presentedLetter;
        public char responseLetter;
        public bool correct;
        public float reactionTimeMs;
        public HemifieldMode hemifield;
        public System.DateTime timestamp;
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
        
        backgroundPanel.color = backgroundColor;
        letterDisplay.text = "";
        
        instructionText.text = "Contrast Sensitivity Test\n\n" +
            "You will see letters at decreasing contrast.\n" +
            "Say the letter you see, or press the matching button.\n\n" +
            "Press TRIGGER to begin.";
    }
    
    public void StartTest(HemifieldMode hemifield)
    {
        hemifieldMode = hemifield;
        currentLogCS = startingLogCS;
        currentTripletIndex = 0;
        correctInTriplet = 0;
        testInProgress = true;
        
        GenerateNewTriplet();
        ShowNextLetter();
    }
    
    private void GenerateNewTriplet()
    {
        currentTriplet = new char[lettersPerTriplet];
        List<char> available = new List<char>(sloanLetters);
        
        for (int i = 0; i < lettersPerTriplet; i++)
        {
            int idx = Random.Range(0, available.Count);
            currentTriplet[i] = available[idx];
            available.RemoveAt(idx); // No repeats in triplet
        }
        
        currentTripletIndex = 0;
        correctInTriplet = 0;
    }
    
    private void ShowNextLetter()
    {
        if (!testInProgress) return;
        
        currentLetter = currentTriplet[currentTripletIndex];
        
        // Calculate letter color from LogCS
        float contrast = LogCSToContrast(currentLogCS);
        Color letterColor = GetLetterColor(contrast);
        
        // Position based on hemifield
        Vector3 position = GetHemifieldPosition(hemifieldMode);
        letterDisplay.transform.localPosition = position;
        
        // Display
        letterDisplay.text = currentLetter.ToString();
        letterDisplay.color = letterColor;
        letterDisplay.fontSize = CalculateFontSize();
        
        awaitingResponse = true;
        
        // Update instruction
        instructionText.text = $"LogCS: {currentLogCS:F2} | Triplet: {currentTripletIndex + 1}/3\n" +
            $"Hemifield: {hemifieldMode}";
    }
    
    /// <summary>
    /// Convert LogCS to Weber contrast (0-1 range)
    /// LogCS = log10(1/threshold), so threshold = 10^(-LogCS)
    /// </summary>
    private float LogCSToContrast(float logCS)
    {
        // Pelli-Robson: LogCS 0.0 = 100% contrast, LogCS 2.25 = 0.56% contrast
        float threshold = Mathf.Pow(10f, -logCS);
        float contrast = 1f - threshold; // Invert for letter darkness
        return Mathf.Clamp01(contrast);
    }
    
    private Color GetLetterColor(float contrast)
    {
        // Letters darker than background
        // contrast = 1.0 means black letters (0,0,0)
        // contrast = 0.0 means same as background (0.5, 0.5, 0.5)
        float luminance = backgroundColor.r * (1f - contrast);
        return new Color(luminance, luminance, luminance, 1f);
    }
    
    private Vector3 GetHemifieldPosition(HemifieldMode hemifield)
    {
        float eccentricity = 10f; // degrees from center
        float distance = viewingDistanceMeters;
        
        switch (hemifield)
        {
            case HemifieldMode.LeftHemifield:
                // Present in LEFT visual field (affected)
                float leftAngle = -eccentricity * Mathf.Deg2Rad;
                return new Vector3(distance * Mathf.Tan(leftAngle), 0, 0);
                
            case HemifieldMode.RightHemifield:
                // Present in RIGHT visual field (intact)
                float rightAngle = eccentricity * Mathf.Deg2Rad;
                return new Vector3(distance * Mathf.Tan(rightAngle), 0, 0);
                
            case HemifieldMode.Central:
            case HemifieldMode.Binocular:
            default:
                return Vector3.zero;
        }
    }
    
    private float CalculateFontSize()
    {
        // 2° visual angle at 1m = ~35mm letter height
        // Convert to Unity units based on canvas scaling
        float letterHeightMeters = 2f * viewingDistanceMeters * Mathf.Tan(letterSizeDegrees * 0.5f * Mathf.Deg2Rad);
        // Assuming 100 units per meter in canvas
        return letterHeightMeters * 100f * 2f; // Approximate font size
    }
    
    /// <summary>
    /// Called when user responds (voice recognition or button press)
    /// </summary>
    public void OnUserResponse(char responseLetter)
    {
        if (!awaitingResponse) return;
        awaitingResponse = false;
        
        bool correct = (char.ToUpper(responseLetter) == currentLetter);
        
        // Record trial
        trialHistory.Add(new TrialResult
        {
            logCS = currentLogCS,
            presentedLetter = currentLetter,
            responseLetter = responseLetter,
            correct = correct,
            hemifield = hemifieldMode,
            timestamp = System.DateTime.Now
        });
        
        // Play feedback
        if (correct)
        {
            correctInTriplet++;
            PlaySound(correctSound);
        }
        else
        {
            PlaySound(incorrectSound);
        }
        
        // Advance
        currentTripletIndex++;
        
        if (currentTripletIndex >= lettersPerTriplet)
        {
            // Triplet complete - evaluate
            EvaluateTriplet();
        }
        else
        {
            // Next letter in triplet
            ShowNextLetter();
        }
    }
    
    private void EvaluateTriplet()
    {
        if (correctInTriplet >= correctToPass)
        {
            // Passed this level - increase difficulty
            PlaySound(levelCompleteSound);
            currentLogCS += logCSStep;
            
            if (currentLogCS > maxLogCS)
            {
                // Maximum contrast sensitivity reached
                EndTest(maxLogCS);
            }
            else
            {
                // Continue to next level
                GenerateNewTriplet();
                ShowNextLetter();
            }
        }
        else
        {
            // Failed this level - test complete
            // Score is the LAST PASSED level
            float finalScore = currentLogCS - logCSStep;
            EndTest(Mathf.Max(0, finalScore));
        }
    }
    
    private void EndTest(float finalLogCS)
    {
        testInProgress = false;
        hemifieldResults[hemifieldMode] = finalLogCS;
        
        letterDisplay.text = "";
        instructionText.text = $"Test Complete!\n\n" +
            $"Contrast Sensitivity: {finalLogCS:F2} LogCS\n" +
            $"Hemifield: {hemifieldMode}\n\n" +
            "Press TRIGGER to test next hemifield\n" +
            "or MENU to view results.";
        
        Debug.Log($"[ContrastSensitivityTest] {hemifieldMode} complete: {finalLogCS:F2} LogCS");
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
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
            testDate = System.DateTime.Now
        };
    }
    
    private float CalculateAsymmetry()
    {
        if (hemifieldResults.ContainsKey(HemifieldMode.LeftHemifield) && 
            hemifieldResults.ContainsKey(HemifieldMode.RightHemifield))
        {
            return hemifieldResults[HemifieldMode.RightHemifield] - 
                   hemifieldResults[HemifieldMode.LeftHemifield];
        }
        return 0f;
    }
}

[System.Serializable]
public class ContrastSensitivityResults
{
    public float centralLogCS;
    public float leftHemifieldLogCS;
    public float rightHemifieldLogCS;
    public float asymmetry; // Right minus Left (positive = left deficit)
    public List<ContrastSensitivityTest.TrialResult> trialHistory;
    public System.DateTime testDate;
    
    public string GetSummary()
    {
        return $"Contrast Sensitivity Results ({testDate:yyyy-MM-dd})\n" +
            $"═══════════════════════════════════════\n" +
            $"Central:         {centralLogCS:F2} LogCS\n" +
            $"Right Hemifield: {rightHemifieldLogCS:F2} LogCS (intact)\n" +
            $"Left Hemifield:  {leftHemifieldLogCS:F2} LogCS (affected)\n" +
            $"Asymmetry:       {asymmetry:F2} LogCS\n" +
            $"═══════════════════════════════════════\n" +
            $"Normal range: 1.65-2.00 LogCS\n" +
            $"Clinically significant change: >0.24 LogCS";
    }
}
```

### Input Handler: ContrastTestInput.cs

```csharp
using UnityEngine;
using UnityEngine.XR;

public class ContrastTestInput : MonoBehaviour
{
    [SerializeField] private ContrastSensitivityTest contrastTest;
    
    [Header("Controller Input")]
    [SerializeField] private XRNode inputSource = XRNode.RightHand;
    
    // Button mapping for Sloan letters
    // Using controller buttons + voice as backup
    private InputDevice device;
    
    void Update()
    {
        if (!device.isValid)
        {
            device = InputDevices.GetDeviceAtXRNode(inputSource);
            return;
        }
        
        // Quick letter input via controller
        // A = first letter shown, B = second, etc.
        // Or use voice recognition for natural input
        
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed) && aPressed)
        {
            // Could map to specific letters or use as "repeat" request
        }
        
        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bPressed) && bPressed)
        {
            // "Can't see" response
            contrastTest.OnUserResponse(' ');
        }
        
        // Trigger to advance/confirm
        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger) && trigger)
        {
            // Start test or confirm selection
        }
    }
    
    /// <summary>
    /// Called from voice recognition system
    /// </summary>
    public void OnVoiceInput(string spoken)
    {
        if (spoken.Length == 1)
        {
            contrastTest.OnUserResponse(spoken[0]);
        }
    }
}
```

### Results Logger Integration

```csharp
// Add to existing DataLogger.cs

public void LogContrastSensitivityResults(ContrastSensitivityResults results)
{
    string filename = $"contrast_sensitivity_{results.testDate:yyyy-MM-dd_HH-mm}.csv";
    string path = Path.Combine(Application.persistentDataPath, "assessments", filename);
    
    Directory.CreateDirectory(Path.GetDirectoryName(path));
    
    using (StreamWriter writer = new StreamWriter(path))
    {
        // Header
        writer.WriteLine("timestamp,logCS,letter_presented,letter_response,correct,hemifield,reaction_ms");
        
        // Trial data
        foreach (var trial in results.trialHistory)
        {
            writer.WriteLine($"{trial.timestamp:O},{trial.logCS:F2},{trial.presentedLetter}," +
                $"{trial.responseLetter},{trial.correct},{trial.hemifield},{trial.reactionTimeMs:F0}");
        }
    }
    
    // Summary file
    string summaryPath = Path.Combine(Application.persistentDataPath, "assessments", "contrast_summary.csv");
    bool writeHeader = !File.Exists(summaryPath);
    
    using (StreamWriter writer = new StreamWriter(summaryPath, append: true))
    {
        if (writeHeader)
        {
            writer.WriteLine("date,central_logcs,left_logcs,right_logcs,asymmetry");
        }
        writer.WriteLine($"{results.testDate:yyyy-MM-dd},{results.centralLogCS:F2}," +
            $"{results.leftHemifieldLogCS:F2},{results.rightHemifieldLogCS:F2},{results.asymmetry:F2}");
    }
    
    Debug.Log($"[DataLogger] Contrast sensitivity results saved to {path}");
}
```

---

## Clinical Reference Values

### Pelli-Robson Normative Data

| Age Group | Mean LogCS | Range |
|-----------|------------|-------|
| 20-30 | 1.95 | 1.80-2.10 |
| 40-50 | 1.90 | 1.70-2.05 |
| 50-60 | 1.80 | 1.65-1.95 |
| 60-70 | 1.70 | 1.50-1.85 |
| 70+ | 1.55 | 1.35-1.75 |

**Your expected baseline (age 52):** ~1.80-1.90 LogCS in intact hemifield

### Clinically Significant Change

- **Minimum detectable change:** 0.15 LogCS (one triplet)
- **Clinically significant improvement:** ≥0.30 LogCS (two triplets)
- **Coefficient of repeatability:** ±0.24 LogCS

### Expected Hemianopia Pattern

Based on your records and the research literature:

| Region | Expected LogCS | Notes |
|--------|---------------|-------|
| Right hemifield (intact) | 1.70-1.90 | May be slightly reduced due to global dimming |
| Central | 1.50-1.80 | Transition zone effects |
| Left hemifield (affected) | 0.50-1.20 | Severely reduced, target for rehabilitation |
| Asymmetry | 0.50-1.00+ | Right minus Left |

---

## Integration with Audiovisual Training

### Using Contrast Results to Personalize Training

```csharp
// In AudioVisualStimulusController.cs

public void ConfigureFromContrastResults(ContrastSensitivityResults results)
{
    // Set starting stimulus contrast based on affected hemifield threshold
    float affectedThreshold = results.leftHemifieldLogCS;
    
    // Start training at 2x threshold contrast (easily visible)
    float startingLogCS = Mathf.Max(0, affectedThreshold - 0.30f);
    float startingContrast = Mathf.Pow(10f, -startingLogCS);
    
    // Configure stimulus
    visualStimulusRenderer.material.color = new Color(
        startingContrast, 
        startingContrast, 
        0f, // Yellow stimulus
        1f
    );
    
    // Adjust eccentricity progression based on asymmetry
    if (results.asymmetry > 0.60f)
    {
        // Severe asymmetry - start closer to midline
        trainingEccentricities = new float[] { 8f, 16f, 24f, 32f };
    }
    else
    {
        // Moderate asymmetry - standard progression
        trainingEccentricities = new float[] { 8f, 24f, 40f, 56f };
    }
    
    Debug.Log($"[AVStimulus] Configured for contrast threshold {startingLogCS:F2} LogCS, " +
        $"asymmetry {results.asymmetry:F2}");
}
```

---

## Test Protocol Schedule

### Baseline (Before Training)
1. **Day 0:** Full assessment
   - Zenni Vision VR acuity check
   - Contrast sensitivity (all 3 hemifield positions)
   - VCSTest.com for external validation (optional)

### During Training (20 sessions over 6-7 weeks)
2. **Every 5 sessions:** Quick contrast check
   - Central + Left hemifield only
   - Track asymmetry reduction

### Post-Training
3. **Session 20+1:** Full reassessment
   - All hemifield positions
   - Compare to baseline
   - Document improvement for potential clinical collaboration

---

## Files to Create

```
Assets/
├── Scripts/
│   ├── Assessment/
│   │   ├── ContrastSensitivityTest.cs
│   │   ├── ContrastTestInput.cs
│   │   ├── ContrastResultsUI.cs
│   │   └── AcuityVerification.cs
│   └── Integration/
│       └── CalibrationManager.cs
├── Prefabs/
│   └── ContrastTestCanvas.prefab
├── Fonts/
│   └── SloanLetters.ttf  (or TextMeshPro asset)
└── Audio/
    ├── correct_tone.wav
    ├── incorrect_tone.wav
    └── level_complete.wav
```

### Sloan Font

The Sloan letters require a specific font for clinical validity. Options:
1. **Louise Sloan Letters** - Public domain, available from Precision Vision
2. **Custom TextMeshPro sprite sheet** - Create from official Sloan specifications

Letter specifications:
- 5x5 grid design
- Stroke width = 1 grid unit
- All letters equal in legibility

---

## References

1. Pelli DG, Robson JG, Wilkins AJ. The design of a new letter chart for measuring contrast sensitivity. *Clinical Vision Sciences*. 1988;2(3):187-199.

2. Elliott DB, Sanderson K, Conkey A. The reliability of the Pelli-Robson contrast sensitivity chart. *Ophthalmic and Physiological Optics*. 1990;10(1):21-24.

3. Daibert-Nido M, et al. Home-Based Visual Rehabilitation in Patients With Hemianopia. *Frontiers in Neurology*. 2021. DOI: 10.3389/fneur.2021.680211

---

*Document created for NeglectFix V1 development*
*December 2024*
