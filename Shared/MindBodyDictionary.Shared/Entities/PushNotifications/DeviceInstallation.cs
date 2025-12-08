namespace MindBodyDictionary.Shared.Entities;

public class DeviceInstallation
{
	public string? InstallationId { get; set; }
	public string? DeviceID { get; set; }
	public string? Platform { get; set; }
	public string? PushChannel { get; set; }
	public List<string> Tags { get; set; } = new();
}
