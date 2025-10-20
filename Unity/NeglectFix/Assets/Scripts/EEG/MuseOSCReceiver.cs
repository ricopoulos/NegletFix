using UnityEngine;
using System;
// Note: This requires extOSC package - install from: https://github.com/Iam1337/extOSC
// Or use OscCore: https://github.com/stella3d/OscCore
using extOSC;

namespace NeglectFix.EEG
{
    /// <summary>
    /// Receives OSC packets from Mind Monitor app streaming Muse EEG data.
    /// Parses band power data and exposes events for other systems.
    ///
    /// Mind Monitor OSC Address Format:
    /// /muse/elements/alpha_absolute - [TP9, AF7, AF8, TP10]
    /// /muse/elements/beta_absolute - [TP9, AF7, AF8, TP10]
    /// /muse/elements/theta_absolute - [TP9, AF7, AF8, TP10]
    /// </summary>
    public class MuseOSCReceiver : MonoBehaviour
    {
        [Header("OSC Configuration")]
        [Tooltip("Port to receive OSC packets (default: 5000)")]
        public int receivePort = 5000;

        [Header("Status")]
        public bool isReceiving = false;
        public float lastPacketTime = 0f;

        // OSC Receiver
        private OSCReceiver receiver;

        // Latest band power data (4 channels: TP9, AF7, AF8, TP10)
        private float[] alphaAbsolute = new float[4];
        private float[] betaAbsolute = new float[4];
        private float[] thetaAbsolute = new float[4];

        // Events for other scripts to subscribe to
        public event Action<float[], float[], float[]> OnBandPowerReceived;
        public event Action<float> OnTP10AlphaReceived;
        public event Action<float> OnTP10BetaReceived;
        public event Action<float> OnTP10ThetaReceived;

        // Electrode index constants (Mind Monitor channel order)
        private const int TP9 = 0;  // Left temporal-parietal
        private const int AF7 = 1;  // Left frontal
        private const int AF8 = 2;  // Right frontal
        private const int TP10 = 3; // Right temporal-parietal (PRIMARY TARGET for neglect)

        void Start()
        {
            InitializeOSCReceiver();
        }

        void InitializeOSCReceiver()
        {
            // Create OSC receiver
            receiver = gameObject.AddComponent<OSCReceiver>();
            receiver.LocalPort = receivePort;

            // Bind OSC addresses to handler methods
            receiver.Bind("/muse/elements/alpha_absolute", OnAlphaReceived);
            receiver.Bind("/muse/elements/beta_absolute", OnBetaReceived);
            receiver.Bind("/muse/elements/theta_absolute", OnThetaReceived);

            Debug.Log($"[MuseOSC] Receiver initialized on port {receivePort}");
            Debug.Log("[MuseOSC] Waiting for Mind Monitor to stream data...");
            Debug.Log("[MuseOSC] Make sure Mind Monitor OSC output is configured with this computer's IP and port 5000");
        }

        private void OnAlphaReceived(OSCMessage message)
        {
            if (message.Values.Count >= 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    alphaAbsolute[i] = message.Values[i].FloatValue;
                }

                // Emit event for TP10 specifically (right parietal - neglect target)
                OnTP10AlphaReceived?.Invoke(alphaAbsolute[TP10]);

                UpdateReceiveStatus();
            }
        }

        private void OnBetaReceived(OSCMessage message)
        {
            if (message.Values.Count >= 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    betaAbsolute[i] = message.Values[i].FloatValue;
                }

                OnTP10BetaReceived?.Invoke(betaAbsolute[TP10]);
                UpdateReceiveStatus();
            }
        }

        private void OnThetaReceived(OSCMessage message)
        {
            if (message.Values.Count >= 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    thetaAbsolute[i] = message.Values[i].FloatValue;
                }

                OnTP10ThetaReceived?.Invoke(thetaAbsolute[TP10]);

                // After receiving all bands, emit combined event
                OnBandPowerReceived?.Invoke(alphaAbsolute, betaAbsolute, thetaAbsolute);

                UpdateReceiveStatus();
            }
        }

        private void UpdateReceiveStatus()
        {
            lastPacketTime = Time.time;
            if (!isReceiving)
            {
                isReceiving = true;
                Debug.Log("[MuseOSC] ✓ Receiving data from Mind Monitor!");
            }
        }

        void Update()
        {
            // Check for timeout (no packets for 3 seconds)
            if (isReceiving && Time.time - lastPacketTime > 3f)
            {
                isReceiving = false;
                Debug.LogWarning("[MuseOSC] No data received for 3 seconds. Check Mind Monitor connection.");
            }
        }

        // Public accessors for latest data
        public float GetTP10Alpha() => alphaAbsolute[TP10];
        public float GetTP10Beta() => betaAbsolute[TP10];
        public float GetTP10Theta() => thetaAbsolute[TP10];

        public float[] GetAllAlpha() => alphaAbsolute;
        public float[] GetAllBeta() => betaAbsolute;
        public float[] GetAllTheta() => thetaAbsolute;

        void OnDestroy()
        {
            if (receiver != null)
            {
                receiver.Close();
            }
        }

        // Debug visualization in Inspector
        void OnGUI()
        {
            if (!isReceiving) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"<b>Muse EEG - TP10 (Right Parietal)</b>");
            GUILayout.Label($"Alpha: {alphaAbsolute[TP10]:F2} μV²");
            GUILayout.Label($"Beta:  {betaAbsolute[TP10]:F2} μV²");
            GUILayout.Label($"Theta: {thetaAbsolute[TP10]:F2} μV²");
            GUILayout.EndArea();
        }
    }
}
