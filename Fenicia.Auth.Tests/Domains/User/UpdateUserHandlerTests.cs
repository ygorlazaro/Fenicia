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
    private readonly DefaultContext db;
    private readonly Faker faker;

    private readonly UpdateUserHandler handler;
    private readonly UserModel testUser;

    public UpdateUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new UpdateUserHandler(db);
        faker = new Faker();

        // Create test user
        testUser = new UserModel
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password()
                .Hash(),
            Name = faker.Person.FullName
        };

        db.AuthUsers.Add(testUser);
        db.SaveChanges();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_UpdatesUserNameSuccessfully()
    {
        // Arrange
        var newName = faker.Person.FullName;
        var request = new UpdateUserCommand(testUser.Id, newName);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);

        // Verify user was updated in database
        var updatedUser = await db.AuthUsers.FindAsync(testUser.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(newName, updatedUser.Name);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_UpdatesUserEmailSuccessfully()
    {
        // Arrange
        var newEmail = faker.Internet.Email();
        var request = new UpdateUserCommand(testUser.Id, Email: newEmail);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newEmail, result.Email);

        // Verify user was updated in database
        var updatedUser = await db.AuthUsers.FindAsync(testUser.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(newEmail, updatedUser.Email);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var request = new UpdateUserCommand(nonExistentUserId, "Test");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var existingEmail = faker.Internet.Email();

        // Create another user with the email
        var anotherUser = new UserModel
        {
            Email = existingEmail,
            Password = faker.Internet.Password()
                .Hash(),
            Name = faker.Person.FullName
        };

        db.AuthUsers.Add(anotherUser);
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateUserCommand(testUser.Id, Email: existingEmail);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(request, CancellationToken.None));

        Assert.Equal("This email already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        // Arrange
        var role = new RoleModel { Name = "Admin" };
        db.AuthRoles.Add(role);
        await db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
        {
            new(Guid.NewGuid(), role.Id) // Non-existent company
        };

        var request = new UpdateUserCommand(testUser.Id, CompaniesRoles: companiesRoles);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var company = new CompanyModel
        {
            Name = faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
        {
            new(company.Id, Guid.NewGuid()) // Non-existent role
        };

        var request = new UpdateUserCommand(testUser.Id, CompaniesRoles: companiesRoles);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }
}
