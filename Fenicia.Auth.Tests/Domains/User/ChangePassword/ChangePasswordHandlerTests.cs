using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.ChangePassword;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User.ChangePassword;

[TestFixture]
public class ChangePasswordHandlerTests
{
    private AuthContext context = null!;
    private ChangePasswordHandler handler = null!;
    private HashPasswordHandler hashPasswordHandler = null!;
    private Faker faker = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.hashPasswordHandler = new HashPasswordHandler();
        this.handler = new ChangePasswordHandler(this.context, this.hashPasswordHandler);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    [Test]
    public async Task Handle_WhenUserExists_ChangesPasswordSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();
        var oldPassword = "old_hashed_password";

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = oldPassword
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ChangePasswordQuery(userId, newPassword);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(userId), "UserId should match");
            Assert.That(result.Name, Is.EqualTo(user.Name), "Name should match");
            Assert.That(result.Email, Is.EqualTo(user.Email), "Email should match");
        }

        var updatedUser = await this.context.Users.FindAsync(userId);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser!.Password, Is.Not.EqualTo(oldPassword), "Password should be changed");
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();
        var query = new ChangePasswordQuery(userId, newPassword);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await this.handler.Handle(query, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Item not found"));
    }

    [Test]
    public async Task Handle_PasswordIsHashedBeforeSaving()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = "old_password"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ChangePasswordQuery(userId, newPassword);

        // Act
        await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var updatedUser = await this.context.Users.FindAsync(userId);
        Assert.That(updatedUser, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedUser!.Password, Is.Not.EqualTo(newPassword), "Password should be hashed");
            Assert.That(updatedUser.Password.Length, Is.GreaterThan(newPassword.Length), "Hashed password should be longer");
        }
    }

    [Test]
    public async Task Handle_VerifiesPasswordCanBeVerified()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = "old_password"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ChangePasswordQuery(userId, newPassword);

        // Act
        await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var updatedUser = await this.context.Users.FindAsync(userId);
        Assert.That(updatedUser, Is.Not.Null);
        
        var verifyHandler = new Fenicia.Auth.Domains.Security.VerifyPassword.VerifyPasswordHandler();
        var isValid = verifyHandler.Handle(newPassword, updatedUser!.Password);
        Assert.That(isValid, Is.True, "New password should be verifiable");
    }

    [Test]
    public async Task Handle_WhenMultipleUsersExist_OnlyUpdatesRequestedUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();
        var oldPassword1 = "old_password_1";
        var oldPassword2 = "old_password_2";

        var user1 = new UserModel
        {
            Id = userId1,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = oldPassword1
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = oldPassword2
        };

        this.context.Users.AddRange(user1, user2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ChangePasswordQuery(userId1, newPassword);

        // Act
        await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var updatedUser1 = await this.context.Users.FindAsync(userId1);
        var updatedUser2 = await this.context.Users.FindAsync(userId2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedUser1!.Password, Is.Not.EqualTo(oldPassword1), "User1 password should change");
            Assert.That(updatedUser2!.Password, Is.EqualTo(oldPassword2), "User2 password should not change");
        }
    }

    [Test]
    public async Task Handle_PreservesOtherUserProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = "old_password"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ChangePasswordQuery(userId, newPassword);

        // Act
        await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var updatedUser = await this.context.Users.FindAsync(userId);
        Assert.That(updatedUser, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedUser!.Email, Is.EqualTo(email), "Email should not change");
            Assert.That(updatedUser.Name, Is.EqualTo(name), "Name should not change");
            Assert.That(updatedUser.Id, Is.EqualTo(userId), "Id should not change");
        }
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();
        var query = new ChangePasswordQuery(userId, newPassword);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await this.handler.Handle(query, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Item not found"));
    }

    [Test]
    public async Task Handle_WhenPasswordIsEmpty_StillHashesAndSaves()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = string.Empty;

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = "old_password"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ChangePasswordQuery(userId, newPassword);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await this.handler.Handle(query, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Invalid password"));
    }

    [Test]
    public async Task Handle_ReturnsCorrectResponseData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = this.faker.Internet.Password();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = "old_password"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ChangePasswordQuery(userId, newPassword);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(userId), "Id should match");
            Assert.That(result.Name, Is.EqualTo(name), "Name should match");
            Assert.That(result.Email, Is.EqualTo(email), "Email should match");
        }
    }
}
