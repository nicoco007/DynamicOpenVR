using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DynamicOpenVR.IO;
using Harmony;
using IPA;
using IPA.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

namespace DynamicOpenVR.BeatSaber
{
    public class Plugin : IBeatSaberPlugin
    {
        public const string ActionSetName = "main";
        public const string LeftTriggerValueAction = "LeftTriggerValue";
        public const string RightTriggerValueAction = "RightTriggerValue";
        public const string MenuAction = "Menu";
        public const string LeftSliceAction = "LeftSlice";
        public const string RightSliceAction = "RightSlice";
        public const string LeftHandPoseName = "LeftHandPose";
        public const string RightHandPoseName = "RightHandPose";

        public static string Name => typeof(Plugin).Namespace;

        public static Logger Logger { get; private set; }

        private HarmonyInstance harmonyInstance;

        public void Init(Logger logger)
        {
            Logger = logger;

            Logger.Info("Starting " + Name);

            if (!OpenVRActionManager.IsRunning)
            {
                Logger.Warn($"OpenVR is not running. {Name} will not be activated.");
                return;
            }

            try
            {
                AddManifestToSteamConfig();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to configure manifest: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            RegisterActionSet();
            ApplyHarmonyPatches();
        }

        private void AddManifestToSteamConfig()
        {
            string steamFolder = GetSteamHomeDirectory();
            string manifestPath = Path.Combine(Environment.CurrentDirectory, "beatsaber.vrmanifest");
            string configPath = Path.Combine(steamFolder, "config", "appconfig.json");
            string globalManifestPath = Path.Combine(steamFolder, "config", "steamapps.vrmanifest");

            JObject beatSaberManifest = ReadBeatSaberManifest(globalManifestPath);

            beatSaberManifest["action_manifest_path"] = "action_manifest.json";

            JObject vrManifest = new JObject
            {
                { "applications", new JArray { beatSaberManifest } }
            };

            WriteBeatSaberManifest(manifestPath, vrManifest);
            
            JObject appConfig = ReadAppConfig(configPath);
            JArray manifestPaths = appConfig["manifest_paths"].Value<JArray>();
            List<JToken> existing = manifestPaths.Where(p => p.Value<string>() == manifestPath).ToList();

            // only rewrite if path isn't in list already or is not at the top
            if (existing.Count != 1 || manifestPaths.IndexOf(existing.FirstOrDefault()) > 0)
            {
                foreach (JToken token in existing)
                {
                    appConfig["manifest_paths"].Value<JArray>().Remove(token);
                }

                appConfig["manifest_paths"].Value<JArray>().Insert(0, manifestPath);

                WriteAppConfig(configPath, appConfig);
            }
        }

        private string GetSteamHomeDirectory()
        {
            Process steamProcess = Process.GetProcessesByName("Steam").FirstOrDefault(p => p.MainModule != null);

            if (steamProcess == null)
            {
                throw new Exception("Steam process could not be found.");
            }

            return Path.GetDirectoryName(steamProcess.MainModule.FileName);
        }

        private JObject ReadBeatSaberManifest(string globalManifestPath)
        {
            if (!File.Exists(globalManifestPath))
            {
                throw new FileNotFoundException("Could not find file " + globalManifestPath);
            }

            JObject beatSaberManifest;

            using (StreamReader reader = new StreamReader(globalManifestPath))
            {
                JObject globalManifest = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                beatSaberManifest = globalManifest["applications"]?.Value<JArray>()?.FirstOrDefault(a => a["app_key"]?.Value<string>() == "steam.app.620980")?.Value<JObject>();
            }

            if (beatSaberManifest == null)
            {
                throw new Exception("Failed to read Beat Saber manifest from " + globalManifestPath);
            }

            return beatSaberManifest;
        }

        private JObject ReadAppConfig(string configPath)
        {
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("Could not find file " + configPath);
            }

            JObject appConfig;

            using (StreamReader reader = new StreamReader(configPath))
            {
                appConfig = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            }

            if (appConfig == null)
            {
                throw new Exception("Could not read app config from " + configPath);
            }

            return appConfig;
        }

        private void WriteBeatSaberManifest(string manifestPath, JObject beatSaberManifest)
        {
            Console.WriteLine("Writing manifest to " + manifestPath);

            using (StreamWriter writer = new StreamWriter(manifestPath))
            {
                writer.Write(JsonConvert.SerializeObject(beatSaberManifest, Formatting.Indented));
            }
        }

        private void WriteAppConfig(string configPath, JObject appConfig)
        {
            Console.WriteLine("Writing app config to " + configPath);

            using (StreamWriter writer = new StreamWriter(configPath))
            {
                writer.Write(JsonConvert.SerializeObject(appConfig, Formatting.Indented));
            }
        }

        private void RegisterActionSet()
        {
            Logger.Info("Registering actions");

            OpenVRActionManager manager = OpenVRActionManager.Instance;

            OVRActionSet actionSet = new OVRActionSet(ActionSetName, OVRActionSetUsage.LeftRight).AddTranslation("en_us", "Beat Saber");

            actionSet.RegisterAction(new VectorInput(LeftTriggerValueAction, OVRActionRequirement.Mandatory).AddTranslation("en_us", "Left Trigger Pull").AddTranslation("fr_fr", "Gâchette gauche (tirer)"));
            actionSet.RegisterAction(new VectorInput(RightTriggerValueAction, OVRActionRequirement.Mandatory).AddTranslation("en_us", "Right Trigger Pull").AddTranslation("fr_fr", "Gâchette droite (tirer)"));
            actionSet.RegisterAction(new BooleanInput(MenuAction, OVRActionRequirement.Mandatory).AddTranslation("en_us", "Menu Button").AddTranslation("fr_fr", "Bouton Menu"));
            actionSet.RegisterAction(new HapticVibrationOutput(LeftSliceAction).AddTranslation("en_us", "Left Slice Haptic Feedback").AddTranslation("fr_fr", "Retour haptique pour coupe de gauche"));
            actionSet.RegisterAction(new HapticVibrationOutput(RightSliceAction).AddTranslation("en_us", "Left Slice Haptic Feedback").AddTranslation("fr_fr", "Retour haptique pour coupe de droite"));
            actionSet.RegisterAction(new PoseInput(LeftHandPoseName, OVRActionRequirement.Mandatory).AddTranslation("en_us", "Left Hand Pose"));
            actionSet.RegisterAction(new PoseInput(RightHandPoseName, OVRActionRequirement.Mandatory).AddTranslation("en_us", "Right Hand Pose"));

            manager.RegisterActionSet(actionSet);
        }

        private void ApplyHarmonyPatches()
        {
            Logger.Info("Applying OpenVR patches");

            harmonyInstance = HarmonyInstance.Create(GetType().Namespace);
            harmonyInstance.PatchAll();
        }

        public void OnApplicationStart() { }
        public void OnApplicationQuit() { }
        public void OnFixedUpdate() { }
        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }
        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene) { }
        public void OnSceneUnloaded(Scene scene) { }
        public void OnUpdate() { }
    }
}
