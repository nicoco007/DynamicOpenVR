using DynamicOpenVR.IO;
using Newtonsoft.Json;

namespace DynamicOpenVR.Manifest
{
    internal class OVRManifestAction
    {
        [JsonProperty(PropertyName = "name")]
        internal readonly string Name;

        [JsonProperty(PropertyName = "requirement")]
        internal readonly OVRActionRequirement Requirement;

        [JsonProperty(PropertyName = "type")]
        internal readonly string Type;

        [JsonProperty(PropertyName = "skeleton", NullValueHandling = NullValueHandling.Ignore)]
        internal readonly string Skeleton;

        internal OVRManifestAction(OVRActionSet actionSet, OVRAction action)
        {
            Name = action.GetActionPath(actionSet.Name);
            Requirement = action.Requirement;
            Type = action.Type;
            Skeleton = (action as SkeletalInput)?.Skeleton;
        }
    }
}
