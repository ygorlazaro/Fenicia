using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.Register.DTOs;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.User.Interfaces;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Register;

public class RegisterServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenCommandIsValid_ReturnsRegisterResponse()
    {
        var mockUserService = new Mock<IUserService>(MockBehavior.Strict);
        var command = new RegisterCommand(
            "test@example.com",
            "password123",
            "Test User",
            new CreateNewUserCompanyCommand("Company Name", "12345678000199"));

        var expectedUser = new CreateNewUserResponse(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            new CreateNewUserCompanyResponse(Guid.NewGuid(), "Company Name", "12345678000199"));

        mockUserService
            .Setup(s => s.CreateNewAsync(It.IsAny<CreateNewUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        var service = new RegisterService(mockUserService.Object);

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Name, result.Name);
        Assert.Equal(expectedUser.Email, result.Email);
        Assert.Equal(expectedUser.Company.Name, result.Company.Name);

        mockUserService.Verify(s => s.CreateNewAsync(It.Is<CreateNewUserCommand>(c => c.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenUserServiceThrows_PropagatesException()
    {
        var mockUserService = new Mock<IUserService>(MockBehavior.Strict);
        var command = new RegisterCommand(
            "test@example.com",
            "password123",
            "Test User",
            new CreateNewUserCompanyCommand("Company Name", "12345678000199"));

        mockUserService
            .Setup(s => s.CreateNewAsync(It.IsAny<CreateNewUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("User creation failed"));

        var service = new RegisterService(mockUserService.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(command, CancellationToken.None));
    }
}
