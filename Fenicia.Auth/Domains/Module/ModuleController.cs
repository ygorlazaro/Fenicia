using System.Net.Mime;

using Fenicia.Auth.Domains.Module.Handlers;
using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Common;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Module;

/// <summary>
///     Controller responsible for handling module-related HTTP endpoints.
///     Provides an endpoint to retrieve available modules with pagination.
/// </summary>
/// <remarks>
///     The GetAllModulesAsync endpoint is publicly accessible ([AllowAnonymous]) to allow
///     unauthenticated users to view available modules. This is useful for marketing pages
///     or module selection during registration. The endpoint requires pagination parameters.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController(GetModulesHandler getModulesHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves a paginated list of available modules.
    /// </summary>
    /// <param name="query">Pagination query parameters (page number and items per page).</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing available modules.</returns>
    /// <remarks>
    ///     This endpoint is publicly accessible. It excludes modules with type Auth from the results.
    ///     The UserId in WideEventContext is set to "Guest" since no authentication is required.
    /// </remarks>
    /// <response code="200">Returns the list of modules successfully.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<GetModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetModuleResponse>>> GetAllModulesAsync([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = "Guest";

        var modulesQuery = new GetModulesQuery(query.Page, query.PerPage);
        var modules = await getModulesHandler.Handle(modulesQuery, ct);

        return Ok(modules);
    }
}