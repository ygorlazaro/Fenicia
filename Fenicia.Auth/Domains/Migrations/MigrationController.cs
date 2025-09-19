namespace Fenicia.Auth.Domains.Migrations;

using Company;
using SubscriptionCredit;
using Fenicia.Common.Migrations.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "God")]
[Route("[controller]")]
[ApiController]
public class MigrationController : ControllerBase
{
    private readonly IMigrationService migrationService;
    private readonly ISubscriptionCreditService subscriptionCreditService;
    private readonly ICompanyService companyService;

    public MigrationController(IMigrationService migrationService, ISubscriptionCreditService subscriptionCreditService, ICompanyService companyService)
    {
        this.migrationService = migrationService;
        this.subscriptionCreditService = subscriptionCreditService;
        this.companyService = companyService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostNewMigrationAsync([FromBody] string cnpj, CancellationToken cancellationToken)
    {
        var company = await this.companyService.GetByCnpjAsync(cnpj, cancellationToken);
        var credits = await this.subscriptionCreditService.GetActiveModulesTypesAsync(company.Data!.Id, cancellationToken);

        await this.migrationService.RunMigrationsAsync(company.Data.Id, credits.Data!, cancellationToken);

        return this.Ok();
    }
}
