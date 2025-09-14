using Fenicia.Auth.Domains.Module.Logic;
using Fenicia.Common;
using Fenicia.Common.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Migrations;

[Authorize]
[Route("[controller]")]
[ApiController]
public class MigrationController : ControllerBase
{
    private readonly IModuleService _moduleService;

    public MigrationController(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    [HttpPost]
    public async Task<IActionResult> PostNewMigrationAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var companyId = ClaimReader.CompanyId(User);
        var modulesResponse = await _moduleService.GetUserModulesAsync(userId, companyId, cancellationToken);
        var modules = modulesResponse.Data ?? [];

        if (modules.Count == 0)
        {
            return NotFound(TextConstants.ThereWasAnErrorSearchingModules);
        }

        var migrationService = new MigrationService();

        await migrationService.RunMigrationsAsync(modules, companyId, cancellationToken);

        return Ok();
    }
}
