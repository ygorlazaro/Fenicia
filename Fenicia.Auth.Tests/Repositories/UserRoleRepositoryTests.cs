namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;
using Domains.UserRole;

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
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        var mockLogger = new Mock<ILogger<UserRoleRepository>>().Object;
        this.context = new AuthContext(this.options);
        this.sut = new UserRoleRepository(this.context, mockLogger);

        this.SetupFakers();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public async Task GetRolesByUserAsyncWhenUserHasRolesReturnsRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roles = new[] { "Admin", "User", "Manager" };
        var userRoles = roles.Select(role => this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.Role, _ => this.roleGenerator.Clone().RuleFor(r => r.Name, role).Generate()).Generate()).ToList();

        await this.context.UserRoles.AddRangeAsync(userRoles, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetRolesByUserAsync(userId, this.cancellationToken);

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
        var result = await this.sut.GetRolesByUserAsync(userId, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ExistsInCompanyAsyncWhenUserExistsInCompanyReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var userRole = this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).Generate();

        await this.context.UserRoles.AddAsync(userRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.ExistsInCompanyAsync(userId, companyId, this.cancellationToken);

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
        var result = await this.sut.ExistsInCompanyAsync(userId, companyId, this.cancellationToken);

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
        var userRole = this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).RuleFor(ur => ur.Role, _ => this.roleGenerator.Clone().RuleFor(r => r.Name, roleName).Generate()).Generate();

        await this.context.UserRoles.AddAsync(userRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.HasRoleAsync(userId, companyId, roleName, this.cancellationToken);

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
        var userRole = this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).RuleFor(ur => ur.Role, _ => this.roleGenerator.Clone().RuleFor(r => r.Name, "DifferentRole").Generate()).Generate();

        await this.context.UserRoles.AddAsync(userRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.HasRoleAsync(userId, companyId, roleName, this.cancellationToken);

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
        var result = await this.sut.HasRoleAsync(userId, companyId, roleName, this.cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    private void SetupFakers()
    {
        this.roleGenerator = new Faker<RoleModel>().RuleFor(r => r.Id, _ => Guid.NewGuid()).RuleFor(r => r.Name, f => f.Name.JobTitle());

        this.userRoleGenerator = new Faker<UserRoleModel>().RuleFor(ur => ur.Id, _ => Guid.NewGuid()).RuleFor(ur => ur.UserId, _ => Guid.NewGuid()).RuleFor(ur => ur.Role, _ => this.roleGenerator.Generate()).RuleFor(ur => ur.CompanyId, _ => Guid.NewGuid());
    }
}
