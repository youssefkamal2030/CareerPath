# DDD + Event-Driven Architecture - Complete Guide

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │ Web API     │  │ gRPC API    │  │ GraphQL     │            │
│  │ Controllers │  │ Services    │  │ Resolvers   │            │
│  └─────────────┘  └─────────────┘  └─────────────┘            │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                       APPLICATION LAYER                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │ Command     │  │ Query       │  │ Event       │            │
│  │ Handlers    │  │ Handlers    │  │ Handlers    │            │
│  └─────────────┘  └─────────────┘  └─────────────┘            │
│                                                                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │ Application │  │ Validators  │  │ Mappers     │            │
│  │ Services    │  │             │  │             │            │
│  └─────────────┘  └─────────────┘  └─────────────┘            │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                         DOMAIN LAYER                            │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │ Aggregates  │  │ Entities    │  │ Value       │            │
│  │             │  │             │  │ Objects     │            │
│  └─────────────┘  └─────────────┘  └─────────────┘            │
│                                                                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │ Domain      │  │ Domain      │  │ Repository  │            │
│  │ Services    │  │ Events      │  │ Interfaces  │            │
│  └─────────────┘  └─────────────┘  └─────────────┘            │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                     INFRASTRUCTURE LAYER                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │ Repositories│  │ Event Store │  │ Message     │            │
│  │             │  │             │  │ Bus         │            │
│  └─────────────┘  └─────────────┘  └─────────────┘            │
│                                                                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │ External    │  │ Persistence │  │ Caching     │            │
│  │ APIs        │  │             │  │             │            │
│  └─────────────┘  └─────────────┘  └─────────────┘            │
└─────────────────────────────────────────────────────────────────┘
```

---

## 1. PRESENTATION LAYER

### 🎯 **Purpose**: Handle HTTP requests, routing, and API contracts

### 📋 **What Goes Here**:
- **Controllers/Endpoints**: HTTP route handling
- **DTOs**: Request/Response models
- **Validation Attributes**: Basic input validation
- **Authentication/Authorization**: JWT, OAuth handling
- **API Documentation**: Swagger/OpenAPI specs
- **Error Handling**: Global exception filters

### 🚫 **What NEVER Goes Here**:
- Business logic
- Database queries
- Domain knowledge
- Complex validations
- Data transformations beyond simple mapping

### 📏 **Rules to Follow**:

#### ✅ **DO**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.Email, request.Username);
        var result = await _mediator.Send(command);
        
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetUser), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }
}
```

#### ❌ **DON'T**:
```csharp
[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
{
    // DON'T: Business logic in controller
    if (await _userRepository.ExistsByEmailAsync(request.Email))
        return BadRequest("Email exists");
    
    // DON'T: Direct database access
    var user = new User { Email = request.Email };
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    
    return Ok(user);
}
```

---

## 2. APPLICATION LAYER

### 🎯 **Purpose**: Orchestrate business operations, handle cross-cutting concerns

### 📋 **What Goes Here**:
- **Command/Query Handlers**: CQRS implementation
- **Application Services**: Orchestrate domain operations
- **Event Handlers**: Handle domain events
- **Validators**: Business rule validation
- **Mappers**: DTO ↔ Domain mapping
- **Transaction Management**: Unit of Work coordination
- **Security**: Authorization policies
- **Caching**: Application-level caching

### 🚫 **What NEVER Goes Here**:
- Domain business logic
- Infrastructure details (database, external APIs)
- HTTP concerns (status codes, routing)
- Domain entities creation (use factories)

### 📏 **Rules to Follow**:

#### ✅ **DO**:
```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserDomainService _userDomainService;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            // Orchestrate domain operations
            var result = await _userDomainService.CreateUserAsync(request.Email, request.Username);
            
            if (result.IsFailure)
                return Result<Guid>.Failure(result.Error);
            
            await _unitOfWork.CompleteAsync();
            await transaction.CommitAsync();
            
            return Result<Guid>.Success(result.Value.Id);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

#### ❌ **DON'T**:
```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // DON'T: Business logic in application layer
        if (string.IsNullOrEmpty(request.Email) || !request.Email.Contains("@"))
            return Result<Guid>.Failure("Invalid email");
        
        // DON'T: Direct entity creation
        var user = new User(request.Email, request.Username);
        
        // DON'T: Direct repository access without domain service
        await _userRepository.AddAsync(user);
        
        return Result<Guid>.Success(user.Id);
    }
}
```

---

## 3. DOMAIN LAYER

### 🎯 **Purpose**: Core business logic, domain rules, and domain models

### 📋 **What Goes Here**:
- **Aggregates**: Business transaction boundaries
- **Entities**: Objects with identity and lifecycle
- **Value Objects**: Immutable objects without identity
- **Domain Services**: Business logic that doesn't fit in entities
- **Domain Events**: Communicate domain state changes
- **Repository Interfaces**: Data access contracts
- **Specifications**: Business rules queries
- **Factories**: Complex object creation logic
- **Domain Exceptions**: Business rule violations

### 🚫 **What NEVER Goes Here**:
- Infrastructure details
- Persistence logic
- External service calls
- UI concerns
- Framework dependencies

### 📏 **Rules to Follow**:

#### ✅ **DO**:
```csharp
// Aggregate Root
public class User : AggregateRoot<UserId>
{
    public Email Email { get; private set; }
    public UserProfile Profile { get; private set; }
    
