using Fenicia.Auth.Services;
using Fenicia.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ModuleController(IModuleService moduleService): ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllModulesAsync()
    {
        var modules = await moduleService.GetAllOrderedAsync();
        
        return Ok(modules);
    }
}