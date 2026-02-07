using Bogus;

using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

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
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        this.context = new AuthContext(this.options);
        this.sut = new UserRoleRepository(this.context);

        SetupFakers();
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
        var userId = Guid.NewGuid();
        var roles = new[] { "Admin", "User", "Manager" };
        var userRoles = roles.Select(role => this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.Role, _ => this.roleGenerator.Clone().RuleFor(r => r.Name, role).Generate()).Generate()).ToList();

        await this.context.UserRoles.AddRangeAsync(userRoles, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetRolesByUserAsync(userId, this.cancellationToken);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Has.Length.EqualTo(roles.Length));
        Assert.That(result, Is.EquivalentTo(roles));
    }

    [Test]
    public async Task GetRolesByUserAsyncWhenUserHasNoRolesReturnsEmptyArray()
    {
        var userId = Guid.NewGuid();

        var result = await this.sut.GetRolesByUserAsync(userId, this.cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ExistsInCompanyAsyncWhenUserExistsInCompanyReturnsTrue()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var userRole = this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).Generate();

        await this.context.UserRoles.AddAsync(userRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.ExistsInCompanyAsync(userId, companyId, this.cancellationToken);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsInCompanyAsyncWhenUserDoesNotExistInCompanyReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var result = await this.sut.ExistsInCompanyAsync(userId, companyId, this.cancellationToken);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasRoleAsyncWhenUserHasRoleInCompanyReturnsTrue()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "Admin";
        var userRole = this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).RuleFor(ur => ur.Role, _ => this.roleGenerator.Clone().RuleFor(r => r.Name, roleName).Generate()).Generate();

        await this.context.UserRoles.AddAsync(userRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.HasRoleAsync(userId, companyId, roleName, this.cancellationToken);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasRoleAsyncWhenUserDoesNotHaveRoleInCompanyReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "Admin";
        var userRole = this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).RuleFor(ur => ur.Role, _ => this.roleGenerator.Clone().RuleFor(r => r.Name, "DifferentRole").Generate()).Generate();

        await this.context.UserRoles.AddAsync(userRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.HasRoleAsync(userId, companyId, roleName, this.cancellationToken);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasRoleAsyncWhenUserDoesNotExistInCompanyReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleName = "Admin";

        var result = await this.sut.HasRoleAsync(userId, companyId, roleName, this.cancellationToken);

        Assert.That(result, Is.False);
    }

    [Test]
    public void AddShouldAddUserRoleToContext()
    {
        var userRole = this.userRoleGenerator.Generate();

        this.sut.Add(userRole);

        Assert.That(this.context.UserRoles.Local, Does.Contain(userRole));
    }

    [Test]
    public async Task GetUserCompaniesAsyncReturnsCompanyInfo()
    {
        var userId = Guid.NewGuid();
        var company = new CompanyModel { Id = Guid.NewGuid(), Name = "Acme Co", Cnpj = "12345678901234" };
        var userRole = this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, company.Id).Generate();

        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.UserRoles.AddAsync(userRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetUserCompaniesAsync(userId, this.cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(company.Id));
            Assert.That(result[0].Company.Name, Is.EqualTo(company.Name));
            Assert.That(result[0].Company.Cnpj, Is.EqualTo(company.Cnpj));
        }
    }

    private void SetupFakers()
    {
        this.roleGenerator = new Faker<RoleModel>().RuleFor(r => r.Id, _ => Guid.NewGuid()).RuleFor(r => r.Name, f => f.Name.JobTitle());

        this.userRoleGenerator = new Faker<UserRoleModel>().RuleFor(ur => ur.Id, _ => Guid.NewGuid()).RuleFor(ur => ur.UserId, _ => Guid.NewGuid()).RuleFor(ur => ur.Role, _ => this.roleGenerator.Generate()).RuleFor(ur => ur.CompanyId, _ => Guid.NewGuid());
    }
}