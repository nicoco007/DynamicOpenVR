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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using Logger = IPA.Logging.Logger;

namespace DynamicOpenVR.BeatSaber
{
    public class Plugin : IBeatSaberPlugin
    {
        public static Logger Logger { get; private set; }
        public static VectorInput LeftTriggerValue { get; private set; }
        public static VectorInput RightTriggerValue { get; private set; }
        public static BooleanInput Menu { get; private set; }
        public static HapticVibrationOutput LeftSlice { get; private set; }
        public static HapticVibrationOutput RightSlice { get; private set; }
        public static PoseInput LeftHandPose { get; private set; }
        public static PoseInput RightHandPose { get; private set; }

        private HarmonyInstance harmonyInstance;

        public void Init(Logger logger)
        {
            Logger = logger;

            Logger.Info("Starting " + typeof(Plugin).Namespace);

            if (!OpenVrActionManager.isRunning)
            {
                Logger.Warn($"OpenVR is not running. {typeof(Plugin).Namespace} will not be activated.");
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
            string appConfigPath = Path.Combine(steamFolder, "config", "appconfig.json");
            string globalManifestPath = Path.Combine(steamFolder, "config", "steamapps.vrmanifest");

            Logger.Debug("Found Steam at " + steamFolder);

            JObject beatSaberManifest = ReadBeatSaberManifest(globalManifestPath);

            beatSaberManifest["action_manifest_path"] = OpenVrActionManager.kActionManifestPath;

            JObject vrManifest = new JObject
            {
                { "applications", new JArray { beatSaberManifest } }
            };

            WriteBeatSaberManifest(manifestPath, vrManifest);
            
            JObject appConfig = ReadAppConfig(appConfigPath);
            JArray manifestPaths = appConfig["manifest_paths"].Value<JArray>();
            List<JToken> existing = manifestPaths.Where(p => p.Value<string>() == manifestPath).ToList();

            // only rewrite if path isn't in list already or is not at the top
            if (manifestPaths.IndexOf(existing.FirstOrDefault()) != 0)
            {
                Logger.Info($"Adding '{manifestPath}' to '{appConfigPath}'");

                foreach (JToken token in existing)
                {
                    appConfig["manifest_paths"].Value<JArray>().Remove(token);
                }

                appConfig["manifest_paths"].Value<JArray>().Insert(0, manifestPath);

                WriteAppConfig(appConfigPath, appConfig);
            }
            else
            {
                Logger.Info("Manifest is already registered");
            }
        }

        private string GetSteamHomeDirectory()
        {
            Process steamProcess = Process.GetProcessesByName("Steam").FirstOrDefault();

            if (steamProcess == null)
            {
                throw new Exception("Steam process could not be found.");
            }

            string path = steamProcess.MainModule?.FileName;

            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("Steam path could not be found.");
            }

            return Path.GetDirectoryName(path);
        }

        private JObject ReadBeatSaberManifest(string globalManifestPath)
        {
            if (!File.Exists(globalManifestPath))
            {
                throw new FileNotFoundException("Could not find file " + globalManifestPath);
            }

            JObject beatSaberManifest;
            
            Logger.Debug("Reading " + globalManifestPath);

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
            
            Logger.Debug("Reading " + configPath);

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
            Logger.Info("Writing manifest to " + manifestPath);

            using (StreamWriter writer = new StreamWriter(manifestPath))
            {
                writer.Write(JsonConvert.SerializeObject(beatSaberManifest, Formatting.Indented));
            }
        }

        private void WriteAppConfig(string configPath, JObject appConfig)
        {
            Logger.Info("Writing app config to " + configPath);

            using (StreamWriter writer = new StreamWriter(configPath))
            {
                writer.Write(JsonConvert.SerializeObject(appConfig, Formatting.Indented));
            }
        }

        private void RegisterActionSet()
        {
            Logger.Info("Registering actions");

            OpenVrActionManager manager = OpenVrActionManager.instance;

            LeftTriggerValue  = manager.RegisterAction(new VectorInput("/actions/main/in/lefttriggervalue"));
            RightTriggerValue = manager.RegisterAction(new VectorInput("/actions/main/in/righttriggervalue"));
            Menu              = manager.RegisterAction(new BooleanInput("/actions/main/in/menu"));
            LeftSlice         = manager.RegisterAction(new HapticVibrationOutput("/actions/main/out/leftslice"));
            RightSlice        = manager.RegisterAction(new HapticVibrationOutput("/actions/main/out/rightslice"));
            LeftHandPose      = manager.RegisterAction(new PoseInput("/actions/main/in/lefthandpose"));
            RightHandPose     = manager.RegisterAction(new PoseInput("/actions/main/in/righthandpose"));
        }

        private void ApplyHarmonyPatches()
        {
            Logger.Info("Applying input patches");

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
