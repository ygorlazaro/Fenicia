using System.Net.Mime;

using Fenicia.Auth.Domains.Company.GetCompaniesByUser;
using Fenicia.Auth.Domains.Company.UpdateCompany;
using Fenicia.Common;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Company;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CompanyController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetCompaniesByUserResponse>> GetByLoggedUser(
        [FromQuery] PaginationQuery query,
        [FromServices] GetCompaniesByUserHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        var result = await handler.Handle(new GetCompaniesByUserQuery(userId, query.Page, query.PerPage), ct);

        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateCompanyCommand request,
        [FromServices] UpdateCompanyHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        await handler.Handle(
            request,
            ct
        );

        return NoContent();
    }
}
