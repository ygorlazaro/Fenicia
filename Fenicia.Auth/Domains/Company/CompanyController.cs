namespace Fenicia.Auth.Domains.Company;

using System.Net.Mime;

using Common;
using Common.Api;

using Data;

using Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route(template: "[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(ILogger<CompanyController> logger, ICompanyService companyService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(Pagination<IEnumerable<CompanyResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<IEnumerable<CompanyResponse>>>> GetByLoggedUser([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(message: "Starting to retrieve companies for logged user");

            var userId = ClaimReader.UserId(User);
            _logger.LogDebug(message: "Retrieved user ID: {UserId}", userId);

            var companies = await _companyService.GetByUserIdAsync(userId, cancellationToken, query.Page, query.PerPage);
            var total = await _companyService.CountByUserIdAsync(userId, cancellationToken);

            if (companies.Data is null)
            {
                _logger.LogWarning(message: "No companies found for user {UserId}", userId);
                return StatusCode((int)companies.Status, companies.Message);
            }

            var response = new Pagination<IEnumerable<CompanyResponse>>(companies.Data, total.Data, query.Page, query.PerPage);

            _logger.LogInformation(message: "Successfully retrieved {Count} companies for user {UserId}", companies.Data.Count(), userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "Error retrieving companies for logged user");
            throw;
        }
    }

    [HttpPatch(template: "{id:guid}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CompanyResponse>> PatchAsync([FromBody] CompanyUpdateRequest request, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(message: "Starting company update process for company ID: {CompanyId}", id);

            var userId = ClaimReader.UserId(User);
            var companyId = ClaimReader.CompanyId(User);

            if (id != companyId)
            {
                _logger.LogWarning(message: "Unauthorized attempt to update company. Requested ID: {RequestedId}, User's Company ID: {UserCompanyId}", id, companyId);
                return Unauthorized();
            }

            var response = await _companyService.PatchAsync(id, userId, request, cancellationToken);

            if (response.Data is null)
            {
                _logger.LogWarning(message: "Failed to update company {CompanyId}. Status: {Status}, Message: {Message}", id, response.Status, response.Message);
                return StatusCode((int)response.Status, response.Message);
            }

            _logger.LogInformation(message: "Successfully updated company with ID: {CompanyId}", id);
            return Ok(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "Error updating company with ID: {CompanyId}", id);
            throw;
        }
    }
}
