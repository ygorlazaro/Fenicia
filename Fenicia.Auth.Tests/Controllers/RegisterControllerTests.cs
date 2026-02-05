using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.API;
using Fenicia.Common.Database.Requests.Auth;
using Fenicia.Common.Database.Responses.Auth;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class RegisterControllerTests
{
    private Mock<IUserService> userServiceMock;
    private RegisterController sut;

    [SetUp]
    public void Setup()
    {
        userServiceMock = new Mock<IUserService>();
        sut = new RegisterController(userServiceMock.Object);
    }

    [Test]
    public async Task CreateNewUserAsync_ReturnsOk()
    {
        var request = new UserRequest { Email = "a@b.com" };
        var response = new UserResponse { Id = Guid.NewGuid(), Email = request.Email };
        userServiceMock.Setup(x => x.CreateNewUserAsync(request, CancellationToken.None)).ReturnsAsync(response);

        var wide = new WideEventContext();

        var result = await sut.CreateNewUserAsync(request, wide, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        Assert.That(ok?.Value, Is.EqualTo(response));
    }
}
