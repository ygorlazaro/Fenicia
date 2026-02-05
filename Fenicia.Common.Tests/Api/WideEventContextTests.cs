using Fenicia.Common.API;

using NUnit.Framework;

namespace Fenicia.Common.Tests.Api;

[TestFixture]
public class WideEventContextTests
{
    [Test]
    public void Operation_Composes_Path_And_Method()
    {
        var ctx = new WideEventContext
        {
            Path = "/api/test",
            Method = "GET"
        };

        Assert.That(ctx.Operation, Is.EqualTo("/api/test GET"));
    }

    [Test]
    public void Can_Set_And_Get_Properties()
    {
        var ctx = new WideEventContext
        {
            Path = "/p",
            Method = "POST",
            StatusCode = 201,
            DurationMs = 123,
            UserId = "u",
            Success = true,
            ErrorCode = "E",
            ErrorMessage = "msg"
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ctx.StatusCode, Is.EqualTo(201));
            Assert.That(ctx.DurationMs, Is.EqualTo(123));
            Assert.That(ctx.UserId, Is.EqualTo("u"));
            Assert.That(ctx.Success, Is.True);
            Assert.That(ctx.ErrorCode, Is.EqualTo("E"));
            Assert.That(ctx.ErrorMessage, Is.EqualTo("msg"));
        }
    }
}
