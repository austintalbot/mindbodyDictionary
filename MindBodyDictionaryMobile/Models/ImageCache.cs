namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a cached image in the local database.
/// </summary>
public class ImageCache
{
	public int ID { get; set; }
	public string FileName { get; set; } = string.Empty;
	public byte[] ImageData { get; set; } = [];
	public DateTime CachedAt { get; set; } = DateTime.UtcNow;
	public string ContentType { get; set; } = "image/png";

	public override string ToString() => FileName;
}
