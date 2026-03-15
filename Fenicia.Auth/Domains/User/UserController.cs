using System.Net.Mime;

using Fenicia.Auth.Domains.Module.Handlers;
using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Auth.Domains.User.Queries;
using Fenicia.Auth.Domains.UserRole.Handlers;
using Fenicia.Auth.Domains.UserRole.Responses;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.User;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UserController(GetUserModuleHandler getUserModuleHandler, GetUserCompaniesHandler getUserCompaniesHandler, GetUserHandler getUserHandler, CreateUserHandler createUserHandler, UpdateUserHandler updateUserHandler, GetUserByIdHandler getUserByIdHandler, DeleteUserHandler deleteUserHandler, UpdateUserPasswordHandler updateUserPasswordHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves modules available to the authenticated user for a specific company.
    /// </summary>
    /// <param name="headers">HTTP headers containing company context.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of modules the user has access to.</returns>
    /// <response code="200">Modules retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserModulesResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetUserModulesResponse>>> GetUserModulesAsync([FromHeader] Headers headers, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var companyId = headers.CompanyId;
            var query = new GetUserModulesQuery(companyId, userId);
            var response = await getUserModuleHandler.Handle(query, ct);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves all companies associated with the authenticated user.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of companies the user is associated with.</returns>
    /// <response code="200">Companies retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserCompaniesResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetUserCompaniesResponse>>> GetUserCompanyAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var response = await getUserCompaniesHandler.Handle(userId, ct);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves a paginated list of users.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Items per page (default 10).</param>
    /// <returns>Paginated list of users.</returns>
    /// <response code="200">Users retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAsync(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersQuery(page, pageSize);
        var result = await getUserHandler.Handle(query, ct);

        return Ok(result);
    }

    /// <summary>
    ///     Retrieves a user by their ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The user details if found.</returns>
    /// <response code="200">User found.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid userId, CancellationToken ct)
    {
        var user = await getUserByIdHandler.Handler(userId, ct);

        return user switch
        {
            null => NotFound(),
            _ => Ok(user)
        };
    }

    /// <summary>
    ///     Creates a new user.
    /// </summary>
    /// <param name="request">The create user command containing user details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created user response.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid request (duplicate email, invalid company, or invalid role).</response>
    /// <exception cref="InvalidRequestException">Email already exists, company not found, or role not found.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> CreateAsync(CreateUserCommand request, CancellationToken ct)
    {
        try
        {
            var result = await createUserHandler.Handle(request, ct);

            return Created(string.Empty, result);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    ///     Updates an existing user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to update.</param>
    /// <param name="request">The update command containing user details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated user response.</returns>
    /// <response code="200">User updated successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found.</response>
    /// <exception cref="InvalidRequestException">User not found, email already exists, or role not found.</exception>
    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "God,Admin")]
    public async Task<IActionResult> UpdateAsync(Guid userId, UpdateUserCommand request, CancellationToken ct)
    {
        try
        {
            var updateRequest = request with { UserId = userId };
            var result = await updateUserHandler.Handle(updateRequest, ct);

            return Ok(result);
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }

    /// <summary>
    ///     Deletes a user (soft delete).
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">User deleted successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found.</response>
    /// <exception cref="InvalidRequestException">User not found.</exception>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid userId, CancellationToken ct)
    {
        try
        {
            await deleteUserHandler.Handle(new DeleteUserCommand(userId), ct);
            return NoContent();
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }

    /// <summary>
    ///     Updates the password of a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="request">The password update command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Password update response.</returns>
    /// <response code="200">Password updated successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found.</response>
    /// <exception cref="InvalidRequestException">User not found.</exception>
    [HttpPatch("{userId:guid}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ChangePasswordAsync(Guid userId, UpdateUserPasswordCommand request, CancellationToken ct)
    {
        try
        {
            var updateRequest = request with { UserId = userId };
            var result = await updateUserPasswordHandler.Handle(updateRequest, ct);

            return Ok(result);
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }
}
