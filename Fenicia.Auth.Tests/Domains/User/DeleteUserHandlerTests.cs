using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class DeleteUserHandlerTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;
    private readonly UserModel _testUser;

    public DeleteUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        _userService = new UserService(_userRepository, _userRoleRepository, _roleRepository, _companyRepository, new SecurityService());
        var faker = new Faker();

        _testUser = new UserModel
        {
            Email = faker.Internet.Email(),
            Password = new SecurityService().Hash(faker.Internet.Password()),
            Name = faker.Person.FullName
        };

        _userRepository.InsertAsync(_testUser, CancellationToken.None).GetAwaiter().GetResult();
        _db.SaveChanges();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_SoftDeletesUserSuccessfully()
    {
        await _userService.DeleteAsync(_testUser.Id, CancellationToken.None);

        var deletedUser = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(deletedUser);
        Assert.NotNull(deletedUser.Deleted);
        Assert.True(deletedUser.Deleted.Value <= DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.DeleteAsync(nonExistentUserId, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyDeleted_ThrowsArgumentException()
    {
        _testUser.Deleted = DateTime.UtcNow;
        await _db.SaveChangesAsync(CancellationToken.None);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.DeleteAsync(_testUser.Id, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task Handle_SoftDelete_UserStillExistsInDatabase()
    {
        await _userService.DeleteAsync(_testUser.Id, CancellationToken.None);

        var user = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(user);
        Assert.NotNull(user.Deleted);

        var totalCount = await _userRepository.Query().IgnoreQueryFilters().CountAsync();
        Assert.Equal(1, totalCount);
    }
}
