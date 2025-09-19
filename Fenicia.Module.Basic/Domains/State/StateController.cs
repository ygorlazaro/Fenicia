namespace Fenicia.Module.Basic.Domains.State;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StateController : ControllerBase
{
    private readonly IStateService _stateProvider;

    public StateController(IStateService stateProvider)
    {
        this._stateProvider = stateProvider;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var states = await _stateProvider.GetAllAsync();

        return Ok(states);
    }
}
