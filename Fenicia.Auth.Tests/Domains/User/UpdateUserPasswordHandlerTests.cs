using Bogus;

using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class UpdateUserPasswordHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdateUserPasswordHandler handler;
    private readonly UserModel testUser;

    public UpdateUserPasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());

        handler = new UpdateUserPasswordHandler(db);
        faker = new Faker();

        testUser = new UserModel
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password()
                .Hash(),
            Name = faker.Person.FullName
        };

        db.AuthUsers.Add(testUser);
        db.SaveChanges();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ChangesPasswordSuccessfully()
    {

        var newPassword = faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(testUser.Id, newPassword);
        var originalPasswordHash = testUser.Password;

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.True(result.Success);
        Assert.Equal("Password changed successfully", result.Message);

        var updatedUser = await db.AuthUsers.FindAsync(testUser.Id);
        Assert.NotNull(updatedUser);

        Assert.NotEqual(originalPasswordHash, updatedUser.Password);
    }

    [Fact]
    public async Task Handle_NewPasswordIsHashed()
    {

        var newPassword = faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(testUser.Id, newPassword);

        await handler.Handle(request, CancellationToken.None);

        var updatedUser = await db.AuthUsers.FindAsync(testUser.Id);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.StartsWith("$2", updatedUser.Password);

        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.Password));
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {

        var nonExistentUserId = Guid.NewGuid();
        var newPassword = faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(nonExistentUserId, newPassword);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }
}
