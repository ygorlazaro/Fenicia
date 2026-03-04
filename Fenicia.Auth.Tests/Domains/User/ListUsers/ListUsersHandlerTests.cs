using Bogus;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.ListUsers;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User.ListUsers;

[TestFixture]
public class ListUsersHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.hashPasswordHandler = new HashPasswordHandler();
        this.handler = new ListUsersHandler(this.context);
        this.faker = new Faker();

        // Create test users
        this.testUsers = new List<AuthUserModel>();
        for (int i = 0; i < 15; i++)
        {
            var user = new AuthUserModel
            {
                Email = this.faker.Internet.Email(),
                Password = this.hashPasswordHandler.Handle(this.faker.Internet.Password()),
                Name = this.faker.Person.FullName
            };
            this.testUsers.Add(user);
            this.context.AuthUsers.Add(user);
        }

        this.context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private ListUsersHandler handler = null!;
    private DefaultContext context = null!;
    private HashPasswordHandler hashPasswordHandler = null!;
    private Faker faker = null!;
    private List<AuthUserModel> testUsers = null!;

    [Test]
    public async Task Handle_WhenNoParameters_ReturnsFirstPageWithDefaultPageSize()
    {
        // Arrange
        var request = new ListUsersQuery();

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(10));
        Assert.That(result.Users.Count, Is.LessThanOrEqualTo(10));
        Assert.That(result.TotalCount, Is.EqualTo(15));
        Assert.That(result.TotalPages, Is.EqualTo(2));
        Assert.That(result.HasPrevious, Is.False);
        Assert.That(result.HasNext, Is.True);
    }

    [Test]
    public async Task Handle_WhenPageSpecified_ReturnsCorrectPage()
    {
        // Arrange
        var request = new ListUsersQuery(page: 2, pageSize: 5);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Page, Is.EqualTo(2));
        Assert.That(result.PageSize, Is.EqualTo(5));
        Assert.That(result.Users.Count, Is.EqualTo(5));
        Assert.That(result.HasPrevious, Is.True);
        Assert.That(result.HasNext, Is.True);
    }

    [Test]
    public async Task Handle_UsersAreOrderedAlphabeticallyByName()
    {
        // Arrange
        var request = new ListUsersQuery(page: 1, pageSize: 15);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Users, Is.Ordered.By("Name"));
    }

    [Test]
    public async Task Handle_WhenSearchTermProvided_FiltersByName()
    {
        // Arrange
        var searchTerm = this.testUsers[0].Name.Split(' ')[0]; // Get first name
        var request = new ListUsersQuery(searchTerm: searchTerm);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Users.Count, Is.GreaterThan(0));
        Assert.That(result.Users.All(u => u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public async Task Handle_WhenSearchTermProvided_FiltersByEmail()
    {
        // Arrange
        var searchTerm = this.testUsers[0].Email.Split('@')[0]; // Get email prefix
        var request = new ListUsersQuery(searchTerm: searchTerm);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Users.Count, Is.EqualTo(1));
        Assert.That(result.Users[0].Email, Does.Contain(searchTerm));
    }

    [Test]
    public async Task Handle_WhenSearchTermNotFound_ReturnsEmptyList()
    {
        // Arrange
        var request = new ListUsersQuery(searchTerm: "nonexistentuser@fakeemail.com");

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Users.Count, Is.EqualTo(0));
        Assert.That(result.TotalCount, Is.EqualTo(0));
    }

    [Test]
    public async Task Handle_WhenLastPage_HasNextIsFalse()
    {
        // Arrange
        var request = new ListUsersQuery(page: 2, pageSize: 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Page, Is.EqualTo(2));
        Assert.That(result.HasNext, Is.False);
        Assert.That(result.HasPrevious, Is.True);
    }

    [Test]
    public async Task Handle_ReturnsUserCompaniesAndRoles()
    {
        // Arrange - Create user with company and role
        var company = new AuthCompanyModel { Name = this.faker.Company.CompanyName() };
        var role = new AuthRoleModel { Name = "Admin" };

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync();

        var userWithRole = new AuthUserModel
        {
            Email = this.faker.Internet.Email(),
            Password = this.hashPasswordHandler.Handle(this.faker.Internet.Password()),
            Name = this.faker.Person.FullName,
            UsersRoles = new List<AuthUserRoleModel>
            {
                new()
                {
                    CompanyId = company.Id,
                    RoleId = role.Id
                }
            }
        };

        this.context.AuthUsers.Add(userWithRole);
        await this.context.SaveChangesAsync();

        var request = new ListUsersQuery(page: 1, pageSize: 20);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var userResult = result.Users.FirstOrDefault(u => u.Id == userWithRole.Id);
        Assert.That(userResult, Is.Not.Null);
        Assert.That(userResult.Companies, Is.Not.Empty);
        Assert.That(userResult.Companies[0].CompanyId, Is.EqualTo(company.Id));
        Assert.That(userResult.Companies[0].RoleId, Is.EqualTo(role.Id));
        Assert.That(userResult.Companies[0].CompanyName, Is.EqualTo(company.Name));
        Assert.That(userResult.Companies[0].RoleName, Is.EqualTo(role.Name));
    }
}
