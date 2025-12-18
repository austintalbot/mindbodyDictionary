namespace MindBodyDictionary.Core
{
    public static class Storage
    {
        public const string ConnectionStringSetting = "CONNECTION_STORAGE";

        public const string ImageBasePath = "https://mbdstoragesa.blob.core.windows.net/mbd-images";

        public static class Containers
        {
            public const string Images = "mbd-images";
        }
    }

    public static class CosmosDB
    {
        public const string LastUpdatedTimeID = "LastUpdatedTime";
        public const string DatabaseName = "MindBodyDictionary";

        public const string ConnectionStringSetting = "CONNECTION_COSMOSDB";

        public static class Containers
        {
            public const string Ailments = "Ailments";
            public const string Faqs = "Faqs";
            public const string Emails = "Emails";
            public const string LastUpdatedTime = "LastUpdatedTime";
        }
    }

    public static class Notifications
    {
        public const string HubName = "mbdnotificationshub";
    }
}
