using Bogus;

using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class CreateUserHandlerTests : IDisposable
{
    public CreateUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        
        this.handler = new CreateUserHandler(this.db);
        this.faker = new Faker();
    }

    private readonly CreateUserHandler handler;
    private readonly DefaultContext db;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesUserSuccessfully()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        var request = new CreateUserCommand(email, password, name);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.NotEqual(Guid.Empty, result.Id);

        // Verify user was saved to database
        var user = await this.db.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        // Create existing user
        var existingUser = new UserModel
        {
            Email = email,
            Password = password.Hash(),
            Name = name
        };

        this.db.AuthUsers.Add(existingUser);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new CreateUserCommand(email, password, "Another " + name);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Equal("This email already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenValidRequestWithCompanies_CreatesUserWithCompaniesSuccessfully()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        // Create company and role
        var company = new CompanyModel
        {
            Name = this.faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        var role = new RoleModel { Name = "Admin" };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<CreateUserRoleCommand>
        {
            new(company.Id, role.Id)
        };

        var request = new CreateUserCommand(email, password, name, companiesRoles);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        // Verify user role was saved to database
        var userRole = await this.db.AuthUserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == result.Id);

        Assert.NotNull(userRole);
        Assert.Equal(company.Id, userRole.CompanyId);
        Assert.Equal(role.Id, userRole.RoleId);
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        var role = new RoleModel { Name = "Admin" };
        this.db.AuthRoles.Add(role);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<CreateUserRoleCommand>
        {
            new(Guid.NewGuid(), role.Id) // Non-existent company
        };

        var request = new CreateUserCommand(email, password, name, companiesRoles);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        var company = new CompanyModel
        {
            Name = this.faker.Company.CompanyName(),
            Cnpj =string.Empty
        };
        this.db.AuthCompanies.Add(company);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<CreateUserRoleCommand>
        {
            new(company.Id, Guid.NewGuid()) // Non-existent role
        };

        var request = new CreateUserCommand(email, password, name, companiesRoles);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_PasswordIsHashed_BeforeSaving()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        var request = new CreateUserCommand(email, password, name);

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var user = this.db.AuthUsers.Local.FirstOrDefault(u => u.Email == email);
        Assert.NotNull(user);
        Assert.NotEqual(password, user.Password); // Password should be hashed
        Assert.StartsWith("$2", user.Password); // BCrypt hashes start with $2
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
