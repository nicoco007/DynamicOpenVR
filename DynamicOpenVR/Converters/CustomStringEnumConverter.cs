using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace DynamicOpenVR.Converters
{
    class CustomStringEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enum);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type enumType = value.GetType();
            MemberInfo memberInfo = enumType.GetMember(value.ToString()).FirstOrDefault(m => m.DeclaringType == enumType);
            JsonEnumValueAttribute attribute = memberInfo.GetCustomAttribute<JsonEnumValueAttribute>();

            if (attribute == null)
            {
                writer.WriteValue(value.ToString());
            }

            writer.WriteValue(attribute.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
