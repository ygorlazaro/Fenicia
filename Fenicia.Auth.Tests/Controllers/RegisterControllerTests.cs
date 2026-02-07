using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class RegisterControllerTests
{
    private RegisterController sut;
    private Mock<IUserService> userServiceMock;

    [SetUp]
    public void Setup()
    {
        this.userServiceMock = new Mock<IUserService>();
        this.sut = new RegisterController(this.userServiceMock.Object);
    }

    [Test]
    public async Task CreateNewUserAsync_ReturnsOk()
    {
        var request = new UserRequest { Email = "a@b.com" };
        var response = new UserResponse { Id = Guid.NewGuid(), Email = request.Email };
        this.userServiceMock.Setup(x => x.CreateNewUserAsync(request, CancellationToken.None)).ReturnsAsync(response);

        var wide = new WideEventContext();

        var result = await this.sut.CreateNewUserAsync(request, wide, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        Assert.That(ok?.Value, Is.EqualTo(response));
    }
}