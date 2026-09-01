# CLAUDE.md

This file documents the architecture pattern and conventions of the Fenicia project. Always follow these rules when modifying or adding code.

## 1. MediatR Removal

The project no longer uses MediatR. All handlers have been removed and replaced by services.

- **DO NOT** add references to the `MediatR` package
- **DO NOT** create classes that implement `IRequest`, `IRequestHandler`, or use `ISender`/`IMediator`
- **DO NOT** create `Handlers/` folders

## 2. Domain Structure

Each domain within `Domains/` must follow this structure:

```
Domains/DomainName/
├── DTOs/                    # All DTOs at root, NO subfolders
│   ├── AddNameCommand.cs
│   ├── AddNameResponse.cs
│   ├── DeleteNameCommand.cs
│   ├── GetAllNameQuery.cs
│   ├── GetAllNameResponse.cs
│   ├── GetNameByIdQuery.cs
│   ├── GetNameByIdResponse.cs
│   ├── UpdateNameCommand.cs
│   ├── UpdateNameResponse.cs
│   └── ... (other DTOs)
├── NameController.cs        # Controller at domain root (if exists)
└── NameService.cs           # Service at domain root, NEXT TO the controller
```

### Rules:
- **DTOs**: all files stay at the root of `DTOs/`, without subfolders `Commands/`, `Queries/`, `Responses/`
- **Services**: stay at the domain root, next to the controller
- **Controllers**: are optional. When they exist, they stay at the domain root. When they don't exist, the domain will have only `DTOs/`, services and repositories.
- **DO NOT** create folders like `Add/`, `Delete/`, `GetAll/`, `GetById/`, `Update/`, `Handlers/`, `Services/` inside the domain
- **Controllers** can access only services and external library integrations when necessary (logs, for example). Never access other entity types or repositories directly.

### RESTful Routes
Follow RESTful principles on routes:
- **DO NOT** use verbs in the URL (ex: `refreshtoken/validate`, `refreshtoken/invalidate`)
- Use only the resource/entity name in **singular** (ex: `refreshtoken`, `role`, `user`)
- Use appropriate HTTP methods: `GET` (read), `POST` (create), `PATCH` (partial update), `DELETE` (remove)
- Correct example: `PATCH /refreshtokens/{id}` instead of `POST /refreshtoken/invalidate`

## 3. Interfaces

Every `XService` class must have a corresponding `IXService` interface.
Every `XRepository` class that adds methods beyond the generic `IRepository<T>` must have a corresponding `IXRepository : IRepository<TModel>` interface declaring those extra methods.

- Interfaces live in `Domains/<DomainName>/Interfaces/`, one file per interface, named exactly after the class they abstract (`IAttachmentRepository.cs` for `AttachmentRepository`, `ICompanyService.cs` for `CompanyService`).
- Constructors — of services depending on other services, of services depending on specialized repositories, and of controllers depending on services — must take the **interface** type, never the concrete class.
  - The one exception that stays as-is: `IRepository<T>` itself, already interface-based, injected directly with no per-entity wrapper needed unless that entity's repository adds extra methods.

## 4. Repository Pattern

Services **CANNOT** access `DbContext` directly. All communication with the database must be done through repositories.

```csharp
namespace Fenicia.Common.Data.Repositories;

public interface IRepository<T> where T : BaseModel
{
    Task<IEnumerable<T>> GetAllAsync(int page = 1, int perPage = 10, CancellationToken ct = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<T> InsertAsync(T model, CancellationToken ct = default);
    Task<T?> UpdateAsync(Guid id, T model, CancellationToken ct = default);
    Task<int> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> DeleteAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    IQueryable<T> Query();
}
```

