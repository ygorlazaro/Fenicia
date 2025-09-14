using Fenicia.Auth.Domains.Module;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Migrations.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Migrations;

[Authorize(Roles = "God")]
[Route("[controller]")]
[ApiController]
public class MigrationController : ControllerBase
{
    private readonly IModuleService _moduleService;

    private readonly IMigrationService _migrationService;

    public MigrationController(IModuleService moduleService, IMigrationService migrationService)
    {
        _moduleService = moduleService;
        _migrationService = migrationService;
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

        await _migrationService.RunMigrationsAsync(modules, companyId, cancellationToken);

        return Ok();
    }
}
