// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
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

using DynamicOpenVR.IO;
using DynamicOpenVR.Manifest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DynamicOpenVR.DefaultBindings;
using UnityEngine;
using UnityEngine.XR;

namespace DynamicOpenVR
{
    public class OpenVRActionManager : MonoBehaviour
    {
        public static readonly string kActionManifestPath = Path.Combine(Environment.CurrentDirectory, "DynamicOpenVR", "action_manifest.json");

        // TODO maybe move this to another more general class since it's checking for OpenVR's status, not OpenVRActionManager's status
        public static bool isRunning
        {
            get
            {
                if (NativeMethods.LoadLibrary("openvr_api.dll") == IntPtr.Zero) return false;
                if (string.Compare(XRSettings.loadedDeviceName, "OpenVR", StringComparison.InvariantCultureIgnoreCase) != 0) return false;
                if (!OpenVRWrapper.isRuntimeInstalled) return false;

                return true;
            }
        }

        private static OpenVRActionManager _instance;

        public static OpenVRActionManager instance
		{
			get
			{
                if (!isRunning)
                {
                    throw new InvalidOperationException("OpenVR is not running");
                }

				if (!_instance)
				{
					GameObject go = new GameObject(nameof(OpenVRActionManager));
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<OpenVRActionManager>();
                }

				return _instance;
			}
        }

        private Dictionary<string, OVRAction> _actions = new Dictionary<string, OVRAction>();
        private ulong[] _actionSetHandles;
        private bool _instantiated = false;

        private void Start()
        {
            _instantiated = true;
            
            CombineAndWriteManifest();
                
            OpenVRWrapper.SetActionManifestPath(kActionManifestPath);

            List<string> actionSetNames = _actions.Values.Select(action => action.GetActionSetName()).Distinct().ToList();
            _actionSetHandles = new ulong[actionSetNames.Count];

            for (int i = 0; i < actionSetNames.Count; i++)
            {
                _actionSetHandles[i] = OpenVRWrapper.GetActionSetHandle(actionSetNames[i]);
            }

            foreach (var action in _actions.Values)
            {
                action.UpdateHandle();
            }
        }

        public void Update()
        {
            if (_actionSetHandles != null)
            {
                OpenVRWrapper.UpdateActionState(_actionSetHandles);
            }
        }

        public T RegisterAction<T>(T action) where T : OVRAction
        {
            if (_instantiated)
            {
                throw new InvalidOperationException("Cannot register new actions once game is running");
            }

            if (_actions.ContainsKey(action.name))
            {
                throw new InvalidOperationException($"An action with the name '{action.name}' was already registered.");
            }

            _actions.Add(action.name, action);

            return action;
        }

        private void CombineAndWriteManifest()
		{
            string[] actionFiles = Directory.GetFiles("DynamicOpenVR/Actions");
            var actionManifests = new List<ActionManifest>();
            ushort version = 0;

            foreach (string actionFile in actionFiles)
            {
                try
                {
                    using (var reader = new StreamReader(actionFile))
                    {
                        string data = reader.ReadToEnd();
                        actionManifests.Add(JsonConvert.DeserializeObject<ActionManifest>(data));
                        version += BitConverter.ToUInt16(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(data)), 0);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"An error of type {ex.GetType().FullName} occured when trying to parse {actionFile}: {ex.Message}");
                }
            }

            List<ManifestDefaultBinding> defaultBindings = CombineAndWriteBindings(version);

            using (var writer = new StreamWriter(kActionManifestPath))
            {
                var manifest = new ActionManifest()
                {
                    version = version,
                    actions = actionManifests.SelectMany(m => m.actions).ToList(),
                    actionSets = actionManifests.SelectMany(m => m.actionSets).ToList(),
                    defaultBindings = defaultBindings,
                    localization = CombineLocalizations(actionManifests)
                };

                writer.WriteLine(JsonConvert.SerializeObject(manifest, Formatting.Indented));
            }
		}

        private List<ManifestDefaultBinding> CombineAndWriteBindings(int manifestVersion)
        {
            string[] bindingFiles = Directory.GetFiles("DynamicOpenVR/Bindings");
            var defaultBindings = new List<DefaultBinding>();

            foreach (string bindingFile in bindingFiles)
            {
                try
                {
                    using (var reader = new StreamReader(bindingFile))
                    {
                        defaultBindings.Add(JsonConvert.DeserializeObject<DefaultBinding>(reader.ReadToEnd()));
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"An error of type {ex.GetType().FullName} occured when trying to parse {bindingFile}: {ex.Message}");
                }
            }

            var combinedBindings = new List<ManifestDefaultBinding>();

            foreach (string controllerType in defaultBindings.Select(b => b.controllerType).Distinct())
            {
                var defaultBinding = new DefaultBinding
                {
                    actionManifestVersion = manifestVersion,
                    name = "Default Beat Saber Bindings",
                    description = "Action bindings for Beat Saber.",
                    controllerType = controllerType,
                    category = "steamvr_input",
                    bindings = MergeBindings(defaultBindings.Where(b => b.controllerType == controllerType))
                };

                string fileName = $"default_bindings_{defaultBinding.controllerType}.json";
                combinedBindings.Add(new ManifestDefaultBinding { ControllerType = controllerType, BindingUrl = fileName });

                using (StreamWriter writer = new StreamWriter(Path.Combine("DynamicOpenVR", fileName)))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(defaultBinding, Formatting.Indented));
                }
            }

            return combinedBindings;
        }

        private Dictionary<string, BindingCollection> MergeBindings(IEnumerable<DefaultBinding> bindingSets)
        {
            var final = new Dictionary<string, BindingCollection>();

            foreach (var bindingSet in bindingSets)
            {
                foreach (KeyValuePair<string, BindingCollection> kvp in bindingSet.bindings)
                {
                    string actionSetName = kvp.Key;
                    BindingCollection bindings = kvp.Value;

                    if (!final.ContainsKey(actionSetName))
                    {
                        final.Add(actionSetName, new BindingCollection());
                    }
                    
                    final[actionSetName].chords.AddRange(bindings.chords);
                    final[actionSetName].haptics.AddRange(bindings.haptics);
                    final[actionSetName].poses.AddRange(bindings.poses);
                    final[actionSetName].skeleton.AddRange(bindings.skeleton);
                    final[actionSetName].sources.AddRange(bindings.sources);
                }
            }

            return final;
        }

        private List<Dictionary<string, string>> CombineLocalizations(IEnumerable<ActionManifest> manifests)
        {
            var combinedLocalizations = new Dictionary<string, Dictionary<string, string>>();

            foreach (var manifest in manifests)
            {
                foreach (var language in manifest.localization)
                {
                    if (!language.ContainsKey("language_tag"))
                    {
                        continue;
                    }

                    if (!combinedLocalizations.ContainsKey(language["language_tag"]))
                    {
                        combinedLocalizations.Add(language["language_tag"], new Dictionary<string, string>() { {"language_tag", language["language_tag"] } });
                    }

                    foreach (var kvp in language.Where(kvp => kvp.Key != "language_tag"))
                    {
                        if (combinedLocalizations.ContainsKey(kvp.Key))
                        {
                            Debug.LogWarning($"Duplicate entry {kvp.Key}");
                        }
                        else
                        {
                            combinedLocalizations[language["language_tag"]].Add(kvp.Key, kvp.Value);
                        }
                    }
                }
            }

            return combinedLocalizations.Values.ToList();
        }
	}
}
