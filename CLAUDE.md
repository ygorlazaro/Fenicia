# CLAUDE.md

Este arquivo documenta o padrão de arquitetura e convenções do projeto Fenicia. Sempre siga estas regras ao modificar ou adicionar código.

## 1. Remoção do MediatR

O projeto não usa mais MediatR. Todos os handlers foram removidos e substituídos por services.

- **NÃO** adicione referências ao pacote `MediatR`
- **NÃO** crie classes que implementem `IRequest`, `IRequestHandler`, ou usem `ISender`/`IMediator`
- **NÃO** crie pastas `Handlers/`

## 2. Estrutura de Domínio

Cada domínio dentro de `Domains/` deve seguir esta estrutura:

```
Domains/NomeDoDominio/
├── DTOs/                    # Todos os DTOs na raiz, SEM subpastas
│   ├── AddNomeCommand.cs
│   ├── AddNomeResponse.cs
│   ├── DeleteNomeCommand.cs
│   ├── GetAllNomeQuery.cs
│   ├── GetAllNomeResponse.cs
│   ├── GetNomeByIdQuery.cs
│   ├── GetNomeByIdResponse.cs
│   ├── UpdateNomeCommand.cs
│   ├── UpdateNomeResponse.cs
│   └── ... (outros DTOs)
├── NomeController.cs        # Controller na raiz do domínio
└── NomeService.cs           # Service na raiz do domínio, AO LADO do controller
```

### Regras:
- **DTOs**: todos os arquivos ficam na raiz de `DTOs/`, sem subpastas `Commands/`, `Queries/`, `Responses/`
- **Services**: ficam na raiz do domínio, ao lado do controller
- **Controllers**: ficam na raiz do domínio
- **NÃO** criar pastas como `Add/`, `Delete/`, `GetAll/`, `GetById/`, `Update/`, `Handlers/`, `Services/` dentro do domínio

## 3. Padrão de Service

Services substituem os handlers antigos. Eles recebem `DefaultContext` via construtor e expõem métodos públicos assíncronos.

```csharp
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectService(DefaultContext db)
{
    public async Task<List<GetAllProjectResponse>> GetAllAsync(GetAllProjectQuery query, CancellationToken ct)
    {
        return await db.Projects
            .Select(p => new GetAllProjectResponse(...))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }

    public async Task<GetProjectByIdResponse?> GetByIdAsync(GetProjectByIdQuery query, CancellationToken ct)
    {
        var entity = await db.Projects.FirstOrDefaultAsync(x => x.Id == query.Id, ct);
        return entity is null ? null : new GetProjectByIdResponse(...);
    }

    public async Task<AddProjectResponse> AddAsync(AddProjectCommand command, CancellationToken ct)
    {
        var entity = new Model { ... };
        db.Add(entity);
        await db.SaveChangesAsync(ct);
        return new AddProjectResponse(...);
    }

    public async Task<UpdateProjectResponse?> UpdateAsync(UpdateProjectCommand command, CancellationToken ct)
    {
        var entity = await db.Projects.FirstOrDefaultAsync(x => x.Id == command.Id, ct);
        if (entity is null) return null;
        entity.Property = command.Property;
        db.Update(entity);
        await db.SaveChangesAsync(ct);
        return new UpdateProjectResponse(...);
    }

    public async Task DeleteAsync(DeleteProjectCommand command, CancellationToken ct)
    {
        var entity = await db.Projects.FirstOrDefaultAsync(x => x.Id == command.Id, ct);
        if (entity is null) return;
        entity.Deleted = DateTime.UtcNow;
        db.Update(entity);
        await db.SaveChangesAsync(ct);
    }
}
```

### Regras:
- Métodos devem ser `public async Task<...>`
- Usar `CancellationToken` como último parâmetro
- Métodos de consulta retornam `Task<List<T>>` ou `Task<T?>`
- Métodos de escrita retornam `Task<T>` ou `Task<T?>` para update
- Delete retorna `Task`
- Usar `record` para DTOs

