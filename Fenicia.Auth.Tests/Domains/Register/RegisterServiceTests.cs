using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.DTOs.Auth.User;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Register;

public class RegisterServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenCommandIsValid_ReturnsRegisterResponse()
    {
        var mockUserService = new Mock<IUserService>(MockBehavior.Strict);
        var company = new CreateNewUserCompanyCommand("Company Name", "12345678000199");
        var command = new RegisterCommand
        {
            Email = "test@example.com",
            Password = "password123",
            Name = "Test User",
            Company = company
        };

        var expectedUser = new CreateNewUserResponse(Guid.NewGuid(), "Test User", "test@example.com", new
            CreateNewUserCompanyResponse(Guid.NewGuid(), "Company Name", "12345678000199"));

        mockUserService
            .Setup(s => s.CreateNewAsync(It.IsAny<CreateNewUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        var service = new RegisterService(mockUserService.Object, new RegisterMapper());

        var result = await service.CreateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Name, result.Name);
        Assert.Equal(expectedUser.Email, result.Email);
        Assert.Equal(expectedUser.Company.Name, result.Company.Name);

        mockUserService.Verify(
            s => s.CreateNewAsync(
                It.Is<CreateNewUserCommand>(c => c.Email == command.Email),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenUserServiceThrows_PropagatesException()
    {
        var mockUserService = new Mock<IUserService>(MockBehavior.Strict);
        var company = new CreateNewUserCompanyCommand("Company Name", "12345678000199");
        var command = new RegisterCommand
        {
            Email = "test@example.com",
            Password = "password123",
            Name = "Test User",
            Company = company
        };

        mockUserService
            .Setup(s => s.CreateNewAsync(It.IsAny<CreateNewUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("User creation failed"));

        var service = new RegisterService(mockUserService.Object, new RegisterMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(command, CancellationToken.None));
    }
}