    private User() { } // EF Constructor
    
    public static Result<User> Create(string email, string username)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result<User>.Failure(emailResult.Error);
        
        var user = new User(UserId.New(), emailResult.Value);
        user.AddDomainEvent(new UserCreatedEvent(user.Id, email));
        
        return Result<User>.Success(user);
    }
    
    public Result CreateProfile(string displayName)
    {
        if (Profile != null)
            return Result.Failure("Profile already exists");
        
        Profile = UserProfile.Create(Id, displayName);
        AddDomainEvent(new ProfileCreatedEvent(Id, Profile.Id));
        
        return Result.Success();
    }
}

// Value Object
public class Email : ValueObject
{
    public string Value { get; private set; }
    
    private Email(string value) => Value = value;
    
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<Email>.Failure("Email cannot be empty");
        
        if (!IsValidEmail(email))
            return Result<Email>.Failure("Invalid email format");
        
        return Result<Email>.Success(new Email(email));
    }
    
    private static bool IsValidEmail(string email) => email.Contains("@");
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

// Domain Service
public class UserDomainService
{
    private readonly IUserRepository _userRepository;
    
    public async Task<Result<User>> CreateUserAsync(string email, string username)
    {
        // Business rule: Email must be unique
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
            return Result<User>.Failure("Email already exists");
        
        return User.Create(email, username);
    }
}
```

#### ❌ **DON'T**:
```csharp
public class User : Entity<Guid>
{
    public string Email { get; set; } // DON'T: Public setters
    public string Username { get; set; }
    
    public User() { } // DON'T: Parameterless constructor only
    
    // DON'T: Infrastructure concerns in domain
    public async Task SaveToDatabase(IUserRepository repository)
    {
        await repository.AddAsync(this);
    }
    
    // DON'T: External service calls
    public async Task SendWelcomeEmail(IEmailService emailService)
    {
        await emailService.SendAsync(Email, "Welcome!");
    }
}
```

---

## 4. INFRASTRUCTURE LAYER

### 🎯 **Purpose**: Implement technical details and external integrations

### 📋 **What Goes Here**:
- **Repository Implementations**: Data access logic
- **DbContext**: Entity Framework configuration
- **External API Clients**: Third-party service integration
- **Message Bus**: Event publishing/subscribing
- **Event Store**: Event sourcing implementation
- **Caching**: Redis, MemoryCache implementation
- **File Storage**: Blob storage, file system
- **Email Services**: SMTP, SendGrid integration
- **Configuration**: Settings, connection strings

### 🚫 **What NEVER Goes Here**:
- Business logic
- Domain rules
- Application orchestration
- HTTP request handling

### 📏 **Rules to Follow**:

#### ✅ **DO**:
```csharp
// Repository Implementation
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email);
    }
    
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}

// Event Publisher
public class EventBus : IEventBus
{
    private readonly IServiceBus _serviceBus;
    
    public async Task PublishAsync<T>(T @event) where T : IDomainEvent
    {
        var message = JsonSerializer.Serialize(@event);
        await _serviceBus.SendAsync(typeof(T).Name, message);
    }
}

