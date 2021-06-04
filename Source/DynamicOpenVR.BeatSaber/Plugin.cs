// <copyright file="Plugin.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2021 Nicolas Gnyra
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DynamicOpenVR.IO;
using HarmonyLib;
using IPA;
using IPA.Utilities;
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
        internal static readonly string kSteamPath = SteamUtilities.GetSteamHomeDirectory();
        internal static readonly string kManifestPath = Path.Combine(UnityGame.InstallPath, "beatsaber.vrmanifest");
        internal static readonly string kAppConfigPath = Path.Combine(kSteamPath, "config", "appconfig.json");
        internal static readonly string kGlobalManifestPath = Path.Combine(kSteamPath, "config", "steamapps.vrmanifest");

        private static readonly string kActionManifestPath = Path.Combine(UnityGame.InstallPath, "DynamicOpenVR", "action_manifest.json");

        private readonly Logger _logger;
        private readonly Harmony _harmonyInstance;

        private OpenVRHelper _openVRHelper;
        private JObject _updatedAppConfig;

        [Init]
        public Plugin(Logger logger)
        {
            _logger = logger;
            _harmonyInstance = new Harmony("com.nicoco007.dynamicopenvr.beatsaber");

            Logging.Logger.handler = new IPALogHandler(logger);
        }

        public static VectorInput leftTriggerValue { get; private set; }

        public static VectorInput rightTriggerValue { get; private set; }

        public static BooleanInput menu { get; private set; }

        public static HapticVibrationOutput leftSlice { get; private set; }

        public static HapticVibrationOutput rightSlice { get; private set; }

        public static PoseInput leftHandPose { get; private set; }

        public static PoseInput rightHandPose { get; private set; }

        public static Vector2Input thumbstick { get; private set; }

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
            else if (scene.name == "MainMenu" && _updatedAppConfig != null)
            {
                AppConfigConfirmationModal.Create(_updatedAppConfig);
            }
        }

        private void OnOpenVREventTriggered(VREvent_t evt)
        {
            if (_openVRHelper == null)
            {
                return;
            }

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

            if (multicastDelegate == null)
            {
                return;
            }

            foreach (Delegate handler in multicastDelegate.GetInvocationList())
            {
                handler.Method.Invoke(handler.Target, args);
            }
        }

        private void AddManifestToSteamConfig()
        {
            JObject beatSaberManifest = ReadBeatSaberManifest(kGlobalManifestPath);

            beatSaberManifest["action_manifest_path"] = kActionManifestPath;

            var vrManifest = new JObject
            {
                { "applications", new JArray { beatSaberManifest } },
            };

            WriteBeatSaberManifest(kManifestPath, vrManifest);

            JObject appConfig = ReadAppConfig(kAppConfigPath);
            JArray manifestPaths = appConfig["manifest_paths"].Value<JArray>();
            var existing = manifestPaths.Where(p => p.Value<string>() == kManifestPath).ToList();
            bool updated = false;

            // only rewrite if path isn't in list already or is not at the top
            if (manifestPaths.IndexOf(existing.FirstOrDefault()) != 0 || existing.Count > 1)
            {
                _logger.Info($"Adding '{kManifestPath}' to app config");

                foreach (JToken token in existing)
                {
                    manifestPaths.Remove(token);
                }

                manifestPaths.Insert(0, kManifestPath);

                updated = true;
            }
            else
            {
                _logger.Info("Manifest is already in app config");
            }

            if (!manifestPaths.Any(s => s.Value<string>().Equals(kGlobalManifestPath, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.Info($"Adding '{kGlobalManifestPath}' to app config");

                manifestPaths.Add(kGlobalManifestPath);

                updated = true;
            }
            else
            {
                _logger.Info("Global manifest is already in app config");
            }

            if (updated)
            {
                _updatedAppConfig = appConfig;
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
