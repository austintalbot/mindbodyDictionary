using System.Threading.Tasks;

namespace MindBodyDictionary.Core.Client
{
    public interface IEmailsClient
    {
        Task<bool> Post(string email);
    }
}
