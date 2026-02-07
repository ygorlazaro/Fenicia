using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class ForgotPasswordServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<IForgotPasswordRepository> forgotPasswordRepositoryMock;
    private Mock<IUserService> userServiceMock;
    private ForgotPasswordService sut;

    [SetUp]
    public void Setup()
    {
        this.forgotPasswordRepositoryMock = new Mock<IForgotPasswordRepository>();
        this.userServiceMock = new Mock<IUserService>();
        this.sut = new ForgotPasswordService(this.forgotPasswordRepositoryMock.Object, this.userServiceMock.Object);
    }

    [Test]
    public async Task ResetPasswordAsyncCallsChangePasswordAndInvalidatesAndReturnsResponse()
    {
        var email = "a@b.com";
        var userId = Guid.NewGuid();
        this.userServiceMock.Setup(x => x.GetUserIdFromEmailAsync(email, this.cancellationToken)).ReturnsAsync(new UserResponse { Id = userId });

        var model = new ForgotPasswordModel { Id = Guid.NewGuid(), UserId = userId, Code = "ABC123", IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(1) };
        this.forgotPasswordRepositoryMock.Setup(x => x.GetFromUserIdAndCodeAsync(userId, model.Code, this.cancellationToken)).ReturnsAsync(model);

        var result = await this.sut.ResetPasswordAsync(new ForgotPasswordResetRequest { Email = email, Code = model.Code, Password = "p" }, this.cancellationToken);

        this.forgotPasswordRepositoryMock.Verify(x => x.GetFromUserIdAndCodeAsync(userId, model.Code, this.cancellationToken), Times.Once);
        this.userServiceMock.Verify(x => x.ChangePasswordAsync(model.UserId, It.IsAny<string>(), this.cancellationToken), Times.Once);
        this.forgotPasswordRepositoryMock.Verify(x => x.InvalidateCodeAsync(model.Id, this.cancellationToken), Times.Once);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(model.Id));
    }

    [Test]
    public void ResetPasswordAsyncWhenCodeInvalidThrows()
    {
        var email = "a@b.com";
        var userId = Guid.NewGuid();
        this.userServiceMock.Setup(x => x.GetUserIdFromEmailAsync(email, this.cancellationToken)).ReturnsAsync(new UserResponse { Id = userId });
        this.forgotPasswordRepositoryMock.Setup(x => x.GetFromUserIdAndCodeAsync(userId, "BAD", this.cancellationToken)).ReturnsAsync((ForgotPasswordModel)null!);

        Assert.ThrowsAsync<InvalidDataException>(async () => await this.sut.ResetPasswordAsync(new ForgotPasswordResetRequest { Email = email, Code = "BAD", Password = "p" }, this.cancellationToken));
    }

    [Test]
    public async Task SaveForgotPasswordAsyncCreatesCodeAndSaves()
    {
        var email = "a@b.com";
        var userId = Guid.NewGuid();
        this.userServiceMock.Setup(x => x.GetUserIdFromEmailAsync(email, this.cancellationToken)).ReturnsAsync(new UserResponse { Id = userId });

        ForgotPasswordModel? captured = null;
        this.forgotPasswordRepositoryMock.Setup(x => x.SaveForgotPasswordAsync(It.IsAny<ForgotPasswordModel>(), this.cancellationToken))
            .Returns((ForgotPasswordModel m, CancellationToken _) =>
            {
                captured = m;
                return Task.FromResult(m);
            });

        var result = await this.sut.SaveForgotPasswordAsync(new ForgotPasswordReset { Email = email }, this.cancellationToken);

        Assert.That(captured, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(captured!.UserId, Is.EqualTo(userId));
            Assert.That(captured.Code, Has.Length.EqualTo(6));
            Assert.That(captured.IsActive, Is.True);
            Assert.That(result, Is.Not.Null);
        }
    }
}