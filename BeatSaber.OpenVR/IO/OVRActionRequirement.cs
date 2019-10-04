using BeatSaber.OpenVR.Converters;
using Newtonsoft.Json;

namespace BeatSaber.OpenVR.IO
{
    [JsonConverter(typeof(CustomStringEnumConverter))]
    public enum OVRActionRequirement
    {
        [JsonEnumValue(Value = "mandatory")] Mandatory,
        [JsonEnumValue(Value = "suggested")] Suggested,
        [JsonEnumValue(Value = "optional")] Optional
    }
}