```csharp
namespace Fenicia.Common.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseModel
{
    public Repository(DefaultContext context)
    {
        DbSet = context.Set<T>();
        Context = context;
    }

    public Repository()
    {
    }

    public DefaultContext Context { get; set; } = null!;
    protected DbSet<T> DbSet { get; set; } = null!;

    public async Task<IEnumerable<T>> GetAllAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
            .Where(e => e.Deleted == null)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null, ct);
    }

    public async Task<T> InsertAsync(T model, CancellationToken ct = default)
    {
        model.Created = DateTime.UtcNow;
        await DbSet.AddAsync(model, ct);
        await SaveChangesAsync(ct);
        return model;
    }

    public async Task<T?> UpdateAsync(Guid id, T model, CancellationToken ct = default)
    {
        var existing = await GetByIdAsync(id, ct);
        if (existing is null)
        {
            return null;
        }

        Context.Entry(existing).CurrentValues.SetValues(model);
        existing.Updated = DateTime.UtcNow;
        await SaveChangesAsync(ct);
        return existing;
    }

    public async Task<int> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null)
        {
            return 0;
        }

        entity.Deleted = DateTime.UtcNow;
        return await SaveChangesAsync(ct);
    }

    public async Task<int> DeleteAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var entities = await DbSet.Where(e => ids.Contains(e.Id) && e.Deleted == null).ToListAsync(ct);
        if (entities.Count == 0)
        {
            return 0;
        }

        foreach (var entity in entities)
        {
            entity.Deleted = DateTime.UtcNow;
        }

        return await SaveChangesAsync(ct);
    }

    public async Task InsertRangeAsync(IEnumerable<T> models, CancellationToken ct = default)
    {
        foreach (var model in models)
        {
            model.Created = DateTime.UtcNow;
        }

        await DbSet.AddRangeAsync(models, ct);
        await SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await DbSet.Where(predicate).ToListAsync(ct);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(predicate, ct);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await DbSet.CountAsync(ct);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await DbSet.CountAsync(predicate, ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await Context.SaveChangesAsync(ct);
    }

    public IQueryable<T> Query()
    {
        return DbSet;
    }
}
```

### Rules:
- All methods are `async`
- All receive `CancellationToken` as the last parameter
- Write methods return `Task<T>` or `Task<T?>`
- Delete returns `Task<int>` with the count of affected records
- `SaveChangesAsync` is used as a transaction; do not call `SaveChangesAsync` outside the repository
- Always filter by `Deleted == null` in queries
- Repositories return only entities or primitives, never DTOs or Responses
- `Context` is public to allow cross-assembly access
- **A domain CANNOT access another domain's repository directly.** If it needs data/external access, use that domain's **service**, not the repository.
- When a repository subclass adds domain-specific methods (e.g. `AttachmentRepository.GetByCommentAsync`), declare an interface for it and register that interface in DI, not the concrete type. Any service consuming those extra methods must depend on the interface.

## 5. Service Pattern

Services receive `IRepository<T>` via constructor and expose public async methods.

```csharp
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.Project.DTOs;

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectService(IRepository<ProjectModel> repository)
{
    public async Task<List<GetAllProjectResponse>> GetAllAsync(GetAllProjectQuery query, CancellationToken ct)
    {
        var projects = await repository.GetAllAsync(query.Page, query.PerPage, ct);
        return projects.Select(p => new GetAllProjectResponse(p.Id, p.Title, ...)).ToList();
    }

    public async Task<GetProjectByIdResponse?> GetByIdAsync(GetProjectByIdQuery query, CancellationToken ct)
    {
        var project = await repository.GetByIdAsync(query.Id, ct);
        return project is null ? null : new GetProjectByIdResponse(...);
    }

    public async Task<AddProjectResponse> AddAsync(AddProjectCommand command, Guid companyId, CancellationToken ct)
    {
        var model = new ProjectModel
        {
            Id = command.Id,
            CompanyId = companyId,
            ...
        };

        var created = await repository.InsertAsync(model, ct);
        return new AddProjectResponse(created.Id, ...);
    }

    public async Task<UpdateProjectResponse?> UpdateAsync(UpdateProjectCommand command, Guid companyId, CancellationToken ct)
    {
        var updated = await repository.UpdateAsync(command.Id, model, ct);
        return updated is null ? null : new UpdateProjectResponse(...);
    }

    public async Task DeleteAsync(DeleteProjectCommand command, CancellationToken ct)
    {
        await repository.DeleteAsync(command.Id, ct);
    }
}
```

### Rules:
- Methods must be `public async Task<...>`
- Use `CancellationToken` as the last parameter
- Services cannot access `DbContext` directly
- Use `record` for DTOs
- Write services must receive `CompanyId` when the entity inherits from `BaseCompanyModel`
- `CompanyId` must never come from the command/query; always from the token
- When Service A depends on Service B, Service A's constructor must take `IServiceB`, not `ServiceB`.

## 6. Controller Pattern

Controllers inject services directly via constructor, without `ISender` or handlers.

```csharp
using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Fenicia.Module.Projects.Domains.Project;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.Project;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GetAllProjectResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var result = await projectService.GetAllAsync(new GetAllProjectQuery(page, perPage), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetProjectByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var result = await projectService.GetByIdAsync(new GetProjectByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AddProjectResponse>> PostAsync([FromBody] AddProjectCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var result = await projectService.AddAsync(command, ClaimReader.UserId(User), ct);
        return new CreatedResult(string.Empty, result);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UpdateProjectResponse>> PatchAsync([FromBody] UpdateProjectCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var result = await projectService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        await projectService.DeleteAsync(new DeleteProjectCommand(id), ct);
        return NoContent();
    }
}
```

