namespace MindBodyDictionaryMobile.Models;

using System.Text.Json.Serialization;

public class ConditionTask
{
  public int ID { get; set; }
  public string Title { get; set; } = string.Empty;
  public bool IsCompleted { get; set; }

  [JsonIgnore]
  public string MbdConditionID { get; set; }
}
