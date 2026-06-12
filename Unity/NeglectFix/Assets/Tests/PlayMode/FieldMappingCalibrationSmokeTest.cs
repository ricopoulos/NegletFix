using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using NeglectFix.Assessment;
using NeglectFix.Utils;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace NeglectFix.Tests.PlayMode
{
    public class FieldMappingCalibrationSmokeTest
    {
        private const float TestTimeoutSeconds = 8f;

        private float previousTimeScale;
        private GameObject root;
        private InputTestFixture input;
        private Keyboard keyboard;
        private string generatedFieldMapFile;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            input = new InputTestFixture();
            input.Setup();
            keyboard = InputSystem.AddDevice<Keyboard>();

            previousTimeScale = Time.timeScale;
            Time.timeScale = 5f;

            var scene = SceneManager.CreateScene($"FieldMappingSmoke_{Guid.NewGuid():N}");
            SceneManager.SetActiveScene(scene);

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 1.6f, 0f);
            cameraObject.AddComponent<Camera>();

            if (UnityEngine.Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length == 0)
                cameraObject.AddComponent<AudioListener>();

            var lightObject = new GameObject("Directional Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (root != null)
            {
                UnityEngine.Object.Destroy(root);
                root = null;
            }

            yield return null;

            Time.timeScale = previousTimeScale;
            DeleteFileIfExists(generatedFieldMapFile);
            generatedFieldMapFile = null;

            keyboard = null;
            input?.TearDown();
            input = null;

            yield return null;
        }

        [UnityTest]
        public IEnumerator ShortCalibration_CompletesAndWritesSpatialTrialLog()
        {
            root = new GameObject("FieldMappingSmokeRoot");
            root.SetActive(false);

            var dataLogger = root.AddComponent<DataLogger>();
            var fieldMap = root.AddComponent<FieldMappingCalibration>();
            fieldMap.dataLogger = dataLogger;
            fieldMap.autoStartSessionLog = false;
            fieldMap.requireReadyConfirmation = false;
            fieldMap.initialDelaySec = 0.01f;
            fieldMap.readyCountdownDurationSec = 0f;
            fieldMap.minInterTrialIntervalSec = 0.01f;
            fieldMap.maxInterTrialIntervalSec = 0.02f;
            fieldMap.stimulusDurationSec = 0.05f;
            fieldMap.responseWindowSec = 0.35f;
            fieldMap.shuffleTrialOrder = false;
            fieldMap.repetitionsPerPoint = 1;
            fieldMap.testAnglesDeg = new[]
            {
                new Vector2(-5f, 0f),
                new Vector2(5f, 0f),
                new Vector2(0f, 8f),
                new Vector2(0f, -8f)
            };

            root.SetActive(true);
            yield return null;

            bool pressSpace = true;
            float deadline = Time.realtimeSinceStartup + TestTimeoutSeconds;
            while (fieldMap.currentPhase != FieldMappingCalibration.CalibrationPhase.Completed &&
                   Time.realtimeSinceStartup < deadline)
            {
                if (fieldMap.currentPhase == FieldMappingCalibration.CalibrationPhase.Mapping)
                {
                    if (pressSpace)
                        input.Press(keyboard.spaceKey);
                    else
                        input.Release(keyboard.spaceKey);

                    pressSpace = !pressSpace;
                }
                else
                {
                    input.Release(keyboard.spaceKey);
                }

                yield return null;
            }

            input.Release(keyboard.spaceKey);

            Assert.That(fieldMap.currentPhase, Is.EqualTo(FieldMappingCalibration.CalibrationPhase.Completed));
            Assert.That(dataLogger.GetFieldMappingTrialsLogged(), Is.EqualTo(4));
            Assert.That(dataLogger.GetCurrentFieldMappingFile(), Is.Not.Empty);

            TextMeshProUGUI completionPrompt = UnityEngine.Object
                .FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None)
                .FirstOrDefault(text => text.text.Contains("FIELD MAP COMPLETE", StringComparison.Ordinal));

            Assert.That(completionPrompt, Is.Not.Null);

            generatedFieldMapFile = dataLogger.GetCurrentFieldMappingFile();

            UnityEngine.Object.Destroy(root);
            root = null;
            yield return null;
            yield return null;

            Assert.That(File.Exists(generatedFieldMapFile), Is.True);

            string[] lines = File.ReadAllLines(generatedFieldMapFile);
            Assert.That(lines, Does.Contain("timestamp_ms,trial_index,repeat_index,axis,horizontal_angle_deg,vertical_angle_deg,stimulus_distance_m,stimulus_world_x,stimulus_world_y,stimulus_world_z,camera_world_x,camera_world_y,camera_world_z,camera_relative_dir_x,camera_relative_dir_y,camera_relative_dir_z,head_yaw_onset_deg,head_pitch_onset_deg,head_roll_onset_deg,stimulus_onset_ms,response_onset_ms,rt_ms,hit,head_yaw_response_deg,head_pitch_response_deg,head_roll_response_deg,camera_relative_dir_response_x,camera_relative_dir_response_y,camera_relative_dir_response_z"));

            string[] rows = lines
                .Where(line => !line.StartsWith("#", StringComparison.Ordinal) &&
                               !line.StartsWith("timestamp_ms", StringComparison.Ordinal) &&
                               !string.IsNullOrWhiteSpace(line))
                .ToArray();

            Assert.That(rows.Length, Is.EqualTo(4));
            Assert.That(rows.Select(row => row.Split(',')[3]).ToArray(), Is.EquivalentTo(new[] { "left", "right", "up", "down" }));

            string[] firstColumns = rows[0].Split(',');
            Assert.That(firstColumns.Length, Is.EqualTo(29));
            Assert.That(float.Parse(firstColumns[4], CultureInfo.InvariantCulture), Is.EqualTo(-5f).Within(0.01f));
            Assert.That(float.Parse(firstColumns[13], CultureInfo.InvariantCulture), Is.LessThan(0f));
            Assert.That(float.Parse(firstColumns[15], CultureInfo.InvariantCulture), Is.GreaterThan(0f));
            Assert.That(lines.Any(line => line.StartsWith("# Recommended rehab locations:", StringComparison.Ordinal)), Is.True);

            DeleteFileIfExists(generatedFieldMapFile);
            generatedFieldMapFile = null;
        }

        private static void DeleteFileIfExists(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                File.Delete(path);
        }
    }
}
