using System.Net.Mime;

using Fenicia.Auth.Domains.Submodule;
using Fenicia.Common;
using Fenicia.Common.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<Pagination<List<SubmoduleService>>>> GetByModuleIdAsync([FromRoute] Guid moduleId, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.UserId = "Guest";

        var submodules = await submoduleService.GetByModuleIdAsync(moduleId, cancellationToken);

        return Ok(submodules);
    }
}
