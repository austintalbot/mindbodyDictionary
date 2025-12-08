using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

public class MbdCondition : MindBodyDictionary.Shared.Entities.MbdCondition
{
	public new List<Tag> Tags { get; set; } = [];
}

/// <summary>
/// Represents a mind-body condition with associated tasks and tags.
/// </summary>
/// <summary>
/// Root object for JSON deserialization of conditions data.
/// </summary>
public class MbdMbdConditionsJson
{
	public List<MbdCondition> Conditions { get; set; } = [];
}
