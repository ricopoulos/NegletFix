using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using NeglectFix.Tasks;
using NeglectFix.Utils;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace NeglectFix.Tests.PlayMode
{
    public class AudioVisualTrainingSmokeTest
    {
        private const float TestTimeoutSeconds = 8f;

        private float previousTimeScale;
        private GameObject root;
        private string schedulerStateFileName;
        private InputTestFixture input;
        private Keyboard keyboard;
        private string generatedSessionFile;
        private string generatedTrialFile;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            input = new InputTestFixture();
            input.Setup();
            keyboard = InputSystem.AddDevice<Keyboard>();

            previousTimeScale = Time.timeScale;
            Time.timeScale = 5f;

            var scene = SceneManager.CreateScene($"AVTrainingSmoke_{Guid.NewGuid():N}");
            SceneManager.SetActiveScene(scene);

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
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

            if (!string.IsNullOrEmpty(schedulerStateFileName))
            {
                string schedulerStatePath = Path.Combine(Application.persistentDataPath, schedulerStateFileName);
                if (File.Exists(schedulerStatePath))
                    File.Delete(schedulerStatePath);
            }

            DeleteFileIfExists(generatedSessionFile);
            DeleteFileIfExists(generatedTrialFile);
            generatedSessionFile = null;
            generatedTrialFile = null;

            keyboard = null;
            input?.TearDown();
            input = null;

            yield return null;
        }

        [UnityTest]
        public IEnumerator ShortSession_CompletesAndWritesTrialLog()
        {
            root = new GameObject("AVTrainingSmokeRoot");
            root.SetActive(false);

            var dataLogger = root.AddComponent<DataLogger>();
            var rewardController = root.AddComponent<RewardController>();

            var scheduler = root.AddComponent<ProgramScheduler>();
            schedulerStateFileName = $"program_state_smoke_{Guid.NewGuid():N}.json";
            scheduler.stateFileName = schedulerStateFileName;
            scheduler.ResetForNewPhase(ProgramScheduler.Paradigm.CongruentPair_WakeForest, 30);

            var training = root.AddComponent<AudioVisualTraining>();
            training.baselineDuration = 0.05f;
            training.blocksPerSession = 1;
            training.blockDurationSec = 2.0f;
            training.interBlockRestSec = 0f;
            training.minInterStimulusIntervalSec = 0.05f;
            training.maxInterStimulusIntervalSec = 0.06f;
            training.responseWindowSec = 1.0f;
            training.stimulusDurationSec = 0.1f;
            training.enableIntactHemifieldControlTrials = true;
            training.intactControlTrialProbability = 1.0f;
            training.minimumRehabTrialsBetweenControlTrials = 1;

            root.SetActive(true);
            yield return null;

            // AudioVisualTraining.Start sets the production cooldown. Shorten it after Start runs.
            training.cooldownDuration = 0.05f;

            bool pressSpace = true;
            float deadline = Time.realtimeSinceStartup + TestTimeoutSeconds;
            while (training.currentPhase != TaskManager.SessionPhase.Completed &&
                   Time.realtimeSinceStartup < deadline)
            {
                if (training.currentPhase == TaskManager.SessionPhase.Training &&
                    rewardController.GetTotalRewards() == 0)
                {
                    if (pressSpace)
                        input.Press(keyboard.spaceKey);
                    else
                        input.Release(keyboard.spaceKey);

                    pressSpace = !pressSpace;
                }
                else if (training.currentPhase == TaskManager.SessionPhase.Ready)
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

            Assert.That(training.currentPhase, Is.EqualTo(TaskManager.SessionPhase.Completed));
            Assert.That(rewardController.GetTotalRewards(), Is.GreaterThan(0));
            Assert.That(dataLogger.GetTrainingTrialsLogged(), Is.GreaterThan(0));
            Assert.That(dataLogger.GetTrainingRehabTrialsLogged(), Is.GreaterThan(0));
            Assert.That(dataLogger.GetTrainingControlTrialsLogged(), Is.GreaterThan(0));
            Assert.That(dataLogger.GetCurrentTrialFile(), Is.Not.Empty);

            TextMeshProUGUI completionPrompt = UnityEngine.Object
                .FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None)
                .FirstOrDefault(text => text.text.Contains("SESSION COMPLETE", StringComparison.Ordinal));

            Assert.That(completionPrompt, Is.Not.Null);
            Assert.That(completionPrompt.text, Does.Contain("You can remove the headset now."));

            generatedSessionFile = dataLogger.GetCurrentSessionFile();
            generatedTrialFile = dataLogger.GetCurrentTrialFile();

            UnityEngine.Object.Destroy(root);
            root = null;
            yield return null;
            yield return null;

            Assert.That(File.Exists(generatedTrialFile), Is.True);

            string[] lines = File.ReadAllLines(generatedTrialFile);
            Assert.That(lines, Does.Contain("timestamp_ms,session_index,block_index,trial_index,eccentricity_deg,hemifield,contrast_logcs,stimulus_onset_ms,audio_onset_ms,response_onset_ms,rt_ms,hit,av_delta_ms,trial_type,is_control_trial,counts_for_rehab_dose"));
            Assert.That(lines.Length, Is.GreaterThan(5));

            string[] rows = lines
                .Where(line => !line.StartsWith("#", StringComparison.Ordinal) &&
                               !line.StartsWith("timestamp_ms", StringComparison.Ordinal) &&
                               !string.IsNullOrWhiteSpace(line))
                .ToArray();

            Assert.That(rows.Length, Is.GreaterThan(0));

            string[] columns = rows[0].Split(',');
            Assert.That(columns[11], Is.EqualTo("1"));
            Assert.That(float.Parse(columns[10], CultureInfo.InvariantCulture), Is.GreaterThanOrEqualTo(0f));

            Assert.That(rows.Any(row =>
            {
                string[] rowColumns = row.Split(',');
                return rowColumns[13] == "rehab" && rowColumns[14] == "0" && rowColumns[15] == "1";
            }), Is.True);

            Assert.That(rows.Any(row =>
            {
                string[] rowColumns = row.Split(',');
                return rowColumns[13] == "right_control" && rowColumns[14] == "1" && rowColumns[15] == "0";
            }), Is.True);

            DeleteFileIfExists(generatedSessionFile);
            DeleteFileIfExists(generatedTrialFile);
            generatedSessionFile = null;
            generatedTrialFile = null;
        }

        private static void DeleteFileIfExists(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                File.Delete(path);
        }
    }
}
