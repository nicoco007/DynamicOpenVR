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
using System.Reflection;
using DynamicOpenVR.BeatSaber.Native;
using DynamicOpenVR.IO;
using HarmonyLib;
using IPA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;
using Logger = IPA.Logging.Logger;

namespace DynamicOpenVR.BeatSaber
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    internal class Plugin
    {
        public static Logger logger { get; private set; }
        public static VectorInput leftTriggerValue { get; private set; }
        public static VectorInput rightTriggerValue { get; private set; }
        public static BooleanInput menu { get; private set; }
        public static HapticVibrationOutput leftSlice { get; private set; }
        public static HapticVibrationOutput rightSlice { get; private set; }
        public static PoseInput leftHandPose { get; private set; }
        public static PoseInput rightHandPose { get; private set; }
        public static Vector2Input thumbstick { get; set; }

        private readonly string _actionManifestPath = Path.Combine(Environment.CurrentDirectory, "DynamicOpenVR", "action_manifest.json");
        
        private Harmony _harmonyInstance;

        private OpenVRHelper _openVRHelper;

        [Init]
        public Plugin(Logger logger)
        {
            Plugin.logger = logger;
            Logging.Logger.handler = new IPALogHandler();
        }

        [OnStart]
        public void OnStart()
        {
            logger.Info("Starting " + typeof(Plugin).Namespace);

            try
            {
                OpenVRUtilities.Init();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to initialize OpenVR API; DynamicOpenVR will not run");
                logger.Error(ex);
                return;
            }

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

            SceneManager.sceneLoaded += OnSceneLoaded;
            OpenVREventHandler.instance.eventTriggered += OnOpenVREventTriggered;
        }

        [OnExit]
        public void OnExit()
        {
            // not really necessary since the game is closing, just following good practices
            leftTriggerValue?.Dispose();
            rightTriggerValue?.Dispose();
            menu?.Dispose();
            leftSlice?.Dispose();
            rightSlice?.Dispose();
            leftHandPose?.Dispose();
            rightHandPose?.Dispose();
            thumbstick?.Dispose();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "PCInit")
            {
                _openVRHelper = Resources.FindObjectsOfTypeAll<OpenVRHelper>().First();
            }
        }

        private void OnOpenVREventTriggered(VREvent_t evt)
        {
            if (_openVRHelper == null) return;

            EVREventType eventType = (EVREventType)evt.eventType;

            if (eventType == EVREventType.VREvent_InputFocusReleased && evt.data.process.pid == 0)
            {
                InvokeEvent(_openVRHelper, nameof(_openVRHelper.inputFocusWasReleasedEvent));
                _openVRHelper.EnableEventSystem();
            }

            if (eventType == EVREventType.VREvent_InputFocusCaptured && evt.data.process.oldPid == 0)
            {
                InvokeEvent(_openVRHelper, nameof(_openVRHelper.inputFocusWasCapturedEvent));
                _openVRHelper.DisableEventSystem();
            }

            if (eventType == EVREventType.VREvent_DashboardActivated)
            {
                InvokeEvent(_openVRHelper, nameof(_openVRHelper.inputFocusWasCapturedEvent));
                _openVRHelper.DisableEventSystem();
            }

            if (eventType == EVREventType.VREvent_DashboardDeactivated)
            {
                InvokeEvent(_openVRHelper, nameof(_openVRHelper.inputFocusWasReleasedEvent));
                _openVRHelper.EnableEventSystem();
            }

            if (eventType == EVREventType.VREvent_Quit)
            {
                Application.Quit();
            }
        }

        private void InvokeEvent<T>(T obj, string name, params object[] args)
        {
            var multicastDelegate = (MulticastDelegate)obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);

            if (multicastDelegate == null) return;

            foreach (Delegate handler in multicastDelegate.GetInvocationList())
            {
                handler.Method.Invoke(handler.Target, args);
            }
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
            
            logger.Trace($"Reading '{globalManifestPath}'");

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
            
            logger.Trace($"Reading '{configPath}'");

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
            thumbstick        = new Vector2Input("/actions/main/in/thumbstick");
        }

        private void ApplyHarmonyPatches()
        {
            logger.Info("Applying input patches");

            _harmonyInstance = new Harmony("com.nicoco007.dynamicopenvr.beatsaber");
            _harmonyInstance.PatchAll();
        }
    }
}
