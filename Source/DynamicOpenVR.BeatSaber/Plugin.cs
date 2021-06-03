// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2021 Nicolas Gnyra

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
        private static readonly string kActionManifestPath = Path.Combine(Environment.CurrentDirectory, "DynamicOpenVR", "action_manifest.json");

        public static VectorInput leftTriggerValue { get; private set; }
        public static VectorInput rightTriggerValue { get; private set; }
        public static BooleanInput menu { get; private set; }
        public static HapticVibrationOutput leftSlice { get; private set; }
        public static HapticVibrationOutput rightSlice { get; private set; }
        public static PoseInput leftHandPose { get; private set; }
        public static PoseInput rightHandPose { get; private set; }
        public static Vector2Input thumbstick { get; set; }

        private readonly Logger _logger;
        private readonly Harmony _harmonyInstance;

        private OpenVRHelper _openVRHelper;

        [Init]
        public Plugin(Logger logger)
        {
            _logger = logger;
            _harmonyInstance = new Harmony("com.nicoco007.dynamicopenvr.beatsaber");

            Logging.Logger.handler = new IPALogHandler(logger);
        }

        [OnStart]
        public void OnStart()
        {
            _logger.Info("Starting " + typeof(Plugin).Namespace);

            try
            {
                OpenVRUtilities.Init();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to initialize OpenVR API; DynamicOpenVR will not run");
                _logger.Error(ex);
                return;
            }

            _logger.Info("Successfully initialized OpenVR API");

            // adding the manifest to config is more of a quality of life thing
            try
            {
                AddManifestToSteamConfig();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to update SteamVR manifest.");
                _logger.Error(ex);
            }

            RegisterActionSet();
            ApplyHarmonyPatches();

            OpenVRActionManager.instance.Initialize(kActionManifestPath);

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

            var eventType = (EVREventType)evt.eventType;

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

            beatSaberManifest["action_manifest_path"] = kActionManifestPath;

            var vrManifest = new JObject
            {
                { "applications", new JArray { beatSaberManifest } }
            };

            WriteBeatSaberManifest(manifestPath, vrManifest);

            JObject appConfig = ReadAppConfig(appConfigPath);
            JArray manifestPaths = appConfig["manifest_paths"].Value<JArray>();
            var existing = manifestPaths.Where(p => p.Value<string>() == manifestPath).ToList();
            bool updated = false;

            // only rewrite if path isn't in list already or is not at the top
            if (manifestPaths.IndexOf(existing.FirstOrDefault()) != 0 || existing.Count > 1)
            {
                _logger.Info($"Adding '{manifestPath}' to app config");

                foreach (JToken token in existing)
                {
                    manifestPaths.Remove(token);
                }

                manifestPaths.Insert(0, manifestPath);

                updated = true;
            }
            else
            {
                _logger.Info("Manifest is already in app config");
            }

            if (!manifestPaths.Any(s => s.Value<string>().Equals(globalManifestPath, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.Info($"Adding '{globalManifestPath}' to app config");

                manifestPaths.Add(globalManifestPath);

                updated = true;
            }
            else
            {
                _logger.Info("Global manifest is already in app config");
            }

            if (updated)
            {
                if (MessageBox.Show(
                        "DynamicOpenVR.BeatSaber has created a .vrmanifest file in your game's root folder and would like to permanently register it within SteamVR. " +
                        $"The file has been created at \"{manifestPath}\" and will be added to the global SteamVR app configuration at \"{appConfigPath}\".\n\n" +
                        "Doing this allows SteamVR to properly recognize that the game is now using the new input system when the game is not running. " +
                        "However, it may cause issues on certain systems. You can opt to skip this temporarily and run the game as-is to confirm that " +
                        "everything works as expected, and you will be prompted with this message again the next time you start the game.\n\n" +
                        "Can DynamicOpenVR.BeatSaber proceed with the changes?",
                        "DynamicOpenVR needs your permission", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _logger.Info($"Writing app config changes to '{appConfigPath}'");
                    WriteAppConfig(appConfigPath, appConfig);
                }
                else
                {
                    _logger.Warn("Manifest registration canceled by user");
                }
            }
        }

        private JObject ReadBeatSaberManifest(string globalManifestPath)
        {
            if (!File.Exists(globalManifestPath))
            {
                throw new FileNotFoundException($"Could not find file '{globalManifestPath}'");
            }

            JObject beatSaberManifest;

            _logger.Trace($"Reading '{globalManifestPath}'");

            using (var reader = new StreamReader(globalManifestPath))
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
                _logger.Warn($"Could not find file '{configPath}'");

                appConfig.Add("manifest_paths", new JArray());

                return appConfig;
            }

            _logger.Trace($"Reading '{configPath}'");

            using (var reader = new StreamReader(configPath))
            {
                appConfig = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            }

            if (appConfig == null)
            {
                _logger.Warn("File is empty");
                appConfig = new JObject();
            }

            if (!appConfig.ContainsKey("manifest_paths"))
            {
                _logger.Warn("manifest_paths is missing");
                appConfig.Add("manifest_paths", new JArray());
            }

            return appConfig;
        }

        private void WriteBeatSaberManifest(string manifestPath, JObject beatSaberManifest)
        {
            _logger.Info($"Writing manifest to '{manifestPath}'");

            using (var writer = new StreamWriter(manifestPath))
            {
                writer.Write(JsonConvert.SerializeObject(beatSaberManifest, Formatting.Indented));
            }
        }

        private void WriteAppConfig(string configPath, JObject appConfig)
        {
            _logger.Info($"Writing app config to '{configPath}'");

            using (var writer = new StreamWriter(configPath))
            {
                writer.Write(JsonConvert.SerializeObject(appConfig, Formatting.Indented));
            }
        }

        private void RegisterActionSet()
        {
            _logger.Info("Registering actions");

            leftTriggerValue = new VectorInput("/actions/main/in/lefttriggervalue");
            rightTriggerValue = new VectorInput("/actions/main/in/righttriggervalue");
            menu = new BooleanInput("/actions/main/in/menu");
            leftSlice = new HapticVibrationOutput("/actions/main/out/leftslice");
            rightSlice = new HapticVibrationOutput("/actions/main/out/rightslice");
            leftHandPose = new PoseInput("/actions/main/in/lefthandpose");
            rightHandPose = new PoseInput("/actions/main/in/righthandpose");
            thumbstick = new Vector2Input("/actions/main/in/thumbstick");
        }

        private void ApplyHarmonyPatches()
        {
            _logger.Info("Applying input patches");

            _harmonyInstance.PatchAll();
        }
    }
}
