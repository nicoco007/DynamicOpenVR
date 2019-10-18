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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DynamicOpenVR.IO
{
    [JsonObject(MemberSerialization.OptIn)]
    public class OVRActionSet
    {
        public string Name { get; }
        public OVRActionSetUsage Usage { get; }

        internal ulong Handle { get; private set; }
        internal IReadOnlyCollection<OVRAction> Actions => new ReadOnlyCollection<OVRAction>(actions.Values.ToList());
        internal IReadOnlyDictionary<string, string> Translations => new ReadOnlyDictionary<string, string>(translations);

        private Dictionary<string, OVRAction> actions = new Dictionary<string, OVRAction>();
        private Dictionary<string, string> translations = new Dictionary<string, string>();

        public OVRActionSet(string name, OVRActionSetUsage usage)
        {
            Name = name.ToLowerInvariant();
            Usage = usage;
        }

        public T RegisterAction<T>(T action) where T : OVRAction
        {
            actions.Add(action.Name, action);

            return action;
        }

        public T GetAction<T>(string name) where T : OVRAction
        {
            name = name.ToLowerInvariant();

            if (!actions.ContainsKey(name))
            {
                throw new ArgumentException($"Action '{name}' is not registered in '{Name}'");
            }

            if (!(actions[name] is T))
            {
                throw new ArgumentException($"Action '{name}' is not of type '{typeof(T).Name}'");
            }

            return (T)actions[name];
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

        public IReadOnlyDictionary<string, string> GetTranslations()
        {
            return new ReadOnlyDictionary<string, string>(translations);
        }

        internal string GetActionSetPath()
        {
            return $"/actions/{Name}";
        }

        internal void UpdateHandles()
        {
            Handle = OpenVRWrapper.GetActionSetHandle(GetActionSetPath());

            foreach (OVRAction action in actions.Values)
            {
                action.UpdateHandle(Name);
            }
        }
    }
}
