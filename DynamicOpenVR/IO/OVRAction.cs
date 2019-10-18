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
            Handle = OpenVRWrapper.GetActionHandle(GetActionPath(actionSetName));
        }
    }
}
