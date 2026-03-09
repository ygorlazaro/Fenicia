using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.DeleteUser;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

[TestFixture]
public class DeleteUserHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.hashPasswordHandler = new HashPasswordHandler();
        this.handler = new DeleteUserHandler(this.context);
        this.faker = new Faker();

        // Create test user
        this.testUser = new UserModel
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

    private DeleteUserHandler handler = null!;
    private DefaultContext context = null!;
    private HashPasswordHandler hashPasswordHandler = null!;
    private Faker faker = null!;
    private UserModel testUser = null!;

    [Test]
    public async Task Handle_WhenValidRequest_SoftDeletesUserSuccessfully()
    {
        // Arrange
        var request = new DeleteUserQuery(this.testUser.Id);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("User deleted successfully"));
        }

        // Verify user was soft deleted (not removed)
        var deletedUser = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.That(deletedUser, Is.Not.Null);
        Assert.That(deletedUser.Deleted, Is.Not.Null);
        Assert.That(deletedUser.Deleted.Value, Is.LessThanOrEqualTo(DateTime.UtcNow));
    }

    [Test]
    public void Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var request = new DeleteUserQuery(nonExistentUserId);

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Is.EqualTo("User not found"));
    }

    [Test]
    public async Task Handle_WhenUserAlreadyDeleted_ThrowsArgumentException()
    {
        // Arrange
        this.testUser.Deleted = DateTime.UtcNow;
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new DeleteUserQuery(this.testUser.Id);

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.That(exception!.Message, Is.EqualTo("User not found"));
    }

    [Test]
    public async Task Handle_SoftDelete_UserStillExistsInDatabase()
    {
        // Arrange
        var request = new DeleteUserQuery(this.testUser.Id);

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert - User should still exist but be marked as deleted
        var user = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Deleted, Is.Not.Null);

        // Verify user count hasn't changed (soft delete, not hard delete)
        // Note: Need IgnoreQueryFilters() to bypass the global soft-delete filter
        var totalCount = await this.context.AuthUsers.IgnoreQueryFilters().CountAsync();
        Assert.That(totalCount, Is.EqualTo(1));
    }
}
