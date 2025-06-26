namespace Fenicia.Module.Basic.Domains.State;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route(template: "[controller]")]
public class StateController(IStateService stateProvider) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var states = await stateProvider.GetAllAsync();

        return Ok(states);
    }
}
