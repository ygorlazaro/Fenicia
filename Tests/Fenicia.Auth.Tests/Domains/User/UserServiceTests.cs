using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

public class UserServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<ISecurityService> _mockSecurityService;
    private readonly Mock<IUserRoleService> _mockUserRoleService;
    private readonly Mock<ICompanyService> _mockCompanyService;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _faker = new Faker();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserRoleService = new Mock<IUserRoleService>();
        _mockRoleService = new Mock<IRoleService>();
        _mockCompanyService = new Mock<ICompanyService>();
        _mockSecurityService = new Mock<ISecurityService>();
        _service = new UserService(_mockUserRepository.Object, _mockUserRoleService.Object, _mockRoleService.Object, _mockCompanyService.Object, _mockSecurityService.Object);
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

        var command = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockCompanyService.Setup(s => s.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>())).ReturnsAsync((CompanyModel?)null);
        _mockSecurityService.Setup(s => s.Hash(password)).Returns("hashed_password");
        _mockRoleService.Setup(s => s.GetRoleAsync("Admin", It.IsAny<CancellationToken>())).ReturnsAsync(new RoleModel { Id = Guid.NewGuid(), Name = "Admin" });

        var result = await _service.CreateNewAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.Equal(companyName, result.Company.Name);
        Assert.Equal(cnpj, result.Company.Cnpj);

        _mockUserRepository.Verify(r => r.InsertAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockCompanyService.Verify(s => s.InsertAsync(It.IsAny<CompanyModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRoleService.Verify(s => s.InsertAsync(It.IsAny<UserRoleModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateNewAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var command = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.CreateNewAsync(command, CancellationToken.None));
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

        var command = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockCompanyService.Setup(s => s.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>())).ReturnsAsync(new CompanyModel { Cnpj = cnpj, Name = "Existing Company" });

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.CreateNewAsync(command, CancellationToken.None));
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

        _mockUserRepository.Verify(r => r.InsertAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailExists_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var request = new CreateUserCommand(email, password, "Another " + name);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.CreateAsync(request, CancellationToken.None));

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

        _mockUserRepository.Verify(r => r.UpdateAsync(user.Id, It.Is<UserModel>(u => u.Deleted != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();

        _mockUserRepository.Setup(r => r.GetByIdAsync(nonExistentUserId, It.IsAny<CancellationToken>())).ReturnsAsync((UserModel?)null);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.DeleteAsync(nonExistentUserId, CancellationToken.None));

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

        _mockUserRepository.Verify(r => r.UpdateAsync(user.Id, It.Is<UserModel>(u => u.Password == "new_hashed_password"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserDoesNotExist_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var query = new UpdatePasswordCommand(nonExistentUserId, newPassword);

        _mockUserRepository.Setup(r => r.GetByIdAsync(nonExistentUserId, It.IsAny<CancellationToken>())).ReturnsAsync((UserModel?)null);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.UpdateHashedPasswordAsync(query, CancellationToken.None));

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

        var result = await _service.UpdateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);
    }
}
