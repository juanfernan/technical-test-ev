using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestEv.Application.Interfaces;
using TestEv.Application.Services;
using TestEv.Domain.Interfaces;
using TestEv.Infrastructure.BackgroundServices;
using TestEv.Infrastructure.Configuration;
using TestEv.Infrastructure.Persistence;
using TestEv.Infrastructure.Services;

namespace TestEv.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.Configure<DynamoDbSettings>(configuration.GetSection(DynamoDbSettings.SectionName));

            var dynamoDbSettings = configuration.GetSection(DynamoDbSettings.SectionName).Get<DynamoDbSettings>();
            var adminSettings = configuration.GetSection(AdminUserSettings.SectionName).Get<AdminUserSettings>();

            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthService>(sp => new AuthService(
                sp.GetRequiredService<IJwtTokenService>(),
                adminSettings?.Username ?? "admin",
                adminSettings?.Password ?? "Admin123!"));

            if (dynamoDbSettings?.UseLocalDb == true && !string.IsNullOrEmpty(dynamoDbSettings.ServiceUrl))
            {
                services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(
                    new AmazonDynamoDBConfig { ServiceURL = dynamoDbSettings.ServiceUrl }));
                services.AddScoped<IProjectRepository, DynamoDbProjectRepository>();
                services.AddSingleton<DynamoDbTableInitializer>();
            }
            else if (dynamoDbSettings?.UseLocalDb == false)
            {
                services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(
                    new AmazonDynamoDBConfig { RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(dynamoDbSettings.Region) }));
                services.AddScoped<IProjectRepository, DynamoDbProjectRepository>();
                services.AddSingleton<DynamoDbTableInitializer>();
            }
            else
            {
                services.AddSingleton<IProjectRepository, InMemoryProjectRepository>();
            }

            return services;
        }

        public static IServiceCollection AddBackgroundWorkers(this IServiceCollection services)
        {
            services.AddHostedService<ProjectStatsWorker>();
            return services;
        }
    }
}
