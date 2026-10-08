# Technology Stack

## Framework & Runtime
- **.NET 8.0**: Primary runtime and SDK
- **ASP.NET Core Web API**: REST API framework with controllers
- **Entity Framework Core 8.0**: ORM with SQL Server provider

## Database
- **SQL Server**: Primary database for application data
- **SQLite**: Alternative database provider (development/testing)
- **Identity Framework**: User authentication and authorization

## Authentication & Security
- **JWT Bearer Tokens**: API authentication mechanism
- **ASP.NET Core Identity**: User management and roles
- **CORS**: Configured to allow all origins (review for production)

## Key Libraries & Packages
- **AutoMapper**: Object-to-object mapping
- **MediatR**: Mediator pattern implementation for CQRS
- **Swashbuckle (Swagger)**: API documentation and testing
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT authentication
- **SMTP Email Service**: Email notifications and communication

## Build System & Deployment
- **MSBuild**: Standard .NET build system
- **Docker**: Containerization with multi-stage builds
- **GitHub Actions**: CI/CD pipeline on push/PR to main branch
- **Railway**: Production hosting platform

## External Integrations
- **AI Team API**: External service for CV analysis and recommendations
- **Email SMTP**: Email service configuration

## Common Commands

### Development
```powershell
# Restore dependencies
dotnet restore CareerPath.sln

# Build the solution
dotnet build CareerPath.sln --no-restore

# Build specific project (API)
dotnet build CareerPath/CareerPath.Api.csproj --no-restore

# Run the API project (development)
dotnet run --project CareerPath/CareerPath.Api.csproj
```

### Database Operations
```powershell
# Add Entity Framework migration
dotnet ef migrations add <MigrationName> --project CareerPath.Infrastructure --startup-project CareerPath

# Update database
dotnet ef database update --project CareerPath.Infrastructure --startup-project CareerPath
```

### Docker
```powershell
# Build Docker image
docker build -t careerpath-api .

# Run container
docker run -p 8080:8080 -p 8081:8081 careerpath-api
```

## Configuration
- **appsettings.json**: Application configuration
- **JWT settings**: Token configuration with issuer, audience, and secret key
- **Connection strings**: Database connection configuration
- **External services**: AI Team API base URL and settings