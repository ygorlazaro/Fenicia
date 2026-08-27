using System.Net.Mime;

using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Common;
using Fenicia.Common.API;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Module;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController(ISender sender) : ControllerBase
{

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<GetModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetModuleResponse>>> GetAllModulesAsync([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = "Guest";

        var modulesQuery = new GetModulesQuery(query.Page, query.PerPage);
        var modules = await sender.Send(modulesQuery, ct);

        return Ok(modules);
    }
}
