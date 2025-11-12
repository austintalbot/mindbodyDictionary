using System.Threading.Tasks;
using MindBodyDictionary.Core.Entities.PushNotifications;

namespace MindBodyDictionary.Core.Client
{
    public interface IInstallationClient
    {
        Task<bool> Create(DeviceInstallation installation, bool isSubscribed);
    }
}
