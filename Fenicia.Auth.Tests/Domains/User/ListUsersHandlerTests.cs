using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.ListUsers;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class ListUsersHandlerTests : IDisposable
{
    private readonly ListUsersHandler handler;
    private readonly DefaultContext context;
    private readonly HashPasswordHandler hashPasswordHandler;
    private readonly Faker faker;
    private readonly List<UserModel> testUsers;

    public ListUsersHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.hashPasswordHandler = new HashPasswordHandler();
        this.handler = new ListUsersHandler(this.context);
        this.faker = new Faker();

        // Create test users
        this.testUsers = [];
        for (var i = 0; i < 15; i++)
        {
            var user = new UserModel
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

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenNoParameters_ReturnsFirstPageWithDefaultPageSize()
    {
        // Arrange
        var request = new ListUsersQuery();

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.True(result.Users.Count <= 10);
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.False(result.HasPrevious);
        Assert.True(result.HasNext);
    }

    [Fact]
    public async Task Handle_WhenPageSpecified_ReturnsCorrectPage()
    {
        // Arrange
        var request = new ListUsersQuery(Page: 2, PageSize: 5);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(5, result.Users.Count);
        Assert.True(result.HasPrevious);
        Assert.True(result.HasNext);
    }

    [Fact]
    public async Task Handle_UsersAreOrderedAlphabeticallyByName()
    {
        // Arrange
        var request = new ListUsersQuery(Page: 1, PageSize: 15);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Users.Select(u => u.Name).OrderBy(n => n), result.Users.Select(u => u.Name));
    }

    [Fact]
    public async Task Handle_WhenSearchTermProvided_FiltersByName()
    {
        // Arrange
        var searchTerm = this.testUsers[0].Name.Split(' ')[0]; // Get first name
        var request = new ListUsersQuery(SearchTerm: searchTerm);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Users);
        Assert.True(result.Users.All(u => u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public async Task Handle_WhenSearchTermProvided_FiltersByEmail()
    {
        // Arrange
        var searchTerm = this.testUsers[0].Email.Split('@')[0]; // Get email prefix
        var request = new ListUsersQuery(SearchTerm: searchTerm);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Users);
        Assert.Contains(searchTerm, result.Users[0].Email);
    }

    [Fact]
    public async Task Handle_WhenSearchTermNotFound_ReturnsEmptyList()
    {
        // Arrange
        var request = new ListUsersQuery(SearchTerm: "nonexistentuser@fakeemail.com");

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Empty(result.Users);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task Handle_WhenLastPage_HasNextIsFalse()
    {
        // Arrange
        var request = new ListUsersQuery(Page: 2, PageSize: 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(2, result.Page);
        Assert.False(result.HasNext);
        Assert.True(result.HasPrevious);
    }

    [Fact]
    public async Task Handle_ReturnsUserCompaniesAndRoles()
    {
        // Arrange - Create user with company and role
        var company = new CompanyModel
        {
            Name = this.faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        var role = new RoleModel { Name = "Admin" };

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var userWithRole = new UserModel
        {
            Email = this.faker.Internet.Email(),
            Password = this.hashPasswordHandler.Handle(this.faker.Internet.Password()),
            Name = this.faker.Person.FullName,
            UsersRoles =
            [
                new()
                {
                    CompanyId = company.Id,
                    RoleId = role.Id
                }
            ]
        };

        this.context.AuthUsers.Add(userWithRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new ListUsersQuery(Page: 1, PageSize: 20);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var userResult = result.Users.FirstOrDefault(u => u.Id == userWithRole.Id);
        Assert.NotNull(userResult);
        Assert.NotEmpty(userResult.Companies);
        
        Assert.Equal(company.Id, userResult.Companies[0].CompanyId);
        Assert.Equal(role.Id, userResult.Companies[0].RoleId);
        Assert.Equal(company.Name, userResult.Companies[0].CompanyName);
        Assert.Equal(role.Name, userResult.Companies[0].RoleName);
    }
}
