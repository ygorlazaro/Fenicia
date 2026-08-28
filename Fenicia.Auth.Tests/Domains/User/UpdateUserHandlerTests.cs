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

public class UpdateUserHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;

    private readonly UserService userService;
    private readonly UserRepository userRepository;
    private readonly UserRoleRepository userRoleRepository;
    private readonly RoleRepository roleRepository;
    private readonly CompanyRepository companyRepository;
    private readonly UserModel testUser;

    public UpdateUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
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
    public async Task Handle_WhenValidRequest_UpdatesUserNameSuccessfully()
    {
        var newName = faker.Person.FullName;
        var request = new UpdateUserCommand(testUser.Id, newName);

        var result = await userService.UpdateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);

        var updatedUser = await userRepository.GetByIdAsync(testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.Equal(newName, updatedUser.Name);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_UpdatesUserEmailSuccessfully()
    {
        var newEmail = faker.Internet.Email();
        var request = new UpdateUserCommand(testUser.Id, Email: newEmail);

        var result = await userService.UpdateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(newEmail, result.Email);

        var updatedUser = await userRepository.GetByIdAsync(testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.Equal(newEmail, updatedUser.Email);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();
        var request = new UpdateUserCommand(nonExistentUserId, "Test");

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.UpdateAsync(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var existingEmail = faker.Internet.Email();

        var anotherUser = new UserModel
        {
            Email = existingEmail,
            Password = SecurityService.Hash(faker.Internet.Password()),
            Name = faker.Person.FullName
        };

        userRepository.InsertAsync(anotherUser, CancellationToken.None).GetAwaiter().GetResult();
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateUserCommand(testUser.Id, Email: existingEmail);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.UpdateAsync(request, CancellationToken.None));

        Assert.Equal("This email already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        var role = new RoleModel { Name = "Admin" };
        await roleRepository.InsertAsync(role, CancellationToken.None);
        await db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
    { new(Guid.NewGuid(), role.Id)
        };

        var request = new UpdateUserCommand(testUser.Id, CompaniesRoles: companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.UpdateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        var company = new CompanyModel
        {
            Name = faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        await companyRepository.InsertAsync(company, CancellationToken.None);
        await db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
    { new(company.Id, Guid.NewGuid())
        };

        var request = new UpdateUserCommand(testUser.Id, CompaniesRoles: companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.UpdateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }
}
