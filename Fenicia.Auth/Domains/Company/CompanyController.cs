namespace Fenicia.Auth.Domains.Company;

using System.Net.Mime;

using Common;
using Common.API;

using Common.Database.Requests;
using Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService companyService;
    private readonly ILogger<CompanyController> logger;

    public CompanyController(ILogger<CompanyController> logger, ICompanyService companyService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(Pagination<IEnumerable<CompanyResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<IEnumerable<CompanyResponse>>>> GetByLoggedUser([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Starting to retrieve companies for logged user");

            var userId = ClaimReader.UserId(this.User);
            this.logger.LogDebug("Retrieved user ID: {userID}", userId);

            var companies = await this.companyService.GetByUserIdAsync(userId, cancellationToken, query.Page, query.PerPage);
            var total = await this.companyService.CountByUserIdAsync(userId, cancellationToken);

            if (companies.Data is null)
            {
                this.logger.LogWarning("No companies found for user {UserID}", userId);
                return this.StatusCode((int)companies.Status, companies.Message);
            }

            var response = new Pagination<IEnumerable<CompanyResponse>>(companies.Data, total.Data, query.Page, query.PerPage);

            this.logger.LogInformation("Successfully retrieved {Count} companies for user {UserID}", companies.Data.Count, userId);
            return this.Ok(response);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving companies for logged user");
            throw;
        }
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CompanyResponse>> PatchAsync([FromBody] CompanyUpdateRequest request, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Starting company update process for company ID: {CompanyID}", id);

            var userId = ClaimReader.UserId(this.User);
            var companyId = ClaimReader.CompanyId(this.User);

            if (id != companyId)
            {
                this.logger.LogWarning("Unauthorized attempt to update company. Requested ID: {RequestedID}, User's Company ID: {UserCompanyID}", id, companyId);
                return this.Unauthorized();
            }

            var response = await this.companyService.PatchAsync(id, userId, request, cancellationToken);

            if (response.Data is null)
            {
                this.logger.LogWarning("Failed to update company {CompanyID}. Status: {Status}, Message: {Message}", id, response.Status, response.Message);
                return this.StatusCode((int)response.Status, response.Message);
            }

            this.logger.LogInformation("Successfully updated company with ID: {CompanyID}", id);
            return this.Ok(response.Data);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error updating company with ID: {CompanyID}", id);
            throw;
        }
    }
}
