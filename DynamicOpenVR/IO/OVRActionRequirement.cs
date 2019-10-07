using DynamicOpenVR.Converters;
using Newtonsoft.Json;

namespace DynamicOpenVR.IO
{
    [JsonConverter(typeof(CustomStringEnumConverter))]
    public enum OVRActionRequirement
    {
        [JsonEnumValue(Value = "mandatory")] Mandatory,
        [JsonEnumValue(Value = "suggested")] Suggested,
        [JsonEnumValue(Value = "optional")] Optional
    }
}
