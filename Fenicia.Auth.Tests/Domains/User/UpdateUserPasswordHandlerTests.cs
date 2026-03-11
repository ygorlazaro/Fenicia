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

public class UpdateUserPasswordHandlerTests : IDisposable
{
    private readonly UpdateUserPasswordHandler handler;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UserModel testUser;

    public UpdateUserPasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        
        this.handler = new UpdateUserPasswordHandler(this.db);
        this.faker = new Faker();

        // Create test user
        this.testUser = new UserModel
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password().Hash(),
            Name = this.faker.Person.FullName
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
    public async Task Handle_WhenValidRequest_ChangesPasswordSuccessfully()
    {
        // Arrange
        var newPassword = this.faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(this.testUser.Id, newPassword);
        var originalPasswordHash = this.testUser.Password;

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.True(result.Success);
        Assert.Equal("Password changed successfully", result.Message);

        // Verify password was updated in database
        var updatedUser = await this.db.AuthUsers.FindAsync(this.testUser.Id);
        Assert.NotNull(updatedUser);
        
        Assert.NotEqual(originalPasswordHash, updatedUser.Password);
    }

    [Fact]
    public async Task Handle_NewPasswordIsHashed()
    {
        // Arrange
        var newPassword = this.faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(this.testUser.Id, newPassword);

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var updatedUser = await this.db.AuthUsers.FindAsync(this.testUser.Id);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(newPassword, updatedUser.Password); // Should be hashed
        Assert.StartsWith("$2", updatedUser.Password); // BCrypt format

        // Verify new password works
        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.Password));
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(nonExistentUserId, newPassword);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }
}
