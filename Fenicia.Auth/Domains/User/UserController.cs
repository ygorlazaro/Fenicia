using System.Net.Mime;

using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Queries;
using Fenicia.Auth.Domains.UserRole.Responses;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.User;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UserController(ISender sender) : ControllerBase
{

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
            var response = await sender.Send(query, ct);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserCompaniesResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetUserCompaniesResponse>>> GetUserCompanyAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var response = await sender.Send(new GetUserCompaniesQuery(userId), ct);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAsync(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersQuery(page, pageSize);
        var result = await sender.Send(query, ct);

        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid userId, CancellationToken ct)
    {
        var user = await sender.Send(new GetUserByIdQuery(userId), ct);

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
    public async Task<IActionResult> CreateAsync(CreateUserCommand request, CancellationToken ct)
    {
        try
        {
            var result = await sender.Send(request, ct);

            return Created(string.Empty, result);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

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
            var result = await sender.Send(updateRequest, ct);

            return Ok(result);
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid userId, CancellationToken ct)
    {
        try
        {
            await sender.Send(new DeleteUserCommand(userId), ct);
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
    public async Task<IActionResult> ChangePasswordAsync(Guid userId, UpdateUserPasswordCommand request, CancellationToken ct)
    {
        try
        {
            var updateRequest = request with { UserId = userId };
            var result = await sender.Send(updateRequest, ct);

            return Ok(result);
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }
}