### Rules:
- Inject only services, never handlers or `ISender`
- Use `ClaimReader.UserId(User).ToString()` to get the authenticated user
- Use `WideEventContext` to pass `UserId`
- Standard returns: `Ok()`, `NotFound()`, `NoContent()`, `Created()`, `Forbid()`
- Pass `CompanyId` to write services when the entity inherits from `BaseCompanyModel`

### OpenAPI Documentation (XML Comments)

All controller actions must have **XML documentation comments** to document the API via OpenAPI/Swagger. This is the **only exception** to the rule of not requiring XML comments in the project.

Each action must document:

- **Description** of what the endpoint does
- **Parameters** for route, query string and body (with payload examples)
- **Success response** (with response example)
- Possible **Status codes** returned
- **Exceptions** that may occur (raised by the controller, service or repository)

To document exceptions correctly, analyze the entire flow: **Controller → Service → Repository**, identifying possible errors at each layer.

```csharp
/// <summary>
/// Gets a user by ID.
/// </summary>
/// <param name="id">User ID</param>
/// <param name="wide">Wide event context</param>
/// <param name="ct">Cancellation token</param>
/// <returns>User data</returns>
/// <response code="200">User found</response>
/// <response code="404">User not found</response>
/// <response code="400">Invalid ID</response>
/// <response code="500">Internal server error</response>
[HttpGet("{id:guid}")]
public async Task<ActionResult<GetUserByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
{
    // ...
}
```

All actions must **mandatorily** have the `[ProducesResponseType]` attribute for each possible return status code, ensuring precise documentation in Swagger/OpenAPI.

```csharp
[HttpGet("{id:guid}")]
[ProducesResponseType(typeof(GetUserByIdResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<GetUserByIdResponse>> GetByIdAsync(...)
```

## 7. DTO Pattern

All DTOs are `record` and stay at the root of `DTOs/`.

```csharp
// Commands
namespace Fenicia.Module.Projects.Domains.Project.DTOs;
public record AddProjectCommand(Guid Id, string Title, string? Description, string Status, DateTime? StartDate, DateTime? EndDate, Guid Owner);

// Queries
namespace Fenicia.Module.Projects.Domains.Project.DTOs;
public record GetAllProjectQuery(int Page = 1, int PerPage = 10);

// Responses
namespace Fenicia.Module.Projects.Domains.Project.DTOs;
public record AddProjectResponse(Guid Id, string Title, string? Description, string Status, DateTime? StartDate, DateTime? EndDate, Guid Owner, Guid CompanyId);
```

### Rules:
- All in the root `DTOs/` folder, without subfolders
- Namespace: `Fenicia.Module.Projects.Domains.Project.DTOs`
- Always use `record`
- Commands are write inputs (POST, PATCH, DELETE)
- Queries are read inputs (GET)
- Responses are outputs

## 8. Test Pattern

Follow the pattern of `Fenicia.Auth.Tests` and `Fenicia.Module.Projects.Tests`.

### Service Tests:
```csharp
using Bogus;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class GetAllProjectServiceTests : IDisposable
{
    private readonly Faker faker;
    private readonly ProjectService service;
    private readonly Mock<IProjectRepository> mockRepository;

    public GetAllProjectServiceTests()
    {
        mockRepository = new Mock<IProjectRepository>();
        service = new ProjectService(mockRepository.Object);
        faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenProjectsExist_ReturnsPaginationWithProjects()
    {
        var projects = new List<ProjectModel>
        {
            new() { Id = Guid.NewGuid(), Title = faker.Commerce.Categories(1).First(), CompanyId = Guid.NewGuid() }
        };

        mockRepository.Setup(r => r.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        var result = await service.GetAllAsync(new GetAllProjectQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }
}
```

### Controller Tests:
```csharp
using System.Security.Claims;
using Bogus;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class ProjectControllerTests : IDisposable
{
    private readonly ProjectController controller;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Mock<IProjectService> mockService;

    public ProjectControllerTests()
    {
        mockService = new Mock<IProjectService>();
        mockHttpContext = new Mock<HttpContext>();
        controller = new ProjectController(mockService.Object) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenProjectsExist_ReturnsOk()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
        mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));
        
        mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GetAllProjectResponse>());

        var result = await controller.GetAsync(null, 1, 10, CancellationToken.None);
        
        Assert.IsType<OkObjectResult>(result.Result);
    }
}
```

