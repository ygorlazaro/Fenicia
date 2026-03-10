using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
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
    private readonly DefaultContext context;
    private readonly UserModel testUser;

    public DeleteUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        var hashPasswordHandler = new HashPasswordHandler();
        this.handler = new DeleteUserHandler(this.context);
        var faker = new Faker();

        // Create test user
        this.testUser = new UserModel
        {
            Email = faker.Internet.Email(),
            Password = hashPasswordHandler.Handle(faker.Internet.Password()),
            Name = faker.Person.FullName
        };

        this.context.AuthUsers.Add(this.testUser);
        this.context.SaveChanges();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_SoftDeletesUserSuccessfully()
    {
        // Arrange
        var request = new DeleteUserCommand(this.testUser.Id);

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        // Verify user was soft deleted (not removed)
        var deletedUser = await this.context.AuthUsers.FindAsync(this.testUser.Id);
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
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyDeleted_ThrowsArgumentException()
    {
        // Arrange
        this.testUser.Deleted = DateTime.UtcNow;
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new DeleteUserCommand(this.testUser.Id);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task Handle_SoftDelete_UserStillExistsInDatabase()
    {
        // Arrange
        var request = new DeleteUserCommand(this.testUser.Id);

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert - User should still exist but be marked as deleted
        var user = await this.context.AuthUsers.FindAsync(this.testUser.Id);
        Assert.NotNull(user);
        Assert.NotNull(user.Deleted);

        // Verify user count hasn't changed (soft delete, not hard delete)
        // Note: Need IgnoreQueryFilters() to bypass the global soft-delete filter
        var totalCount = await this.context.AuthUsers.IgnoreQueryFilters().CountAsync();
        Assert.Equal(1, totalCount);
    }
}
