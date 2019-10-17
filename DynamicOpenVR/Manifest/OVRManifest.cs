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
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DynamicOpenVR.Manifest
{
    internal class OVRManifest
    {
        [JsonProperty(PropertyName = "actions")]
        internal readonly IList<OVRManifestAction> Actions = new List<OVRManifestAction>();

        [JsonProperty(PropertyName = "action_sets")]
        internal readonly IList<OVRManifestActionSet> ActionSets = new List<OVRManifestActionSet>();

        [JsonProperty(PropertyName = "default_bindings")]
        internal readonly IEnumerable<OVRDefaultBinding> DefaultBindings = new List<OVRDefaultBinding>() { new OVRDefaultBinding() { BindingUrl = "bindings_index_controller.json", ControllerType = "knuckles" } };

        [JsonProperty(PropertyName = "localization")]
        internal readonly IEnumerable<Dictionary<string, string>> Localization;

        internal OVRManifest(IEnumerable<OVRActionSet> actionSets)
        {
            var localization = new Dictionary<string, Dictionary<string, string>>();

            foreach (OVRActionSet actionSet in actionSets)
            {
                ActionSets.Add(new OVRManifestActionSet(actionSet));
                AddTranslations(localization, actionSet.GetTranslations(), actionSet.GetActionSetPath());

                foreach (OVRAction action in actionSet.Actions)
                {
                    Actions.Add(new OVRManifestAction(actionSet, action));
                    AddTranslations(localization, action.GetTranslations(), action.GetActionPath(actionSet.Name));
                }
            }

            Localization = localization.Values;
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
