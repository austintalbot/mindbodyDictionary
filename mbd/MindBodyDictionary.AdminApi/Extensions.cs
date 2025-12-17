using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MindBodyDictionary.AdminApi;

public static class Extensions
{
    /// <summary>
    /// Formats an Ienumerable as a Json that works within DataTables
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static JObject ToDataTableJson<TSource>(this IEnumerable<TSource> source)
    {
        Console.WriteLine("ToDataTableJson function");
        JObject listJObject = new()
        {
            { "data", JArray.FromObject(source) }
        };

        return listJObject;
    }

    public static string ToMbdImageName(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // Remove extension if present
        var name = Path.GetFileNameWithoutExtension(input);

        // Identify suffix and mapping
        string suffix = "";

        // Check for numeric suffixes 1 and 2 first (Safe check at end of string)
        if (name.EndsWith("1"))
        {
            suffix = "Negative";
            name = name.Substring(0, name.Length - 1);
        }
        else if (name.EndsWith("2"))
        {
            suffix = "Positive";
            name = name.Substring(0, name.Length - 1);
        }
        else if (name.EndsWith("Negative", StringComparison.OrdinalIgnoreCase))
        {
            suffix = "Negative";
            name = name.Substring(0, name.Length - "Negative".Length);
        }
        else if (name.EndsWith("Positive", StringComparison.OrdinalIgnoreCase))
        {
            suffix = "Positive";
            name = name.Substring(0, name.Length - "Positive".Length);
        }
        else if (name.Contains("Negative", StringComparison.OrdinalIgnoreCase))
        {
            suffix = "Negative";
            name = Regex.Replace(name, "Negative", "", RegexOptions.IgnoreCase);
        }
        else if (name.Contains("Positive", StringComparison.OrdinalIgnoreCase))
        {
            suffix = "Positive";
            name = Regex.Replace(name, "Positive", "", RegexOptions.IgnoreCase);
        }

        // Split into words
        var words = name.Split(new[] { ' ', '-', '_', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < words.Length; i++)
        {
            var word = words[i].ToLower();
            // Remove non-alphanumeric
            word = Regex.Replace(word, "[^a-z0-9]", "");
            if (string.IsNullOrEmpty(word)) continue;

            if (sb.Length > 0)
            {
                // Capitalize first letter for subsequent words
                word = char.ToUpper(word[0]) + word.Substring(1);
            }
            sb.Append(word);
        }

        if (!string.IsNullOrEmpty(suffix))
        {
            sb.Append(suffix);
        }

        return sb.ToString();
    }
}
