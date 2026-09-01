using Bogus;
using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.DTOs;
using Fenicia.Auth.Domains.ForgotPassword.Interfaces;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Moq;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class ForgotPasswordServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IForgotPasswordRepository> _mockRepository;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ISecurityService> _mockSecurityService;
    private readonly ForgotPasswordService _service;

    public ForgotPasswordServiceTests()
    {
        _faker = new Faker();
        _mockRepository = new Mock<IForgotPasswordRepository>();
        _mockUserService = new Mock<IUserService>();
        _mockSecurityService = new Mock<ISecurityService>();
        _service = new ForgotPasswordService(_mockRepository.Object, _mockUserService.Object, _mockSecurityService.Object);
    }

    [Fact]
    public async Task AddAsync_WhenEmailExists_CreatesForgotPasswordCodeSuccessfully()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AddForgotPasswordCommand(email);

        await _service.AddAsync(command, CancellationToken.None);

        _mockRepository.Verify(r => r.InsertAsync(It.IsAny<ForgotPasswordModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        var email = _faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.AddAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task AddAsync_WhenEmailHasDifferentCase_ThrowsItemNotExistsException()
    {
        var email = _faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(upperCaseEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        var command = new AddForgotPasswordCommand(upperCaseEmail);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.AddAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task AddAsync_WhenMultipleUsersExist_CreatesCodeForCorrectUser()
    {
        var userId1 = Guid.NewGuid();
        var email1 = _faker.Internet.Email();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);

        var command = new AddForgotPasswordCommand(email1);

        await _service.AddAsync(command, CancellationToken.None);

        _mockRepository.Verify(r => r.InsertAsync(It.Is<ForgotPasswordModel>(fp => fp.UserId == userId1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenCalledMultipleTimesForSameUser_CreatesMultipleCodes()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AddForgotPasswordCommand(email);

        await _service.AddAsync(command, CancellationToken.None);
        await _service.AddAsync(command, CancellationToken.None);

        _mockRepository.Verify(r => r.InsertAsync(It.IsAny<ForgotPasswordModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddAsync_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        var email = _faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.AddAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task AddAsync_VerifiesCodeIsUnique()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = _faker.Internet.Email();
        var email2 = _faker.Internet.Email();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);
        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        var command1 = new AddForgotPasswordCommand(email1);
        var command2 = new AddForgotPasswordCommand(email2);

        await _service.AddAsync(command1, CancellationToken.None);
        await _service.AddAsync(command2, CancellationToken.None);

        _mockRepository.Verify(r => r.InsertAsync(It.IsAny<ForgotPasswordModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddAsync_WhenIpAddressAndUserAgentProvided_StoresThemCorrectly()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        const string ipAddress = "192.168.1.1";
        const string userAgent = "Mozilla/5.0 (Test Browser)";

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AddForgotPasswordCommand(email, ipAddress, userAgent);

        await _service.AddAsync(command, CancellationToken.None);

        _mockRepository.Verify(r => r.InsertAsync(It.Is<ForgotPasswordModel>(fp => fp.IpAddress == ipAddress && fp.UserAgent == userAgent), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetAsync_WhenValidCode_ResetsPasswordSuccessfully()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetActiveByUserIdAndCodeAsync(user.Id, code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forgotPassword);
        _mockSecurityService.Setup(s => s.Hash(newPassword)).Returns("hashed_password");

        var command = new ResetPasswordCommand(email, newPassword, code);

        await _service.ResetAsync(command, CancellationToken.None);

        _mockRepository.Verify(r => r.UpdateAsync(forgotPassword.Id, It.Is<ForgotPasswordModel>(fp => !fp.IsActive), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserService.Verify(s => s.UpdateHashedPasswordAsync(It.IsAny<UpdatePasswordCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetAsync_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task ResetAsync_WhenCodeDoesNotExist_ThrowsInvalidDataException()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        const string invalidCode = "INVALID";
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetActiveByUserIdAndCodeAsync(user.Id, invalidCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ForgotPasswordModel?)null);

        var command = new ResetPasswordCommand(email, newPassword, invalidCode);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task ResetAsync_WhenCodeIsInactive_ThrowsInvalidDataException()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetActiveByUserIdAndCodeAsync(user.Id, code, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ForgotPasswordModel?)null);

        var command = new ResetPasswordCommand(email, newPassword, code);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task ResetAsync_WhenCodeIsExpired_ThrowsInvalidDataException()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetActiveByUserIdAndCodeAsync(user.Id, code, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ForgotPasswordModel?)null);

        var command = new ResetPasswordCommand(email, newPassword, code);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task ResetAsync_WhenCodeBelongsToDifferentUser_ThrowsInvalidDataException()
    {
        Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        _faker.Internet.Email();
        var email2 = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);
        _mockRepository.Setup(r => r.GetActiveByUserIdAndCodeAsync(userId2, code, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ForgotPasswordModel?)null);

        var command = new ResetPasswordCommand(email2, newPassword, code);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task ResetAsync_WhenCodeIsUsedSecondTime_ThrowsInvalidDataException()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetActiveByUserIdAndCodeAsync(user.Id, code, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ForgotPasswordModel?)null);

        var command = new ResetPasswordCommand(email, newPassword, code);

        await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task ResetAsync_VerifiesPasswordWasActuallyChanged()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        const string originalPassword = "OriginalPassword123!";
        const string newPassword = "NewPassword456!";

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = originalPassword
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockRepository.Setup(r => r.GetActiveByUserIdAndCodeAsync(user.Id, code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forgotPassword);
        _mockSecurityService.Setup(s => s.Hash(newPassword)).Returns("hashed_new_password");

        var command = new ResetPasswordCommand(email, newPassword, code);

        await _service.ResetAsync(command, CancellationToken.None);

        _mockUserService.Verify(s => s.UpdateHashedPasswordAsync(It.Is<UpdatePasswordCommand>(p => p.UserId == userId && p.Password == "hashed_new_password"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetAsync_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        _mockUserService.Setup(s => s.FirstByEmailOrDefaultAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }
}
