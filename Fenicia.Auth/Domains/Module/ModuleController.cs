using System.Net.Mime;
using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Common;
using Fenicia.Common.DTOs.Auth.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Module;

/// <summary>
/// Controller for managing module-related operations.
/// </summary>
/// <param name="service"></param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController(IModuleService service) : ControllerBase
{
    /// <summary>
    ///   Retrieves a paginated list of all active modules.
    /// </summary>
    /// <param name="query">The pagination query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paginated list of modules.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<ModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ModuleResponse>>> GetAllModulesAsync(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var modules = await service.GetAllModulesAsync(query, cancellationToken);

        return Ok(modules);
    }
}
