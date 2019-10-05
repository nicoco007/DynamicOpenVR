using BeatSaber.OpenVR.IO;
using BeatSaber.OpenVR.Model;
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

		private readonly string fileName = Path.Combine(Environment.CurrentDirectory, "action_manifest.json");

        private Dictionary<string, OVRActionSet> actionSets = new Dictionary<string, OVRActionSet>();
        private bool instantiated = false;

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

        private void Start()
        {
            instantiated = true;

            WriteManifest();

            OpenVRApi.SetActionManifestPath(fileName);

            foreach (OVRActionSet actionSet in actionSets.Values)
            {
                actionSet.UpdateHandle();

                foreach (OVRAction action in actionSet.Actions)
                {
                    action.UpdateHandle();
                }
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

            actionSets.Add(actionSet.Key, actionSet);
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
			OVRActionManifest manifest = new OVRActionManifest()
			{
				ActionSets = actionSets.Values,
				Actions = actionSets.Values.SelectMany(set => set.Actions),
                Localization = GetLocalizationList()
			};

			byte[] jsonString = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(manifest, Formatting.Indented));

			using (FileStream stream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				Plugin.Logger.Info("Writing OpenVR manifest");

                stream.SetLength(0);
				stream.Seek(0, SeekOrigin.Begin);
				stream.Write(jsonString, 0, jsonString.Length);
			}
		}

        private IEnumerable<Dictionary<string, string>> GetLocalizationList()
        {
            var localization = new Dictionary<string, Dictionary<string, string>>();

            foreach (OVRActionSet actionSet in actionSets.Values)
            {
                AddTranslations(localization, actionSet.Translations, actionSet.Name);

                foreach (OVRAction action in actionSet.Actions)
                {
                    AddTranslations(localization, action.Translations, action.Name);
                }
            }

            return localization.Values;
        }

        private void AddTranslations(Dictionary<string, Dictionary<string, string>> localization, IReadOnlyDictionary<string, string> translations, string key)
        {
            foreach (KeyValuePair<string, string> translation in translations)
            {
                if (!localization.ContainsKey(translation.Key))
                {
                    localization.Add(translation.Key, new Dictionary<string, string>());
                    localization[translation.Key].Add("language_tag", translation.Key);
                }

                localization[translation.Key].Add(key, translation.Value);
            }
        }
	}
}
