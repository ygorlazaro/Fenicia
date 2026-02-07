using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Company;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CompanyController(ICompanyService companyService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(Pagination<IEnumerable<CompanyResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<IEnumerable<CompanyResponse>>>> GetByLoggedUser([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        var companies = await companyService.GetByUserIdAsync(userId, ct, query.Page, query.PerPage);
        var total = await companyService.CountByUserIdAsync(userId, ct);
        var response = new Pagination<IEnumerable<CompanyResponse>>(companies, total, query);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CompanyResponse>> PatchAsync([FromBody] CompanyUpdateRequest request, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        var response = await companyService.PatchAsync(id, userId, request, ct);

        return Ok(response);
    }
}