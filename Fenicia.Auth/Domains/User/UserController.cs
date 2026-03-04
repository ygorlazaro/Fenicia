using System.Net.Mime;

using Fenicia.Auth.Domains.User.ChangeUserPassword;
using Fenicia.Auth.Domains.User.CreateUser;
using Fenicia.Auth.Domains.User.DeleteUser;
using Fenicia.Auth.Domains.User.GetUserModules;
using Fenicia.Auth.Domains.User.ListUsers;
using Fenicia.Auth.Domains.User.UpdateUser;
using Fenicia.Auth.Domains.UserRole.GetUserCompanies;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    /// <summary>
    /// Get user modules
    /// </summary>
    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserModulesResponse))]
    public async Task<ActionResult<List<GetUserModulesResponse>>> GetUserModulesAsync(
        [FromHeader] Headers headers,
        [FromServices] GetUserModuleHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var companyId = headers.CompanyId;

        wide.UserId = userId.ToString();
        var query = new GetUserModulesQuery(companyId, userId);

        var response = await handler.Handler(query, ct);

        return Ok(response);
    }

    /// <summary>
    /// Get user companies
    /// </summary>
    [HttpGet("company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserCompaniesResponse))]
    public async Task<ActionResult<List<GetUserCompaniesResponse>>> GetUserCompanyAsync(
        [FromServices] GetUserCompaniesHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);

        wide.UserId = userId.ToString();

        var response = await handler.Handle(userId, ct);

        return Ok(response);
    }

    /// <summary>
    /// List all users with pagination and alphabetical sorting
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromServices] ListUsersHandler handler = null!,
        CancellationToken ct = default)
    {
        // Validate role - only God and Admin can list users
        ClaimReader.ValidateRole(this.User, "God");
        
        var query = new ListUsersQuery(page, pageSize, searchTerm);
        var result = await handler.Handle(query, ct);
        
        return Ok(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(
        Guid userId,
        [FromServices] DefaultContext context = null!,
        CancellationToken ct = default)
    {
        // Validate role - only God and Admin can get user details
        ClaimReader.ValidateRole(this.User, "God");

        var user = await context.AuthUsers
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.RoleModel)
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.CompanyModel)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null || user.Deleted.HasValue)
        {
            return NotFound(new { Message = "User not found" });
        }

        // God users can see all users
        // Admin users can only see users from their companies
        var userRoles = this.User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
        
        if (userRoles.Contains("God", StringComparer.OrdinalIgnoreCase))
        {
            // God users can see all users
            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Created,
                user.Updated,
                Companies = user.UsersRoles.Select(ur => new
                {
                    ur.CompanyId,
                    CompanyName = ur.CompanyModel.Name,
                    ur.RoleId,
                    RoleName = ur.RoleModel.Name
                }).ToList()
            });
        }

        // For Admin users, check if they have access to this user's companies
        var currentUserId = ClaimReader.UserId(this.User);
        var adminCompanies = await context.UserRoles
            .Where(ur => ur.UserId == currentUserId)
            .Select(ur => ur.CompanyId)
            .ToListAsync(ct);

        var userHasAccessToCompany = user.UsersRoles
            .Any(ur => adminCompanies.Contains(ur.CompanyId));

        if (!userHasAccessToCompany)
        {
            return NotFound(new { Message = "User not found" });
        }

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.Created,
            user.Updated,
            Companies = user.UsersRoles.Select(ur => new
            {
                ur.CompanyId,
                CompanyName = ur.CompanyModel.Name,
                ur.RoleId,
                RoleName = ur.RoleModel.Name
            }).ToList()
        });
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> CreateAsync(
        CreateUserQuery request,
        [FromServices] CreateUserHandler handler = null!,
        CancellationToken ct = default)
    {
        // Validate role - only God and Admin can create users
        ClaimReader.ValidateRole(this.User, "God");

        var userRoles = this.User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
        var currentUserId = ClaimReader.UserId(this.User);

        // Admin users can only create users for their companies
        if (userRoles.Contains("Admin", StringComparer.OrdinalIgnoreCase) && 
            !userRoles.Contains("God", StringComparer.OrdinalIgnoreCase))
        {
            if (request.CompaniesRoles != null && request.CompaniesRoles.Any())
            {
                var adminCompanies = await this.HttpContext.RequestServices
                    .GetRequiredService<DefaultContext>()
                    .UserRoles
                    .Where(ur => ur.UserId == currentUserId)
                    .Select(ur => ur.CompanyId)
                    .ToListAsync(ct);

                var invalidCompanies = request.CompaniesRoles
                    .Where(cr => !adminCompanies.Contains(cr.CompanyId))
                    .ToList();

                if (invalidCompanies.Any())
                {
                    return BadRequest(new 
                    { 
                        Message = "You can only create users for companies you have access to",
                        InvalidCompanies = invalidCompanies.Select(c => c.CompanyId).ToList()
                    });
                }
            }
        }

        var result = await handler.Handle(request, ct);

        return Created($"/user/{result.Id}", result);
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateAsync(
        Guid userId,
        UpdateUserQuery request,
        [FromServices] UpdateUserHandler handler = null!,
        [FromServices] DefaultContext context = null!,
        CancellationToken ct = default)
    {
        // Validate role - only God and Admin can update users
        ClaimReader.ValidateRole(this.User, "God");

        var userRoles = this.User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
        var currentUserId = ClaimReader.UserId(this.User);

        // Check if user exists
        var user = await context.AuthUsers
            .Include(u => u.UsersRoles)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null || user.Deleted.HasValue)
        {
            return NotFound(new { Message = "User not found" });
        }

        // Admin users can only update users from their companies
        if (userRoles.Contains("Admin", StringComparer.OrdinalIgnoreCase) && 
            !userRoles.Contains("God", StringComparer.OrdinalIgnoreCase))
        {
            var adminCompanies = await context.UserRoles
                .Where(ur => ur.UserId == currentUserId)
                .Select(ur => ur.CompanyId)
                .ToListAsync(ct);

            var userHasAccessToCompany = user.UsersRoles
                .Any(ur => adminCompanies.Contains(ur.CompanyId));

            if (!userHasAccessToCompany)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Validate that admin is not trying to assign companies they don't have access to
            if (request.CompaniesRoles != null && request.CompaniesRoles.Any())
            {
                var invalidCompanies = request.CompaniesRoles
                    .Where(cr => !adminCompanies.Contains(cr.CompanyId))
                    .ToList();

                if (invalidCompanies.Any())
                {
                    return BadRequest(new 
                    { 
                        Message = "You can only assign companies you have access to",
                        InvalidCompanies = invalidCompanies.Select(c => c.CompanyId).ToList()
                    });
                }
            }
        }

        // Update user ID in request
        var updateRequest = request with { UserId = userId };
        var result = await handler.Handle(updateRequest, ct);

        return Ok(result);
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        Guid userId,
        [FromServices] DeleteUserHandler handler = null!,
        [FromServices] DefaultContext context = null!,
        CancellationToken ct = default)
    {
        // Validate role - only God users can delete users
        ClaimReader.ValidateRole(this.User, "God");

        var userRoles = this.User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

        // Only God users can delete users
        if (!userRoles.Contains("God", StringComparer.OrdinalIgnoreCase))
        {
            return Unauthorized(new { Message = "Only God users can delete users" });
        }

        // Check if user exists
        var user = await context.AuthUsers.FindAsync(userId, ct);

        if (user == null || user.Deleted.HasValue)
        {
            return NotFound(new { Message = "User not found" });
        }

        // Prevent self-deletion
        var currentUserId = ClaimReader.UserId(this.User);
        if (userId == currentUserId)
        {
            return BadRequest(new { Message = "Cannot delete yourself" });
        }

        var result = await handler.Handle(new DeleteUserQuery(userId), ct);

        return Ok(result);
    }

    /// <summary>
    /// Change user password (admin function)
    /// </summary>
    [HttpPatch("{userId:guid}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ChangePasswordAsync(
        Guid userId,
        ChangeUserPasswordQuery request,
        [FromServices] ChangeUserPasswordHandler handler = null!,
        [FromServices] DefaultContext context = null!,
        CancellationToken ct = default)
    {
        // Validate role - only God and Admin can change passwords
        ClaimReader.ValidateRole(this.User, "God");

        var userRoles = this.User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
        var currentUserId = ClaimReader.UserId(this.User);

        // Check if user exists
        var user = await context.AuthUsers
            .Include(u => u.UsersRoles)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null || user.Deleted.HasValue)
        {
            return NotFound(new { Message = "User not found" });
        }

        // Admin users can only change passwords for users from their companies
        if (userRoles.Contains("Admin", StringComparer.OrdinalIgnoreCase) && 
            !userRoles.Contains("God", StringComparer.OrdinalIgnoreCase))
        {
            var adminCompanies = await context.UserRoles
                .Where(ur => ur.UserId == currentUserId)
                .Select(ur => ur.CompanyId)
                .ToListAsync(ct);

            var userHasAccessToCompany = user.UsersRoles
                .Any(ur => adminCompanies.Contains(ur.CompanyId));

            if (!userHasAccessToCompany)
            {
                return NotFound(new { Message = "User not found" });
            }
        }

        // Update user ID in request
        var updateRequest = request with { UserId = userId };
        var result = await handler.Handle(updateRequest, ct);

        return Ok(result);
    }
}
