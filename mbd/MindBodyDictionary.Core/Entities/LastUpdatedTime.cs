using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MindBodyDictionary.Core.Entities
{
    public class LastUpdatedTime
    {

        private DateTime _lastUpdated;

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string id { get; set; }

      [JsonProperty(PropertyName = "lastUpdate", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set {
                SummaryNegative = value.ToString();
                _lastUpdated = value;
             }
        }


        [JsonProperty(PropertyName = "summaryNegative", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SummaryNegative { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string name { get; set; }
    }
}
