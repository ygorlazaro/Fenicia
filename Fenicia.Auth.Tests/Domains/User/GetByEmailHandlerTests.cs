using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class GetByEmailHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UserService userService;
    private readonly UserRepository userRepository;
    private readonly UserRoleRepository userRoleRepository;
    private readonly RoleRepository roleRepository;
    private readonly CompanyRepository companyRepository;

    public GetByEmailHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        userRepository = new UserRepository(db);
        userRoleRepository = new UserRoleRepository(db);
        roleRepository = new RoleRepository(db);
        companyRepository = new CompanyRepository(db);
        userService = new UserService(userRepository, userRoleRepository, roleRepository, companyRepository);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsUserResponse()
    {
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var name = faker.Person.FullName;
        var password = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        userRepository.InsertAsync(user, CancellationToken.None).GetAwaiter().GetResult();
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId, result.Id);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.Equal(password, result.Password);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNull()
    {
        var email = faker.Internet.Email();

        var result = await userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
        var name = faker.Person.FullName;
        var password = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        userRepository.InsertAsync(user, CancellationToken.None).GetAwaiter().GetResult();
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await userService.GetByEmailAsync(upperCaseEmail, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_ReturnsCorrectUser()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";
        var name1 = faker.Person.FullName;
        var name2 = faker.Person.FullName;
        var password1 = faker.Internet.Password();
        var password2 = faker.Internet.Password();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = name1,
            Password = password1
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = name2,
            Password = password2
        };

        db.AuthUsers.AddRange(user1, user2);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await userService.GetByEmailAsync(email1, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId1, result.Id);
        Assert.Equal(email1, result.Email);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        var email = faker.Internet.Email();

        var result = await userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenEmailContainsExtraSpaces_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var emailWithSpaces = " test@example.com ";
        var name = faker.Person.FullName;
        var password = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        userRepository.InsertAsync(user, CancellationToken.None).GetAwaiter().GetResult();
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await userService.GetByEmailAsync(emailWithSpaces, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesResponseContainsAllFields()
    {
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var name = faker.Person.FullName;
        var password = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        userRepository.InsertAsync(user, CancellationToken.None).GetAwaiter().GetResult();
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.NotNull(result);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(result.Email);
        Assert.NotNull(result.Name);
        Assert.NotNull(result.Password);
    }
}
