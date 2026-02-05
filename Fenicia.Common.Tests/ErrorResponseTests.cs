namespace Fenicia.Common.Tests;

[TestFixture]
public class ErrorResponseTests
{
    [Test]
    public void ErrorResponse_Message_Defaults_To_Null_And_Can_Be_Set()
    {
        var e = new ErrorResponse();
        Assert.That(e.Message, Is.Null);

        e.Message = "an error";
        Assert.That(e.Message, Is.EqualTo("an error"));
    }
}
