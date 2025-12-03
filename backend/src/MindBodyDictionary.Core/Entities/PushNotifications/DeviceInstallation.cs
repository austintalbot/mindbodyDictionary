namespace MindBodyDictionary.Core.Entities.PushNotifications
{
	public class DeviceInstallation
	{
		public string InstallationId { get; set; }
		public string DeviceID { get; set; }
		public string Platform { get; set; }
		public string Handle { get; set; }
		public string[] Tags { get; set; }
	}
}
