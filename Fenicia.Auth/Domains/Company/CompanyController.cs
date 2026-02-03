using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.Api;
using Fenicia.Common.API;

using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

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
    public async Task<ActionResult<Pagination<IEnumerable<CompanyResponse>>>> GetByLoggedUser([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.Operation = "Get Companies for Logged User";
        wide.UserId = ClaimReader.UserId(User).ToString();

        var userId = ClaimReader.UserId(User);

        var companies = await companyService.GetByUserIdAsync(userId, cancellationToken, query.Page, query.PerPage);
        var total = await companyService.CountByUserIdAsync(userId, cancellationToken);

        if (companies.Data is null)
        {
            return StatusCode((int)companies.Status, companies.Message);
        }

        var response = new Pagination<IEnumerable<CompanyResponse>>(companies.Data, total.Data, query.Page, query.PerPage);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CompanyResponse>> PatchAsync([FromBody] CompanyUpdateRequest request, [FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.Operation = "Update Company";
        wide.UserId = ClaimReader.UserId(User).ToString();

        var userId = ClaimReader.UserId(User);
        var response = await companyService.PatchAsync(id, userId, request, cancellationToken);

        if (response.Data is null)
        {
            return StatusCode((int)response.Status, response.Message);
        }

        return Ok(response.Data);
    }
}
