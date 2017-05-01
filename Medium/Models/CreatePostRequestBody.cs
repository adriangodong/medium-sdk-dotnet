using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Medium.Models
{
    public class CreatePostRequestBody
    {
        public string Title { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentFormat ContentFormat { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
        public string CanonicalUrl { get; set; }
        public PublishStatus PublishStatus { get; set; }
        public License License { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool? NotifyFollowers { get; set; }
    }
}