### Rules:
- **Repository tests**: unchanged. These are the one place a real `DefaultContext` with `UseInMemoryDatabase` is correct — the point of a repository test is verifying real EF Core query/persistence behavior.
- **Service tests**: constructor dependencies (repositories, other services) must be Moq mocks against their interfaces. Never construct a real repository, never construct a real dependent service, never spin up a `DefaultContext`, inside a Service test.
- **Controller tests**: same rule — mock `IXService` via Moq, inject the mock into the controller. Never construct a real service or `DefaultContext` inside a Controller test.
- Use `Faker` from Bogus for test data
- Always include `using System.Security.Claims` in controller tests
- Always configure `mockHttpContext.Setup(x => x.User).Returns(...)` for tests that use `ClaimReader`
- **All new or refactored code must include corresponding unit tests.**
- **Every Service, Provider, Repository and Controller needs corresponding unit tests.**
- **One test file per tested class.** Do not create multiple test files for the same production class.

### Mocking Dependencies

- Use **Moq** to mock constructor dependencies in tests.
- Prefer generating mock data with **Bogus** whenever possible, instead of hardcoding values.
- Example:
  ```csharp
  var mockRepo = new Mock<IRepository<ProductModel>>();
  mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(new ProductModel { Id = Guid.NewGuid(), Name = faker.Commerce.ProductName() });
  ```

## 9. General Conventions

- **Namespaces**: always use the full domain namespace
  - Controllers: `Fenicia.Module.Projects.Domains.Project`
  - Services: `Fenicia.Module.Projects.Domains.Project`
  - DTOs: `Fenicia.Module.Projects.Domains.Project.DTOs`
- **Usings**: always include `Fenicia.Common.API` in controllers to access `ClaimReader`
- **CancellationToken**: always include as the last parameter in async methods
- **Nullable reference types**: enabled in projects
- **Record types**: use `record` for DTOs and commands
- **Primary constructors**: use for services (ex: `public class ProjectService(DefaultContext db)`)
- **Private fields**: use `_camelCase` (ex: `private readonly DefaultContext _db;`)
  - **Async methods**: always end with `Async` (ex: `GetAsync`)
  - **LINQ**: use LINQ query expression syntax whenever possible instead of lambda expressions
  - **Mapping**: use Mapperly for transformation between entities and DTOs/responses
  - **Mandatory tests**: all new or refactored code must include corresponding unit tests
  - **Domain isolation**: a domain CANNOT access another domain's repository directly; use the corresponding domain's service
  - **XML comments**: not required
- **Unnecessary usings**: removal not required
- **Migrations**: excluded from style rules

## 10. Style Rules (StyleCop/EditorConfig)

The project uses strict style rules defined in `.editorconfig`. Before committing, ensure the build has no StyleCop errors.

### Mandatory Rules:
- **SA1201**: member order (fields > constructors > properties > events > methods)
- **SA1202**: order by visibility (public > internal > protected > private)
- **SA1203**: `const` fields before non-`const` fields
- **SA1214**: `static readonly` fields before instance fields
- **SA1208**: `System.*` usings before others
- **SA1210**: usings in alphabetical order
- **SA1211**: static usings after normal usings
- **SA1503/SA1519**: braces required even in one-line blocks
- **IDE0130**: namespace must match folder structure
- **CA1852**: internal classes should not be inherited unless designed for it
- **CA1031**: do not catch generic `Exception` without specific handling
- **CA2201**: do not throw `Exception` or `SystemException` directly

### Exceptions:
- **Migrations**: files in `Migrations/` are excluded from the above rules
- **SA0001/IDE0005**: globally disabled (do not require XML documentation nor removal of usings)

## 11. What NOT to do

- ❌ Create `Handlers/` folders
- ❌ Create subfolders in `DTOs/` (`Commands/`, `Queries/`, `Responses/`)
- ❌ Create `Services/` folders inside domains
- ❌ Use `MediatR` or `ISender`
- ❌ Implement `IRequest` or `IRequestHandler`
- ❌ Inject handlers in controllers
- ❌ Use `MediatR` in tests (mock services directly)
- ❌ Services accessing `DbContext` directly
- ❌ Repositories returning DTOs or Responses
- ❌ Throw generic `Exception` (use specific types)
- ❌ Catch generic `Exception` without specific handling
- ❌ Omit braces in `if`/`for`/`while` blocks
- ❌ Use `this.` in instance methods
- ❌ Forget to include `CancellationToken` as the last parameter in async methods
- ❌ Service tests instantiating a real repository, a real dependent service, or a real `DefaultContext`
- ❌ Controller tests instantiating a real service or `DefaultContext`
- ❌ A `XService`/`XRepository` (with extra methods) without a matching interface in `Domains/<Name>/Interfaces/`
- ❌ Injecting a concrete `XService`/specialized `XRepository` class instead of its interface, anywhere
