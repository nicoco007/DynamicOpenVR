// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019 Nicolas Gnyra

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.

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

            OVRActionSet dummy = new OVRActionSet("dummy", OVRActionSetUsage.LeftRight);
            
            dummy.RegisterAction(new BooleanInput("boolean"));
            dummy.RegisterAction(new VectorInput("vector1"));
            dummy.RegisterAction(new Vector2Input("vector2"));
            dummy.RegisterAction(new Vector3Input("vector3"));

            manager.RegisterActionSet(dummy);
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
