using System.Collections.Generic;
using Newtonsoft.Json;

namespace MindBodyDictionary.Core.Entities
{
    public class Ailment
    {
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string id { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "summaryNegative", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SummaryNegative { get; set; }

        [JsonProperty(PropertyName = "summaryPositive", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SummaryPositive { get; set; }
        
        [JsonProperty(PropertyName = "affirmations", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Affirmations { get; set; }
        
        [JsonProperty(PropertyName = "physicalConnections", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> PhysicalConnections { get; set; }

        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Tags { get; set; }

        [JsonProperty(PropertyName = "recommendations", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Recommendation> Recommendations { get; set; }
        
        [JsonProperty(PropertyName = "imageShareOverrideAilmentName", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ImageShareOverrideAilmentName { get; set; }

        [JsonProperty(PropertyName = "subscriptionOnly")]
        public bool SubscriptionOnly { get; set; }

        [JsonIgnore]
        public string ImageOneUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ImageShareOverrideAilmentName))
                {
                    return $"{Storage.ImageBasePath}/{ImageShareOverrideAilmentName}1.png";
                }
                else
                {
                    return $"{Storage.ImageBasePath}/{Name}1.png";
                }
               
            }
        }

        [JsonIgnore]
        public string ImageTwoUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ImageShareOverrideAilmentName))
                {
                    return $"{Storage.ImageBasePath}/{ImageShareOverrideAilmentName}2.png";
                }
                else
                {
                    return $"{Storage.ImageBasePath}/{Name}2.png";
                }

            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}