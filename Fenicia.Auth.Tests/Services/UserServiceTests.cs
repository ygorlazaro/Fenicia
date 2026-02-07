using Bogus;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;
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
    private Mock<IMigrationService> migrationServiceMock;
    private Mock<IRoleRepository> roleRepositoryMock;
    private Mock<ISecurityService> securityServiceMock;
    private UserService sut;
    private Mock<IUserRepository> userRepositoryMock;
    private Mock<IUserRoleRepository> userRoleRepositoryMock;

    [SetUp]
    public void Setup()
    {
        this.userRepositoryMock = new Mock<IUserRepository>();
        this.roleRepositoryMock = new Mock<IRoleRepository>();
        this.userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        this.companyRepositoryMock = new Mock<ICompanyRepository>();
        this.securityServiceMock = new Mock<ISecurityService>();
        this.loginAttemptServiceMock = new Mock<ILoginAttemptService>();
        this.migrationServiceMock = new Mock<IMigrationService>();

        this.migrationServiceMock
            .Setup(x => x.RunMigrationsAsync(It.IsAny<Guid>(), It.IsAny<List<ModuleType>>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        this.sut = new UserService(this.userRepositoryMock.Object, this.roleRepositoryMock.Object,
            this.userRoleRepositoryMock.Object, this.companyRepositoryMock.Object, this.securityServiceMock.Object,
            this.loginAttemptServiceMock.Object, this.migrationServiceMock.Object);

        this.faker = new Faker();
    }

    [Test]
    public async Task GetForLoginAsyncWithValidCredentialsReturnsUser()
    {
        var request = new TokenRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password()
        };

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = this.faker.Internet.Password(),
            Name = this.faker.Name.FullName()
        };

        this.userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, this.cancellationToken)).ReturnsAsync(user);

        this.securityServiceMock.Setup(x => x.VerifyPassword(request.Password, user.Password)).Returns(true);

        var result = await this.sut.GetForLoginAsync(request, this.cancellationToken);

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
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password()
        };

        this.userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, this.cancellationToken))
            .ReturnsAsync((UserModel)null!);

        Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.sut.GetForLoginAsync(request, this.cancellationToken));
    }

    [Test]
    public async Task CreateNewUserAsyncWithValidDataCreatesUser()
    {
        var request = new UserRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password(),
            Name = this.faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(14, "0123456789")
            }
        };

        var hashedPassword = this.faker.Internet.Password();
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        this.userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, this.cancellationToken))
            .ReturnsAsync(false);
        this.companyRepositoryMock
            .Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, true, this.cancellationToken))
            .ReturnsAsync(false);
        this.securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns(hashedPassword);
        this.userRepositoryMock.Setup(x => x.Add(It.IsAny<UserModel>()));
        this.companyRepositoryMock.Setup(x => x.Add(It.IsAny<CompanyModel>()));
        this.roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(this.cancellationToken)).ReturnsAsync(adminRole);

        var result = await this.sut.CreateNewUserAsync(request, this.cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Email, Is.EqualTo(request.Email));
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        });

        this.userRepositoryMock.Verify(x => x.SaveChangesAsync(this.cancellationToken), Times.Once);
    }

    [Test]
    public void CreateNewUserAsyncWithExistingUserReturnsBadRequest()
    {
        var request = new UserRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password(),
            Name = this.faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(14, "0123456789")
            }
        };

        this.userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, this.cancellationToken))
            .ReturnsAsync(true);

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.sut.CreateNewUserAsync(request, this.cancellationToken));
    }

    [Test]
    public async Task ExistsInCompanyAsyncReturnsCorrectResult()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const bool exists = true;

        this.userRoleRepositoryMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(exists);

        var result = await this.sut.ExistsInCompanyAsync(userId, companyId, this.cancellationToken);

        Assert.That(result, Is.EqualTo(exists));
    }

    [Test]
    public async Task GetUserForRefreshAsyncWithValidUserReturnsUser()
    {
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Name.FullName()
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        this.userRepositoryMock.Setup(x => x.GetByIdAsync(userId, this.cancellationToken)).ReturnsAsync(user);

        var result = await this.sut.GetUserForRefreshAsync(userId, this.cancellationToken);

        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public void GetUserForRefreshAsyncWithInvalidUserReturnsUnauthorized()
    {
        var userId = Guid.NewGuid();

        this.userRepositoryMock.Setup(x => x.GetByIdAsync(userId, this.cancellationToken))
            .ReturnsAsync((UserModel)null!);

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await this.sut.GetUserForRefreshAsync(userId, this.cancellationToken));
    }

    [Test]
    public void CreateNewUserAsyncWithMissingAdminRoleReturnsInternalServerError()
    {
        var request = new UserRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password(),
            Name = this.faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(14, "0123456789")
            }
        };

        this.userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, this.cancellationToken))
            .ReturnsAsync(false);
        this.companyRepositoryMock
            .Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, true, this.cancellationToken))
            .ReturnsAsync(false);
        this.roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(this.cancellationToken)).ReturnsAsync((RoleModel)null!);
        this.securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns("hashedPassword");

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.sut.CreateNewUserAsync(request, this.cancellationToken));
    }

    [Test]
    public void GetForLoginAsyncWithTooManyAttemptsThrowsPermissionDenied()
    {
        var request = new TokenRequest
            { Email = this.faker.Internet.Email(), Password = this.faker.Internet.Password() };

        this.loginAttemptServiceMock.Setup(x => x.GetAttemptsAsync(request.Email, this.cancellationToken))
            .ReturnsAsync(5);

        Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.sut.GetForLoginAsync(request, this.cancellationToken));
    }

    [Test]
    public void GetForLoginAsyncWithInvalidPasswordIncrementsAttemptsAndThrows()
    {
        var request = new TokenRequest
            { Email = this.faker.Internet.Email(), Password = this.faker.Internet.Password() };

        var user = new UserModel
        {
            Id = Guid.NewGuid(), Email = request.Email, Password = this.faker.Internet.Password(),
            Name = this.faker.Name.FullName()
        };

        this.loginAttemptServiceMock.Setup(x => x.GetAttemptsAsync(request.Email, this.cancellationToken))
            .ReturnsAsync(0);
        this.userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, this.cancellationToken)).ReturnsAsync(user);
        this.securityServiceMock.Setup(x => x.VerifyPassword(request.Password, user.Password)).Returns(false);
        this.loginAttemptServiceMock.Setup(x => x.IncrementAttemptsAsync(request.Email)).Returns(Task.CompletedTask);

        Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.sut.GetForLoginAsync(request, this.cancellationToken));

        this.loginAttemptServiceMock.Verify(x => x.IncrementAttemptsAsync(request.Email), Times.Once);
    }

    [Test]
    public void CreateNewUserAsyncWithExistingCompanyThrows()
    {
        var request = new UserRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password(),
            Name = this.faker.Name.FullName(),
            Company = new CompanyRequest
                { Name = this.faker.Company.CompanyName(), Cnpj = this.faker.Random.String2(14, "0123456789") }
        };

        this.userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, this.cancellationToken))
            .ReturnsAsync(false);
        this.companyRepositoryMock
            .Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, true, this.cancellationToken))
            .ReturnsAsync(true);

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.sut.CreateNewUserAsync(request, this.cancellationToken));
    }

    [Test]
    public void GetUserIdFromEmailAsyncWhenNotFoundThrows()
    {
        this.userRepositoryMock.Setup(x => x.GetUserIdFromEmailAsync(It.IsAny<string>(), this.cancellationToken))
            .ReturnsAsync((Guid?)null);

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.sut.GetUserIdFromEmailAsync("noone@example.com", this.cancellationToken));
    }

    [Test]
    public async Task ChangePasswordAsyncUpdatesPasswordAndReturnsUser()
    {
        var userId = Guid.NewGuid();
        var user = new UserModel
            { Id = userId, Email = this.faker.Internet.Email(), Name = this.faker.Name.FullName(), Password = "old" };

        this.userRepositoryMock.Setup(x => x.GetByIdAsync(userId, this.cancellationToken)).ReturnsAsync(user);
        this.securityServiceMock.Setup(x => x.HashPassword("newpwd")).Returns("hashedNew");
        this.userRepositoryMock.Setup(x => x.SaveChangesAsync(this.cancellationToken)).ReturnsAsync(1);

        var result = await this.sut.ChangePasswordAsync(userId, "newpwd", this.cancellationToken);

        Assert.That(result.Id, Is.EqualTo(userId));
    }

    [Test]
    public void ChangePasswordAsyncWhenUserNotFoundThrows()
    {
        var userId = Guid.NewGuid();

        this.userRepositoryMock.Setup(x => x.GetByIdAsync(userId, this.cancellationToken))
            .ReturnsAsync((UserModel)null!);

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.sut.ChangePasswordAsync(userId, "pwd", this.cancellationToken));
    }
}