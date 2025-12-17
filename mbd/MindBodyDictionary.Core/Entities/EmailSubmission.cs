using System;
using Newtonsoft.Json;

namespace MindBodyDictionary.Core.Entities
{
    public class EmailSubmission
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public string Email { get; set; }
        public DateTime SaveDateTime { get; set; }
    }
}
