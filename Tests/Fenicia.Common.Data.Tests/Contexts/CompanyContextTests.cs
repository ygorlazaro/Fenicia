using System.Security.Claims;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Fenicia.Common.Data.Tests.Contexts;

public class CompanyContextTests
{
    [Fact]
    public void CompanyId_ShouldReturnEmpty_WhenNoClaimsOrHeaders()
    {
        var companyContext = CreateCompanyContext(null);

        companyContext.CompanyId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CompanyId_ShouldReturnEmpty_WhenClaimIsInvalidGuid()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim("company_id", "not-a-guid")
                ]))
        };

        var companyContext = CreateCompanyContext(httpContext);

        companyContext.CompanyId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CompanyId_ShouldReturnEmpty_WhenHeaderIsInvalidGuid()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["CompanyId"] = "not-a-guid";

        var companyContext = CreateCompanyContext(httpContext);

        companyContext.CompanyId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CompanyId_ShouldReturnClaimValue_WhenJwtClaimExists()
    {
        var companyId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim("company_id", companyId.ToString())
                ]))
        };

        var companyContext = CreateCompanyContext(httpContext);

        companyContext.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public void CompanyId_ShouldReturnHeaderValue_WhenHeaderExists()
    {
        var companyId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["CompanyId"] = companyId.ToString();

        var companyContext = CreateCompanyContext(httpContext);

        companyContext.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public void CompanyId_ShouldReturnClaimValue_WhenBothClaimAndHeaderExistAndMatch()
    {
        var companyId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim("company_id", companyId.ToString())
                ]))
        };
        httpContext.Request.Headers["CompanyId"] = companyId.ToString();

        var companyContext = CreateCompanyContext(httpContext);

        companyContext.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public void CompanyId_ShouldThrow_WhenClaimAndHeaderMismatch()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim("company_id", Guid.NewGuid().ToString())
                ]))
        };
        httpContext.Request.Headers["CompanyId"] = Guid.NewGuid().ToString();

        var companyContext = CreateCompanyContext(httpContext);

        Action act = () => companyContext.CompanyId.Should().NotBe(Guid.Empty);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CompanyId_ShouldSupportCompanyIdClaimAlias()
    {
        var companyId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim("companyId", companyId.ToString())
                ]))
        };

        var companyContext = CreateCompanyContext(httpContext);

        companyContext.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public void CompanyId_ShouldSupportCompanyIdHeaderAlias()
    {
        var companyId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["companyId"] = companyId.ToString();

        var companyContext = CreateCompanyContext(httpContext);

        companyContext.CompanyId.Should().Be(companyId);
    }

    private static CompanyContext CreateCompanyContext(HttpContext? httpContext)
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(a => a.HttpContext).Returns(httpContext!);

        return new CompanyContext(accessor.Object);
    }
}