namespace Fenicia.Common.Tests;

[TestFixture]
public class TextConstantsTests
{
    [Test]
    public void Key_Constants_Have_Expected_Values()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(TextConstants.InvalidJwtSecretMessage, Is.EqualTo("Invalid Jwt Secret"));
            Assert.That(TextConstants.InvalidUsernameOrPasswordMessage, Is.EqualTo("Invalid username or password"));
            Assert.That(TextConstants.PermissionDeniedMessage, Is.EqualTo("Permission denied"));
            Assert.That(TextConstants.ItemNotFoundMessage, Is.EqualTo("Item not found"));
            Assert.That(TextConstants.TooManyAttempts, Is.EqualTo("Too many login attempts. Try again later"));
        }

    }
}
