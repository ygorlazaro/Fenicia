using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class GetUserHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly UserService userService;
    private readonly UserRepository userRepository;
    private readonly UserRoleRepository userRoleRepository;
    private readonly RoleRepository roleRepository;
    private readonly CompanyRepository companyRepository;

    public GetUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        userRepository = new UserRepository(db);
        userRoleRepository = new UserRoleRepository(db);
        roleRepository = new RoleRepository(db);
        companyRepository = new CompanyRepository(db);
        userService = new UserService(userRepository, userRoleRepository, roleRepository, companyRepository);

        var faker = new Faker();

        for (var i = 0; i < 15; i++)
        {
            var user = new UserModel
            {
                Email = faker.Internet.Email(),
                Password = SecurityService.Hash(faker.Internet.Password()),
                Name = faker.Person.FullName
            };
            userRepository.InsertAsync(user, CancellationToken.None).GetAwaiter().GetResult();
        }

        db.SaveChanges();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenNoParameters_ReturnsFirstPageWithDefaultPerPage()
    {
        var result = await userService.GetAllAsync(1, 10, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PerPage);
        Assert.True(result.Data.Count <= 10);
        Assert.Equal(15, result.Total);
        Assert.Equal(2, result.Pages);
    }

    [Fact]
    public async Task Handle_WhenPageSpecified_ReturnsCorrectPage()
    {
        var result = await userService.GetAllAsync(2, 5, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PerPage);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task Handle_UsersAreOrderedAlphabeticallyByName()
    {
        var result = await userService.GetAllAsync(1, 15, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(result.Data.Select(u => u.Name).OrderBy(n => n), result.Data.Select(u => u.Name));
    }

    [Fact]
    public async Task Handle_WhenLastPage_HasNextIsFalse()
    {
        var result = await userService.GetAllAsync(2, 10, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(2, result.Page);
    }
}
