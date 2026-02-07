using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class ForgotPasswordControllerTests
{
    private Mock<IForgotPasswordService> forgotPasswordServiceMock;
    private ForgotPasswordController sut;

    [SetUp]
    public void Setup()
    {
        this.forgotPasswordServiceMock = new Mock<IForgotPasswordService>();
        this.sut = new ForgotPasswordController(this.forgotPasswordServiceMock.Object);
    }

    [Test]
    public async Task ForgotPasswordSetsWideUserIdAndReturnsResponse()
    {
        var request = new ForgotPasswordReset { Email = "a@b.com" };
        var wide = new WideEventContext();
        var response = new ForgotPasswordResponse { Id = Guid.NewGuid() };

        this.forgotPasswordServiceMock.Setup(x => x.SaveForgotPasswordAsync(request, CancellationToken.None)).ReturnsAsync(response);

        var result = await this.sut.ForgotPassword(request, wide, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(wide.UserId, Is.EqualTo(request.Email));
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        var ok = (OkObjectResult)result;
        Assert.That(ok.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task ResetPasswordSetsWideUserIdAndReturnsResponse()
    {
        var request = new ForgotPasswordResetRequest { Email = "a@b.com", Code = "ABC123", Password = "p" };
        var wide = new WideEventContext();
        var response = new ForgotPasswordResponse { Id = Guid.NewGuid() };

        this.forgotPasswordServiceMock.Setup(x => x.ResetPasswordAsync(request, CancellationToken.None)).ReturnsAsync(response);

        var result = await this.sut.ResetPassword(request, wide, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(wide.UserId, Is.EqualTo(request.Email));
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        var ok = (OkObjectResult)result;
        Assert.That(ok.Value, Is.EqualTo(response));
    }
}