using System.Text.Json.Serialization;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// JSON serialization context for the application.
/// Provides type-safe, compile-time JSON serialization/deserialization for model classes.
/// </summary>
[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(ProjectTask))]
[JsonSerializable(typeof(ProjectsJson))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Tag))]
public partial class JsonContext : JsonSerializerContext
{
}
