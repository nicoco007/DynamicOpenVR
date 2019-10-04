using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BeatSaber.OpenVR.IO
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class OVRAction
    {
        [JsonProperty(PropertyName = "name")] internal string Name => $"/actions/{Parent.Key}/{direction}/{Key}";
        [JsonProperty(PropertyName = "requirement")] internal OVRActionRequirement Requirement { get; }
        [JsonProperty(PropertyName = "type")] internal string Type { get; }

        public string Key { get; }

        internal OVRActionSet Parent { get; set; }
        internal ulong Handle { get; private set; }
        internal IReadOnlyDictionary<string, string> Translations => new ReadOnlyDictionary<string, string>(translations);

        private string direction;
        private Dictionary<string, string> translations = new Dictionary<string, string>();

        protected OVRAction(string key, OVRActionRequirement requirement, string type, string direction)
        {
            Key = key;
            Requirement = requirement;
            Type = type;

            this.direction = direction;
        }

        public OVRAction AddTranslation(string language, string text)
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
            Handle = OpenVRApi.GetActionHandle(Name);
        }
	}
}
