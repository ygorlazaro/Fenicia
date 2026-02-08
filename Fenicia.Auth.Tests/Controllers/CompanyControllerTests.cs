using System.Security.Claims;

using Fenicia.Auth.Domains.Company;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

[TestFixture]
public class CompanyControllerTests
{
    [SetUp]
    public void Setup()
    {
        this.companyServiceMock = new Mock<ICompanyService>();
        this.controller = new CompanyController(this.companyServiceMock.Object);
        this.query = new PaginationQuery { Page = 1, PerPage = 10 };
        this.cancellationToken = CancellationToken.None;
    }

    private Mock<ICompanyService> companyServiceMock;
    private CompanyController controller;
    private PaginationQuery query;
    private CancellationToken cancellationToken;

    [Test]
    public async Task GetByLoggedUserReturnsOkWhenCompaniesExist()
    {
        var userId = Guid.NewGuid();
        var companies = new List<CompanyResponse>
            { new() { Id = Guid.NewGuid(), Name = "Test", Cnpj = "12345678901234" } };
        const int countResponse = 1;

        this.companyServiceMock
            .Setup(x => x.GetByUserIdAsync(userId, this.cancellationToken, this.query.Page, this.query.PerPage))
            .ReturnsAsync(companies);
        this.companyServiceMock.Setup(x => x.CountByUserIdAsync(userId, this.cancellationToken))
            .ReturnsAsync(countResponse);
        this.controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");

        this.controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        var wide = new WideEventContext { UserId = userId.ToString() };

        var result = await this.controller.GetByLoggedUser(this.query, wide, this.cancellationToken);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.TypeOf<Pagination<IEnumerable<CompanyResponse>>>());
    }

    [Test]
    public async Task GetByLoggedUserReturnsNotFoundWhenNoCompanies()
    {
        var userId = Guid.NewGuid();
        var companiesResponse = new List<CompanyResponse>();
        var countResponse = 0;
        this.companyServiceMock
            .Setup(x => x.GetByUserIdAsync(userId, this.cancellationToken, this.query.Page, this.query.PerPage))
            .ReturnsAsync(companiesResponse);
        this.companyServiceMock.Setup(x => x.CountByUserIdAsync(userId, this.cancellationToken))
            .ReturnsAsync(countResponse);
        this.controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        this.controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        var result = await this.controller.GetByLoggedUser(this.query, wide, this.cancellationToken);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.TypeOf<Pagination<IEnumerable<CompanyResponse>>>());
        var pagination = okResult.Value as Pagination<IEnumerable<CompanyResponse>>;
        Assert.That(pagination, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pagination.Total, Is.Zero);
            Assert.That(pagination.Data, Is.Empty);
        }
    }

    [Test]
    public async Task PatchAsync_ReturnsOkWhenCompanyUpdated()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new CompanyUpdateRequest { Name = "New Name" };
        var response = new CompanyResponse { Id = companyId, Name = "New Name", Cnpj = "12345678901234" };

        this.companyServiceMock.Setup(x => x.PatchAsync(companyId, userId, request, this.cancellationToken))
            .ReturnsAsync(response);
        this.controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        this.controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        var result = await this.controller.PatchAsync(request, companyId, wide, this.cancellationToken);

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
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new CompanyUpdateRequest { Name = "New Name" };

        this.companyServiceMock.Setup(x => x.PatchAsync(companyId, userId, request, this.cancellationToken))
            .ReturnsAsync((CompanyResponse?)null);
        this.controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        this.controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        var result = await this.controller.PatchAsync(request, companyId, wide, this.cancellationToken);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.Null);
    }

    [Test]
    public void PatchAsync_ThrowsWhenServiceErrors()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var request = new CompanyUpdateRequest { Name = "New Name" };

        this.companyServiceMock.Setup(x => x.PatchAsync(companyId, userId, request, this.cancellationToken))
            .ThrowsAsync(new Exception("service error"));
        this.controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        this.controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        Assert.ThrowsAsync<Exception>(async () =>
            await this.controller.PatchAsync(request, companyId, wide, this.cancellationToken));
    }

    [Test]
    public void GetByLoggedUserThrowsExceptionOnServiceError()
    {
        var userId = Guid.NewGuid();
        this.companyServiceMock
            .Setup(x => x.GetByUserIdAsync(userId, this.cancellationToken, this.query.Page, this.query.PerPage))
            .ThrowsAsync(new Exception("Service error"));
        this.controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        this.controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var wide = new WideEventContext { UserId = userId.ToString() };

        Assert.ThrowsAsync<Exception>(async () =>
            await this.controller.GetByLoggedUser(this.query, wide, this.cancellationToken));
    }
}