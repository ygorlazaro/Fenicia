using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.State;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StateController(IStateService stateProvider) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var states = await stateProvider.GetAllAsync();

        return this.Ok(states);
    }
}
