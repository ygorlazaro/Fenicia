using Bogus;

using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class UpdateUserHandlerTests : IDisposable
{
    public UpdateUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        this.handler = new UpdateUserHandler(this.db);
        this.faker = new Faker();

        // Create test user
        this.testUser = new UserModel
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password().Hash(),
            Name = this.faker.Person.FullName
        };

        this.db.AuthUsers.Add(this.testUser);
        this.db.SaveChanges();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly UpdateUserHandler handler;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UserModel testUser;

    [Fact]
    public async Task Handle_WhenValidRequest_UpdatesUserNameSuccessfully()
    {
        // Arrange
        var newName = this.faker.Person.FullName;
        var request = new UpdateUserCommand(this.testUser.Id,
            Name: newName);

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newName,
            result.Name);

        // Verify user was updated in database
        var updatedUser = await this.db.AuthUsers.FindAsync(this.testUser.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(newName,
            updatedUser.Name);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_UpdatesUserEmailSuccessfully()
    {
        // Arrange
        var newEmail = this.faker.Internet.Email();
        var request = new UpdateUserCommand(this.testUser.Id,
            Email: newEmail);

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newEmail,
            result.Email);

        // Verify user was updated in database
        var updatedUser = await this.db.AuthUsers.FindAsync(this.testUser.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(newEmail,
            updatedUser.Email);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var request = new UpdateUserCommand(nonExistentUserId,
            Name: "Test");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request,
                CancellationToken.None));

        Assert.Equal("User not found",
            exception.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var existingEmail = this.faker.Internet.Email();

        // Create another user with the email
        var anotherUser = new UserModel
        {
            Email = existingEmail,
            Password = this.faker.Internet.Password().Hash(),
            Name = this.faker.Person.FullName
        };

        this.db.AuthUsers.Add(anotherUser);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateUserCommand(this.testUser.Id,
            Email: existingEmail);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request,
                CancellationToken.None));

        Assert.Equal("This email already exists",
            exception.Message);
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        // Arrange
        var role = new RoleModel { Name = "Admin" };
        this.db.AuthRoles.Add(role);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
        {
            new(Guid.NewGuid(),
                role.Id) // Non-existent company
        };

        var request = new UpdateUserCommand(this.testUser.Id,
            CompaniesRoles: companiesRoles);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request,
                CancellationToken.None));

        Assert.Contains("not found",
            exception.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var company = new CompanyModel
        {
            Name = this.faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        this.db.AuthCompanies.Add(company);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
        {
            new(company.Id,
                Guid.NewGuid()) // Non-existent role
        };

        var request = new UpdateUserCommand(this.testUser.Id,
            CompaniesRoles: companiesRoles);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request,
                CancellationToken.None));

        Assert.Contains("not found",
            exception.Message);
    }
}
