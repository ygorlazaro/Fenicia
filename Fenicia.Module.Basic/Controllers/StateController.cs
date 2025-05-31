using Fenicia.Common.Api;
using Fenicia.Module.Basic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StateController(IStateService stateProvider):ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var states = await stateProvider.GetAllAsync();
        
        return Ok(states);
    }
}