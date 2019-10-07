using System;

namespace DynamicOpenVR.Converters
{
    internal class JsonEnumValueAttribute : Attribute
    {
        public string Value { get; set; }
    }
}
