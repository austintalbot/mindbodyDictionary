using System.Net.Http;
using System.Threading.Tasks;
using MindBodyDictionary.Core.Entities.PushNotifications;
using Newtonsoft.Json.Linq;

namespace MindBodyDictionary.Core.Client
{
	public class InstallationClient : MindBodyClient, IInstallationClient
	{
		public async Task<bool> Create(DeviceInstallation installation, bool isSubscribed)
		{
			return await TryPutPost(HttpMethod.Post, $"RegisterDevice?code=iWrpN7HvcERLyatAEtt7SDYc2xs9yQOQxZrFUqt7buS6eGgJBatluw==&subscribed={isSubscribed}", JObject.FromObject(installation).ToString(), () => Create(installation,isSubscribed));
		}
	}
}
