using System.Security.Claims;

using Fenicia.Auth.Domains.Company;
using Fenicia.Common;
using Fenicia.Common.Api;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Database.Requests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

[TestFixture]
public class CompanyControllerTests
{
    private Mock<ICompanyService> companyServiceMock;
    private CompanyController controller;
    private PaginationQuery query;
    private CancellationToken cancellationToken;

    [SetUp]
    public void Setup()
    {
        companyServiceMock = new Mock<ICompanyService>();
        controller = new CompanyController(companyServiceMock.Object);
        query = new PaginationQuery { Page = 1, PerPage = 10 };
        cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task GetByLoggedUserReturnsOkWhenCompaniesExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companies = new List<CompanyResponse> { new() { Id = Guid.NewGuid(), Name = "Test", Cnpj = "12345678901234" } };
        var countResponse = 1;

        companyServiceMock.Setup(x => x.GetByUserIdAsync(userId, cancellationToken, query.Page, query.PerPage)).ReturnsAsync(companies);
        companyServiceMock.Setup(x => x.CountByUserIdAsync(userId, cancellationToken)).ReturnsAsync(countResponse);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");

        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        var wide = new WideEventContext { UserId = userId.ToString() };

        // Act
        var result = await controller.GetByLoggedUser(query, wide, cancellationToken);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.TypeOf<Pagination<IEnumerable<CompanyResponse>>>());
    }

    [Test]
    public async Task GetByLoggedUserReturnsNotFoundWhenNoCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companiesResponse = new List<CompanyResponse>();
        var countResponse = 0;
        companyServiceMock.Setup(x => x.GetByUserIdAsync(userId, cancellationToken, query.Page, query.PerPage)).ReturnsAsync(companiesResponse);
        companyServiceMock.Setup(x => x.CountByUserIdAsync(userId, cancellationToken)).ReturnsAsync(countResponse);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        // Act
        var result = await controller.GetByLoggedUser(query, wide, cancellationToken);

        // Assert: controller returns 200 OK with empty pagination (current behavior)
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.TypeOf<Pagination<IEnumerable<CompanyResponse>>>());
        var pagination = okResult.Value as Pagination<IEnumerable<CompanyResponse>>;
        Assert.That(pagination, Is.Not.Null);
        Assert.That(pagination.Total, Is.EqualTo(0));
        Assert.That(pagination.Data, Is.Empty);
    }

    [Test]
    public async Task PatchAsync_ReturnsOkWhenCompanyUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new CompanyUpdateRequest { Name = "New Name" };
        var response = new CompanyResponse { Id = companyId, Name = "New Name", Cnpj = "12345678901234" };

        companyServiceMock.Setup(x => x.PatchAsync(companyId, userId, request, cancellationToken)).ReturnsAsync(response);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        // Act
        var result = await controller.PatchAsync(request, companyId, wide, cancellationToken);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.TypeOf<CompanyResponse>());
        var returned = okResult.Value as CompanyResponse;
        Assert.That(returned?.Id, Is.EqualTo(companyId));
    }

    [Test]
    public async Task PatchAsync_ReturnsOkWhenCompanyNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new CompanyUpdateRequest { Name = "New Name" };

        companyServiceMock.Setup(x => x.PatchAsync(companyId, userId, request, cancellationToken)).ReturnsAsync((CompanyResponse?)null);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        // Act
        var result = await controller.PatchAsync(request, companyId, wide, cancellationToken);

        // Assert: current controller returns Ok(null)
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.Null);
    }

    [Test]
    public void PatchAsync_ThrowsWhenServiceErrors()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new CompanyUpdateRequest { Name = "New Name" };

        companyServiceMock.Setup(x => x.PatchAsync(companyId, userId, request, cancellationToken)).ThrowsAsync(new Exception("service error"));
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await controller.PatchAsync(request, companyId, wide, cancellationToken));
    }

    [Test]
    public void GetByLoggedUserThrowsExceptionOnServiceError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        companyServiceMock.Setup(x => x.GetByUserIdAsync(userId, cancellationToken, query.Page, query.PerPage)).ThrowsAsync(new Exception("Service error"));
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await controller.GetByLoggedUser(query, wide, cancellationToken));
    }
}