## 4. Padrão de Controller

Controllers injetam services diretamente via construtor, sem `ISender` ou handlers.

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
public class ProjectController(ProjectService projectService) : ControllerBase
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
        var result = await projectService.AddAsync(command, ct);
        return new CreatedResult(string.Empty, result);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UpdateProjectResponse>> PatchAsync([FromBody] UpdateProjectCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var result = await projectService.UpdateAsync(command with { Id = id }, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task Task DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        await projectService.DeleteAsync(new DeleteProjectCommand(id), ct);
        return NoContent();
    }
}
```

### Regras:
- Injetar apenas services, nunca handlers ou `ISender`
- Usar `ClaimReader.UserId(User).ToString()` para obter o usuário autenticado
- Usar `WideEventContext` para passar `UserId`
- Retornos padrão: `Ok()`, `NotFound()`, `NoContent()`, `Created()`, `Forbid()`

## 5. Padrão de DTOs

Todos os DTOs são `record` e ficam na raiz de `DTOs/`.

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

### Regras:
- Todos na pasta `DTOs/` raiz, sem subpastas
- Namespace: `Fenicia.Module.Projects.Domains.Project.DTOs`
- Usar `record` sempre
- Commands são inputs de escrita (POST, PATCH, DELETE)
- Queries são inputs de leitura (GET)
- Responses são outputs

## 6. Padrão de Testes

Seguir o padrão dos testes de `Fenicia.Auth.Tests` e `Fenicia.Module.Basic.Tests`.

### Service Tests:
```csharp
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class GetAllProjectServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProjectService service;

    public GetAllProjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new ProjectService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenProjectsExist_ReturnsPaginationWithProjects()
    {
        // Arrange
        var project = new ProjectModel { Id = Guid.NewGuid(), Title = faker.Commerce.Categories(1).First(), CompanyId = Guid.NewGuid() };
        db.Projects.Add(project);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await service.GetAllAsync(new GetAllProjectQuery(1, 10), CancellationToken.None);

        // Assert
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
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class ProjectControllerTests : IDisposable
{
    private readonly ProjectController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;

    public ProjectControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var service = new ProjectService(db);
        mockHttpContext = new Mock<HttpContext>();
        controller = new ProjectController(service) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenProjectsExist_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
        mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));
        
        // Act
        var result = await controller.GetAsync(null, 1, 10, CancellationToken.None);
        
        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }
}
```

### Regras:
- Service tests instanciam o service diretamente com `new Service(db)`
- Controller tests instanciam o service e injetam no controller
- Usar `Faker` do Bogus para dados de teste
- Usar `DefaultContext` com banco in-memory
- Sempre incluir `using System.Security.Claims` nos controller tests
- Sempre configurar `mockHttpContext.Setup(x => x.User).Returns(...)` para tests que usam `ClaimReader`

## 7. Convenções Gerais

- **Namespaces**: sempre usar o namespace completo do domínio
  - Controllers: `Fenicia.Module.Projects.Domains.Project`
  - Services: `Fenicia.Module.Projects.Domains.Project`
  - DTOs: `Fenicia.Module.Projects.Domains.Project.DTOs`
- **Usings**: sempre incluir `Fenicia.Common.API` em controllers para acessar `ClaimReader`
- **CancellationToken**: sempre incluir como último parâmetro em métodos assíncronos
- **Nullable reference types**: habilitado nos projetos
- **Record types**: usar `record` para DTOs e commands
- **Primary constructors**: usar para services (ex: `public class ProjectService(DefaultContext db)`)

## 8. O que NÃO fazer

- ❌ Criar pastas `Handlers/`
- ❌ Criar subpastas em `DTOs/` (`Commands/`, `Queries/`, `Responses/`)
- ❌ Criar pastas `Services/` dentro de domínios
- ❌ Usar `MediatR` ou `ISender`
- ❌ Implementar `IRequest` ou `IRequestHandler`
- ❌ Injetar handlers em controllers
- ❌ Usar `MediatR` nos testes (mockar services diretamente)
