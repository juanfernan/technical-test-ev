# TestEv API

RESTful API for project management built with .NET 10

## Architecture

This project follows Clean Architecture principles with four layers:

- **Domain**: Core business entities, exceptions, and repository interfaces. No external dependencies.
- **Application**: Use cases, DTOs, validators, and service interfaces. Depends only on Domain.
- **Infrastructure**: External concerns implementation (DynamoDB, JWT, background services). Implements Domain interfaces.
- **Api**: HTTP layer with controllers, middleware, and configuration. Entry point of the application.

## Database Configuration

The application supports three database modes configured via `appsettings.json`:

1. **In-Memory** (default): When `DynamoDb:UseLocalDb` is null or not set. Data is stored in memory and lost on restart. Suitable for quick testing without external dependencies.

2. **DynamoDB Local**: When `DynamoDb:UseLocalDb` is true. Connects to a local DynamoDB instance (docker container). Used for development and testing.

3. **AWS DynamoDB**: When `DynamoDb:UseLocalDb` is false. Connects to AWS DynamoDB using the configured region.

For this technical test, credentials and connection strings are hardcoded in configuration files to avoid exposing AWS RDS/DynamoDB resources in a public repository. In a production environment, these should be managed through environment variables and AWS Secrets Manager

## Running the Application

### Local Development

```
cd TestEv
dotnet run --project TestEv.Api
```

The API will be available at http://localhost:5258

### User and pass for the authentication

```
{
  "username": "admin",
  "password": "Admin123!"
}
```

### Docker

```
cd TestEv
docker-compose up --build
```

The API will be available at http://localhost:5000

## Testing

```
cd TestEv
dotnet test
```

## Technologies

- .NET 10
- ASP.NET Core Web API
- FluentValidation
- JWT Authentication
- DynamoDB (AWS SDK)
- xUnit, Moq, FluentAssertions
- Docker

