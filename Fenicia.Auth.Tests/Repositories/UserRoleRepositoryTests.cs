using Bogus;

using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class UserRoleRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private AuthContext context;
    private DbContextOptions<AuthContext> options;
    private Faker<RoleModel> roleGenerator;
    private UserRoleRepository sut;
    private Faker<UserRoleModel> userRoleGenerator;

    [SetUp]
    public void Setup()
    {
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        context = new AuthContext(options);
        sut = new UserRoleRepository(context);

        SetupFakers();
    }

    [TearDown]
    public void TearDown()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task GetRolesByUserAsyncWhenUserHasRolesReturnsRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roles = new[] { "Admin", "User", "Manager" };
        var userRoles = roles.Select(role => userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.Role, _ => roleGenerator.Clone().RuleFor(r => r.Name, role).Generate()).Generate()).ToList();

        await context.UserRoles.AddRangeAsync(userRoles, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.GetRolesByUserAsync(userId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Has.Length.EqualTo(roles.Length));
        Assert.That(result, Is.EquivalentTo(roles));
    }

    [Test]
    public async Task GetRolesByUserAsyncWhenUserHasNoRolesReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await sut.GetRolesByUserAsync(userId, cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ExistsInCompanyAsyncWhenUserExistsInCompanyReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var userRole = userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).Generate();

        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsInCompanyAsyncWhenUserDoesNotExistInCompanyReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        // Act
        var result = await sut.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasRoleAsyncWhenUserHasRoleInCompanyReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "Admin";
        var userRole = userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).RuleFor(ur => ur.Role, _ => roleGenerator.Clone().RuleFor(r => r.Name, roleName).Generate()).Generate();

        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.HasRoleAsync(userId, companyId, roleName, cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasRoleAsyncWhenUserDoesNotHaveRoleInCompanyReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "Admin";
        var userRole = userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).RuleFor(ur => ur.Role, _ => roleGenerator.Clone().RuleFor(r => r.Name, "DifferentRole").Generate()).Generate();

        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.HasRoleAsync(userId, companyId, roleName, cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasRoleAsyncWhenUserDoesNotExistInCompanyReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleName = "Admin";

        // Act
        var result = await sut.HasRoleAsync(userId, companyId, roleName, cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void AddShouldAddUserRoleToContext()
    {
        // Arrange
        var userRole = userRoleGenerator.Generate();

        // Act
        sut.Add(userRole);

        // Assert
        Assert.That(context.UserRoles.Local, Does.Contain(userRole));
    }

    [Test]
    public async Task GetUserCompaniesAsyncReturnsCompanyInfo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var company = new CompanyModel { Id = Guid.NewGuid(), Name = "Acme Co", Cnpj = "12345678901234" };
        var userRole = userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, company.Id).Generate();

        await context.Companies.AddAsync(company, cancellationToken);
        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.GetUserCompaniesAsync(userId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(company.Id));
        Assert.That(result[0].Company.Name, Is.EqualTo(company.Name));
        Assert.That(result[0].Company.Cnpj, Is.EqualTo(company.Cnpj));
    }

    private void SetupFakers()
    {
        roleGenerator = new Faker<RoleModel>().RuleFor(r => r.Id, _ => Guid.NewGuid()).RuleFor(r => r.Name, f => f.Name.JobTitle());

        userRoleGenerator = new Faker<UserRoleModel>().RuleFor(ur => ur.Id, _ => Guid.NewGuid()).RuleFor(ur => ur.UserId, _ => Guid.NewGuid()).RuleFor(ur => ur.Role, _ => roleGenerator.Generate()).RuleFor(ur => ur.CompanyId, _ => Guid.NewGuid());
    }
}
