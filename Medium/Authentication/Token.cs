using System;
using Medium.Helpers;
using Newtonsoft.Json;

namespace Medium.Authentication
{
    public class Token
    {
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        public Scope[] Scope { get; set; }

        [JsonProperty(PropertyName = "expires_at")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime ExpiresAt { get; set; }
    }
}