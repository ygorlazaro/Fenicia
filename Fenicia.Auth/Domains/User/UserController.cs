using System.Net.Mime;
using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.Module;
using Fenicia.Common.DTOs.Auth.User;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserCompanyResponse = Fenicia.Common.DTOs.Auth.UserRole.UserCompanyResponse;

namespace Fenicia.Auth.Domains.User;

/// <summary>
/// Controller for managing user-related operations.
/// </summary>
[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UserController(IUserService userService, IModuleService moduleService, IUserRoleService userRoleService) : ControllerBase
{
    /// <summary>
    /// Gets modules for a specific user and company.
    /// </summary>
    /// <param name="id">ID of the user</param>
    /// <param name="headers">Request headers (includes CompanyId)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of modules for the user in the company</returns>
    /// <response code="200">Modules found</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User does not have permission to access modules for this company</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id:guid}/module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleByUserResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<ModuleByUserResponse>>> GetUserModulesAsync(
        [FromRoute] Guid id,
        [FromHeader] Headers headers,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);

            await userService.EnsureCanAccessUserAsync(userId, id, headers.CompanyId, cancellationToken);

            var companyId = headers.CompanyId;
            var response = await moduleService.GetUserModulesAsync(companyId, id, cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets companies associated with a user.
    /// </summary>
    /// <param name="id">ID of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of companies for the user</returns>
    /// <response code="200">Companies found</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User does not have permission to access companies for this user</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id:guid}/company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserCompanyResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserCompanyResponse>>> GetUserCompanyAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var loggedInUserId = ClaimReader.UserId(User);

            await userService.EnsureCanAccessUserAsync(loggedInUserId, id, null, cancellationToken);

            var response = await userRoleService.GetUserCompaniesAsync(id, cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets all users with pagination.
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    /// <response code="200">List of users returned successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await userService.GetAllAsync(new UserRequest(), page, pageSize, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User data</returns>
    /// <response code="200">User found</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);

        return user switch
        {
            null => NotFound(),
            _ => Ok(user)
        };
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">User data (email, password, name, roles)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user data</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Email already exists or invalid data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync(
        UserRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await userService.CreateAsync(request, cancellationToken);

            return Created(string.Empty, result);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Updated user data (name, email, roles by company)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user data</returns>
    /// <response code="200">User updated successfully</response>
    /// <response code="400">Email already exists or invalid data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "God,Admin")]
    public async Task<IActionResult> UpdateAsync(
        Guid userId,
        UserRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            request.Id = userId;
            var result = await userService.UpdateAsync(request, cancellationToken);

            return Ok(result);
        }
        catch (BadRequestException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Removes a user (soft delete).
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content (204) if removed successfully</returns>
    /// <response code="204">User removed successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await userService.DeleteAsync(userId, cancellationToken);
            return NoContent();
        }
        catch (BadRequestException)
        {
            return NotFound();
        }
    }
}
