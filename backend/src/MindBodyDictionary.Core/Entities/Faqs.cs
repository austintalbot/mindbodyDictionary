using Newtonsoft.Json;

namespace MindBodyDictionary.Core.Entities
{
    public class Faqs
	{


            [JsonProperty(PropertyName = "question")]
            public string Question { get; set; }
            [JsonProperty(PropertyName = "shortAnswer")]
            public string ShortAnswer { get; set; }
            [JsonProperty(PropertyName = "answer")]
            public string Answer { get; set; }


    }
}
