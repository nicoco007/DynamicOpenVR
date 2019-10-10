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
