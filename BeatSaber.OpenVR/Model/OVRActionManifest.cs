using BeatSaber.OpenVR.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BeatSaber.OpenVR.Model
{
    internal class OVRActionManifest
    {
        [JsonProperty(PropertyName = "actions")]
        internal IEnumerable<OVRAction> Actions;

        [JsonProperty(PropertyName = "action_sets")]
        internal IEnumerable<OVRActionSet> ActionSets;

        [JsonProperty(PropertyName = "default_bindings")]
        internal IEnumerable<DefaultBinding> DefaultBindings = new List<DefaultBinding>() { new DefaultBinding() { BindingUrl = "bindings_index_controller.json", ControllerType = "knuckles" } };

        [JsonProperty(PropertyName = "localization")]
        internal IEnumerable<Dictionary<string, string>> Localization;
	}
}
