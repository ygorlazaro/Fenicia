using System.Net.Mime;

using Fenicia.Auth.Domains.Module.GetModules;
using Fenicia.Common;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Module;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<GetModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetModuleResponse>>> GetAllModulesAsync(
        [FromQuery] PaginationQuery query,
        [FromServices] GetModulesHandler handler,
        WideEventContext wide,
        CancellationToken cancellationToken)
    {
        wide.UserId = "Guest";

        var modules = await handler.Handle(new GetModulesRequest(query.Page, query.PerPage), cancellationToken);

        return Ok(modules);
    }
}
