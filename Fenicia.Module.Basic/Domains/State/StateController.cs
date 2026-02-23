using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.State.GetAll;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.State;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StateController(GetAllStateHandler getAllStateHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<StateModel>> GetAllAsync(CancellationToken ct)
    {
        var states = await getAllStateHandler.Handle(new GetAllStateQuery(), ct);

        return Ok(states);
    }
}
