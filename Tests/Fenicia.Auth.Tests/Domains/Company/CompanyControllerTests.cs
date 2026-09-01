using System.Security.Claims;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Company.DTOs;
using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Company;

public class CompanyControllerTests : IDisposable
{
    private readonly CompanyController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<ICompanyService> _serviceMock;
    private readonly Guid _testUserId;

    public CompanyControllerTests()
    {
        _testUserId = Guid.NewGuid();

        _serviceMock = new Mock<ICompanyService>();

        _mockHttpContext = new Mock<HttpContext>();

        _controller = new CompanyController(_serviceMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object }
        };

        SetupUserClaims(_testUserId);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByLoggedUser_WhenUserHasNoCompanies_ReturnsOkWithEmptyPagination()
    {
        var query = new PaginationQuery();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _serviceMock
            .Setup(s => s.GetCompaniesByUserAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<IEnumerable<GetCompaniesByUserResponse>>([], 0, query.Page, query.PerPage));

        var result = await _controller.GetByLoggedUser(query, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<IEnumerable<GetCompaniesByUserResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Empty(returnedPagination.Data);
        Assert.Equal(0, returnedPagination.Total);
        Assert.Equal(_testUserId.ToString(), wide.UserId);

        _serviceMock.Verify(
            s => s.GetCompaniesByUserAsync(_testUserId, query.Page, query.PerPage, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetByLoggedUser_WhenUserHasCompanies_ReturnsOkWithPagination()
    {
        var companyId = Guid.NewGuid();
        var query = new PaginationQuery();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var companies = new List<GetCompaniesByUserResponse>
        {
            new(companyId, "Company Name", "12345678901234", "Admin")
        };

        _serviceMock
            .Setup(s => s.GetCompaniesByUserAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<IEnumerable<GetCompaniesByUserResponse>>(companies, companies.Count, query.Page, query.PerPage));

        var result = await _controller.GetByLoggedUser(query, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<IEnumerable<GetCompaniesByUserResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Single(returnedPagination.Data);
        Assert.Equal(1, returnedPagination.Total);
        Assert.Equal("Company Name", returnedPagination.Data.First().Name);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetByLoggedUser_SetsWideEventContextUserId()
    {
        var query = new PaginationQuery();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _serviceMock
            .Setup(s => s.GetCompaniesByUserAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<IEnumerable<GetCompaniesByUserResponse>>([], 0, query.Page, query.PerPage));

        await _controller.GetByLoggedUser(query, wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenUserIsAdminAndCompanyExists_ReturnsNoContent()
    {
        var companyId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var request = new UpdateCompanyRequest("New Name");

        var result = await _controller.PatchAsync(companyId, request, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.Equal(204, noContentResult.StatusCode);
        Assert.Equal(_testUserId.ToString(), wide.UserId);

        _serviceMock.Verify(
            s => s.UpdateAsync(companyId, _testUserId, request.Name, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task PatchAsync_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ItemNotExistsException());

        var request = new UpdateCompanyRequest("New Name");

        var result = await _controller.PatchAsync(companyId, request, wide, cancellationToken);

        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenUserIsNotAdmin_ReturnsForbidden()
    {
        var companyId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PermissionDeniedException());

        var request = new UpdateCompanyRequest("New Name");

        var result = await _controller.PatchAsync(companyId, request, wide, cancellationToken);

        Assert.IsType<ForbidResult>(result);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        var companyId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var request = new UpdateCompanyRequest("New Name");

        await _controller.PatchAsync(companyId, request, wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public void CompanyController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(CompanyController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void CompanyController_HasRouteAttribute()
    {
        var controllerType = typeof(CompanyController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void CompanyController_HasProducesAttribute()
    {
        var controllerType = typeof(CompanyController);

        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    [Fact]
    public void PatchAsync_HasAuthorizeRolesAttribute()
    {
        var controllerType = typeof(CompanyController);
        var methodInfo = controllerType.GetMethod(nameof(CompanyController.PatchAsync));

        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
