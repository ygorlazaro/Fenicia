using System.Net.Mime;

using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.State.GetAll;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.State;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class StateController(GetAllStateHandler getAllStateHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StateModel>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StateModel>> GetAllAsync(CancellationToken ct)
    {
        var states = await getAllStateHandler.Handle(new GetAllStateQuery(), ct);

        return Ok(states);
    }
}
