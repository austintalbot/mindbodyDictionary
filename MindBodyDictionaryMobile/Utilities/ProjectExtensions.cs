using System.Diagnostics.CodeAnalysis;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Utilities;

/// <summary>
/// Project Model Extensions
/// </summary>
public static class ProjectExtensions
{
	/// <summary>
	/// Check if the project is null or new.
	/// </summary>
	/// <param name="project"></param>
	/// <returns></returns>
	public static bool IsNullOrNew([NotNullWhen(false)] this Project? project) => project is null || project.ID == 0;

	/// <summary>
	/// Check if the condition is null or new.
	/// </summary>
	/// <param name="condition"></param>
	/// <returns></returns>
	public static bool IsNullOrNew([NotNullWhen(false)] this MbdCondition? condition) => condition is null || condition.ID == 0;
}
