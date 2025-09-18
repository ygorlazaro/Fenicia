namespace Fenicia.Module.Basic.Domains.State;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StateController : ControllerBase
{
    private readonly IStateService stateProvider;

    public StateController(IStateService stateProvider)
    {
        this.stateProvider = stateProvider;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var states = await this.stateProvider.GetAllAsync();

        return this.Ok(states);
    }
}
