namespace MindBodyDictionaryMobile.Models;

using System;

public class UserListItem
{
  public int ID { get; set; }
  public string Name { get; set; }
  public string Url { get; set; }
  public int RecommendationType { get; set; }
  public DateTime AddedAt { get; set; }
  public bool IsCompleted { get; set; }
}
