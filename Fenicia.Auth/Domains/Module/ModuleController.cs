using System.Net.Mime;

using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Auth.Domains.Module.Logic;
using Fenicia.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Module;

/// <summary>
/// Controller responsible for managing module-related operations
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController : ControllerBase
{
    private readonly ILogger<ModuleController> _logger;
    private readonly IModuleService _moduleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleController"/> class
    /// </summary>
    /// <param name="logger">The logger instance for recording module-related activities</param>
    /// <param name="moduleService">The service handling module business logic</param>
    public ModuleController(ILogger<ModuleController> logger, IModuleService moduleService)
    {
        _logger = logger;
        _moduleService = moduleService;
    }

    /// <summary>
    /// Retrieves all available modules in the system with pagination support
    /// </summary>
    /// <param name="query">The pagination parameters including page number and items per page</param>
    /// <param name="cancellationToken">Token for cancelling the operation if needed</param>
    /// <returns>A paginated list of module responses</returns>
    /// <response code="200">Returns the paginated list of all modules</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="500">If there was an internal server error during the operation</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<ModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<ModuleResponse>>>> GetAllModulesAsync(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving modules with pagination: Page {Page}, Items per page {PerPage}",
                query.Page, query.PerPage);

            var modules = await _moduleService.GetAllOrderedAsync(cancellationToken, query.Page, query.PerPage);
            var total = await _moduleService.CountAsync(cancellationToken);

            if (modules.Data is null)
            {
                _logger.LogWarning("No modules data found. Status: {Status}, Message: {Message}",
                    modules.Status, modules.Message);
                return StatusCode((int)modules.Status, modules.Message);
            }

            var pagination = new Pagination<List<ModuleResponse>>(
                modules.Data,
                total.Data,
                query.Page,
                query.PerPage
            );

            _logger.LogInformation("Successfully retrieved {Count} modules", modules.Data.Count);
            return Ok(pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving modules");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }
}
