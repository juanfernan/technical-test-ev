using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;
using TestEv.Domain.Entities;
using TestEv.Domain.Interfaces;
using TestEv.Infrastructure.Configuration;

namespace TestEv.Infrastructure.Persistence
{
    public class DynamoDbProjectRepository : IProjectRepository
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string _tableName;

        public DynamoDbProjectRepository(IAmazonDynamoDB dynamoDbClient, IOptions<DynamoDbSettings> settings)
        {
            _dynamoDbClient = dynamoDbClient;
            _tableName = settings.Value.ProjectsTableName;
        }

        public async Task<Project?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id } }
            }
            };

            var response = await _dynamoDbClient.GetItemAsync(request, cancellationToken);

            if (!response.IsItemSet || response.Item.Count == 0)
                return null;

            return MapToProject(response.Item);
        }

        public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var request = new ScanRequest { TableName = _tableName };
            var response = await _dynamoDbClient.ScanAsync(request, cancellationToken);
            return response.Items.Select(MapToProject);
        }

        public async Task<IEnumerable<Project>> GetByFilterAsync(string? status, string? ownerId, CancellationToken cancellationToken = default)
        {
            var filterExpressions = new List<string>();
            var expressionAttributeValues = new Dictionary<string, AttributeValue>();

            if (!string.IsNullOrEmpty(status))
            {
                filterExpressions.Add("StatusValue = :status");
                expressionAttributeValues[":status"] = new AttributeValue { S = status };
            }

            if (!string.IsNullOrEmpty(ownerId))
            {
                filterExpressions.Add("OwnerId = :ownerId");
                expressionAttributeValues[":ownerId"] = new AttributeValue { S = ownerId };
            }

            var request = new ScanRequest { TableName = _tableName };

            if (filterExpressions.Count > 0)
            {
                request.FilterExpression = string.Join(" AND ", filterExpressions);
                request.ExpressionAttributeValues = expressionAttributeValues;
            }

            var response = await _dynamoDbClient.ScanAsync(request, cancellationToken);
            return response.Items.Select(MapToProject);
        }

        public async Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = MapToItem(project)
            };

            await _dynamoDbClient.PutItemAsync(request, cancellationToken);
            return project;
        }

        public async Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = MapToItem(project)
            };

            await _dynamoDbClient.PutItemAsync(request, cancellationToken);
            return project;
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id } }
            }
            };

            await _dynamoDbClient.DeleteItemAsync(request, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            var project = await GetByIdAsync(id, cancellationToken);
            return project != null;
        }

        private static Dictionary<string, AttributeValue> MapToItem(Project project)
        {
            return new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = project.Id } },
            { "Name", new AttributeValue { S = project.Name } },
            { "Description", new AttributeValue { S = project.Description } },
            { "OwnerId", new AttributeValue { S = project.OwnerId } },
            { "StatusValue", new AttributeValue { S = project.Status.ToString().ToLowerInvariant() } },
            { "CreatedAt", new AttributeValue { S = project.CreatedAt.ToString("O") } },
            { "UpdatedAt", new AttributeValue { S = project.UpdatedAt.ToString("O") } }
        };
        }

        private static Project MapToProject(Dictionary<string, AttributeValue> item)
        {
            var status = Enum.TryParse<ProjectStatus>(item["StatusValue"].S, true, out var parsedStatus)
                ? parsedStatus
                : ProjectStatus.Active;

            return Project.Hydrate(
                id: item["Id"].S,
                name: item["Name"].S,
                description: item["Description"].S,
                ownerId: item["OwnerId"].S,
                status: status,
                createdAt: DateTime.Parse(item["CreatedAt"].S),
                updatedAt: DateTime.Parse(item["UpdatedAt"].S));
        }
    }
}
