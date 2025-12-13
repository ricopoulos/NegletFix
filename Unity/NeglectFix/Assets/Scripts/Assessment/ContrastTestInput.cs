using UnityEngine;
using System;

// Uncomment when Meta XR SDK is installed:
// #define OVR_INSTALLED

namespace NeglectFix.Assessment
{
    /// <summary>
    /// Handles VR controller and keyboard input for contrast sensitivity testing.
    /// Supports Quest controllers and keyboard fallback for editor testing.
    ///
    /// Input mapping:
    /// - Trigger: Start test / Confirm
    /// - B button: "Can't see" response
    /// - Thumbstick: Navigate letter selection (if using visual picker)
    /// - Keyboard A-Z: Direct letter input (editor testing)
    /// </summary>
    public class ContrastTestInput : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ContrastSensitivityTest contrastTest;

#if OVR_INSTALLED
        [Header("Input Settings")]
        [Tooltip("Which hand controller to use")]
        [SerializeField] private OVRInput.Controller controller = OVRInput.Controller.RTouch;
#endif

        [Tooltip("Enable keyboard input for editor testing")]
        [SerializeField] private bool enableKeyboardInput = true;

        [Header("Letter Selection UI (Optional)")]
        [Tooltip("If using visual letter picker instead of voice")]
        [SerializeField] private bool useVisualPicker = false;
        [SerializeField] private LetterPickerUI letterPicker;

        // Valid Sloan letters
        private readonly char[] validLetters = { 'C', 'D', 'H', 'K', 'N', 'O', 'R', 'S', 'V', 'Z' };

        // Debounce
        private float lastInputTime;
        private const float INPUT_DEBOUNCE = 0.2f;

        // Events
        public event Action OnStartRequested;
        public event Action OnCantSeePressed;

        void Start()
        {
            if (contrastTest == null)
            {
                contrastTest = FindObjectOfType<ContrastSensitivityTest>();
            }

            if (contrastTest == null)
            {
                Debug.LogError("[ContrastTestInput] ContrastSensitivityTest not found!");
                enabled = false;
            }
        }

        void Update()
        {
            // Debounce check
            if (Time.time - lastInputTime < INPUT_DEBOUNCE) return;

            HandleVRInput();

            if (enableKeyboardInput)
            {
                HandleKeyboardInput();
            }
        }

        private void HandleVRInput()
        {
#if OVR_INSTALLED
            // Trigger - Start test or advance
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                lastInputTime = Time.time;

                if (!contrastTest.IsTestInProgress)
                {
                    OnStartRequested?.Invoke();
                    contrastTest.StartFullTestSequence();
                }
            }

            // B button - "Can't see" / blank response
            if (OVRInput.GetDown(OVRInput.Button.Two, controller)) // B on right controller
            {
                lastInputTime = Time.time;

                if (contrastTest.IsAwaitingResponse)
                {
                    OnCantSeePressed?.Invoke();
                    contrastTest.OnUserResponse(' ');
                }
            }

            // A button - could be used for repeat request
            if (OVRInput.GetDown(OVRInput.Button.One, controller)) // A on right controller
            {
                // Future: Request letter repeat or audio cue
            }

            // Visual picker navigation (if enabled)
            if (useVisualPicker && letterPicker != null && contrastTest.IsAwaitingResponse)
            {
                Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);

                if (Mathf.Abs(thumbstick.x) > 0.5f || Mathf.Abs(thumbstick.y) > 0.5f)
                {
                    letterPicker.Navigate(thumbstick);
                    lastInputTime = Time.time;
                }

                // Thumbstick press to confirm selection
                if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, controller))
                {
                    char selected = letterPicker.GetSelectedLetter();
                    contrastTest.OnUserResponse(selected);
                    lastInputTime = Time.time;
                }
            }
