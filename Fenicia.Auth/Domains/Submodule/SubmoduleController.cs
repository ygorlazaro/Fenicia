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
public class SubmoduleController : ControllerBase
{
    [HttpGet("{moduleId:Guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetByModuleResponse>>> GetByModuleIdAsync(
        [FromRoute] Guid moduleId,
        [FromServices] GetByModuleIdHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = "Guest";

        var submodules = await handler.Handle(moduleId, ct);

        return Ok(submodules);
    }
}
