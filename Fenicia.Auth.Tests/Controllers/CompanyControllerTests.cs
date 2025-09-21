namespace Fenicia.Auth.Tests.Controllers;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

using Fenicia.Auth.Domains.Company;
using Fenicia.Common;
using Fenicia.Common.Database.Responses;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

[TestFixture]
public class CompanyControllerTests
{
    private Mock<ICompanyService> companyServiceMock;
    private Mock<ILogger<CompanyController>> loggerMock;
    private CompanyController controller;
    private PaginationQuery query;
    private CancellationToken cancellationToken;

    [SetUp]
    public void Setup()
    {
        companyServiceMock = new Mock<ICompanyService>();
        loggerMock = new Mock<ILogger<CompanyController>>();
        controller = new CompanyController(loggerMock.Object, companyServiceMock.Object);
        query = new PaginationQuery { Page = 1, PerPage = 10 };
        cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task GetByLoggedUserReturnsOkWhenCompaniesExist()
    {
        // Arrange
        var userId = System.Guid.NewGuid();
        var companies = new List<CompanyResponse> { new() { Id = System.Guid.NewGuid(), Name = "Test", Cnpj = "12345678901234" } };
        var companiesResponse = new ApiResponse<List<CompanyResponse>>(companies, System.Net.HttpStatusCode.OK);
        var countResponse = new ApiResponse<int>(1, System.Net.HttpStatusCode.OK);
        companyServiceMock.Setup(x => x.GetByUserIdAsync(userId, cancellationToken, query.Page, query.PerPage)).ReturnsAsync(companiesResponse);
        companyServiceMock.Setup(x => x.CountByUserIdAsync(userId, cancellationToken)).ReturnsAsync(countResponse);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        // Act
        var result = await controller.GetByLoggedUser(query, cancellationToken);

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
        var userId = System.Guid.NewGuid();
        var companiesResponse = new ApiResponse<List<CompanyResponse>>(null, System.Net.HttpStatusCode.NotFound, "Not found");
        var countResponse = new ApiResponse<int>(0, System.Net.HttpStatusCode.OK);
        companyServiceMock.Setup(x => x.GetByUserIdAsync(userId, cancellationToken, query.Page, query.PerPage)).ReturnsAsync(companiesResponse);
        companyServiceMock.Setup(x => x.CountByUserIdAsync(userId, cancellationToken)).ReturnsAsync(countResponse);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        // Act
        var result = await controller.GetByLoggedUser(query, cancellationToken);

        // Assert
        Assert.That(result.Result, Is.TypeOf<ObjectResult>());
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)System.Net.HttpStatusCode.NotFound));
    }

    [Test]
    public void GetByLoggedUserThrowsExceptionOnServiceError()
    {
        // Arrange
        var userId = System.Guid.NewGuid();
        companyServiceMock.Setup(x => x.GetByUserIdAsync(userId, cancellationToken, query.Page, query.PerPage)).ThrowsAsync(new Exception("Service error"));
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await controller.GetByLoggedUser(query, cancellationToken));
    }
}
