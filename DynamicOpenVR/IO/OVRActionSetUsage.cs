using DynamicOpenVR.Converters;
using Newtonsoft.Json;

namespace DynamicOpenVR.IO
{
    [JsonConverter(typeof(CustomStringEnumConverter))]
    public enum OVRActionSetUsage
    {
        [JsonEnumValue(Value = "leftright")] LeftRight,
        [JsonEnumValue(Value = "single")] Single,
        [JsonEnumValue(Value = "hidden")] Hidden
    }
}
