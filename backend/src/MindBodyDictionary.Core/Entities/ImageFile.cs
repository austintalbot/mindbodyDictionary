using System;
using Newtonsoft.Json;

namespace MindBodyDictionary.Core.Entities
{
    public class ImageFile
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("conditionId")]
        public string ConditionId { get; set; } = string.Empty;

        [JsonProperty("filePath")]
        public string FilePath { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonProperty("tags")]
        public string[]? Tags { get; set; }
    }
}
