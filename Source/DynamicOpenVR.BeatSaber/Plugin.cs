// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2020 Nicolas Gnyra

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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DynamicOpenVR.IO;
using Harmony;
using IPA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;
using Logger = IPA.Logging.Logger;

namespace DynamicOpenVR.BeatSaber
{
    public class Plugin : IBeatSaberPlugin
    {
        public static Logger logger { get; private set; }
        public static VectorInput leftTriggerValue { get; private set; }
        public static VectorInput rightTriggerValue { get; private set; }
        public static BooleanInput menu { get; private set; }
        public static HapticVibrationOutput leftSlice { get; private set; }
        public static HapticVibrationOutput rightSlice { get; private set; }
        public static PoseInput leftHandPose { get; private set; }
        public static PoseInput rightHandPose { get; private set; }

        private readonly string _actionManifestPath = Path.Combine(Environment.CurrentDirectory, "DynamicOpenVR", "action_manifest.json");
        private readonly HashSet<EVREventType> _pauseEvents = new HashSet<EVREventType>(new [] { EVREventType.VREvent_InputFocusCaptured, EVREventType.VREvent_DashboardActivated, EVREventType.VREvent_OverlayShown });
        
        private HarmonyInstance _harmonyInstance;
        private bool _initialized;

        public void Init(Logger logger)
        {
            Plugin.logger = logger;
            Logging.Logger.handler = new IPALogHandler();
        }

        public void OnApplicationStart()
        {
            logger.Info("Starting " + typeof(Plugin).Namespace);

            OpenVRUtilities.Init();

            logger.Info("Successfully initialized OpenVR API");
                
            // adding the manifest to config is more of a quality of life thing
            try
            {
                AddManifestToSteamConfig();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to update SteamVR manifest.");
                logger.Error(ex);
            }

            RegisterActionSet();
            ApplyHarmonyPatches();

            OpenVRActionManager.instance.Initialize(_actionManifestPath);

            _initialized = true;
        }

        public void OnApplicationQuit()
        {
            // not really necessary here, just following good practices
            leftTriggerValue?.Dispose();
            rightTriggerValue?.Dispose();
            menu?.Dispose();
            leftSlice?.Dispose();
            rightSlice?.Dispose();
            leftHandPose?.Dispose();
            rightHandPose?.Dispose();
        }

