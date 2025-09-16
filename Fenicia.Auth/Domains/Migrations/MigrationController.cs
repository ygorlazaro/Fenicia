using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Common.Migrations.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Migrations;

[Authorize(Roles = "God")]
[Route("[controller]")]
[ApiController]
public class MigrationController : ControllerBase
{
    private readonly IMigrationService _migrationService;
    private readonly ISubscriptionCreditService _subscriptionCreditService;
    private readonly ICompanyService _companyService;

    public MigrationController(IMigrationService migrationService, ISubscriptionCreditService subscriptionCreditService, ICompanyService companyService)
    {
        _migrationService = migrationService;
        _subscriptionCreditService = subscriptionCreditService;
        _companyService = companyService;
    }

    [HttpPost]
    public async Task<IActionResult> PostNewMigrationAsync([FromBody] string cnpj, CancellationToken cancellationToken)
    {
        var company = await _companyService.GetByCnpjAsync(cnpj, cancellationToken);
        var credits = await _subscriptionCreditService.GetActiveModulesTypesAsync(company.Data!.Id, cancellationToken);

        await _migrationService.RunMigrationsAsync(company.Data.Id, credits.Data!, cancellationToken);

        return Ok();
    }
}
