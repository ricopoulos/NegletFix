using System.IO;
using System.Linq;
using NeglectFix.Assessment;
using NeglectFix.Tasks;
using NeglectFix.Utils;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NeglectFix.EditorTools
{
    public static class AvTrainingManualSmokeSceneBuilder
    {
        private const string SmokeScenePath = "Assets/Scenes/AVTrainingManualSmoke.unity";
        private const string Session1PilotScenePath = "Assets/Scenes/AVTrainingSession1Pilot.unity";
        private const string QuickReadyCheckScenePath = "Assets/Scenes/AVTrainingQuickReadyCheck.unity";

        [MenuItem("NeglectFix/Testing/Create AV Training Manual Smoke Scene")]
        public static void CreateSceneFromMenu()
        {
            CreateScene(SmokeScenePath, addToBuildSettings: true, ScenePreset.ManualSmoke);
        }

        [MenuItem("NeglectFix/Testing/Create AV Training Session 1 Pilot Scene")]
        public static void CreateSession1PilotSceneFromMenu()
        {
            CreateScene(Session1PilotScenePath, addToBuildSettings: false, ScenePreset.Session1Pilot);
        }

        [MenuItem("NeglectFix/Testing/Create AV Training Quick Ready Check Scene")]
        public static void CreateQuickReadyCheckSceneFromMenu()
        {
            CreateScene(QuickReadyCheckScenePath, addToBuildSettings: false, ScenePreset.QuickReadyCheck);
        }

        public static void CreateSceneFromBatch()
        {
            CreateScene(SmokeScenePath, addToBuildSettings: true, ScenePreset.ManualSmoke);
        }

        public static void CreateSession1PilotSceneFromBatch()
        {
            CreateScene(Session1PilotScenePath, addToBuildSettings: false, ScenePreset.Session1Pilot);
        }

        public static void CreateQuickReadyCheckSceneFromBatch()
        {
            CreateScene(QuickReadyCheckScenePath, addToBuildSettings: false, ScenePreset.QuickReadyCheck);
        }

        public static void BuildAndroidApkFromBatch()
        {
            CreateScene(SmokeScenePath, addToBuildSettings: true, ScenePreset.ManualSmoke);

            var apkPath = GetCommandLineArgument("-apkPath");
            if (string.IsNullOrWhiteSpace(apkPath))
                apkPath = "Builds/AVTrainingManualSmoke.apk";

            BuildAndroidApk(SmokeScenePath, apkPath);
        }

        public static void BuildSession1PilotAndroidApkFromBatch()
        {
            CreateScene(Session1PilotScenePath, addToBuildSettings: false, ScenePreset.Session1Pilot);

            var apkPath = GetCommandLineArgument("-apkPath");
            if (string.IsNullOrWhiteSpace(apkPath))
                apkPath = "Builds/AVTrainingSession1Pilot.apk";

            BuildAndroidApk(Session1PilotScenePath, apkPath);
        }

        public static void BuildQuickReadyCheckAndroidApkFromBatch()
        {
            CreateScene(QuickReadyCheckScenePath, addToBuildSettings: false, ScenePreset.QuickReadyCheck);

            var apkPath = GetCommandLineArgument("-apkPath");
            if (string.IsNullOrWhiteSpace(apkPath))
                apkPath = "Builds/AVTrainingQuickReadyCheck.apk";

            BuildAndroidApk(QuickReadyCheckScenePath, apkPath);
        }

        private enum ScenePreset
        {
            ManualSmoke,
            Session1Pilot,
            QuickReadyCheck
        }

        private static void BuildAndroidApk(string scenePath, string apkPath)
        {
            var directory = Path.GetDirectoryName(apkPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var buildOptions = new BuildPlayerOptions
            {
                scenes = new[] { scenePath },
                locationPathName = apkPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.Development
            };

            var report = BuildPipeline.BuildPlayer(buildOptions);
            var summary = report.summary;
            Debug.Log($"[NeglectFix] Android smoke APK build result: {summary.result} ({summary.totalSize} bytes) -> {apkPath}");

            if (summary.result != BuildResult.Succeeded)
                EditorApplication.Exit(1);
        }

        private static void CreateScene(string scenePath, bool addToBuildSettings, ScenePreset preset)
        {
            Directory.CreateDirectory("Assets/Scenes");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.45f, 0.45f, 0.45f);
            RenderSettings.skybox = null;

            CreateCamera();
            CreateLight();
            CreateAvTrainingSystem(preset);

            EditorSceneManager.SaveScene(scene, scenePath);

            if (addToBuildSettings)
                AddSceneToBuildSettings(scenePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[NeglectFix] AV training scene ready: {scenePath}");
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 1.6f, 0f);

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 100f;

            cameraObject.AddComponent<AudioListener>();

            var cameraData = cameraObject.AddComponent<UniversalAdditionalCameraData>();
            cameraData.allowXRRendering = true;
            cameraData.renderPostProcessing = false;
        }

        private static void CreateLight()
        {
            var lightObject = new GameObject("Directional Light");
            lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
        }

        private static void CreateAvTrainingSystem(ScenePreset preset)
        {
            var root = new GameObject("AVTrainingSystem_ManualSmoke");

            var dataLogger = root.AddComponent<DataLogger>();
            dataLogger.autoStartLogging = false;
            dataLogger.logFolderName = "Logs";

            var rewardController = root.AddComponent<RewardController>();
            rewardController.mode = RewardController.RewardMode.OpenLoop;
            rewardController.cooldownDuration = 0.1f;
            rewardController.rewardDuration = 0.75f;
            rewardController.useSpatialAudio = false;

            var scheduler = root.AddComponent<ProgramScheduler>();
            scheduler.totalSessionsPlanned = 1;
            scheduler.sessionsPerWeek = 7;
            scheduler.reMeasurementSessions = new int[0];
            scheduler.resetStateOnAwake = true;

            var training = root.AddComponent<AudioVisualTraining>();
            training.dataLogger = dataLogger;
            training.trialLogger = dataLogger;
            training.rewardController = rewardController;
            training.programScheduler = scheduler;
            training.blocksPerSession = 1;
            training.interBlockRestSec = 0f;
            training.stimulusDurationSec = 0.45f;
            training.responseWindowSec = 1.5f;
            training.generatedStimulusPattern = AudioVisualTraining.StimulusPattern.SolidDisk;
            training.stimulusAngularSizeDeg = 5f;
            training.minimumGeneratedStimulusContrast = 0.9f;
            training.generatedStimulusTextureSize = 128;
            training.ensureOpaqueTrainingBackdrop = true;
            training.opaqueBackdropDistanceMeters = 2.4f;
            training.opaqueBackdropWidthMeters = 5.2f;
            training.opaqueBackdropHeightMeters = 3.4f;
            training.opaqueBackdropLuminance = training.stimulusBackgroundLuminance;
            training.baselineResults = CreateEricBaselineResults();
            training.enableLegacyKeyboardFallback = true;
            training.enableAvReadyPrompt = true;
            training.readyPromptEnabled = true;
            training.showReadyPromptInHeadset = true;
            training.requireReadyConfirmation = true;
            training.showCompletionPromptInHeadset = true;
            training.completionPromptInstructions = "You can remove the headset now.\nThank you.";
            training.showCenterFixationCross = true;
            training.fixationCrossDistanceMeters = 1.15f;
            training.fixationCrossSizeMeters = 0.04f;
            training.fixationCrossThicknessMeters = 0.004f;
            training.fixationCrossLuminance = 0.92f;
            training.enablePracticeBlock = false;
            training.practiceIntroDurationSec = 4f;
            training.practiceDurationSec = 15f;
            training.readyPromptInstructions = "This is rehab training, not a contrast test.\nStart on the center cross. When a marker appears, move your eyes to it and press.\nKeep your head still.";
            training.readyCountdownInstructions = "Find the center cross.\nKeep your head still. Move only your eyes after each marker appears.";
            training.baselinePromptInstructions = "Calibrating before training starts.\nKeep the headset still and look at the center cross.";
            training.practicePromptInstructions = "Practice first.\nStart on the center cross. When a marker appears, move only your eyes to it and press.\nKeep your head still. These practice trials are not counted.";

            if (preset == ScenePreset.Session1Pilot)
            {
                root.name = "AVTrainingSystem_Session1Pilot";
                scheduler.stateFileName = "program_state_session1_pilot.json";
                training.readyCountdownDuration = 2f;
                training.baselineDuration = 5f;
                training.blockDurationSec = 120f;
                training.cooldownDurationSec = 5f;
                training.minInterStimulusIntervalSec = 1.2f;
                training.maxInterStimulusIntervalSec = 2.5f;
                training.stimulusAngularSizeDeg = 6f;
                training.enablePracticeBlock = true;
                training.practiceIntroDurationSec = 4f;
                training.practiceDurationSec = 15f;
                training.baselinePromptInstructions = "Calibrating before practice.\nKeep the headset still and look at the center cross.";
            }
            else if (preset == ScenePreset.QuickReadyCheck)
            {
                root.name = "AVTrainingSystem_QuickReadyCheck";
                scheduler.stateFileName = "program_state_quick_ready_check.json";
                training.readyCountdownDuration = 1f;
                training.baselineDuration = 2f;
                training.blockDurationSec = 10f;
                training.cooldownDurationSec = 2f;
                training.minInterStimulusIntervalSec = 0.5f;
                training.maxInterStimulusIntervalSec = 0.75f;
                training.stimulusAngularSizeDeg = 8f;
                training.minimumGeneratedStimulusContrast = 1f;
            }
            else
            {
                scheduler.stateFileName = "program_state_manual_smoke.json";
                training.readyCountdownDuration = 2f;
                training.baselineDuration = 5f;
                training.blockDurationSec = 60f;
                training.cooldownDurationSec = 5f;
                training.minInterStimulusIntervalSec = 0.75f;
                training.maxInterStimulusIntervalSec = 1.25f;
            }
        }

        private static ContrastSensitivityResults CreateEricBaselineResults()
        {
            return new ContrastSensitivityResults
            {
                centralLogCS = 1.05f,
                leftHemifieldLogCS = 0f,
                rightHemifieldLogCS = 2.25f,
                asymmetry = 2.25f
            };
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var existingScenes = EditorBuildSettings.scenes
                .Where(scene => scene.path != scenePath)
                .ToList();

            existingScenes.Insert(0, new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = existingScenes.ToArray();
        }

        private static string GetCommandLineArgument(string name)
        {
            var args = System.Environment.GetCommandLineArgs();

            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == name)
                    return args[i + 1];
            }

            return null;
        }
    }
}
