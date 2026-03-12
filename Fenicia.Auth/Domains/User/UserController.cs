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
public class UserController(
    GetUserModuleHandler getUserModuleHandler,
    GetUserCompaniesHandler getUserCompaniesHandler,
    GetUserHandler getUserHandler,
    CreateUserHandler createUserHandler,
    UpdateUserHandler updateUserHandler,
    GetUserByIdHandler getUserByIdHandler,
    DeleteUserHandler deleteUserHandler,
    UpdateUserPasswordHandler updateUserPasswordHandler) : ControllerBase
{
    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserModulesResponse))]
    public async Task<ActionResult<List<GetUserModulesResponse>>> GetUserModulesAsync(
        [FromHeader] Headers headers,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        var companyId = headers.CompanyId;
        var query = new GetUserModulesQuery(companyId,
            userId);
        var response = await getUserModuleHandler.Handle(query,
            ct);

        return Ok(response);
    }

    [HttpGet("company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserCompaniesResponse))]
    public async Task<ActionResult<List<GetUserCompaniesResponse>>> GetUserCompanyAsync(
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        var response = await getUserCompaniesHandler.Handle(userId,
            ct);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAsync(
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersQuery(page,
            pageSize);
        var result = await getUserHandler.Handle(query,
            ct);

        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(
        Guid userId,
        CancellationToken ct)
    {
        var user = await getUserByIdHandler.Handler(userId,
            ct);

        return user switch
        {
            null => NotFound(),
            _ => Ok(user)
        };

    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> CreateAsync(
        CreateUserCommand request,
        CancellationToken ct)
    {
        var result = await createUserHandler.Handle(request,
            ct);

        return Created($"/user/{result.Id}",
            result);
    }

    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "God,Admin")]
    public async Task<IActionResult> UpdateAsync(
        Guid userId,
        UpdateUserCommand request,
        CancellationToken ct)
    {
        try
        {
            var updateRequest = request with { UserId = userId };
            var result = await updateUserHandler.Handle(updateRequest,
                ct);

            return Ok(result);
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        Guid userId,
        CancellationToken ct)
    {
        try
        {
            await deleteUserHandler.Handle(new DeleteUserCommand(userId),
                ct);
            return NoContent();
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{userId:guid}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ChangePasswordAsync(
        Guid userId,
        UpdateUserPasswordCommand request,
        CancellationToken ct)
    {
        try
        {
            var updateRequest = request with { UserId = userId };
            var result = await updateUserPasswordHandler.Handle(updateRequest,
                ct);

            return Ok(result);
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }
}
