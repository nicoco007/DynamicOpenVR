// <copyright file="Plugin.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2023 Nicolas Gnyra
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

extern alias UnityXROpenVR;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DynamicOpenVR.BeatSaber.Input;
using DynamicOpenVR.BeatSaber.InputCollections;
using DynamicOpenVR.IO;
using DynamicOpenVR.SteamVR;
using DynamicOpenVR.SteamVR.VRManifest;
using HarmonyLib;
using IPA;
using IPA.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using UnityXROpenVR::Unity.XR.OpenVR;
using UnityXROpenVR::Valve.VR;
using Zenject;
using Logger = IPA.Logging.Logger;

namespace DynamicOpenVR.BeatSaber
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    internal class Plugin
    {
        internal const string kOpenVRLoaderHarmonyCategory = "OpenVRLoader";

        internal static readonly string kSteamPath = SteamUtilities.GetSteamHomeDirectory();
        internal static readonly string kManifestPath = Path.Combine(UnityGame.InstallPath, "beatsaber.vrmanifest");
        internal static readonly string kAppConfigPath = Path.Combine(kSteamPath, "config", "appconfig.json");
        internal static readonly string kGlobalManifestPath = Path.Combine(kSteamPath, "config", "steamapps.vrmanifest");

        internal static readonly JsonSerializerSettings kJsonSerializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            },
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        private static readonly string kActionManifestPath = Path.Combine(UnityGame.InstallPath, "DynamicOpenVR", "action_manifest.json");

        private readonly Logger _logger;

        private bool _initializing = false;
        private AppConfig _updatedAppConfig;

        [Init]
        public Plugin(Logger logger)
        {
            _logger = logger;
            harmony = new Harmony("com.nicoco007.dynamicopenvr.beatsaber");

            Logging.Logger.handler = new IPALogHandler(logger);
        }

        public static UnityXRActions unityXRActions { get; private set; }

        internal static Harmony harmony { get; private set; }

        [OnStart]
        public void OnStart()
        {
            _logger.Info("Starting " + typeof(Plugin).Namespace);

            SceneManager.sceneLoaded += OnSceneLoaded;

            harmony.PatchAllUncategorized();
        }

        [OnExit]
        public void OnExit()
        {
            unityXRActions?.Dispose();

            SceneManager.sceneLoaded -= OnSceneLoaded;

            harmony.UnpatchSelf();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "GameLoader":
                    InitializeOpenVR();
                    break;

                case "MainMenu" when _updatedAppConfig != null:
                    // TODO: this should be a patch on a menu installer
                    SceneContext sceneContext = Resources.FindObjectsOfTypeAll<SceneContext>().First(sc => sc.gameObject.scene.name == "MainMenu");
                    sceneContext.OnPostInstall.AddListener(() =>
                    {
                        AppConfigConfirmationModal.Create(sceneContext.Container, _updatedAppConfig);
                    });
                    break;
            }
        }

        private void InitializeOpenVR()
        {
            if (_initializing)
            {
                return;
            }

            _initializing = true;

            OpenVRHandProvider.Register();
            OpenVRActionManager.instance.Configure("Beat Saber", kActionManifestPath);

            RegisterActionSet();

            XRManagerSettings manager = XRGeneralSettings.Instance.Manager;

            if (!manager.isInitializationComplete)
            {
                _logger.Error($"{nameof(XRManagerSettings)} hasn't completed initialization!");
                return;
            }

            if (manager.activeLoaders.Count == 0)
            {
                _logger.Error("No active XR loaders found");
                return;
            }

            _logger.Trace($"Disabling current XR Loader '{manager.activeLoader.name}'");

            manager.StopSubsystems();
            manager.DeinitializeLoader();

            // This should match the contents of OpenVRSettings.asset since it's read by the native plugin first
            OpenVRSettings settings = ScriptableObject.CreateInstance<OpenVRSettings>();
            settings.name = "OpenVRSettings";
            settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.SinglePassInstanced;
            settings.InitializationType = OpenVRSettings.InitializationTypes.Scene;
            settings.ActionManifestFileRelativeFilePath = null;
            settings.MirrorView = OpenVRSettings.MirrorViewModes.Left;

            _logger.Trace($"Creating {nameof(OpenVRLoader)}");

            OpenVRLoader loader = ScriptableObject.CreateInstance<OpenVRLoaderWithInputSystem>();
            loader.name = "OpenVR Loader";

            // add to registered loaders or else TryAddLoader won't work
            manager.m_RegisteredLoaders.Add(loader);

            // make sure OpenVR is the first manager is the list
            if (!manager.TryAddLoader(loader, 0))
            {
                _logger.Error($"Failed to add loader to {nameof(XRManagerSettings)}");
                return;
            }

            _logger.Info("Initializing OpenVR");

            EVRInitError error = EVRInitError.None;

            do
            {
                OpenVR.Init(ref error);

                if (error == EVRInitError.Init_AnotherAppLaunching)
                {
                    _logger.Trace("Another app is launching (SteamVR Home?); trying again...");
                    Thread.Sleep(250);
                }
            }
            while (error == EVRInitError.Init_AnotherAppLaunching);

            _logger.Info($"Initializing {nameof(OpenVRLoader)}");

            manager.InitializeLoaderSync();

            if (manager.activeLoader != loader)
            {
                if (manager.activeLoader != null)
                {
                    _logger.Error($"Failed to initialize {nameof(OpenVRLoader)}; current loader is {manager.activeLoader.name}.");
                }
                else
                {
                    _logger.Error($"Failed to initialize {nameof(OpenVRLoader)}; no loader is currently active.");
                }

                return;
            }

            manager.StartSubsystems();

            try
            {
                AddManifestToSteamConfig();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to update SteamVR manifest.");
                _logger.Error(ex);
            }
        }

        private void AddManifestToSteamConfig()
        {
            VRApplication beatSaberManifest = ReadBeatSaberManifest(kGlobalManifestPath);

            beatSaberManifest.actionManifestPath = kActionManifestPath;
            beatSaberManifest.defaultBindings?.Clear();

            var vrManifest = new VRApplicationManifest
            {
                applications = new[] { beatSaberManifest },
            };

            WriteBeatSaberManifest(kManifestPath, vrManifest);

            AppConfig appConfig = ReadAppConfig(kAppConfigPath);
            List<string> manifestPaths = appConfig.manifestPaths;
            var existing = manifestPaths.Where(p => string.Equals(p, kManifestPath, StringComparison.InvariantCultureIgnoreCase)).ToList();
            bool updated = false;

            // only rewrite if path isn't in list already or is not at the top
            if (manifestPaths.IndexOf(existing.FirstOrDefault()) != 0 || existing.Count > 1)
            {
                _logger.Info($"Adding '{kManifestPath}' to app config");

                foreach (string token in existing)
                {
                    manifestPaths.Remove(token);
                }

                manifestPaths.Insert(0, kManifestPath);

                updated = true;
            }
            else
            {
                _logger.Trace("Manifest is already in app config");
            }

            if (!manifestPaths.Any(p => string.Equals(p, kGlobalManifestPath, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.Info($"Adding '{kGlobalManifestPath}' to app config");

                manifestPaths.Add(kGlobalManifestPath);

                updated = true;
            }
            else
            {
                _logger.Trace("Global manifest is already in app config");
            }

            if (updated)
            {
                _updatedAppConfig = appConfig;
            }
        }

        private VRApplication ReadBeatSaberManifest(string globalManifestPath)
        {
            if (!File.Exists(globalManifestPath))
            {
                throw new FileNotFoundException($"Could not find file '{globalManifestPath}'");
            }

            VRApplication beatSaberManifest;

            _logger.Trace($"Reading '{globalManifestPath}'");

            using (var reader = new StreamReader(globalManifestPath))
            {
                VRApplicationManifest globalManifest = JsonConvert.DeserializeObject<VRApplicationManifest>(reader.ReadToEnd(), kJsonSerializerSettings);
                beatSaberManifest = globalManifest.applications?.FirstOrDefault(a => a.appKey == "steam.app.620980");
            }

            return beatSaberManifest ?? throw new Exception($"Beat Saber manifest not found in '{globalManifestPath}'");
        }

        private AppConfig ReadAppConfig(string configPath)
        {
            if (!File.Exists(configPath))
            {
                _logger.Warn($"Could not find file '{configPath}'");
                return new AppConfig();
            }

            _logger.Trace($"Reading '{configPath}'");

            AppConfig appConfig;

            using (var reader = new StreamReader(configPath))
            {
                appConfig = JsonConvert.DeserializeObject<AppConfig>(reader.ReadToEnd(), kJsonSerializerSettings) ?? new AppConfig();
            }

            return appConfig;
        }

        private void WriteBeatSaberManifest(string manifestPath, VRApplicationManifest beatSaberManifest)
        {
            _logger.Info($"Writing manifest to '{manifestPath}'");

            using StreamWriter writer = new(manifestPath);
            writer.Write(JsonConvert.SerializeObject(beatSaberManifest, kJsonSerializerSettings));
        }

        private void RegisterActionSet()
        {
            _logger.Info("Registering actions");

            // mappings are based on https://docs.unity3d.com/Manual/xr_input.html
            unityXRActions = new UnityXRActions
            {
                headsetOnHead = new BooleanInput("/actions/unity/in/headset_on_head"),
                left = new UnityXRActionsHand
                {
                    pose = new PoseInput("/actions/unity/in/left_pose"),
                    primaryButton = new BooleanInput("/actions/unity/in/left_primary_button"),
                    primaryTouch = new BooleanInput("/actions/unity/in/left_primary_touch"),
                    secondaryButton = new BooleanInput("/actions/unity/in/left_secondary_button"),
                    secondaryTouch = new BooleanInput("/actions/unity/in/left_secondary_touch"),
                    grip = new VectorInput("/actions/unity/in/left_grip"),
                    gripButton = new BooleanInput("/actions/unity/in/left_grip_button"),
                    trigger = new VectorInput("/actions/unity/in/left_trigger"),
                    triggerButton = new BooleanInput("/actions/unity/in/left_trigger_button"),
                    menuButton = new BooleanInput("/actions/unity/in/left_menu_button"),
                    primary2DAxis = new Vector2Input("/actions/unity/in/left_primary_2d_axis"),
                    primary2DAxisClick = new BooleanInput("/actions/unity/in/left_primary_2d_axis_click"),
                    primary2DAxisTouch = new BooleanInput("/actions/unity/in/left_primary_2d_axis_touch"),
                    skeleton = new SkeletalInput("/actions/unity/in/left_hand_animation", onlyUpdateSummaryData: false),
                    haptics = new HapticVibrationOutput("/actions/unity/out/left_haptics"),
                },
                right = new UnityXRActionsHand
                {
                    pose = new PoseInput("/actions/unity/in/right_pose"),
                    primaryButton = new BooleanInput("/actions/unity/in/right_primary_button"),
                    primaryTouch = new BooleanInput("/actions/unity/in/right_primary_touch"),
                    secondaryButton = new BooleanInput("/actions/unity/in/right_secondary_button"),
                    secondaryTouch = new BooleanInput("/actions/unity/in/right_secondary_touch"),
                    grip = new VectorInput("/actions/unity/in/right_grip"),
                    gripButton = new BooleanInput("/actions/unity/in/right_grip_button"),
                    trigger = new VectorInput("/actions/unity/in/right_trigger"),
                    triggerButton = new BooleanInput("/actions/unity/in/right_trigger_button"),
                    menuButton = new BooleanInput("/actions/unity/in/right_menu_button"),
                    primary2DAxis = new Vector2Input("/actions/unity/in/right_primary_2d_axis"),
                    primary2DAxisClick = new BooleanInput("/actions/unity/in/right_primary_2d_axis_click"),
                    primary2DAxisTouch = new BooleanInput("/actions/unity/in/right_primary_2d_axis_touch"),
                    skeleton = new SkeletalInput("/actions/unity/in/right_hand_animation", onlyUpdateSummaryData: false),
                    haptics = new HapticVibrationOutput("/actions/unity/out/right_haptics"),
                },
            };
        }
    }
}
