<!-- arko:start -->
## Security — Arko (required)

This project uses Arko for security scanning via MCP. Any AI coding agent
working here MUST follow this loop — it is not optional:

1. **Scan after writing or changing code, before presenting the work as done.**
   Call `arko_scan_code` on the files you changed (or `arko_scan_project` on a
   dependency manifest). Read line 1 of the result: the gate.
2. **If the gate fails** (`GATE: FAIL — …`), call `arko_fix_all`, apply every
   fix in the plan, then `arko_validate_fix` on the updated files. Repeat until
   a fresh scan reports `GATE: PASS`.
3. **Never** conclude the task, commit, or open a PR while the gate is failing,
   and never dismiss, suppress, or downgrade a finding silently — surface it and
   let a human decide.

A scan that is still running is NOT a pass — re-run it; only a terminal
`GATE: PASS` clears the loop.
<!-- arko:end -->

---

# Fenicia Code Patterns & Standards

This document defines mandatory patterns and conventions for all code in this repository.

## 1. XML Documentation (MANDATORY)

**All public types must have XML documentation:**

- **Interfaces**: `/// <summary>` on interface and every member
- **Classes** (DTOs, Models, Services, Repositories, Controllers, etc.): `/// <summary>` on class, constructors, methods, and properties
- **Enums**: `/// <summary>` on enum and each value
- **Methods**: `/// <summary>`, `/// <param>`, `/// <returns>`, `/// <exception>`, `/// <param name="cancellationToken">`

```csharp
/// <summary>
/// Repository implementation for managing address data.
/// </summary>
/// <param name="context">The database context.</param>
public class AddressRepository(DbContext context) : Repository<AddressModel>(context), IAddressRepository
{
    /// <summary>
    /// Checks if there are any active companies associated with the specified company ID.
    /// </summary>
    /// <param name="companyId">The ID of the company to check.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The CompanyModel if found, or null if not found.</returns>
    public Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        ...
    }
}
```

## 2. Primary Constructors (MANDATORY)

**Use primary constructors for all classes and records:**

```csharp
// ✓ Correct - Primary constructor with DI
public sealed class AddressService(IAddressRepository repository) : IAddressService { ... }

// ✓ Correct - Record with primary constructor
public record UploadFileResponse(
    [Required] Guid Id,
    [Required] string OriginalFileName);

// ✗ Wrong - Parameterless constructor with null assignments
public class CustomerService(
    ICustomerRepository repository,
    IPersonService personService) : ICustomerService
{
    public CustomerService()
        : this(null!, null!)  // FORBIDDEN
    { }
}
```

**FORBIDDEN**: Any constructor that passes `null!` or default values for required dependencies.

## 3. Repository Pattern

```csharp
// Interface
public interface IAddressRepository : IRepository<AddressModel>
{
    Task<AddressModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
}

// Implementation
public class AddressRepository(DbContext context) : Repository<AddressModel>(context), IAddressRepository
{
    public Task<AddressModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(a => a.State)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}
```

- Extend `IRepository<T>` base interface
- Implement in `{Domain}Repository` class
- Inject `DbContext` via primary constructor
- Return domain models (not DTOs)

## 4. Service Pattern

```csharp
// Interface - returns DTOs, accepts CancellationToken
public interface IAddressService
{
    Task<AddressResponse> AddAsync(AddressRequest command, CancellationToken cancellationToken = default);
    Task<AddressResponse?> UpdateAsync(Guid id, AddressRequest command, CancellationToken cancellationToken = default);
}

// Implementation - sealed, primary constructor
public sealed class AddressService(IAddressRepository repository) : IAddressService
{
    public async Task<AddressResponse> AddAsync(
        AddressRequest command,
        CancellationToken cancellationToken = default)
    {
        var address = new AddressModel { ... };
        await repository.InsertAsync(address, cancellationToken);
        var saved = await ReloadAsync(address.Id, cancellationToken);
        return MapToResponse(saved);
    }
}
```

- Interface defines contract returning **DTOs** (not domain models)
- All async methods accept `CancellationToken = default`
- Implementation is `sealed`
- Uses primary constructor for DI
- Maps domain models → DTOs in service (or via Mapper)

## 5. Mapper Pattern (Static Classes)

