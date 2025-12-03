namespace TestEv.Infrastructure.Configuration
{
    public class DynamoDbSettings
    {
        public const string SectionName = "DynamoDb";

        public string ServiceUrl { get; set; } = string.Empty;
        public string Region { get; set; } = "us-east-1";
        public string ProjectsTableName { get; set; } = "Projects";
        public bool? UseLocalDb { get; set; } = null;
    }
}
