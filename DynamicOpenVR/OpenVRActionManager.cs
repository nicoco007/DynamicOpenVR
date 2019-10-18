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
using System.Text;
using UnityEngine;

namespace DynamicOpenVR
{
	public class OpenVRActionManager : MonoBehaviour
	{
        public static readonly string ActionManifestFileName = Path.Combine(Environment.CurrentDirectory, "action_manifest.json");

        public static bool IsRunning => OpenVRWrapper.IsRunning;

        private static OpenVRActionManager instance;

        static OpenVRActionManager()
        {
            if (OpenVRWrapper.IsRunning && File.Exists(ActionManifestFileName))
            {
                // set early so OpenVR doesn't think no bindings exist at startup
                OpenVRWrapper.SetActionManifestPath(ActionManifestFileName);
            }
        }

        public static OpenVRActionManager Instance
		{
			get
			{
                if (!OpenVRWrapper.IsRunning)
                {
                    throw new InvalidOperationException("OpenVR is not running");
                }

				if (!instance)
				{
					GameObject go = new GameObject(nameof(OpenVRActionManager));
					DontDestroyOnLoad(go);
					instance = go.AddComponent<OpenVRActionManager>();
                }

				return instance;
			}
        }

        private Dictionary<string, OVRActionSet> actionSets = new Dictionary<string, OVRActionSet>();
        private bool instantiated = false;

        private void Start()
        {
            instantiated = true;

            WriteManifest();
                
            OpenVRWrapper.SetActionManifestPath(ActionManifestFileName);

            foreach (OVRActionSet actionSet in actionSets.Values)
            {
                actionSet.UpdateHandles();
            }
        }

        public void Update()
        {
            if (actionSets.Count > 0)
            {
                OpenVRWrapper.UpdateActionState(actionSets.Values.Select(actionSet => actionSet.Handle).ToArray());
            }
        }

        public void RegisterActionSet(OVRActionSet actionSet)
        {
            if (instantiated)
            {
                throw new InvalidOperationException("Cannot register new action sets once game is running");
            }

            actionSets.Add(actionSet.Name, actionSet);
        }

        public OVRActionSet GetActionSet(string actionSetName)
        {
            actionSetName = actionSetName.ToLowerInvariant();

            if (!actionSets.ContainsKey(actionSetName))
            {
                throw new ArgumentException($"Action set '{actionSetName}' has not been registered");
            }

            return actionSets[actionSetName];
        }

        public T GetAction<T>(string actionSetName, string actionName) where T : OVRAction
        {
            actionSetName = actionSetName.ToLowerInvariant();

            return GetActionSet(actionSetName).GetAction<T>(actionName);
        }

        private void WriteManifest()
		{
            OVRManifest manifest = new OVRManifest(actionSets.Values);

			byte[] jsonString = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(manifest, Formatting.Indented));

			using (FileStream stream = File.Open(ActionManifestFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                stream.SetLength(0);
                stream.Seek(0, SeekOrigin.Begin);
                stream.Write(jsonString, 0, jsonString.Length);
			}
		}
	}
}
