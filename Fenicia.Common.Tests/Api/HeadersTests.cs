using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Fenicia.Common.API;

using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Common.Tests.Api;

[TestFixture]
public class HeadersTests
{
    [Test]
    public void CompanyId_Has_FromHeader_And_Required_Attributes()
    {
        var prop = typeof(Headers).GetProperty("CompanyId");
        Assert.That(prop, Is.Not.Null);

        var fromHeader = prop.GetCustomAttribute<FromHeaderAttribute>();
        Assert.That(fromHeader, Is.Not.Null);
        Assert.That(fromHeader.Name, Is.EqualTo("x-company"));

        var required = prop.GetCustomAttribute<RequiredAttribute>();
        Assert.That(required, Is.Not.Null);
    }
}
