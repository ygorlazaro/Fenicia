using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.UpdateUser;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class UpdateUserHandlerTests : IDisposable
{
    public UpdateUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.hashPasswordHandler = new HashPasswordHandler();
        this.handler = new UpdateUserHandler(this.context);
        this.faker = new Faker();

        // Create test user
        this.testUser = new UserModel
        {
            Email = this.faker.Internet.Email(),
            Password = this.hashPasswordHandler.Handle(this.faker.Internet.Password()),
            Name = this.faker.Person.FullName
        };

        this.context.AuthUsers.Add(this.testUser);
        this.context.SaveChanges();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly UpdateUserHandler handler;
    private readonly DefaultContext context;
    private readonly HashPasswordHandler hashPasswordHandler;
    private readonly Faker faker;
    private readonly UserModel testUser;

    [Fact]
    public async Task Handle_WhenValidRequest_UpdatesUserNameSuccessfully()
    {
        // Arrange
        var newName = this.faker.Person.FullName;
        var request = new UpdateUserQuery(this.testUser.Id, Name: newName);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);
        Assert.NotNull(result.Updated);

        // Verify user was updated in database
        var updatedUser = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(newName, updatedUser.Name);
        Assert.NotNull(updatedUser.Updated);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_UpdatesUserEmailSuccessfully()
    {
        // Arrange
        var newEmail = this.faker.Internet.Email();
        var request = new UpdateUserQuery(this.testUser.Id, Email: newEmail);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newEmail, result.Email);

        // Verify user was updated in database
        var updatedUser = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(newEmail, updatedUser.Email);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var request = new UpdateUserQuery(nonExistentUserId, Name: "Test");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
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
            Password = this.hashPasswordHandler.Handle(this.faker.Internet.Password()),
            Name = this.faker.Person.FullName
        };

        this.context.AuthUsers.Add(anotherUser);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateUserQuery(this.testUser.Id, Email: existingEmail);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Equal("This email already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        // Arrange
        var role = new RoleModel { Name = "Admin" };
        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(Guid.NewGuid(), role.Id) // Non-existent company
        };

        var request = new UpdateUserQuery(this.testUser.Id, CompaniesRoles: companiesRoles);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var company = new CompanyModel
        {
            Name = this.faker.Company.CompanyName(),
            TimeZone = string.Empty,
            Cnpj = string.Empty
        };
        this.context.AuthCompanies.Add(company);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(company.Id, Guid.NewGuid()) // Non-existent role
        };

        var request = new UpdateUserQuery(this.testUser.Id, CompaniesRoles: companiesRoles);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }
}