```csharp
public static class AddressMapper
{
    /// <summary>
    /// Maps an AddressModel to an AddressResponse.
    /// </summary>
    public static AddressResponse MapToResponse(AddressModel address)
    {
        return new AddressResponse(
            address.Id,
            address.Street,
            address.Number,
            address.Complement,
            address.Neighborhood,
            address.ZipCode!,
            address.StateId,
            address.State?.Name,
            address.City,
            address.Country);
    }

    /// <summary>
    /// Maps an AddressRequest to an AddressModel.
    /// </summary>
    public static AddressModel MapToModel(AddressRequest request)
    {
        return new AddressModel
        {
            Id = request.Id,
            Street = request.Street,
            ...
        };
    }
}
```

- Static class named `{Entity}Mapper`
- Methods: `MapTo{Dto}Response`, `MapTo{Entity}Model`
- XML docs on every method
- No instance state

## 6. DTO Pattern (Records/Classes with Primary Constructors)

```csharp
/// <summary>
/// Request DTO for creating or updating an address.
/// </summary>
public class AddressRequest()
{
    /// <summary>
    /// Initializes a new instance with the specified details.
    /// </summary>
    public AddressRequest(
        string street,
        string number,
        string? complement,
        ...
        string? country)
        : this()
    {
        Street = street;
        Number = number;
        ...
    }

    /// <summary>Gets or sets the street name.</summary>
    [Required]
    [MaxLength(50)]
    public string Street { get; set; } = string.Empty;
    ...
}
```

- Use `record` for immutable response DTOs, `class` for request DTOs
- Primary constructor with parameter documentation
- Default parameterless constructor for model binding
- Validation attributes on properties
- XML docs on class, constructor, and every property

## 7. Controller Pattern

```csharp
/// <summary>
/// Controller responsible for handling address-related operations.
/// </summary>
/// <param name="service">The address service.</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class AddressController(IAddressService service) : ControllerBase
{
    /// <summary>
    /// Adds a new address.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddressResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddressResponse>> PostAsync(
        [FromBody] AddressRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await service.AddAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAsync), new { id = result.Id }, result);
    }
}
```

- Primary constructor for service injection
- XML docs on class and every action
- Standard HTTP attributes
- `CancellationToken` on all async actions
- Proper status codes and response types

## 8. Dependency Injection Registration

```csharp
// In Program.cs
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();
```

- Use `AddScoped` for repositories and services
- Register in the appropriate `Program.cs` (Auth vs Module.Basic)
- Match interface → implementation

## 9. Cancellation Token Convention

```csharp
// ✓ Correct - default parameter
public async Task<AddressResponse> AddAsync(
    AddressRequest command,
    CancellationToken cancellationToken = default)

// ✗ Wrong - no cancellation token
public async Task<AddressResponse> AddAsync(AddressRequest command)
```

**All async methods must accept `CancellationToken cancellationToken = default`**

## 10. Namespace Structure

```
Fenicia.Auth.Domains.{Domain}
├── Interfaces/
│   ├── I{Domain}Repository.cs
│   └── I{Domain}Service.cs
├── {Domain}Repository.cs
├── {Domain}Service.cs
├── {Domain}Mapper.cs (if needed)
└── {Domain}Controller.cs (if needed)

Fenicia.Common.DTOs.Auth.{Domain}
├── {Domain}Request.cs
└── {Domain}Response.cs

Fenicia.Common.Data.Models.Auth
└── {Domain}Model.cs
```

---

## 11. DTO Naming Conventions (Basic Module)

**MANDATORY**: All DTOs in `Fenicia.Common.DTOs.Basic` must follow these naming conventions:

### Request DTOs
- **Unified naming**: Use `{Entity}Request` for both create and update operations
  - ✓ `CustomerRequest` (replaces `AddCustomerRequest`, `UpdateCustomerRequest`)
  - ✓ `EmployeeRequest` (replaces `AddEmployeeRequest`, `UpdateEmployeeRequest`)
  - ✗ `AddCustomerRequest`, `UpdateCustomerRequest`, `CreateCustomerCommand`, `UpdateCustomerCommand`

