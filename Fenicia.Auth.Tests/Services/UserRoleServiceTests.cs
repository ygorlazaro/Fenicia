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
        repoMock = new Mock<IUserRoleRepository>();
        sut = new UserRoleService(repoMock.Object);
    }

    [Test]
    public async Task GetRolesByUserAsync_ReturnsRolesFromRepository()
    {
        var userId = Guid.NewGuid();
        var roles = new[] { "Admin", "User" };

        repoMock.Setup(r => r.GetRolesByUserAsync(userId, cancellationToken)).ReturnsAsync(roles);

        var result = await sut.GetRolesByUserAsync(userId, cancellationToken);

        Assert.That(result, Is.EquivalentTo(roles));
        repoMock.Verify(r => r.GetRolesByUserAsync(userId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetRolesByUserAsync_WhenEmpty_ReturnsEmpty()
    {
        var userId = Guid.NewGuid();
        repoMock.Setup(r => r.GetRolesByUserAsync(userId, cancellationToken)).ReturnsAsync([]);

        var result = await sut.GetRolesByUserAsync(userId, cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetUserCompaniesAsync_ConvertsUserRolesToCompanyResponses()
    {
        var userId = Guid.NewGuid();
        var company = new CompanyModel { Id = Guid.NewGuid(), Name = "Acme", Cnpj = "12345678901234" };
        var role = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        var userRole = new UserRoleModel { Id = Guid.NewGuid(), Company = company, Role = role };

        repoMock.Setup(r => r.GetUserCompaniesAsync(userId, cancellationToken)).ReturnsAsync([userRole]);

        var result = await sut.GetUserCompaniesAsync(userId, cancellationToken);

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

        repoMock.Setup(r => r.HasRoleAsync(userId, companyId, roleName, cancellationToken)).ReturnsAsync(true);

        var result = await sut.HasRoleAsync(userId, companyId, roleName, cancellationToken);

        Assert.That(result, Is.True);
        repoMock.Verify(r => r.HasRoleAsync(userId, companyId, roleName, cancellationToken), Times.Once);
    }
}
