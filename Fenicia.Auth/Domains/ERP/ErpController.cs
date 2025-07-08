namespace Fenicia.Auth.Domains.ERP;

using System.Net.Mime;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Module.Logic;

using State.Logic;

[Authorize(Roles = "God")]
[ApiController]
[Route(template: "[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ErpController(IModuleService moduleService, IStateService stateService) : ControllerBase
{
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
