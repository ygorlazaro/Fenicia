using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Submodule;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class SubmoduleController(ISubmoduleService submoduleService) : ControllerBase
{
    [HttpGet("{moduleId:Guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<SubmoduleService>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<SubmoduleService>>>> GetByModuleIdAsync([FromRoute] Guid moduleId, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = "Guest";

        var submodules = await submoduleService.GetByModuleIdAsync(moduleId, ct);

        return Ok(submodules);
    }
}