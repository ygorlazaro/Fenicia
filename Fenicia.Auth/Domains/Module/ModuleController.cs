using System.Net.Mime;

using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Auth.Domains.Module.Logic;
using Fenicia.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Module;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController(ILogger<ModuleController> logger, IModuleService moduleService)
    : ControllerBase
{
    /// <summary>
    /// Retrieves all available modules in the system
    /// </summary>
    /// <response code="200">Returns the list of all modules</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ModuleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Pagination<List<ModuleResponse>>>> GetAllModulesAsync(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken
    )
    {
        var modules = await moduleService.GetAllOrderedAsync(cancellationToken, query.Page, query.PerPage);
        var total = await moduleService.CountAsync(cancellationToken);

        if (modules.Data is null)
        {
            return StatusCode((int)modules.Status, modules.Message);
        }

        var pagination = new Pagination<List<ModuleResponse>>(
            modules.Data,
            total.Data,
            query.Page,
            query.PerPage
        );

        logger.LogInformation("Getting modules");

        return Ok(pagination);
    }
}
