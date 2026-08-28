using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class UpdateUserPasswordHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UserService userService;
    private readonly UserRepository userRepository;
    private readonly UserRoleRepository userRoleRepository;
    private readonly RoleRepository roleRepository;
    private readonly CompanyRepository companyRepository;
    private readonly UserModel testUser;

    public UpdateUserPasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        userRepository = new UserRepository(db);
        userRoleRepository = new UserRoleRepository(db);
        roleRepository = new RoleRepository(db);
        companyRepository = new CompanyRepository(db);
        userService = new UserService(userRepository, userRoleRepository, roleRepository, companyRepository);
        faker = new Faker();

        testUser = new UserModel
        {
            Email = faker.Internet.Email(),
            Password = SecurityService.Hash(faker.Internet.Password()),
            Name = faker.Person.FullName
        };

        userRepository.InsertAsync(testUser, CancellationToken.None).GetAwaiter().GetResult();
        db.SaveChanges();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ChangesPasswordSuccessfully()
    {
        var newPassword = faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(testUser.Id, newPassword);
        var originalPasswordHash = testUser.Password;

        var result = await userService.UpdatePasswordAsync(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.True(result.Success);
        Assert.Equal("Password changed successfully", result.Message);

        var updatedUser = await userRepository.GetByIdAsync(testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);

        Assert.NotEqual(originalPasswordHash, updatedUser.Password);
    }

    [Fact]
    public async Task Handle_NewPasswordIsHashed()
    {
        var newPassword = faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(testUser.Id, newPassword);

        await userService.UpdatePasswordAsync(request, CancellationToken.None);

        var updatedUser = await userRepository.GetByIdAsync(testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.StartsWith("$2", updatedUser.Password);

        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.Password));
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();
        var newPassword = faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(nonExistentUserId, newPassword);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.UpdatePasswordAsync(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }
}
