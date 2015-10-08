using System;
using Medium.Models;
using Newtonsoft.Json;

namespace Medium.Helpers
{
    internal class LicenseEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Enum.Parse(typeof(License), reader.Value.ToString().Replace("-", string.Empty), true) ??
                   existingValue;
        }
    }
}