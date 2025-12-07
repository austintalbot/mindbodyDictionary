using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

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
