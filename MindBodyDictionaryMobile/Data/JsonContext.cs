using System.Text.Json.Serialization;
using MindBodyDictionaryMobile.Models;

[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(ProjectTask))]
[JsonSerializable(typeof(ProjectsJson))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(MindBodyDictionaryMobile.Models.Tag))]
public partial class JsonContext : JsonSerializerContext { }
