using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Fenicia.Common;

namespace Fenicia.Auth.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CompanyController(ILogger<CompanyController> logger, ICompanyService companyService) : ControllerBase
{
    /// <summary>
    /// Retrieves companies associated with the logged-in user
    /// </summary>
    /// <response code="200">Returns the list of companies for the authenticated user</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Pagination<IEnumerable<CompanyResponse>>>> GetByLoggedUser([FromQuery] PaginationQuery query)
    {
        var userId = ClaimReader.UserId(User);
        var companies = await companyService.GetByUserIdAsync(userId, query.Page, query.PerPage);
        var total = await companyService.CountByUserIdAsync(userId);
        
        logger.LogInformation("Getting companies");
        
        var response = new Pagination<IEnumerable<CompanyResponse>>(companies, total, query.Page, query.PerPage);

        return Ok(companies);
    }

    /// <summary>
    /// Updates a specific company's information
    /// </summary>
    /// <param name="request">The company information to update</param>
    /// <param name="id">The ID of the company to update</param>
    /// <returns>The updated company information</returns>
    /// <response code="200">Returns the updated company information</response>
    /// <response code="400">If the request is invalid or update fails</response>
    /// <response code="401">If the user is not authenticated or not authorized to update this company</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CompanyResponse>> PatchAsync(
        [FromBody] CompanyRequest request,
        [FromRoute] Guid id)
    {
        var userId = ClaimReader.UserId(User);
        var companyId = ClaimReader.CompanyId(User);
        
        if (id != companyId)
        {
            return Unauthorized();
        }

        var response = await companyService.PatchAsync(id, userId, request);
        
        logger.LogInformation("Company updated - {id}", [id]);

        return response is null ? BadRequest() : Ok(response);
    }
}