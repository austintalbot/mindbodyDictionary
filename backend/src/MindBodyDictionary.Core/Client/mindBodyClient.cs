using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MindBodyDictionary.Core.Client
{
    public class MindBodyClient : HttpClient
    {
        //should move the below into a client Settings if we start using authentication?
        //Store the codes that go in the requests in the client?
        //Currently does not use the retry func
        private bool testingServer = false;

        protected MindBodyClient()
        {
            MaxResponseContentBufferSize = 999999999;
            if (testingServer)
            {
                BaseAddress = new Uri("https://mindbodydictionaryfunctions.azurewebsites.net/api/");
            }
            else
            {
                BaseAddress = new Uri("https://mindbodydictionaryfunctions.azurewebsites.net/api/");
            }

        }

        public async Task<T> TryGet<T>(string uri, Func<Task<T>> retryFunc)
        {
            HttpResponseMessage response = await this.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                JToken json = JToken.Parse(content);
                var toReturn = json.ToObject<T>();

                return toReturn;
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<T> TryPutPost<T>(HttpMethod method, string uri, string bodyContent, Func<Task<T>> retryFunc)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, uri);

            if (bodyContent != null)
            {
                request.Content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
            }
            HttpResponseMessage response = await this.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (content is T)
                {
                    return (T)Convert.ChangeType(content, typeof(T));
                }
                else
                {
                    if (typeof(T) == typeof(bool))
                    {
                        return (T)(object)true;
                    }
                    JToken json = JToken.Parse(content);
                    var toReturn = json.ToObject<T>();
                    return toReturn;
                }
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }


        public async Task<T> TryPut<T>(string url, string bodyContent, Func<Task<T>> retryFunc)
        {
            return await TryPutPost(HttpMethod.Put, url, bodyContent, retryFunc);
        }

        public async Task<T> TryPut<T>(string url, JToken bodyContent, Func<Task<T>> retryFunc)
        {
            return await TryPut(url, bodyContent.ToString(Formatting.None), retryFunc);
        }

        public async Task<T> TryPut<T>(string url, object bodyContent, Func<Task<T>> retryFunc)
        {
            if (bodyContent is string content)
            {
                return await TryPut(url, content, retryFunc);
            }
            else
            {
                return await TryPut(url, JToken.FromObject(bodyContent), retryFunc);
            }
        }

        public async Task<T> TryPost<T>(string url, string bodyContent, Func<Task<T>> retryFunc)
        {
            return await TryPutPost(HttpMethod.Post, url, bodyContent, retryFunc);
        }

        public async Task<T> TryPost<T>(string url, JToken bodyContent, Func<Task<T>> retryFunc)
        {
            return await TryPost(url, bodyContent.ToString(Formatting.None), retryFunc);
        }

        public async Task<T> TryPost<T>(string url, object bodyContent, Func<Task<T>> retryFunc)
        {
            if (bodyContent is string content)
            {
                return await TryPost(url, content, retryFunc);
            }
            else
            {
                return await TryPost(url, JToken.FromObject(bodyContent), retryFunc);
            }
        }
    }
}

