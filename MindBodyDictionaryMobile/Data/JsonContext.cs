using System.Text.Json.Serialization;
using MindBodyDictionary.Shared.Entities;
using MindBodyDictionaryMobile.Models;

[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(ProjectsJson))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Tag))]
public partial class JsonContext : JsonSerializerContext
{
}
