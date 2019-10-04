using BeatSaber.OpenVR.Converters;
using Newtonsoft.Json;

namespace BeatSaber.OpenVR.IO
{
    [JsonConverter(typeof(CustomStringEnumConverter))]
    public enum OVRActionSetUsage
    {
        [JsonEnumValue(Value = "leftright")] LeftRight,
        [JsonEnumValue(Value = "single")] Single,
        [JsonEnumValue(Value = "hidden")] Hidden
    }
}
