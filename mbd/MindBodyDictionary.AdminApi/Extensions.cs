
namespace MindBodyDictionary.AdminApi
{
	internal static class Extensions
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
	}
}
