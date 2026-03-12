using Bogus;

using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class DeleteUserHandlerTests : IDisposable
{
    private readonly DeleteUserHandler handler;
    private readonly DefaultContext db;
    private readonly UserModel testUser;

    public DeleteUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        this.handler = new DeleteUserHandler(this.db);
        var faker = new Faker();

        // Create test user
        this.testUser = new UserModel
        {
            Email = faker.Internet.Email(),
            Password =faker.Internet.Password().Hash(),
            Name = faker.Person.FullName
        };

        this.db.AuthUsers.Add(this.testUser);
        this.db.SaveChanges();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_SoftDeletesUserSuccessfully()
    {
        // Arrange
        var request = new DeleteUserCommand(this.testUser.Id);

        // Act
        await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        // Verify user was soft deleted (not removed)
        var deletedUser = await this.db.AuthUsers.FindAsync(this.testUser.Id);
        Assert.NotNull(deletedUser);
        Assert.NotNull(deletedUser.Deleted);
        Assert.True(deletedUser.Deleted.Value <= DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var request = new DeleteUserCommand(nonExistentUserId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request,
                CancellationToken.None));

        Assert.Equal("User not found",
            exception.Message);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyDeleted_ThrowsArgumentException()
    {
        // Arrange
        this.testUser.Deleted = DateTime.UtcNow;
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new DeleteUserCommand(this.testUser.Id);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request,
                CancellationToken.None));

        Assert.Equal("User not found",
            exception.Message);
    }

    [Fact]
    public async Task Handle_SoftDelete_UserStillExistsInDatabase()
    {
        // Arrange
        var request = new DeleteUserCommand(this.testUser.Id);

        // Act
        await this.handler.Handle(request,
            CancellationToken.None);

        // Assert - User should still exist but be marked as deleted
        var user = await this.db.AuthUsers.FindAsync(this.testUser.Id);
        Assert.NotNull(user);
        Assert.NotNull(user.Deleted);

        // Verify user count hasn't changed (soft delete, not hard delete)
        // Note: Need IgnoreQueryFilters() to bypass the global soft-delete filter
        var totalCount = await this.db.AuthUsers.IgnoreQueryFilters().CountAsync();
        Assert.Equal(1,
            totalCount);
    }
}
