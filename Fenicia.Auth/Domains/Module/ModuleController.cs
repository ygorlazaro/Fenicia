using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Module;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController(IModuleService moduleService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<ModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<ModuleResponse>>>> GetAllModulesAsync([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.UserId = "Guest";

        var modules = await moduleService.GetAllOrderedAsync(cancellationToken, query.Page, query.PerPage);
        var total = await moduleService.CountAsync(cancellationToken);
        var pagination = new Pagination<List<ModuleResponse>>(modules, total, query);

        return Ok(pagination);
    }
}
