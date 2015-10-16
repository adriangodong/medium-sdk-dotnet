using System;
using Newtonsoft.Json;

namespace Medium.Helpers
{
    internal class UnixTimestampConverter : JsonConverter
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
            var timestamp = reader.Value as long?;
            return !timestamp.HasValue ? null : Helper.FromUnixTimestampMs(timestamp);
        }
    }
}