        private void AddManifestToSteamConfig()
        {
            string steamFolder = SteamUtilities.GetSteamHomeDirectory();
            string manifestPath = Path.Combine(Environment.CurrentDirectory, "beatsaber.vrmanifest");
            string appConfigPath = Path.Combine(steamFolder, "config", "appconfig.json");
            string globalManifestPath = Path.Combine(steamFolder, "config", "steamapps.vrmanifest");

            JObject beatSaberManifest = ReadBeatSaberManifest(globalManifestPath);

            beatSaberManifest["action_manifest_path"] = _actionManifestPath;

            JObject vrManifest = new JObject
            {
                { "applications", new JArray { beatSaberManifest } }
            };

            WriteBeatSaberManifest(manifestPath, vrManifest);
            
            JObject appConfig = ReadAppConfig(appConfigPath);
            JArray manifestPaths = appConfig["manifest_paths"].Value<JArray>();
            List<JToken> existing = manifestPaths.Where(p => p.Value<string>() == manifestPath).ToList();
            bool updated = false;

            // only rewrite if path isn't in list already or is not at the top
            if (manifestPaths.IndexOf(existing.FirstOrDefault()) != 0 || existing.Count > 1)
            {
                logger.Info($"Adding '{manifestPath}' to app config");

                foreach (JToken token in existing)
                {
                    manifestPaths.Remove(token);
                }

                manifestPaths.Insert(0, manifestPath);

                updated = true;
            }
            else
            {
                logger.Info("Manifest is already in app config");
            }

            if (!manifestPaths.Any(s => s.Value<string>().Equals(globalManifestPath, StringComparison.InvariantCultureIgnoreCase)))
            {
                logger.Info($"Adding '{globalManifestPath}' to app config");

                manifestPaths.Add(globalManifestPath);

                updated = true;
            }
            else
            {
                logger.Info("Global manifest is already in app config");
            }

            if (updated)
            {
                if (MessageBox.Show(
                        "DynamicOpenVR.BeatSaber has created a .vrmanifest file in your game's root folder and would like to register it within SteamVR. " +
                        $"The file has been created at \"{manifestPath}\" and will be added to the global SteamVR app configuration at \"{appConfigPath}\". " +
                        "Doing this allows SteamVR to properly recognize that the game is now using the new input system when the game is not running. " +
                        "However, it may cause issues on certain systems. You can choose to do this now or wait until you've run the game with " +
                        "DynamicOpenVR enabled and checked everything works.\n\nCan DynamicOpenVR.BeatSaber proceed with the changes?",
                        "DynamicOpenVR needs your permission", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    logger.Info($"Writing app config changes to '{appConfigPath}'");
                    WriteAppConfig(appConfigPath, appConfig);
                }
                else
                {
                    logger.Warn("Manifest registration canceled by user");
                }
            }
        }

        private JObject ReadBeatSaberManifest(string globalManifestPath)
        {
            if (!File.Exists(globalManifestPath))
            {
                throw new FileNotFoundException("Could not find file " + globalManifestPath);
            }

            JObject beatSaberManifest;
            
            logger.Debug($"Reading '{globalManifestPath}'");

            using (StreamReader reader = new StreamReader(globalManifestPath))
            {
                JObject globalManifest = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                beatSaberManifest = globalManifest["applications"]?.Value<JArray>()?.FirstOrDefault(a => a["app_key"]?.Value<string>() == "steam.app.620980")?.Value<JObject>();
            }

            if (beatSaberManifest == null)
            {
                throw new Exception($"Beat Saber manifest not found in '{globalManifestPath}'");
            }

            return beatSaberManifest;
        }

        private JObject ReadAppConfig(string configPath)
        {
            var appConfig = new JObject();

            if (!File.Exists(configPath))
            {
                logger.Warn($"Could not find file '{configPath}'");

                appConfig.Add("manifest_paths", new JArray());

                return appConfig;
            }
            
            logger.Debug($"Reading '{configPath}'");

            using (StreamReader reader = new StreamReader(configPath))
            {
                appConfig = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            }

            if (appConfig == null)
            {
                logger.Warn("File is empty");
                appConfig = new JObject();
            }

            if (!appConfig.ContainsKey("manifest_paths"))
            {
                logger.Warn("manifest_paths is missing");
                appConfig.Add("manifest_paths", new JArray());
            }

            return appConfig;
        }

        private void WriteBeatSaberManifest(string manifestPath, JObject beatSaberManifest)
        {
            logger.Info($"Writing manifest to '{manifestPath}'");

            using (StreamWriter writer = new StreamWriter(manifestPath))
            {
                writer.Write(JsonConvert.SerializeObject(beatSaberManifest, Formatting.Indented));
            }
        }

        private void WriteAppConfig(string configPath, JObject appConfig)
        {
            logger.Info($"Writing app config to '{configPath}'");

            using (StreamWriter writer = new StreamWriter(configPath))
            {
                writer.Write(JsonConvert.SerializeObject(appConfig, Formatting.Indented));
            }
        }

        private void RegisterActionSet()
        {
            logger.Info("Registering actions");

            leftTriggerValue  = new VectorInput("/actions/main/in/lefttriggervalue");
            rightTriggerValue = new VectorInput("/actions/main/in/righttriggervalue");
            menu              = new BooleanInput("/actions/main/in/menu");
            leftSlice         = new HapticVibrationOutput("/actions/main/out/leftslice");
            rightSlice        = new HapticVibrationOutput("/actions/main/out/rightslice");
            leftHandPose      = new PoseInput("/actions/main/in/lefthandpose");
            rightHandPose     = new PoseInput("/actions/main/in/righthandpose");
        }

        private void ApplyHarmonyPatches()
        {
            logger.Info("Applying input patches");

            _harmonyInstance = HarmonyInstance.Create(GetType().Namespace);
            _harmonyInstance.PatchAll();
        }

        public void OnFixedUpdate() { }
        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }
        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene) { }
        public void OnSceneUnloaded(Scene scene) { }

        public void OnUpdate()
        {
            if (_initialized)
            {
                VREvent_t evt = default;
                if (OpenVR.System.PollNextEvent(ref evt, (uint)Marshal.SizeOf(typeof(VREvent_t))) && _pauseEvents.Contains((EVREventType) evt.eventType))
                {
                    Resources.FindObjectsOfTypeAll<PauseController>().FirstOrDefault()?.Pause();
                }
            }
        }
    }
}
