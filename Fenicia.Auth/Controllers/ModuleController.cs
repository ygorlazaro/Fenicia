using System.Net.Mime;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

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
        [FromQuery] PaginationQuery query
    )
    {
        var modules = await moduleService.GetAllOrderedAsync(query.Page, query.PerPage);
        var total = await moduleService.CountAsync();

        if (modules.Data is null)
        {
            return StatusCode((int)modules.StatusCode, modules.Message);
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
