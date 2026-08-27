using Bogus;

using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class CheckUserExistsHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UserService userService;

    public CheckUserExistsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        userService = new UserService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ReturnsTrue()
    {

        var email = faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await userService.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.True(result, "Should return true when email exists");
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ReturnsFalse()
    {

        var email = faker.Internet.Email();

        var result = await userService.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.False(result, "Should return false when email doesn't exist");
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ReturnsFalse()
    {

        var email = faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await userService.ExistsByEmailAsync(upperCaseEmail, CancellationToken.None);

        Assert.False(result, "Email comparison is case-sensitive");
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_OnlyMatchesExactEmail()
    {

        const string email1 = "user1@example.com";
        const string email2 = "user2@example.com";

        var user1 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email1,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email2,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.AddRange(user1, user2);
        await db.SaveChangesAsync(CancellationToken.None);

        var result1 = await userService.ExistsByEmailAsync(email1, CancellationToken.None);
        var result2 = await userService.ExistsByEmailAsync(email2, CancellationToken.None);
        var result3 = await userService.ExistsByEmailAsync("other@example.com", CancellationToken.None);

        Assert.True(result1, "Should find user1");
        Assert.True(result2, "Should find user2");
        Assert.False(result3, "Should not find other user");
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsFalse()
    {

        var email = faker.Internet.Email();

        var result = await userService.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.False(result, "Should return false with empty database");
    }

    [Fact]
    public async Task Handle_WhenEmailContainsExtraSpaces_ReturnsFalse()
    {

        const string email = "test@example.com";
        const string emailWithSpaces = " test@example.com ";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await userService.ExistsByEmailAsync(emailWithSpaces, CancellationToken.None);

        Assert.False(result, "Should not match email with extra spaces");
    }

    [Fact]
    public async Task Handle_WhenEmailHasExtraCharacters_ReturnsFalse()
    {

        const string email = "test@example.com";
        const string emailWithExtra = "test@example.com.";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await userService.ExistsByEmailAsync(emailWithExtra, CancellationToken.None);

        Assert.False(result, "Should not match email with extra characters");
    }
}
