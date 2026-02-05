using NUnit.Framework;
using System.Security.Claims;
using Fenicia.Common.API;

namespace Fenicia.Common.Tests.Api;

[TestFixture]
public class ClaimReaderTests
{
    [Test]
    public void UserId_Returns_Guid_When_Present()
    {
        var id = Guid.NewGuid();
        var claims = new[] { new Claim("userId", id.ToString()) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var result = ClaimReader.UserId(principal);

        Assert.That(result, Is.EqualTo(id));
    }

    [Test]
    public void UserId_Missing_Throws_UnauthorizedAccessException()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        Assert.Throws<UnauthorizedAccessException>(() => ClaimReader.UserId(principal));
    }

    [Test]
    public void ValidateRole_Allows_When_Role_Present()
    {
        var claims = new[] { new Claim("role", "admin") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        Assert.DoesNotThrow(() => ClaimReader.ValidateRole(principal, "admin"));
    }

    [Test]
    public void ValidateRole_Throws_When_Role_Missing()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        Assert.Throws<UnauthorizedAccessException>(() => ClaimReader.ValidateRole(principal, "admin"));
    }
}
