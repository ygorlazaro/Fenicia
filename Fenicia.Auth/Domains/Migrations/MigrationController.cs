using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Common.Migrations.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Migrations;

[Authorize(Roles = "God")]
[Route("[controller]")]
[ApiController]
public class MigrationController(IMigrationService migrationService, ISubscriptionCreditService subscriptionCreditService, ICompanyService companyService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostNewMigrationAsync([FromBody] string cnpj, CancellationToken ct)
    {
        var company = await companyService.GetByCnpjAsync(cnpj, ct);
        var credits = await subscriptionCreditService.GetActiveModulesTypesAsync(company.Id, ct);

        await migrationService.RunMigrationsAsync(company.Id, credits, ct);

        return Ok();
    }
}
