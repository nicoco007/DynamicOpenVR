using BeatSaber.OpenVR.IO;
using BeatSaber.OpenVR.Manifest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BeatSaber.OpenVR
{
	public class OpenVRActionManager : MonoBehaviour
	{
		private static OpenVRActionManager instance;

        public static OpenVRActionManager Instance
		{
			get
			{
				if (!instance)
				{
					GameObject go = new GameObject(nameof(OpenVRActionManager));
					DontDestroyOnLoad(go);
					instance = go.AddComponent<OpenVRActionManager>();
                }

				return instance;
			}
        }

        private readonly string FileName = Path.Combine(Environment.CurrentDirectory, "action_manifest.json");

        private Dictionary<string, OVRActionSet> actionSets = new Dictionary<string, OVRActionSet>();
        private bool instantiated = false;

        private void Start()
        {
            instantiated = true;

            WriteManifest();

            OpenVRApi.SetActionManifestPath(FileName);

            foreach (OVRActionSet actionSet in actionSets.Values)
            {
                actionSet.UpdateHandles();
            }
        }

        public void Update()
        {
            OpenVRApi.UpdateActionState(actionSets.Values.Select(actionSet => actionSet.Handle).ToArray());
        }

        public void RegisterActionSet(OVRActionSet actionSet)
        {
            if (instantiated)
            {
                throw new InvalidOperationException("Cannot register new action sets once game is running");
            }

            actionSets.Add(actionSet.Name, actionSet);
        }

        public OVRActionSet GetActionSet(string actionSetKey)
        {
            if (!actionSets.ContainsKey(actionSetKey))
            {
                throw new ArgumentException($"Action set '{actionSetKey}' has not been registered");
            }

            return actionSets[actionSetKey];
        }

        public T GetAction<T>(string actionSetName, string actionName) where T : OVRAction
        {
            return GetActionSet(actionSetName).GetAction<T>(actionName);
        }

        private void WriteManifest()
		{
            OVRManifest manifest = new OVRManifest(actionSets.Values);

			byte[] jsonString = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(manifest, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

			using (FileStream stream = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				Plugin.Logger.Info("Writing OpenVR manifest");

                stream.SetLength(0);
				stream.Seek(0, SeekOrigin.Begin);
				stream.Write(jsonString, 0, jsonString.Length);
			}
		}
	}
}
