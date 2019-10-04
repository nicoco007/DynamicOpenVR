using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BeatSaber.OpenVR.IO
{
    [JsonObject(MemberSerialization.OptIn)]
    public class OVRActionSet
    {
        [JsonProperty(PropertyName = "name")] internal string Name => $"/actions/{Key}";
        [JsonProperty(PropertyName = "usage")] internal OVRActionSetUsage Usage { get; }

        public string Key { get; }

        internal ulong Handle { get; private set; }
        internal IReadOnlyCollection<OVRAction> Actions => new ReadOnlyCollection<OVRAction>(actions.Values.ToList());
        internal IReadOnlyDictionary<string, string> Translations => new ReadOnlyDictionary<string, string>(translations);

        private Dictionary<string, OVRAction> actions = new Dictionary<string, OVRAction>();
        private Dictionary<string, string> translations = new Dictionary<string, string>();

        public OVRActionSet(string key, OVRActionSetUsage usage)
        {
            Key = key;
            Usage = usage;
        }

        public T RegisterAction<T>(T action) where T : OVRAction
        {
            if (action.Parent != null)
            {
                throw new ArgumentException($"Action '{action.Name}' already registered in action set '{action.Parent.Name}'");
            }

            action.Parent = this;
            actions.Add(action.Key, action);

            return action;
        }

        public T GetAction<T>(string key) where T : OVRAction
        {
            if (!actions.ContainsKey(key))
            {
                throw new ArgumentException($"Action '{key}' is not registered in '{Key}'");
            }

            if (!(actions[key] is T))
            {
                throw new ArgumentException($"Action '{key}' is not of type '{typeof(T).Name}'");
            }

            return (T)actions[key];
        }

        public OVRActionSet AddTranslation(string language, string text)
        {
            if (translations.ContainsKey(language))
            {
                translations[language] = text;
            }
            else
            {
                translations.Add(language, text);
            }

            return this;
        }

        internal void UpdateHandle()
        {
            Handle = OpenVRApi.GetActionSetHandle(Name);
        }
    }
}