### Response DTOs
- **Unified naming**: Use `{Entity}Response` for single item, list items, and create/update responses
  - ✓ `CustomerResponse` (replaces `GetCustomerByIdResponse`, `GetAllCustomerResponse`, `AddCustomerResponse`, `UpdateCustomerResponse`)
  - ✓ `EmployeeResponse` (replaces `GetEmployeeByIdResponse`, `GetAllEmployeeResponse`, `AddEmployeeResponse`, `UpdateEmployeeResponse`)
  - ✗ `GetCustomerByIdResponse`, `GetAllCustomerResponse`, `AddCustomerResponse`, `UpdateCustomerResponse`

### Query/Filter DTOs
- **Rename rule**: All `*Query.cs` files must be renamed to `*Request.cs` (e.g., `CustomerInsightsQuery.cs` → `CustomerInsightsRequest.cs`)
- Use `{Entity}Request` for specialized query/filter parameters
  - ✓ `CustomerInsightsRequest`, `EmployeePerformanceRequest`, `ProductPerformanceRequest`
  - ✗ `CustomerInsightsQuery`, `GetCustomerInsightsQuery`, `GetEmployeePerformanceQuery`
- Use `{Entity}Query` for pagination/filtering parameters
  - ✓ `CustomerQuery`, `EmployeeQuery`, `ProductQuery`
  - ✗ `GetAllCustomerQuery`, `GetAllEmployeeQuery`

### DataSource DTOs (Dropdowns/Lookups)
- Use `{Entity}DataSourceResponse` for simplified lookup responses
  - ✓ `CustomerDataSourceResponse`, `EmployeeDataSourceResponse`, `ProductDataSourceResponse`
  - ✗ `GetAllCustomerForDataSourceResponse`, `GetAllEmployeeForDataSourceResponse`

### Specialized Analytics/Insights DTOs
- Keep descriptive names for complex analytics responses
  - ✓ `CustomerInsightsResponse`, `EmployeePerformanceResponse`, `ProductPerformanceResponse`
  - ✓ `CustomerSummaryResponse`, `CustomerOrderHistoryResponse`, `CustomerRiskAlertResponse`

### Verb Removal Rules
**FORBIDDEN**: Any DTO name containing these verbs:
- `Add`, `Create`, `Insert`
- `Update`, `Modify`, `Edit`, `Patch`
- `Delete`, `Remove`, `Destroy`
- `Get`, `Fetch`, `Retrieve`, `Find`
- `ById`, `ByIdAsync` (when only ID is needed — use bare `Guid` parameter instead)

### ID Parameter Convention
- **FORBIDDEN**: `Get*ByIdQuery` or `Delete*Request` classes that only contain an `Id` property
- **REQUIRED**: Use bare `Guid` parameter in service methods and controller actions
  - ✓ `Task<CustomerResponse?> GetByIdAsync(Guid id, ...)`
  - ✓ `Task DeleteAsync(Guid id, ...)`
  - ✗ `Task<CustomerResponse?> GetByIdAsync(GetCustomerByIdQuery query, ...)`
  - ✗ `Task DeleteAsync(DeleteCustomerRequest command, ...)`

### DTO Unification Rules
- **Request**: Single `{Entity}Request` class for both POST and PATCH/PUT
- **Response**: Single `{Entity}Response` class for GET (single), GET (list), POST, PATCH/PUT
- **DataSource**: Single `{Entity}DataSourceResponse` for dropdown/lookup scenarios

---

# Compliance Checklist for Code Review

When reviewing code, verify:

- [ ] All public types have `/// <summary>` documentation
- [ ] All constructors use primary constructor syntax
- [ ] No `null!` passed to required dependencies
- [ ] All async methods have `CancellationToken = default`
- [ ] Services return DTOs, not domain models
- [ ] Repositories extend `IRepository<T>`
- [ ] Mappers are static classes with `MapTo*` methods
- [ ] DTOs have validation attributes and XML docs
- [ ] Controllers have XML docs and proper HTTP attributes
- [ ] DI registrations use `AddScoped`
- [ ] DTO names follow Basic module naming conventions (no verbs, unified request/response)
- [ ] No `Get*ByIdQuery` or `Delete*Request` single-ID wrapper classes
- [ ] DataSource DTOs use `{Entity}DataSourceResponse` naming