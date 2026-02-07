using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Models.Auth;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class UserRoleServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<IUserRoleRepository> repoMock = null!;
    private UserRoleService sut = null!;

    [SetUp]
    public void Setup()
    {
        this.repoMock = new Mock<IUserRoleRepository>();
        this.sut = new UserRoleService(this.repoMock.Object);
    }

    [Test]
    public async Task GetRolesByUserAsync_ReturnsRolesFromRepository()
    {
        var userId = Guid.NewGuid();
        var roles = new[] { "Admin", "User" };

        this.repoMock.Setup(r => r.GetRolesByUserAsync(userId, this.cancellationToken)).ReturnsAsync(roles);

        var result = await this.sut.GetRolesByUserAsync(userId, this.cancellationToken);

        Assert.That(result, Is.EquivalentTo(roles));
        this.repoMock.Verify(r => r.GetRolesByUserAsync(userId, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetRolesByUserAsync_WhenEmpty_ReturnsEmpty()
    {
        var userId = Guid.NewGuid();
        this.repoMock.Setup(r => r.GetRolesByUserAsync(userId, this.cancellationToken)).ReturnsAsync([]);

        var result = await this.sut.GetRolesByUserAsync(userId, this.cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetUserCompaniesAsync_ConvertsUserRolesToCompanyResponses()
    {
        var userId = Guid.NewGuid();
        var company = new CompanyModel { Id = Guid.NewGuid(), Name = "Acme", Cnpj = "12345678901234" };
        var role = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        var userRole = new UserRoleModel { Id = Guid.NewGuid(), Company = company, Role = role };

        this.repoMock.Setup(r => r.GetUserCompaniesAsync(userId, this.cancellationToken)).ReturnsAsync([userRole]);

        var result = await this.sut.GetUserCompaniesAsync(userId, this.cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(company.Id));
            Assert.That(result[0].Name, Is.EqualTo(company.Name));
            Assert.That(result[0].Cnpj, Is.EqualTo(company.Cnpj));
        }
    }

    [Test]
    public async Task HasRoleAsync_DelegatesToRepository()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "Admin";

        this.repoMock.Setup(r => r.HasRoleAsync(userId, companyId, roleName, this.cancellationToken)).ReturnsAsync(true);

        var result = await this.sut.HasRoleAsync(userId, companyId, roleName, this.cancellationToken);

        Assert.That(result, Is.True);
        this.repoMock.Verify(r => r.HasRoleAsync(userId, companyId, roleName, this.cancellationToken), Times.Once);
    }
}