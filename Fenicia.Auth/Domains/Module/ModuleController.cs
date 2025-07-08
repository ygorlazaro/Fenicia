namespace Fenicia.Auth.Domains.Module;

using System.Net.Mime;

using Common;

using Data;

using Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route(template: "[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController : ControllerBase
{
    private readonly ILogger<ModuleController> _logger;
    private readonly IModuleService _moduleService;

    public ModuleController(ILogger<ModuleController> logger, IModuleService moduleService)
    {
        _logger = logger;
        _moduleService = moduleService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<ModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<ModuleResponse>>>> GetAllModulesAsync([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(message: "Retrieving modules with pagination: Page {Page}, Items per page {PerPage}", query.Page, query.PerPage);

            var modules = await _moduleService.GetAllOrderedAsync(cancellationToken, query.Page, query.PerPage);
            var total = await _moduleService.CountAsync(cancellationToken);

            if (modules.Data is null)
            {
                _logger.LogWarning(message: "No modules data found. Status: {Status}, Message: {Message}", modules.Status, modules.Message);
                return StatusCode((int)modules.Status, modules.Message);
            }

            var pagination = new Pagination<List<ModuleResponse>>(modules.Data, total.Data, query.Page, query.PerPage);

            _logger.LogInformation(message: "Successfully retrieved {Count} modules", modules.Data.Count);
            return Ok(pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "Error occurred while retrieving modules");
            return StatusCode(StatusCodes.Status500InternalServerError, value: "An error occurred while processing your request");
        }
    }
}
