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

public class GetUserServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public GetUserServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        var userRoleService = new UserRoleService(_userRoleRepository);
        var roleService = new RoleService(_roleRepository);
        var companyService = new CompanyService(_companyRepository);
        _userService = new UserService(_userRepository, userRoleService, roleService, companyService, new SecurityService());

        var faker = new Faker();

        for (var i = 0; i < 15; i++)
        {
            var user = new UserModel
            {
                Email = faker.Internet.Email(),
                Password = new SecurityService().Hash(faker.Internet.Password()),
                Name = faker.Person.FullName
            };
            _userRepository.InsertAsync(user, CancellationToken.None).GetAwaiter().GetResult();
        }

        _db.SaveChanges();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenNoParameters_ReturnsFirstPageWithDefaultPerPage()
    {
        var result = await _userService.GetAllAsync(1, 10, CancellationToken.None);

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
        var result = await _userService.GetAllAsync(2, 5, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PerPage);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task Handle_UsersAreOrderedAlphabeticallyByName()
    {
        var result = await _userService.GetAllAsync(1, 15, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(result.Data.Select(u => u.Name).OrderBy(n => n), result.Data.Select(u => u.Name));
    }

    [Fact]
    public async Task Handle_WhenLastPage_HasNextIsFalse()
    {
        var result = await _userService.GetAllAsync(2, 10, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(2, result.Page);
    }
}
