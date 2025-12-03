using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TestEv.Infrastructure.Configuration;

namespace TestEv.Infrastructure.Persistence
{
    public class DynamoDbTableInitializer
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly DynamoDbSettings _settings;
        private readonly ILogger<DynamoDbTableInitializer> _logger;

        public DynamoDbTableInitializer(
            IAmazonDynamoDB dynamoDbClient,
            IOptions<DynamoDbSettings> settings,
            ILogger<DynamoDbTableInitializer> logger)
        {
            _dynamoDbClient = dynamoDbClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            await CreateTableIfNotExistsAsync(_settings.ProjectsTableName, "Id");
        }

        private async Task CreateTableIfNotExistsAsync(string tableName, string partitionKeyName)
        {
            try
            {
                await _dynamoDbClient.DescribeTableAsync(tableName);
                _logger.LogInformation("Table {TableName} already exists", tableName);
            }
            catch (ResourceNotFoundException)
            {
                _logger.LogInformation("Creating table {TableName}", tableName);

                var request = new CreateTableRequest
                {
                    TableName = tableName,
                    AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = partitionKeyName,
                        AttributeType = "S"
                    }
                },
                    KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = partitionKeyName,
                        KeyType = "HASH"
                    }
                },
                    BillingMode = BillingMode.PAY_PER_REQUEST
                };

                await _dynamoDbClient.CreateTableAsync(request);
                _logger.LogInformation("Table {TableName} created successfully", tableName);
            }
        }
    }
}
