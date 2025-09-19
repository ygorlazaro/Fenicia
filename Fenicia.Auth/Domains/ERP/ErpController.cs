namespace Fenicia.Auth.Domains.ERP;

using System.Net.Mime;

using Module;
using State;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "God")]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ErpController : ControllerBase
{
    private readonly IModuleService moduleService;
    private readonly IStateService stateService;

    public ErpController(IModuleService moduleService, IStateService stateService)
    {
        this.moduleService = moduleService;
        this.stateService = stateService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoadInfoAsync(CancellationToken cancellationToken)
    {
        var modules = await moduleService.LoadModulesAtDatabaseAsync(cancellationToken);
        var states = await stateService.LoadStatesAtDatabaseAsync(cancellationToken);

        var response = new
        {
            Modules = modules,
            States = states
        };

        return Ok(response);
    }
}
