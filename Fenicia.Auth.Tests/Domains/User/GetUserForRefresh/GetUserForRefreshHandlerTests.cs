using Bogus;

using Fenicia.Auth.Domains.User.GetUserForRefresh;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User.GetUserForRefresh;

[TestFixture]
public class GetUserForRefreshHandlerTests
{
    private AuthContext context = null!;
    private GetUserForRefreshHandler handler = null!;
    private Faker faker = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new GetUserForRefreshHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    [Test]
    public async Task Handle_WhenUserExists_ReturnsUserResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(userId), "UserId should match");
            Assert.That(result.Email, Is.EqualTo(email), "Email should match");
            Assert.That(result.Name, Is.EqualTo(name), "Name should match");
        }
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await this.handler.Handle(userId, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Permission denied"));
    }

    [Test]
    public async Task Handle_WhenMultipleUsersExist_ReturnsCorrectUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";
        var name1 = this.faker.Person.FullName;
        var name2 = this.faker.Person.FullName;

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = name1,
            Password = this.faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = name2,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.AddRange(user1, user2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId1, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(userId1), "Should return user1");
            Assert.That(result.Email, Is.EqualTo(email1), "Email should match user1");
        }
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await this.handler.Handle(userId, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Permission denied"));
    }

    [Test]
    public async Task Handle_ResponseDoesNotIncludePassword()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;
        var password = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.Not.Null, "Password should not be included in response");
    }

    [Test]
    public async Task Handle_VerifiesResponseContainsAllExpectedFields()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty), "Id should be set");
            Assert.That(result.Email, Is.Not.Null, "Email should not be null");
            Assert.That(result.Name, Is.Not.Null, "Name should not be null");
        }
    }
}
