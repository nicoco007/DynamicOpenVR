using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BeatSaber.OpenVR.IO
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class OVRAction
    {
        public string Name { get; }
        public OVRActionRequirement Requirement { get; }

        internal string Type { get; }
        internal string Direction { get; set; }
        internal ulong Handle { get; private set; }
        internal IReadOnlyDictionary<string, string> Translations => new ReadOnlyDictionary<string, string>(translations);

        private Dictionary<string, string> translations = new Dictionary<string, string>();

        protected OVRAction(string name, OVRActionRequirement requirement, string type, string direction)
        {
            Name = name;
            Requirement = requirement;
            Type = type;
            Direction = direction;
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
        
        internal string GetActionPath(string actionSetName)
        {
            return $"/actions/{actionSetName}/{Direction}/{Name}";
        }

        internal void UpdateHandle(string actionSetName)
        {
            Handle = OpenVRApi.GetActionHandle(GetActionPath(actionSetName));
        }
	}
}
