# Project Structure

## Clean Architecture Organization

The project follows Clean Architecture principles with clear separation of concerns across multiple projects:

### Core Projects
- **CareerPath.Domain**: Domain entities, value objects, and business logic
- **CareerPath.Application**: Application services, interfaces, use cases, and business workflows
- **CareerPath.Contracts**: Data contracts, DTOs, and API models
- **CareerPath.Infrastructure**: Data access, external services, and infrastructure concerns
- **CareerPath** (API): Web API controllers, authentication, and presentation layer
- **EmailConfiguration** (Shared): Email service configuration and utilities

### Architecture Layers

```
┌─────────────────────────────────────┐
│           Presentation              │
│         (CareerPath API)            │
├─────────────────────────────────────┤
│           Application               │
│      (CareerPath.Application)       │
├─────────────────────────────────────┤
│             Domain                  │
│       (CareerPath.Domain)           │
├─────────────────────────────────────┤
│          Infrastructure             │
│    (CareerPath.Infrastructure)      │
└─────────────────────────────────────┘
```

## Key Directories

### API Layer (`CareerPath/`)
- **Controllers/**: REST API controllers
  - `AIEndPointsController.cs`: AI services and CV analysis
  - `AuthController.cs`: Authentication and user management
  - `CompaniesController.cs`: Company management
  - `JobApplicationController.cs`: Job application tracking
  - `UserProfileController.cs`: User profile management
- **Program.cs**: Application startup and dependency injection
- **Properties/**: Launch settings and configuration

### Domain Layer (`CareerPath.Domain/`)
- **Applications/**: Job application and favorite job entities
- **Companies/**: Company-related domain models
- **Identity/**: User and authentication entities
- **Recommendations/**: Job recommendation domain logic
- **Events/**: Domain events for business processes

### Application Layer (`CareerPath.Application/`)
- **Interfaces/**: Service and repository contracts
- **Services/**: Business logic implementation
- **Handlers/**: Command and query handlers (MediatR)
- **Configuration/**: External service settings
- **Profiles/**: AutoMapper configuration

### Infrastructure Layer (`CareerPath.Infrastructure/`)
- **Data/**: Entity Framework DbContext and configurations
- **Repository/**: Data access implementations
- **Services/**: External service integrations
- **DbInitializer/**: Database seeding and initialization

## Naming Conventions

### Files and Classes
- **Controllers**: `[Feature]Controller.cs` (e.g., `CompaniesController.cs`)
- **Services**: `I[Feature]Service.cs` (interface) and `[Feature]Service.cs` (implementation)
- **Repositories**: `I[Entity]Repository.cs` (interface) and `[Entity]Repository.cs` (implementation)
- **Entities**: PascalCase singular nouns (e.g., `JobApplication.cs`)

### Project References
- API project references all other projects
- Application layer references Domain and Contracts
- Infrastructure references Application and Contracts
- Domain has minimal external dependencies

## Database Context
- **ApplicationDbContext**: Main application data with Identity integration
- **AIDataAnalysisDbContext**: Separate context for AI analysis data

## Configuration Files
- **appsettings.json**: Environment-specific configuration
- **CareerPath.sln**: Solution file defining all projects
- **Dockerfile**: Multi-stage Docker build configuration
- **.github/workflows/dotnet.yml**: CI/CD pipeline configuration

## Key Patterns
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management
- **Mediator Pattern**: Decoupled request handling via MediatR
- **Dependency Injection**: Service registration in Program.cs
- **Clean Architecture**: Dependency inversion and separation of concerns