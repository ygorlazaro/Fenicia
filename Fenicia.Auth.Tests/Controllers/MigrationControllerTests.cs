using Fenicia.Auth.Domains.Migrations;
using Fenicia.Common.Migrations.Services;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class MigrationControllerTests
{
    private Mock<IMigrationService> migrationServiceMock;
    private Mock<ISubscriptionCreditService> creditServiceMock;
    private Mock<ICompanyService> companyServiceMock;
    private MigrationController sut;

    [SetUp]
    public void Setup()
    {
        migrationServiceMock = new Mock<IMigrationService>();
        creditServiceMock = new Mock<ISubscriptionCreditService>();
        companyServiceMock = new Mock<ICompanyService>();

        sut = new MigrationController(migrationServiceMock.Object, creditServiceMock.Object, companyServiceMock.Object);
    }

    [Test]
    public async Task PostNewMigrationAsync_CallsRunMigrationsAndReturnsOk()
    {
        var cnpj = "12345678901234";
        var company = new CompanyResponse { Id = Guid.NewGuid() };
        var credits = new List<ModuleType>();

        companyServiceMock.Setup(x => x.GetByCnpjAsync(cnpj, CancellationToken.None)).ReturnsAsync(company);
        creditServiceMock.Setup(x => x.GetActiveModulesTypesAsync(company.Id, CancellationToken.None)).ReturnsAsync(credits);

        var result = await sut.PostNewMigrationAsync(cnpj, CancellationToken.None);

        Assert.That(result, Is.TypeOf<OkResult>());
        migrationServiceMock.Verify(x => x.RunMigrationsAsync(company.Id, credits, CancellationToken.None), Times.Once);
    }
}
