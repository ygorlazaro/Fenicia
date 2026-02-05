using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Moq;

namespace Fenicia.Auth.Tests.Services;

public class ForgotPasswordServiceTests
{
    private Mock<IForgotPasswordRepository> forgotPasswordRepositoryMock;
    private Mock<IUserService> userServiceMock;
    private ForgotPasswordService sut;
    private CancellationToken cancellationToken = CancellationToken.None;

    [SetUp]
    public void Setup()
    {
        forgotPasswordRepositoryMock = new Mock<IForgotPasswordRepository>();
        userServiceMock = new Mock<IUserService>();
        sut = new ForgotPasswordService(forgotPasswordRepositoryMock.Object, userServiceMock.Object);
    }

    [Test]
    public async Task ResetPasswordAsyncCallsChangePasswordAndInvalidatesAndReturnsResponse()
    {
        // Arrange
        var email = "a@b.com";
        var userId = Guid.NewGuid();
        userServiceMock.Setup(x => x.GetUserIdFromEmailAsync(email, cancellationToken)).ReturnsAsync(new UserResponse { Id = userId });

        var model = new ForgotPasswordModel { Id = Guid.NewGuid(), UserId = userId, Code = "ABC123", IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(1) };
        forgotPasswordRepositoryMock.Setup(x => x.GetFromUserIdAndCodeAsync(userId, model.Code, cancellationToken)).ReturnsAsync(model);

        // Act
        var result = await sut.ResetPasswordAsync(new ForgotPasswordRequestReset { Email = email, Code = model.Code, Password = "p" }, cancellationToken);

        // Assert
        forgotPasswordRepositoryMock.Verify(x => x.GetFromUserIdAndCodeAsync(userId, model.Code, cancellationToken), Times.Once);
        userServiceMock.Verify(x => x.ChangePasswordAsync(model.UserId, It.IsAny<string>(), cancellationToken), Times.Once);
        forgotPasswordRepositoryMock.Verify(x => x.InvalidateCodeAsync(model.Id, cancellationToken), Times.Once);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(model.Id));
    }

    [Test]
    public void ResetPasswordAsyncWhenCodeInvalidThrows()
    {
        // Arrange
        var email = "a@b.com";
        var userId = Guid.NewGuid();
        userServiceMock.Setup(x => x.GetUserIdFromEmailAsync(email, cancellationToken)).ReturnsAsync(new UserResponse { Id = userId });
        forgotPasswordRepositoryMock.Setup(x => x.GetFromUserIdAndCodeAsync(userId, "BAD", cancellationToken)).ReturnsAsync((ForgotPasswordModel)null!);

        // Act & Assert
        Assert.ThrowsAsync<InvalidDataException>(async () => await sut.ResetPasswordAsync(new ForgotPasswordRequestReset { Email = email, Code = "BAD", Password = "p" }, cancellationToken));
    }

    [Test]
    public async Task SaveForgotPasswordAsyncCreatesCodeAndSaves()
    {
        // Arrange
        var email = "a@b.com";
        var userId = Guid.NewGuid();
        userServiceMock.Setup(x => x.GetUserIdFromEmailAsync(email, cancellationToken)).ReturnsAsync(new UserResponse { Id = userId });

        ForgotPasswordModel? captured = null;
        forgotPasswordRepositoryMock.Setup(x => x.SaveForgotPasswordAsync(It.IsAny<ForgotPasswordModel>(), cancellationToken))
            .Returns((ForgotPasswordModel m, CancellationToken ct) =>
            {
                captured = m;
                return Task.FromResult(m);
            });

        // Act
        var result = await sut.SaveForgotPasswordAsync(new ForgotPasswordRequest { Email = email }, cancellationToken);

        // Assert
        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.UserId, Is.EqualTo(userId));
        Assert.That(captured.Code.Length, Is.EqualTo(6));
        Assert.That(captured.IsActive, Is.True);
        Assert.That(result, Is.Not.Null);
    }
}
