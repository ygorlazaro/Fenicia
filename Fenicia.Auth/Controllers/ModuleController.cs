using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Fenicia.Auth.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController(ILogger<ModuleController> logger, IModuleService moduleService) : ControllerBase
{
    /// <summary>
    /// Retrieves all available modules in the system
    /// </summary>
    /// <response code="200">Returns the list of all modules</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ModuleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ModuleResponse>>> GetAllModulesAsync()
    {
        var modules = await moduleService.GetAllOrderedAsync();
        
        logger.LogInformation("Getting modules");
        
        return Ok(modules);
    }
}