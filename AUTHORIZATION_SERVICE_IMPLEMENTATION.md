# Authorization Service Implementation

## Overview

This document describes the implementation of authorization service injection for unit testing in the Fenicia.Auth project. The goal was to enable proper role-based authorization checks while maintaining testability.

## Problem

The original implementation had the following issues:

1. **`[Authorize]` attribute doesn't work in unit tests** - ASP.NET Core middleware isn't present
2. **Authorization checks were mixed with business logic** - Making tests difficult to write
3. **Handlers had direct DbContext dependency only** - No way to check user roles from claims
4. **Tests expected `UnauthorizedAccessException` but got `InvalidRequestException`** - Wrong order of validation

## Solution

### 1. Created `IUserAuthorizationService`

A dedicated service for authorization checks that can be easily mocked in unit tests:

```csharp
public interface IUserAuthorizationService
{
    Task<bool> HasGodRoleAsync(Guid userId, CancellationToken ct = default);
    Task<bool> HasAdminRoleAsync(Guid userId, Guid companyId, CancellationToken ct = default);
    Task<bool> CanAccessUserAsync(Guid currentUserId, Guid targetUserId, CancellationToken ct = default);
}
```

### 2. Implemented `UserAuthorizationService`

```csharp
public class UserAuthorizationService(DefaultContext db) : IUserAuthorizationService
{
    public async Task<bool> HasGodRoleAsync(Guid userId, CancellationToken ct = default)
    {
        return await db.AuthUserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "God", ct);
    }
    
    // ... other methods
}
```

### 3. Updated Handlers

All handlers that require authorization now depend on:
- `IHttpContextAccessor` - To get the current user from HTTP context
- `IUserAuthorizationService` - To check user roles and permissions

Example:
```csharp
public class UpdateUserHandler(
    DefaultContext db,
    IHttpContextAccessor httpContextAccessor,
    IUserAuthorizationService authorizationService)
{
    public virtual async Task<UpdateUserResponse> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var httpContext = httpContextAccessor.HttpContext;
        UserAuthorizationService.RequireHttpContext(httpContext);
        
        var currentUserId = UserAuthorizationService.GetCurrentUserId(httpContext);

        // Check authorization FIRST - before business logic
        var canAccess = await authorizationService.CanAccessUserAsync(currentUserId, command.UserId, ct);

        if (!canAccess)
        {
            throw new UnauthorizedAccessException();
        }

        // Then proceed with business logic
        var user = await db.AuthUsers.ExisingAsync(command.UserId, ct);
        // ...
    }
}
```

### 4. Updated Tests

Tests now mock both `IHttpContextAccessor` and `IUserAuthorizationService`:

```csharp
public UserControllerTests()
{
    this.mockHttpContext = new Mock<HttpContext>();
    this.mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
    this.mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(this.mockHttpContext.Object);
    this.mockAuthorizationService = new Mock<IUserAuthorizationService>();

    var updateUserHandler = new UpdateUserHandler(
        this.context, 
        this.mockHttpContextAccessor.Object, 
        this.mockAuthorizationService.Object);
    
    // ...
}

private void SetupUserClaims(Guid userId, string? role = null)
{
    // Setup claims...
    
    // Setup authorization service mock based on role
    var hasGodRole = role == "God";
    this.mockAuthorizationService
        .Setup(x => x.HasGodRoleAsync(userId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(hasGodRole);
}
```

### 5. Registered Services in DI Container

Updated `Program.cs`:

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
```

### 6. Added Authorization Policies

Updated `FeniciaAuthenticationExtensions.cs`:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("God", policy => policy.RequireRole("God"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});
```

## Authorization Rules

### Update User
- **God role**: Can update any user
- **Regular users**: Can only update themselves

### Delete User
- **God role**: Can delete any user
- **Regular users**: Cannot delete users (always unauthorized)

### Create User
- **God role**: Can create new users
- **Regular users**: Cannot create users (always unauthorized)

### Update Password
- **God role**: Can update any user's password
- **Regular users**: Can only update their own password

### List Users (GetAsync)
- **God role**: Can list all users
- **Regular users**: Cannot list all users (always unauthorized)

## Key Benefits

1. **Testable**: Authorization logic can be easily mocked in unit tests
2. **Separation of Concerns**: Authorization is separate from business logic
3. **Consistent**: All handlers follow the same pattern
4. **Correct Order**: Authorization is checked BEFORE business validation
5. **Proper Exceptions**: Throws `UnauthorizedAccessException` for auth failures

## Files Changed

### New Files
- `Fenicia.Auth/Services/IUserAuthorizationService.cs`
- `Fenicia.Auth/Services/UserAuthorizationService.cs`
- `Fenicia.Auth/Services/AuthorizationServiceExtensions.cs`

### Modified Files
- `Fenicia.Auth/Program.cs` - Registered services
- `Fenicia.Common.Api/Startup/FeniciaAuthenticationExtensions.cs` - Added authorization policies
- `Fenicia.Auth/Domains/User/Handlers/*.cs` - Updated handlers with authorization
- `Fenicia.Auth.Tests/Domains/User/*.cs` - Updated tests with mocks

## Testing

Run the tests to verify the implementation:

```bash
dotnet test Fenicia.Auth.Tests/Fenicia.Auth.Tests.csproj --filter "FullyQualifiedName~UserControllerTests"
```

Key tests that should now pass:
- `UpdateAsync_WithoutGodRole_ThrowsUnauthorizedAccessException`
- `DeleteAsync_WithoutGodRole_ReturnsUnauthorized`
- `CreateAsync_WithoutGodRole_ThrowsUnauthorizedAccessException`
- `GetAsync_WithoutGodRole_ThrowsUnauthorizedAccessException`
- `ChangePasswordAsync_WithoutGodRole_ThrowsUnauthorizedAccessException`

## Notes

- The `[Authorize]` attribute is still kept on controllers for runtime authentication
- Handler-level authorization provides fine-grained control and testability
- The `UserAuthorizationService` can be extended with more complex authorization logic as needed
