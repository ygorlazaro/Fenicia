using Bogus;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.ChangeUserPassword;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User.ChangeUserPassword;

[TestFixture]
public class ChangeUserPasswordHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.hashPasswordHandler = new HashPasswordHandler();
        this.handler = new ChangeUserPasswordHandler(this.context, this.hashPasswordHandler);
        this.faker = new Faker();

        // Create test user
        this.testUser = new AuthUserModel
        {
            Email = this.faker.Internet.Email(),
            Password = this.hashPasswordHandler.Handle(this.faker.Internet.Password()),
            Name = this.faker.Person.FullName
        };

        this.context.AuthUsers.Add(this.testUser);
        this.context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private ChangeUserPasswordHandler handler = null!;
    private DefaultContext context = null!;
    private HashPasswordHandler hashPasswordHandler = null!;
    private Faker faker = null!;
    private AuthUserModel testUser = null!;

    [Test]
    public async Task Handle_WhenValidRequest_ChangesPasswordSuccessfully()
    {
        // Arrange
        var newPassword = this.faker.Internet.Password();
        var request = new ChangeUserPasswordQuery(this.testUser.Id, newPassword);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.Message, Is.EqualTo("Password changed successfully"));

        // Verify password was updated in database
        var updatedUser = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser.Password, Is.Not.EqualTo(this.testUser.Password));
        Assert.That(updatedUser.Updated, Is.Not.Null);
    }

    [Test]
    public async Task Handle_NewPasswordIsHashed()
    {
        // Arrange
        var newPassword = this.faker.Internet.Password();
        var request = new ChangeUserPasswordQuery(this.testUser.Id, newPassword);

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var updatedUser = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser.Password, Is.Not.EqualTo(newPassword)); // Should be hashed
        Assert.That(updatedUser.Password, Does.StartWith("$2")); // BCrypt format

        // Verify new password works
        var passwordValid = this.hashPasswordHandler.Handle(newPassword);
        Assert.That(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.Password), Is.True);
    }

    [Test]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();
        var request = new ChangeUserPasswordQuery(nonExistentUserId, newPassword);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Is.EqualTo("User not found"));
    }

    [Test]
    public async Task Handle_UpdatesTimestamp()
    {
        // Arrange
        var newPassword = this.faker.Internet.Password();
        var request = new ChangeUserPasswordQuery(this.testUser.Id, newPassword);

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var updatedUser = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser.Updated, Is.Not.Null);
        Assert.That(updatedUser.Updated.Value, Is.GreaterThanOrEqualTo(this.testUser.Updated ?? DateTime.MinValue));
    }
}
