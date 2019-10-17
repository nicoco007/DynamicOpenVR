// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019 Nicolas Gnyra

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.

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
