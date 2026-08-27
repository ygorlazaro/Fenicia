using System.Net.Mime;

using Fenicia.Auth.Domains.Module.Responses;
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
public class ModuleController(ModuleService service) : ControllerBase
{

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<GetModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetModuleResponse>>> GetAllModulesAsync([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = "Guest";

        var modules = await service.GetAllModulesAsync(query.Page, query.PerPage, ct);

        return Ok(modules);
    }
}
