using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicOpenVR.Bindings;

namespace DynamicOpenVR.IO
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class OVRAction
    {
        public string Name { get; }
        public OVRActionRequirement Requirement { get; }

        internal string Type { get; }
        internal string Direction { get; set; }
        internal ulong Handle { get; private set; }

        private Dictionary<string, string> translations = new Dictionary<string, string>();
        protected Dictionary<string, OVRBinding> bindings = new Dictionary<string, OVRBinding>();

        protected OVRAction(string name, OVRActionRequirement requirement, string type, string direction)
        {
            Name = name.ToLowerInvariant();
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

        public IReadOnlyDictionary<string, string> GetTranslations()
        {
            return new ReadOnlyDictionary<string, string>(translations);
        }

        internal IEnumerable<OVRBinding> GetBindings()
        {
            return bindings.Values;
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
