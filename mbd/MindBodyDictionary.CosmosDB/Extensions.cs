using Microsoft.Azure.Cosmos;

namespace MindBodyDictionary.CosmosDB
{
    public static class Extensions
    {
        public static async Task<List<T>> QueryAsync<T>(this CosmosClient client, string databaseName, string containerName, string query)
        {
            var container = client.GetContainer(databaseName, containerName);
            var queryDefinition = new QueryDefinition(query);
            var feed = container.GetItemQueryIterator<T>(queryDefinition);

            var toReturn = new List<T>();
            if (feed != null)
            {
                while (feed.HasMoreResults)
                {
                    var response = await feed.ReadNextAsync();
                    toReturn.AddRange(response);
                }
            }

            return toReturn;
        }

        public static async Task<T?> GetItemAsync<T>(this CosmosClient client, string databaseName, string containerName, string query, Func<T,bool> itemSelector)
        {
            var list = await client.QueryAsync<T>(
                databaseName: databaseName,
                containerName: containerName,
                query: query);

            if (list == null)
            {
                return default;
            }
            else
            {
                return list.Where(itemSelector).FirstOrDefault();
            }
        }
    }
}
