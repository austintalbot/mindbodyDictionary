using System.Net.Http;
using System.Threading.Tasks;

namespace MindBodyDictionary.Core.Client
{
	public class EmailsClient : MindBodyClient, IEmailsClient
	{
		public async Task<bool> Post(string email)
		{
			return await TryPutPost(HttpMethod.Post, $"SaveEmail?code=CZ41RSmPKVT2UcQDf4gaaeepMh6jU3tdXgkXwFc3A57FtJ6pr2t2fQ==", email, () => Post(email));
		}
	}
}
