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
    public async Task<IActionResult> PostNewMigrationAsync([FromBody] string cnpj, CancellationToken cancellationToken)
    {
        var company = await companyService.GetByCnpjAsync(cnpj, cancellationToken);
        var credits = await subscriptionCreditService.GetActiveModulesTypesAsync(company.Data!.Id, cancellationToken);

        await migrationService.RunMigrationsAsync(company.Data.Id, credits.Data!, cancellationToken);

        return Ok();
    }
}