#endif
        }

        private void HandleKeyboardInput()
        {
            // Check for letter keys using proper KeyCode mapping
            if (Input.GetKeyDown(KeyCode.C)) { TryRespondWithLetter('C'); return; }
            if (Input.GetKeyDown(KeyCode.D)) { TryRespondWithLetter('D'); return; }
            if (Input.GetKeyDown(KeyCode.H)) { TryRespondWithLetter('H'); return; }
            if (Input.GetKeyDown(KeyCode.K)) { TryRespondWithLetter('K'); return; }
            if (Input.GetKeyDown(KeyCode.N)) { TryRespondWithLetter('N'); return; }
            if (Input.GetKeyDown(KeyCode.O)) { TryRespondWithLetter('O'); return; }
            if (Input.GetKeyDown(KeyCode.R)) { TryRespondWithLetter('R'); return; }
            if (Input.GetKeyDown(KeyCode.S)) { TryRespondWithLetter('S'); return; }
            if (Input.GetKeyDown(KeyCode.V)) { TryRespondWithLetter('V'); return; }
            if (Input.GetKeyDown(KeyCode.Z)) { TryRespondWithLetter('Z'); return; }

            // Space - Start test
            if (Input.GetKeyDown(KeyCode.Space))
            {
                lastInputTime = Time.time;

                if (!contrastTest.IsTestInProgress)
                {
                    OnStartRequested?.Invoke();
                    contrastTest.StartFullTestSequence();
                }
            }

            // Backspace or Escape - "Can't see"
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
            {
                lastInputTime = Time.time;

                if (contrastTest.IsAwaitingResponse)
                {
                    OnCantSeePressed?.Invoke();
                    contrastTest.OnUserResponse(' ');
                }
            }
        }

        private void TryRespondWithLetter(char letter)
        {
            lastInputTime = Time.time;
            Debug.Log($"[ContrastTestInput] Key pressed: {letter}, IsAwaitingResponse: {contrastTest.IsAwaitingResponse}");

            if (contrastTest.IsAwaitingResponse)
            {
                contrastTest.OnUserResponse(letter);
            }
        }

        /// <summary>
        /// Called from external voice recognition system
        /// </summary>
        public void OnVoiceInput(string spoken)
        {
            if (!contrastTest.IsAwaitingResponse) return;
            if (string.IsNullOrEmpty(spoken)) return;

            // Extract first character
            char letter = char.ToUpper(spoken[0]);

            // Validate it's a Sloan letter
            if (IsValidSloanLetter(letter))
            {
                contrastTest.OnUserResponse(letter);
            }
            else
            {
                Debug.Log($"[ContrastTestInput] Invalid letter: {letter}. Valid: C,D,H,K,N,O,R,S,V,Z");
            }
        }

        private bool IsValidSloanLetter(char c)
        {
            foreach (char valid in validLetters)
            {
                if (c == valid) return true;
            }
            return false;
        }

        /// <summary>
        /// Start a specific hemifield test directly
        /// </summary>
        public void StartHemifieldTest(ContrastSensitivityTest.HemifieldMode hemifield)
        {
            if (!contrastTest.IsTestInProgress)
            {
                contrastTest.StartTest(hemifield);
            }
        }
    }

    /// <summary>
    /// Optional visual letter picker for users who can't use voice input
    /// </summary>
    public class LetterPickerUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI[] letterButtons;
        private int selectedIndex = 0;
        private readonly char[] letters = { 'C', 'D', 'H', 'K', 'N', 'O', 'R', 'S', 'V', 'Z' };

        public void Navigate(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // Horizontal navigation
                selectedIndex += direction.x > 0 ? 1 : -1;
            }
            else
            {
                // Vertical navigation (5 per row)
                selectedIndex += direction.y > 0 ? -5 : 5;
            }

            selectedIndex = Mathf.Clamp(selectedIndex, 0, letters.Length - 1);
            UpdateVisuals();
        }

        public char GetSelectedLetter()
        {
            return letters[selectedIndex];
        }

        private void UpdateVisuals()
        {
            for (int i = 0; i < letterButtons.Length && i < letters.Length; i++)
            {
                if (letterButtons[i] != null)
                {
                    letterButtons[i].color = (i == selectedIndex) ? Color.yellow : Color.white;
                }
            }
        }
    }
}
