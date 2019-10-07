using DynamicOpenVR.IO;
using Newtonsoft.Json;

namespace DynamicOpenVR.Manifest
{
    internal class OVRManifestActionSet
    {
        [JsonProperty(PropertyName = "name")]
        internal readonly string Name;

        [JsonProperty(PropertyName = "usage")]
        internal readonly OVRActionSetUsage Usage;

        internal OVRManifestActionSet(OVRActionSet actionSet)
        {
            Name = actionSet.GetActionSetPath();
            Usage = actionSet.Usage;
        }
    }
}
