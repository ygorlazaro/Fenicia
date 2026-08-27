using Bogus;

using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class DeleteUserHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly UserService userService;
    private readonly UserModel testUser;

    public DeleteUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        userService = new UserService(db);
        var faker = new Faker();

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
    public async Task Handle_WhenValidRequest_SoftDeletesUserSuccessfully()
    {

        await userService.DeleteAsync(testUser.Id, CancellationToken.None);

        var deletedUser = await db.AuthUsers.FindAsync(testUser.Id);
        Assert.NotNull(deletedUser);
        Assert.NotNull(deletedUser.Deleted);
        Assert.True(deletedUser.Deleted.Value <= DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {

        var nonExistentUserId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.DeleteAsync(nonExistentUserId, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyDeleted_ThrowsArgumentException()
    {

        testUser.Deleted = DateTime.UtcNow;
        await db.SaveChangesAsync(CancellationToken.None);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.DeleteAsync(testUser.Id, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task Handle_SoftDelete_UserStillExistsInDatabase()
    {

        await userService.DeleteAsync(testUser.Id, CancellationToken.None);

        var user = await db.AuthUsers.FindAsync(testUser.Id);
        Assert.NotNull(user);
        Assert.NotNull(user.Deleted);

        var totalCount = await db.AuthUsers.IgnoreQueryFilters().CountAsync();
        Assert.Equal(1, totalCount);
    }
}
