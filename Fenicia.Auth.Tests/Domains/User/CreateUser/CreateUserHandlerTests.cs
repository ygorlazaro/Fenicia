using Bogus;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.CheckUserExists;
using Fenicia.Auth.Domains.User.CreateUser;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User.CreateUser;

[TestFixture]
public class CreateUserHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.checkUserExistsHandler = new CheckUserExistsHandler(this.context);
        this.hashPasswordHandler = new HashPasswordHandler();
        this.handler = new CreateUserHandler(this.context, this.checkUserExistsHandler, this.hashPasswordHandler);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private CreateUserHandler handler = null!;
    private DefaultContext context = null!;
    private CheckUserExistsHandler checkUserExistsHandler = null!;
    private HashPasswordHandler hashPasswordHandler = null!;
    private Faker faker = null!;

    [Test]
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
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(email));
        Assert.That(result.Name, Is.EqualTo(name));
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));

        // Verify user was saved to database
        var user = await this.context.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Email, Is.EqualTo(email));
        Assert.That(user.Name, Is.EqualTo(name));
    }

    [Test]
    public async Task Handle_WhenEmailExists_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        // Create existing user
        var existingUser = new AuthUserModel
        {
            Email = email,
            Password = this.hashPasswordHandler.Handle(password),
            Name = name
        };

        this.context.AuthUsers.Add(existingUser);
        await this.context.SaveChangesAsync();

        var request = new CreateUserQuery(email, password, "Another " + name);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Is.EqualTo("This email already exists"));
    }

    [Test]
    public async Task Handle_WhenValidRequestWithCompanies_CreatesUserWithCompaniesSuccessfully()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        // Create company and role
        var company = new AuthCompanyModel { Name = this.faker.Company.CompanyName() };
        var role = new AuthRoleModel { Name = "Admin" };

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync();

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(company.Id, role.Id)
        };

        var request = new CreateUserQuery(email, password, name, companiesRoles);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.CompaniesRoles, Is.Not.Empty);
        Assert.That(result.CompaniesRoles.Count, Is.EqualTo(1));
        Assert.That(result.CompaniesRoles[0].CompanyId, Is.EqualTo(company.Id));
        Assert.That(result.CompaniesRoles[0].RoleId, Is.EqualTo(role.Id));

        // Verify user role was saved to database
        var userRole = await this.context.AuthUsersRoles
            .FirstOrDefaultAsync(ur => ur.UserId == result.Id);
        
        Assert.That(userRole, Is.Not.Null);
        Assert.That(userRole.CompanyId, Is.EqualTo(company.Id));
        Assert.That(userRole.RoleId, Is.EqualTo(role.Id));
    }

    [Test]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        var role = new AuthRoleModel { Name = "Admin" };
        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync();

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(Guid.NewGuid(), role.Id) // Non-existent company
        };

        var request = new CreateUserQuery(email, password, name, companiesRoles);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Does.Contain("not found"));
    }

    [Test]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        var company = new AuthCompanyModel { Name = this.faker.Company.CompanyName() };
        this.context.AuthCompanies.Add(company);
        await this.context.SaveChangesAsync();

        var companiesRoles = new List<UserCompanyRoleCommand>
        {
            new(company.Id, Guid.NewGuid()) // Non-existent role
        };

        var request = new CreateUserQuery(email, password, name, companiesRoles);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Does.Contain("not found"));
    }

    [Test]
    public void Handle_PasswordIsHashed_BeforeSaving()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;

        var request = new CreateUserQuery(email, password, name);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var user = this.context.AuthUsers.Local.FirstOrDefault(u => u.Email == email);
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Password, Is.Not.EqualTo(password)); // Password should be hashed
        Assert.That(user.Password, Does.StartWith("$2")); // BCrypt hashes start with $2
    }
}
