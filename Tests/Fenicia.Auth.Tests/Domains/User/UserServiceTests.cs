using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.User;
using Fenicia.Common.Exceptions;
using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

public class UserServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<ICompanyService> _mockCompanyService;
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<ISecurityService> _mockSecurityService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserRoleService> _mockUserRoleService;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _faker = new Faker();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserRoleService = new Mock<IUserRoleService>();
        _mockRoleService = new Mock<IRoleService>();
        _mockCompanyService = new Mock<ICompanyService>();
        _mockSecurityService = new Mock<ISecurityService>();
        _service = new UserService(
            _mockUserRepository.Object,
            _mockUserRoleService.Object,
            _mockRoleService.Object,
            _mockCompanyService.Object,
            _mockSecurityService.Object);
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailExists_ReturnsTrue()
    {
        var email = _faker.Internet.Email();

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.True(result, "Should return true when email exists");
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailDoesNotExist_ReturnsFalse()
    {
        var email = _faker.Internet.Email();

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.False(result, "Should return false when email doesn't exist");
    }

    [Fact]
    public async Task CreateNewAsync_WhenValidRequest_CreatesUserAndCompanySuccessfully()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var command = new CreateNewUserCommand(
            email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj, companyName));

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockCompanyService.Setup(s => s.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompanyModel?)null);
        _mockSecurityService.Setup(s => s.Hash(password)).Returns("hashed_password");
        _mockRoleService.Setup(s => s.GetRoleAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RoleModel { Id = Guid.NewGuid(), Name = "Admin" });

        var result = await _service.CreateNewAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.Equal(companyName, result.Company.Name);
        Assert.Equal(cnpj, result.Company.Cnpj);

        _mockUserRepository.Verify(
            r => r.InsertAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mockCompanyService.Verify(
            s => s.InsertAsync(It.IsAny<CompanyModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mockUserRoleService.Verify(
            s => s.InsertAsync(It.IsAny<UserRoleModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateNewAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var command = new CreateNewUserCommand(
            email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj, companyName));

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await _service.CreateNewAsync(command, CancellationToken.None));
        Assert.Equal("This email already exists", ex.Message);
    }

    [Fact]
    public async Task CreateNewAsync_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var command = new CreateNewUserCommand(
            email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj, companyName));

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockCompanyService.Setup(s => s.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanyModel { Cnpj = cnpj, Name = "Existing Company" });

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await _service.CreateNewAsync(command, CancellationToken.None));
        Assert.Equal("Company with this CNPJ already exists.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_CreatesUserSuccessfully()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;

        var request = new CreateUserCommand(email, password, name);

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockSecurityService.Setup(s => s.Hash(password)).Returns("hashed_password");

        var result = await _service.CreateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.NotEqual(Guid.Empty, result.Id);

        _mockUserRepository.Verify(
            r => r.InsertAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailExists_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var request = new CreateUserCommand(email, password, "Another " + name);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await _service.CreateAsync(request, CancellationToken.None));

        Assert.Equal("This email already exists", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WhenValidRequest_SoftDeletesUserSuccessfully()
    {
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        await _service.DeleteAsync(userId, CancellationToken.None);

        _mockUserRepository.Verify(
            r => r.UpdateAsync(user.Id, It.Is<UserModel>(u => u.Deleted != null), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();

        _mockUserRepository.Setup(r => r.GetByIdAsync(nonExistentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(() =>
            _service.DeleteAsync(nonExistentUserId, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUserResponse()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;
        var password = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.GetByEmailAsync(email, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.Equal(password, result.Password);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var email = _faker.Internet.Email();

        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        var result = await _service.GetByEmailAsync(email, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserExists_ChangesPasswordSuccessfully()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        const string oldPassword = "old_hashed_password";

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = oldPassword
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockSecurityService.Setup(s => s.Hash(newPassword)).Returns("new_hashed_password");

        var query = new UpdatePasswordCommand(userId, newPassword);

        var result = await _service.UpdateHashedPasswordAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);

        _mockUserRepository.Verify(
            r => r.UpdateAsync(
                user.Id,
                It.Is<UserModel>(u => u.Password == "new_hashed_password"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserDoesNotExist_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var query = new UpdatePasswordCommand(nonExistentUserId, newPassword);

        _mockUserRepository.Setup(r => r.GetByIdAsync(nonExistentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await _service.UpdateHashedPasswordAsync(query, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenValidRequest_UpdatesUserNameSuccessfully()
    {
        var userId = Guid.NewGuid();
        var newName = _faker.Person.FullName;
        var request = new UpdateUserCommand(userId, newName);

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = "Old Name",
            Password = _faker.Internet.Password()
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(userId, It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.Query()).Returns(new List<UserModel>().AsQueryable());
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.UpdateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserChangesOwnPassword_WithCorrectCurrentPassword_ChangesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var currentPassword = _faker.Internet.Password();
        var newPassword = _faker.Internet.Password();
        const string hashedCurrentPassword = "hashed_current_password";
        const string hashedNewPassword = "hashed_new_password";

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = hashedCurrentPassword
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockSecurityService.Setup(s => s.Verify(currentPassword, hashedCurrentPassword)).Returns(true);
        _mockSecurityService.Setup(s => s.Hash(newPassword)).Returns(hashedNewPassword);
        _mockUserRepository.Setup(r => r.UpdateAsync(user.Id, It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "User" } }]);

        var command = new UpdateUserPasswordCommand(userId, currentPassword, newPassword, newPassword);
        var result = await _service.UpdatePasswordAsync(userId, command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        _mockUserRepository.Verify(
            r => r.UpdateAsync(
                user.Id,
                It.Is<UserModel>(u => u.Password == hashedNewPassword),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserChangesOwnPassword_WithWrongCurrentPassword_ThrowsInvalidRequest()
    {
        var userId = Guid.NewGuid();
        var currentPassword = _faker.Internet.Password();
        var newPassword = _faker.Internet.Password();
        const string hashedCurrentPassword = "hashed_current_password";

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = hashedCurrentPassword
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockSecurityService.Setup(s => s.Verify(currentPassword, hashedCurrentPassword)).Returns(false);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "User" } }]);

        var command = new UpdateUserPasswordCommand(userId, currentPassword, newPassword, newPassword);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await _service.UpdatePasswordAsync(userId, command, CancellationToken.None));

        Assert.Equal("Senha atual incorreta.", exception.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserChangesOwnPassword_WithoutCurrentPassword_ThrowsInvalidRequest()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_password"
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "User" } }]);

        var command = new UpdateUserPasswordCommand(userId, null, newPassword, newPassword);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await _service.UpdatePasswordAsync(userId, command, CancellationToken.None));

        Assert.Equal("Senha atual é obrigatória.", exception.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenGodChangesAnyUserPassword_ChangesSuccessfully()
    {
        var godId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        const string hashedNewPassword = "hashed_new_password";

        var godUser = new UserModel
        {
            Id = godId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_god_password"
        };

        var targetUser = new UserModel
        {
            Id = targetUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_target_password"
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(godId, It.IsAny<CancellationToken>())).ReturnsAsync(godUser);
        _mockUserRepository.Setup(r => r.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>())).ReturnsAsync(targetUser);
        _mockSecurityService.Setup(s => s.Hash(newPassword)).Returns(hashedNewPassword);
        _mockUserRepository.Setup(r => r.UpdateAsync(targetUserId, It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetUser);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(godId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "God" } }]);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "User" } }]);

        var command = new UpdateUserPasswordCommand(targetUserId, null, newPassword, newPassword);
        var result = await _service.UpdatePasswordAsync(godId, command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        _mockUserRepository.Verify(
            r => r.UpdateAsync(
                targetUserId,
                It.Is<UserModel>(u => u.Password == hashedNewPassword),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenAdminChangesUserPassword_InSameCompany_ChangesSuccessfully()
    {
        var adminId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        const string hashedNewPassword = "hashed_new_password";

        var adminUser = new UserModel
        {
            Id = adminId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_admin_password"
        };

        var targetUser = new UserModel
        {
            Id = targetUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_target_password"
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(adminId, It.IsAny<CancellationToken>())).ReturnsAsync(adminUser);
        _mockUserRepository.Setup(r => r.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>())).ReturnsAsync(targetUser);
        _mockSecurityService.Setup(s => s.Hash(newPassword)).Returns(hashedNewPassword);
        _mockUserRepository.Setup(r => r.UpdateAsync(targetUserId, It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetUser);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "Admin" }, CompanyId = companyId }]);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "User" }, CompanyId = companyId }]);

        var command = new UpdateUserPasswordCommand(targetUserId, null, newPassword, newPassword);
        var result = await _service.UpdatePasswordAsync(adminId, command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        _mockUserRepository.Verify(
            r => r.UpdateAsync(
                targetUserId,
                It.Is<UserModel>(u => u.Password == hashedNewPassword),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenAdminChangesUserPassword_InDifferentCompany_ThrowsInvalidRequest()
    {
        var adminId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var adminCompanyId = Guid.NewGuid();
        var targetCompanyId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();

        var adminUser = new UserModel
        {
            Id = adminId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_admin_password"
        };

        var targetUser = new UserModel
        {
            Id = targetUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_target_password"
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(adminId, It.IsAny<CancellationToken>())).ReturnsAsync(adminUser);
        _mockUserRepository.Setup(r => r.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>())).ReturnsAsync(targetUser);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "Admin" }, CompanyId = adminCompanyId }]);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "User" }, CompanyId = targetCompanyId }]);

        var command = new UpdateUserPasswordCommand(targetUserId, null, newPassword, newPassword);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await _service.UpdatePasswordAsync(adminId, command, CancellationToken.None));

        Assert.Equal("Usuário não pertence à mesma empresa.", exception.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenAdminChangesAdminPassword_ThrowsInvalidRequest()
    {
        var adminId = Guid.NewGuid();
        var targetAdminId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();

        var adminUser = new UserModel
        {
            Id = adminId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_admin_password"
        };

        var targetAdmin = new UserModel
        {
            Id = targetAdminId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_target_password"
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(adminId, It.IsAny<CancellationToken>())).ReturnsAsync(adminUser);
        _mockUserRepository.Setup(r => r.GetByIdAsync(targetAdminId, It.IsAny<CancellationToken>())).ReturnsAsync(targetAdmin);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "Admin" }, CompanyId = companyId }]);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(targetAdminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "Admin" }, CompanyId = companyId }]);

        var command = new UpdateUserPasswordCommand(targetAdminId, null, newPassword, newPassword);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await _service.UpdatePasswordAsync(adminId, command, CancellationToken.None));

        Assert.Equal("Admin só pode alterar senha de usuários.", exception.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserChangesAnotherUserPassword_ThrowsUnauthorized()
    {
        var userId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_user_password"
        };

        var targetUser = new UserModel
        {
            Id = targetUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "hashed_target_password"
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>())).ReturnsAsync(targetUser);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "User" } }]);

        var command = new UpdateUserPasswordCommand(targetUserId, null, newPassword, newPassword);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _service.UpdatePasswordAsync(userId, command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenPasswordsDoNotMatch_ThrowsInvalidRequest()
    {
        var userId = Guid.NewGuid();
        var currentPassword = _faker.Internet.Password();
        var newPassword = _faker.Internet.Password();
        var confirmPassword = _faker.Internet.Password();
        const string hashedCurrentPassword = "hashed_current_password";

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = hashedCurrentPassword
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockSecurityService.Setup(s => s.Verify(currentPassword, hashedCurrentPassword)).Returns(true);
        _mockUserRoleService.Setup(s => s.GetUserRolesByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserRoleModel { Role = new RoleModel { Name = "User" } }]);

        var command = new UpdateUserPasswordCommand(userId, currentPassword, newPassword, confirmPassword);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await _service.UpdatePasswordAsync(userId, command, CancellationToken.None));

        Assert.Equal("Senhas não coincidem.", exception.Message);
    }
}