// External Service
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    
    public async Task SendAsync(string to, string subject, string body)
    {
        // Implementation details
    }
}
```

#### ❌ **DON'T**:
```csharp
public class UserRepository : IUserRepository
{
    public async Task<User> GetByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        
        // DON'T: Business logic in repository
        if (user != null && user.IsActive)
        {
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        
        return user;
    }
}
```

---

## 5. EVENT-DRIVEN ARCHITECTURE RULES

### 🎯 **Event Flow**:
```
Domain Operation → Domain Event → Event Handler → Side Effects
```

### 📋 **Event Types**:

#### **Domain Events** (Internal):
```csharp
public class UserCreatedEvent : IDomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
    
    public UserCreatedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }
}
```

#### **Integration Events** (External):
```csharp
public class UserRegisteredIntegrationEvent : IIntegrationEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}
```

### 📏 **Event Rules**:

#### ✅ **DO**:
- Raise events from aggregate roots
- Handle events asynchronously
- Make events immutable
- Use past tense for event names
- Keep events simple and focused
- Handle event failures gracefully

#### ❌ **DON'T**:
- Raise events from value objects
- Make events depend on external services
- Modify aggregates in event handlers
- Use events for synchronous operations

---

## 6. GENERAL ARCHITECTURAL RULES

### 🎯 **Dependency Rules**:
```
Presentation → Application → Domain ← Infrastructure
```

### 📏 **Golden Rules**:

#### ✅ **DO**:
1. **Single Responsibility**: Each class has one reason to change
2. **Dependency Inversion**: Depend on abstractions, not concretions
3. **Tell, Don't Ask**: Objects should tell other objects what to do
4. **Fail Fast**: Validate early and fail explicitly
5. **Immutability**: Prefer immutable objects where possible
6. **Event Sourcing**: Use events to capture state changes
7. **CQRS**: Separate read and write operations
8. **Bounded Contexts**: Keep related concepts together

#### ❌ **DON'T**:
1. **Anemic Domain Models**: Avoid data classes without behavior
2. **God Objects**: Avoid classes that do too much
3. **Tight Coupling**: Avoid direct dependencies between layers
4. **Primitive Obsession**: Avoid using primitives for domain concepts
5. **Feature Envy**: Avoid classes that use other classes' data excessively
6. **Shotgun Surgery**: Avoid changes that affect many classes
7. **Magic Numbers/Strings**: Use constants or value objects instead
8. **Leaky Abstractions**: Don't expose implementation details

---

## 7. PROJECT STRUCTURE

```
src/
├── CareerPath.API/                    # Presentation Layer
│   ├── Controllers/
│   ├── DTOs/
│   ├── Filters/
│   ├── Middleware/
│   └── Program.cs
│
├── CareerPath.Application/            # Application Layer
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── Events/
│   ├── Services/
│   ├── Validators/
│   ├── Mappers/
│   └── Interfaces/
│
├── CareerPath.Domain/                 # Domain Layer
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Aggregates/
│   ├── Services/
│   ├── Events/
│   ├── Repositories/
│   ├── Specifications/
│   ├── Exceptions/
│   └── Common/
│
├── CareerPath.Infrastructure/         # Infrastructure Layer
│   ├── Persistence/
│   ├── Repositories/
│   ├── Services/
│   ├── EventBus/
│   ├── Caching/
│   ├── ExternalServices/
│   └── Configuration/
│
└── CareerPath.Shared/                 # Shared Kernel
    ├── Constants/
    ├── Enums/
    ├── Extensions/
    └── Utilities/
```

---

## 8. TESTING STRATEGY

### 📋 **Test Pyramid**:

#### **Unit Tests** (70%):
- Domain entities and value objects
- Domain services
- Application services
- Validators

#### **Integration Tests** (20%):
- Repository implementations
- External service integrations
- Event handlers
- Database operations

#### **End-to-End Tests** (10%):
- API endpoints
- Complete user workflows
- Cross-system integrations

### 📏 **Testing Rules**:

#### ✅ **DO**:
- Test business logic thoroughly
- Mock external dependencies
- Use AAA pattern (Arrange, Act, Assert)
- Keep tests simple and focused
- Test edge cases and error conditions

#### ❌ **DON'T**:
- Test framework code
- Over-mock internal dependencies
- Write integration tests for everything
- Test implementation details

---

## 9. PERFORMANCE CONSIDERATIONS

### 📋 **Optimization Strategies**:

#### **Database**:
- Use proper indexing
- Implement read replicas for queries
- Use connection pooling
- Optimize N+1 queries

#### **Caching**:
- Cache expensive operations
- Use distributed caching for scale
- Implement cache invalidation strategies
- Cache query results, not entities

#### **Events**:
- Process events asynchronously
- Use message queues for reliability
- Implement event replay capabilities
- Handle duplicate events (idempotency)

### 📏 **Performance Rules**:

#### ✅ **DO**:
- Measure before optimizing
- Use async/await properly
- Implement proper error handling
- Monitor application performance
- Use pagination for large datasets

#### ❌ **DON'T**:
- Premature optimization
- Ignore memory leaks
- Block async operations
- Load unnecessary data
- Ignore database query performance

---

## 10. SECURITY BEST PRACTICES

### 📋 **Security Layers**:

#### **Authentication**:
- JWT tokens with proper expiration
- Refresh token rotation
- Multi-factor authentication
- Password hashing with salt

#### **Authorization**:
- Role-based access control
- Resource-based permissions
- Principle of least privilege
- Audit logging

#### **Data Protection**:
- Encrypt sensitive data
- Use HTTPS everywhere
- Validate all inputs
- Sanitize outputs

### 📏 **Security Rules**:

#### ✅ **DO**:
- Validate all inputs
- Use parameterized queries
- Implement rate limiting
- Log security events
- Keep dependencies updated

#### ❌ **DON'T**:
- Trust user input
- Expose sensitive information
- Use weak encryption
- Ignore security headers
- Store passwords in plain text

---

This guide provides a comprehensive framework for building scalable, maintainable applications using DDD and Event-Driven Architecture principles. Follow these rules consistently to ensure your application remains clean, testable, and adaptable to changing requirements.