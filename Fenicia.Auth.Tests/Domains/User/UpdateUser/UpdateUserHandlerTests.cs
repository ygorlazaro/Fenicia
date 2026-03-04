using Bogus;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.UpdateUser;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User.UpdateUser;

[TestFixture]
public class UpdateUserHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.hashPasswordHandler = new HashPasswordHandler();
        this.handler = new UpdateUserHandler(this.context);
        this.faker = new Faker();

        // Create test user
        this.testUser = new AuthUserModel
        {
            Email = this.faker.Internet.Email(),
            Password = this.hashPasswordHandler.Handle(this.faker.Internet.Password()),
            Name = this.faker.Person.FullName
        };

        this.context.AuthUsers.Add(this.testUser);
        this.context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private UpdateUserHandler handler = null!;
    private DefaultContext context = null!;
    private HashPasswordHandler hashPasswordHandler = null!;
    private Faker faker = null!;
    private AuthUserModel testUser = null!;

    [Test]
    public async Task Handle_WhenValidRequest_UpdatesUserNameSuccessfully()
    {
        // Arrange
        var newName = this.faker.Person.FullName;
        var request = new UpdateUserQuery(this.testUser.Id, Name: newName);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(newName));
        Assert.That(result.Updated, Is.Not.Null);

        // Verify user was updated in database
        var updatedUser = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser.Name, Is.EqualTo(newName));
        Assert.That(updatedUser.Updated, Is.Not.Null);
    }

    [Test]
    public async Task Handle_WhenValidRequest_UpdatesUserEmailSuccessfully()
    {
        // Arrange
        var newEmail = this.faker.Internet.Email();
        var request = new UpdateUserQuery(this.testUser.Id, Email: newEmail);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(newEmail));

        // Verify user was updated in database
        var updatedUser = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser.Email, Is.EqualTo(newEmail));
    }

    [Test]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var request = new UpdateUserQuery(nonExistentUserId, Name: "Test");

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Is.EqualTo("User not found"));
    }

    [Test]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var existingEmail = this.faker.Internet.Email();
        
        // Create another user with the email
        var anotherUser = new AuthUserModel
        {
            Email = existingEmail,
            Password = this.hashPasswordHandler.Handle(this.faker.Internet.Password()),
            Name = this.faker.Person.FullName
        };

        this.context.AuthUsers.Add(anotherUser);
        await this.context.SaveChangesAsync();

        var request = new UpdateUserQuery(this.testUser.Id, Email: existingEmail);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Is.EqualTo("This email already exists"));
    }

    [Test]
    public async Task Handle_WhenUpdatingCompanies_RemovesOldRolesAndAddsNewOnes()
    {
        // Arrange
        var company1 = new AuthCompanyModel { Name = this.faker.Company.CompanyName() };
        var company2 = new AuthCompanyModel { Name = this.faker.Company.CompanyName() };
        var role1 = new AuthRoleModel { Name = "Admin" };
        var role2 = new AuthRoleModel { Name = "User" };

        this.context.AuthCompanies.AddRange(company1, company2);
        this.context.AuthRoles.AddRange(role1, role2);
        await this.context.SaveChangesAsync();

        // Add initial role
        var initialRole = new AuthUserRoleModel
        {
            UserId = this.testUser.Id,
            CompanyId = company1.Id,
            RoleId = role1.Id
        };

        this.context.AuthUsersRoles.Add(initialRole);
        await this.context.SaveChangesAsync();

        // Update with new company/role
        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(company2.Id, role2.Id)
        };

        var request = new UpdateUserQuery(this.testUser.Id, CompaniesRoles: companiesRoles);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.CompaniesRoles, Has.Count.EqualTo(1));
        Assert.That(result.CompaniesRoles[0].CompanyId, Is.EqualTo(company2.Id));
        Assert.That(result.CompaniesRoles[0].RoleId, Is.EqualTo(role2.Id));

        // Verify in database
        var userRoles = await this.context.AuthUsersRoles
            .Where(ur => ur.UserId == this.testUser.Id)
            .ToListAsync();

        Assert.That(userRoles, Has.Count.EqualTo(1));
        Assert.That(userRoles[0].CompanyId, Is.EqualTo(company2.Id));
        Assert.That(userRoles[0].RoleId, Is.EqualTo(role2.Id));
    }

    [Test]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        // Arrange
        var role = new AuthRoleModel { Name = "Admin" };
        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync();

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(Guid.NewGuid(), role.Id) // Non-existent company
        };

        var request = new UpdateUserQuery(this.testUser.Id, CompaniesRoles: companiesRoles);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Does.Contain("not found"));
    }

    [Test]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var company = new AuthCompanyModel { Name = this.faker.Company.CompanyName() };
        this.context.AuthCompanies.Add(company);
        await this.context.SaveChangesAsync();

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(company.Id, Guid.NewGuid()) // Non-existent role
        };

        var request = new UpdateUserQuery(this.testUser.Id, CompaniesRoles: companiesRoles);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Does.Contain("not found"));
    }
}
