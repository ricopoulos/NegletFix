using System.IO;
using System.Linq;
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

        public static void CreateSceneFromBatch()
        {
            CreateScene(SmokeScenePath, addToBuildSettings: true, ScenePreset.ManualSmoke);
        }

        public static void CreateSession1PilotSceneFromBatch()
        {
            CreateScene(Session1PilotScenePath, addToBuildSettings: false, ScenePreset.Session1Pilot);
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

        private enum ScenePreset
        {
            ManualSmoke,
            Session1Pilot
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

            var training = root.AddComponent<AudioVisualTraining>();
            training.dataLogger = dataLogger;
            training.trialLogger = dataLogger;
            training.rewardController = rewardController;
            training.programScheduler = scheduler;
            training.blocksPerSession = 1;
            training.interBlockRestSec = 0f;
            training.stimulusDurationSec = 0.25f;
            training.responseWindowSec = 1.5f;
            training.generatedStimulusPattern = AudioVisualTraining.StimulusPattern.SolidDisk;
            training.stimulusAngularSizeDeg = 3f;
            training.generatedStimulusTextureSize = 128;
            training.enableLegacyKeyboardFallback = true;

            if (preset == ScenePreset.Session1Pilot)
            {
                root.name = "AVTrainingSystem_Session1Pilot";
                scheduler.stateFileName = "program_state_session1_pilot.json";
                training.baselineDuration = 30f;
                training.blockDurationSec = 300f;
                training.cooldownDurationSec = 30f;
                training.minInterStimulusIntervalSec = 1.2f;
                training.maxInterStimulusIntervalSec = 2.5f;
                training.stimulusAngularSizeDeg = 4f;
            }
            else
            {
                scheduler.stateFileName = "program_state_manual_smoke.json";
                training.baselineDuration = 5f;
                training.blockDurationSec = 60f;
                training.cooldownDurationSec = 5f;
                training.minInterStimulusIntervalSec = 0.75f;
                training.maxInterStimulusIntervalSec = 1.25f;
            }
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
