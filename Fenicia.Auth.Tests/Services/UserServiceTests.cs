using Bogus;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests.Auth;
using Fenicia.Common.Database.Responses.Auth;
using Fenicia.Common.Enums;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Migrations.Services;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class UserServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<ICompanyRepository> companyRepositoryMock;
    private Faker faker;
    private Mock<ILoginAttemptService> loginAttemptServiceMock;
    private Mock<IRoleRepository> roleRepositoryMock;
    private Mock<ISecurityService> securityServiceMock;
    private UserService sut;
    private Mock<IUserRepository> userRepositoryMock;
    private Mock<IUserRoleRepository> userRoleRepositoryMock;
    private Mock<IMigrationService> migrationServiceMock;

    [SetUp]
    public void Setup()
    {
        userRepositoryMock = new Mock<IUserRepository>();
        roleRepositoryMock = new Mock<IRoleRepository>();
        userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        companyRepositoryMock = new Mock<ICompanyRepository>();
        securityServiceMock = new Mock<ISecurityService>();
        loginAttemptServiceMock = new Mock<ILoginAttemptService>();
        migrationServiceMock = new Mock<IMigrationService>();

        migrationServiceMock.Setup(x => x.RunMigrationsAsync(It.IsAny<Guid>(), It.IsAny<List<ModuleType>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        sut = new UserService(userRepositoryMock.Object, roleRepositoryMock.Object, userRoleRepositoryMock.Object, companyRepositoryMock.Object, securityServiceMock.Object, loginAttemptServiceMock.Object, migrationServiceMock.Object);

        faker = new Faker();
    }

    [Test]
    public async Task GetForLoginAsyncWithValidCredentialsReturnsUser()
    {
        var request = new TokenRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password()
        };

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = faker.Internet.Password(),
            Name = faker.Name.FullName()
        };

        userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, cancellationToken)).ReturnsAsync(user);

        securityServiceMock.Setup(x => x.VerifyPassword(request.Password, user.Password)).Returns(true);

        var result = await sut.GetForLoginAsync(request, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.Name, Is.EqualTo(user.Name));
        });
    }

    [Test]
    public void GetForLoginAsyncWithInvalidUserReturnsBadRequest()
    {
        var request = new TokenRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password()
        };

        userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, cancellationToken)).ReturnsAsync((UserModel)null!);

        Assert.ThrowsAsync<PermissionDeniedException>(async () => await sut.GetForLoginAsync(request, cancellationToken));
    }

    [Test]
    public async Task CreateNewUserAsyncWithValidDataCreatesUser()
    {
        var request = new UserRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password(),
            Name = faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(14, "0123456789")
            }
        };

        var hashedPassword = faker.Internet.Password();
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, cancellationToken)).ReturnsAsync(false);
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, true, cancellationToken)).ReturnsAsync(false);
        securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns(hashedPassword);
        userRepositoryMock.Setup(x => x.Add(It.IsAny<UserModel>()));
        companyRepositoryMock.Setup(x => x.Add(It.IsAny<CompanyModel>()));
        roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(cancellationToken)).ReturnsAsync(adminRole);

        var result = await sut.CreateNewUserAsync(request, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Email, Is.EqualTo(request.Email));
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        });

        userRepositoryMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Test]
    public void CreateNewUserAsyncWithExistingUserReturnsBadRequest()
    {
        var request = new UserRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password(),
            Name = faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(14, "0123456789")
            }
        };

        userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, cancellationToken)).ReturnsAsync(true);

        Assert.ThrowsAsync<ArgumentException>(async () => await sut.CreateNewUserAsync(request, cancellationToken));
    }

    [Test]
    public async Task ExistsInCompanyAsyncReturnsCorrectResult()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const bool exists = true;

        userRoleRepositoryMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(exists);

        var result = await sut.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        Assert.That(result, Is.EqualTo(exists));
    }

    [Test]
    public async Task GetUserForRefreshAsyncWithValidUserReturnsUser()
    {
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Name.FullName()
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        userRepositoryMock.Setup(x => x.GetByIdAsync(userId, cancellationToken)).ReturnsAsync(user);

        var result = await sut.GetUserForRefreshAsync(userId, cancellationToken);

        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public void GetUserForRefreshAsyncWithInvalidUserReturnsUnauthorized()
    {
        var userId = Guid.NewGuid();

        userRepositoryMock.Setup(x => x.GetByIdAsync(userId, cancellationToken)).ReturnsAsync((UserModel)null!);

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.GetUserForRefreshAsync(userId, cancellationToken));
    }

    [Test]
    public void CreateNewUserAsyncWithMissingAdminRoleReturnsInternalServerError()
    {
        var request = new UserRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password(),
            Name = faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(14, "0123456789")
            }
        };

        userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, cancellationToken)).ReturnsAsync(false);
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, true, cancellationToken)).ReturnsAsync(false);
        roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(cancellationToken)).ReturnsAsync((RoleModel)null!);
        securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns("hashedPassword");

        Assert.ThrowsAsync<ArgumentException>(async () => await sut.CreateNewUserAsync(request, cancellationToken));
    }

    [Test]
    public void GetForLoginAsyncWithTooManyAttemptsThrowsPermissionDenied()
    {
        var request = new TokenRequest { Email = faker.Internet.Email(), Password = faker.Internet.Password() };

        loginAttemptServiceMock.Setup(x => x.GetAttemptsAsync(request.Email, cancellationToken)).ReturnsAsync(5);

        Assert.ThrowsAsync<PermissionDeniedException>(async () => await sut.GetForLoginAsync(request, cancellationToken));
    }

    [Test]
    public void GetForLoginAsyncWithInvalidPasswordIncrementsAttemptsAndThrows()
    {
        var request = new TokenRequest { Email = faker.Internet.Email(), Password = faker.Internet.Password() };

        var user = new UserModel { Id = Guid.NewGuid(), Email = request.Email, Password = faker.Internet.Password(), Name = faker.Name.FullName() };

        loginAttemptServiceMock.Setup(x => x.GetAttemptsAsync(request.Email, cancellationToken)).ReturnsAsync(0);
        userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, cancellationToken)).ReturnsAsync(user);
        securityServiceMock.Setup(x => x.VerifyPassword(request.Password, user.Password)).Returns(false);
        loginAttemptServiceMock.Setup(x => x.IncrementAttemptsAsync(request.Email)).Returns(Task.CompletedTask);

        Assert.ThrowsAsync<PermissionDeniedException>(async () => await sut.GetForLoginAsync(request, cancellationToken));

        loginAttemptServiceMock.Verify(x => x.IncrementAttemptsAsync(request.Email), Times.Once);
    }

    [Test]
    public void CreateNewUserAsyncWithExistingCompanyThrows()
    {
        var request = new UserRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password(),
            Name = faker.Name.FullName(),
            Company = new CompanyRequest { Name = faker.Company.CompanyName(), Cnpj = faker.Random.String2(14, "0123456789") }
        };

        userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, cancellationToken)).ReturnsAsync(false);
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, true, cancellationToken)).ReturnsAsync(true);

        Assert.ThrowsAsync<ArgumentException>(async () => await sut.CreateNewUserAsync(request, cancellationToken));
    }

    [Test]
    public void GetUserIdFromEmailAsyncWhenNotFoundThrows()
    {
        userRepositoryMock.Setup(x => x.GetUserIdFromEmailAsync(It.IsAny<string>(), cancellationToken)).ReturnsAsync((Guid?)null);

        Assert.ThrowsAsync<ArgumentException>(async () => await sut.GetUserIdFromEmailAsync("noone@example.com", cancellationToken));
    }

    [Test]
    public async Task ChangePasswordAsyncUpdatesPasswordAndReturnsUser()
    {
        var userId = Guid.NewGuid();
        var user = new UserModel { Id = userId, Email = faker.Internet.Email(), Name = faker.Name.FullName(), Password = "old" };

        userRepositoryMock.Setup(x => x.GetByIdAsync(userId, cancellationToken)).ReturnsAsync(user);
        securityServiceMock.Setup(x => x.HashPassword("newpwd")).Returns("hashedNew");
        userRepositoryMock.Setup(x => x.SaveChangesAsync(cancellationToken)).ReturnsAsync(1);

        var result = await sut.ChangePasswordAsync(userId, "newpwd", cancellationToken);

        Assert.That(result.Id, Is.EqualTo(userId));
    }

    [Test]
    public void ChangePasswordAsyncWhenUserNotFoundThrows()
    {
        var userId = Guid.NewGuid();

        userRepositoryMock.Setup(x => x.GetByIdAsync(userId, cancellationToken)).ReturnsAsync((UserModel)null!);

        Assert.ThrowsAsync<ArgumentException>(async () => await sut.ChangePasswordAsync(userId, "pwd", cancellationToken));
    }
}
