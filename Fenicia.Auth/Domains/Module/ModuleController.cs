using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.Api;
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
        wide.Operation = "GetAllModules";

        var modules = await moduleService.GetAllOrderedAsync(cancellationToken, query.Page, query.PerPage);
        var total = await moduleService.CountAsync(cancellationToken);

        if (modules.Data is null)
        {
            return StatusCode((int)modules.Status, modules.Message);
        }

        var pagination = new Pagination<List<ModuleResponse>>(modules.Data, total.Data, query.Page, query.PerPage);

        return Ok(pagination);
    }
}
