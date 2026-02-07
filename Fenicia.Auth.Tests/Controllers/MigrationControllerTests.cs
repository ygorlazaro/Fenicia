using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Migrations;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Migrations.Services;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class MigrationControllerTests
{
    private Mock<ICompanyService> companyServiceMock;
    private Mock<ISubscriptionCreditService> creditServiceMock;
    private Mock<IMigrationService> migrationServiceMock;
    private MigrationController sut;

    [SetUp]
    public void Setup()
    {
        this.migrationServiceMock = new Mock<IMigrationService>();
        this.creditServiceMock = new Mock<ISubscriptionCreditService>();
        this.companyServiceMock = new Mock<ICompanyService>();

        this.sut = new MigrationController(this.migrationServiceMock.Object, this.creditServiceMock.Object,
            this.companyServiceMock.Object);
    }

    [Test]
    public async Task PostNewMigrationAsync_CallsRunMigrationsAndReturnsOk()
    {
        const string cnpj = "12345678901234";
        var company = new CompanyResponse { Id = Guid.NewGuid() };
        var credits = new List<ModuleType>();

        this.companyServiceMock.Setup(x => x.GetByCnpjAsync(cnpj, CancellationToken.None)).ReturnsAsync(company);
        this.creditServiceMock.Setup(x => x.GetActiveModulesTypesAsync(company.Id, CancellationToken.None))
            .ReturnsAsync(credits);

        var result = await this.sut.PostNewMigrationAsync(cnpj, CancellationToken.None);

        Assert.That(result, Is.TypeOf<OkResult>());
        this.migrationServiceMock.Verify(x => x.RunMigrationsAsync(company.Id, credits, CancellationToken.None),
            Times.Once);
    }
}