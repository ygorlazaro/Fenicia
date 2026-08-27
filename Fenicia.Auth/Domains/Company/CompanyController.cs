using System.Net.Mime;

using Fenicia.Auth.Domains.Company.Commands;

using Fenicia.Auth.Domains.Company.Queries;
using Fenicia.Auth.Domains.Company.Responses;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Company;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CompanyController(ISender sender) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetCompaniesByUserResponse>> GetByLoggedUser([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var getCompaniesByUserQuery = new GetCompaniesByUserQuery(userId, query.Page, query.PerPage);
            var result = await sender.Send(getCompaniesByUserQuery, ct);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchAsync([FromRoute] Guid id, [FromBody] UpdateCompanyCommand request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var company = request with { CompanyId = id };

            await sender.Send(company, ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ItemNotExistsException ex)
        {
            return NotFound(ex.Message);
        }
        catch (PermissionDeniedException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
