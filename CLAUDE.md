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

## 3. Padrão de Repository

Services **NÃO** podem acessar `DbContext` diretamente. Toda comunicação com banco deve ser feita através de repositories.

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

### Regras:
- Todos os métodos são `async`
- Todos recebem `CancellationToken` como último parâmetro
- Métodos de escrita retornam `Task<T>` ou `Task<T?>`
- Delete retorna `Task<int>` com quantidade de registros afetados
- `SaveChangesAsync` é usado como transação; não chamar `SaveChangesAsync` fora do repository
- Sempre filtrar por `Deleted == null` nas queries
- Repositories retornam apenas entidades ou primitivos, nunca DTOs ou Responses
- `Context` é público para permitir acesso cross-assembly

## 4. Padrão de Service

Services recebem `IRepository<T>` via construtor e expõem métodos públicos assíncronos.

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

### Regras:
- Métodos devem ser `public async Task<...>`
- Usar `CancellationToken` como último parâmetro
- Services não acessam `DbContext` diretamente
- Usar `record` para DTOs
- Services de escrita devem receber `CompanyId` quando a entidade herda de `BaseCompanyModel`
- `CompanyId` nunca deve vir do command/query; sempre do token

## 5. Padrão de Controller

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

### Regras:
- Injetar apenas services, nunca handlers ou `ISender`
- Usar `ClaimReader.UserId(User).ToString()` para obter o usuário autenticado
- Usar `WideEventContext` para passar `UserId`
- Retornos padrão: `Ok()`, `NotFound()`, `NoContent()`, `Created()`, `Forbid()`
- Passar `CompanyId` para services de escrita quando a entidade herda de `BaseCompanyModel`

## 6. Padrão de DTOs

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

## 7. Padrão de Testes

Seguir o padrão dos testes de `Fenicia.Auth.Tests` e `Fenicia.Module.Projects.Tests`.

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

## 8. Convenções Gerais

- **Namespaces**: sempre usar o namespace completo do domínio
  - Controllers: `Fenicia.Module.Projects.Domains.Project`
  - Services: `Fenicia.Module.Projects.Domains.Project`
  - DTOs: `Fenicia.Module.Projects.Domains.Project.DTOs`
- **Usings**: sempre incluir `Fenicia.Common.API` em controllers para acessar `ClaimReader`
- **CancellationToken**: sempre incluir como último parâmetro em métodos assíncronos
- **Nullable reference types**: habilitado nos projetos
- **Record types**: usar `record` para DTOs e commands
- **Primary constructors**: usar para services (ex: `public class ProjectService(DefaultContext db)`)
- **Campos privados**: usar `_camelCase` (ex: `private readonly DefaultContext _db;`)
- **Métodos async**: sempre terminam com `Async` (ex: `GetAsync`)
- **XML comments**: não obrigatórios
- **Usings desnecessários**: não obrigatória remoção
- **Migrations**: excluídas das regras de estilo

## 9. Regras de Estilo (StyleCop/EditorConfig)

O projeto usa regras rigorosas de estilo definidas no `.editorconfig`. Antes de commitar, garanta que o build não tem erros de StyleCop.

### Regras obrigatórias:
- **SA1201**: ordem de membros (campos > construtores > propriedades > eventos > métodos)
- **SA1202**: ordem por visibilidade (public > internal > protected > private)
- **SA1203**: campos `const` antes de campos não-`const`
- **SA1214**: campos `static readonly` antes de campos de instância
- **SA1208**: `using` de `System.*` antes dos demais
- **SA1210**: `using` em ordem alfabética
- **SA1211**: `using` estáticos depois dos normais
- **SA1503/SA1519**: chaves obrigatórias mesmo em blocos de uma linha
- **IDE0130**: namespace deve bater com estrutura de pastas
- **CA1852**: classes internas não devem ser herdadas a menos que projetadas para isso
- **CA1031**: não capturar `Exception` genérica sem tratamento específico
- **CA2201**: não lançar `Exception` ou `SystemException` diretamente

### Exceções:
- **Migrations**: arquivos em `Migrations/` estão excluídos das regras acima
- **SA0001/IDE0005**: desabilitados globalmente (não exigem documentação XML nem remoção de usings)

## 10. O que NÃO fazer

- ❌ Criar pastas `Handlers/`
- ❌ Criar subpastas em `DTOs/` (`Commands/`, `Queries/`, `Responses/`)
- ❌ Criar pastas `Services/` dentro de domínios
- ❌ Usar `MediatR` ou `ISender`
- ❌ Implementar `IRequest` ou `IRequestHandler`
- ❌ Injetar handlers em controllers
- ❌ Usar `MediatR` nos testes (mockar services diretamente)
- ❌ Services acessarem `DbContext` diretamente
- ❌ Repositories retornarem DTOs ou Responses
- ❌ Lançar `Exception` genérica (usar tipos específicos)
- ❌ Capturar `Exception` genérica sem tratamento específico
- ❌ Omitir chaves em blocos `if`/`for`/`while`
- ❌ Usar `this.` em métodos de instância
- ❌ Esquecer de incluir `CancellationToken` como último parâmetro em métodos async
