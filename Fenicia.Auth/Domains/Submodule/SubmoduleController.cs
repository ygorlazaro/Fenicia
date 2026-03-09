using System.Net.Mime;

using Fenicia.Auth.Domains.Submodule.GetByModuleId;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Submodule;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class SubmoduleController(
    GetByModuleIdHandler getByModuleIdHandler) : ControllerBase
{
    [HttpGet("{moduleId:Guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetByModuleResponse>>> GetByModuleIdAsync(
        [FromRoute] Guid moduleId,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = "Guest";

        var submodules = await getByModuleIdHandler.Handle(moduleId, ct);

        return Ok(submodules);
    }
}
