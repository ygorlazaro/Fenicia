using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.CreateUser;
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

        this.context = new DefaultContext(options, new TestCompanyContext());
        var checkUserExistsHandler = new CheckUserExistsHandler(this.context);
        this.hashPasswordHandler = new HashPasswordHandler();
        this.handler = new CreateUserHandler(this.context, checkUserExistsHandler, this.hashPasswordHandler);
        this.faker = new Faker();
    }

    private readonly CreateUserHandler handler;
    private readonly DefaultContext context;
    private readonly HashPasswordHandler hashPasswordHandler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesUserSuccessfully()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        var request = new CreateUserQuery(email, password, name);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.NotEqual(Guid.Empty, result.Id);

        // Verify user was saved to database
        var user = await this.context.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
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
            Password = this.hashPasswordHandler.Handle(password),
            Name = name
        };

        this.context.AuthUsers.Add(existingUser);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new CreateUserQuery(email, password, "Another " + name);

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
            TimeZone = string.Empty,
            Cnpj = string.Empty
        };
        var role = new RoleModel { Name = "Admin" };

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(company.Id, role.Id)
        };

        var request = new CreateUserQuery(email, password, name, companiesRoles);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.CompaniesRoles);
        Assert.Single(result.CompaniesRoles);
        Assert.Equal(company.Id, result.CompaniesRoles[0].CompanyId);
        Assert.Equal(role.Id, result.CompaniesRoles[0].RoleId);

        // Verify user role was saved to database
        var userRole = await this.context.AuthUserRoles
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
        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(Guid.NewGuid(), role.Id) // Non-existent company
        };

        var request = new CreateUserQuery(email, password, name, companiesRoles);

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
            TimeZone = string.Empty,
            Cnpj =string.Empty
        };
        this.context.AuthCompanies.Add(company);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(company.Id, Guid.NewGuid()) // Non-existent role
        };

        var request = new CreateUserQuery(email, password, name, companiesRoles);

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

        var request = new CreateUserQuery(email, password, name);

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var user = this.context.AuthUsers.Local.FirstOrDefault(u => u.Email == email);
        Assert.NotNull(user);
        Assert.NotEqual(password, user.Password); // Password should be hashed
        Assert.StartsWith("$2", user.Password); // BCrypt hashes start with $2
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
