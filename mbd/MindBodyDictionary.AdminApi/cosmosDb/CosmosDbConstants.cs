namespace backend.CosmosDB;

public static class CosmosDbConstants
{
    public const string DatabaseName = "MindBodyDictionary";
    public const string LastUpdatedTimeID = "LastUpdatedTime";

    public static class Containers
    {
        public const string MbdConditions = "MbdConditions";
        public const string Emails = "Emails";
		    public const string Faqs = "Faqs";
        public const string System = "System";
        public const string MbdMovementLinks = "MbdMovementLinks";
        public const string LastUpdatedTime = "LastUpdatedTime";
    }
}
