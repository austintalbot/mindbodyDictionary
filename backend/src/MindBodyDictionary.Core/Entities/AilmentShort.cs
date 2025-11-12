using System.Collections.Generic;
using Newtonsoft.Json;

namespace MindBodyDictionary.Core.Entities
{
    public class AilmentShort
    {
        [JsonProperty(PropertyName = "id")] public string Id { get; set; }
        public string Name { get; set; }
        public List<string> PhysicalConnections { get; set; }
        public List<string> Tags { get; set; }
        public bool SubscriptionOnly { get; set; }

        #region Maybe

        public string imageShareOverrideAilmentName { get; set; }

        [JsonIgnore]
        public string imageOneUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(imageShareOverrideAilmentName))
                {
                    return $"https://mdbfunctionstorage.blob.core.windows.net/mdb-images/{imageShareOverrideAilmentName}2.png";
                }
                else
                {
                    return $"https://mdbfunctionstorage.blob.core.windows.net/mdb-images/{Name}2.png";
                }

            }
        }

        [JsonIgnore]
        public string imageTwoUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(imageShareOverrideAilmentName))
                {
                    return $"https://mdbfunctionstorage.blob.core.windows.net/mdb-images/{imageShareOverrideAilmentName}2.png";
                }
                else
                {
                    return $"https://mdbfunctionstorage.blob.core.windows.net/mdb-images/{Name}2.png";
                }

            }
        }

        #endregion

       
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
