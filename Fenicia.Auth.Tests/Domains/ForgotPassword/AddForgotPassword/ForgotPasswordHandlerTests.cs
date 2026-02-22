using Bogus;

using Fenicia.Auth.Domains.ForgotPassword.AddForgotPassword;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword.AddForgotPassword;

[TestFixture]
public class ForgotPasswordHandlerTests
{
    private AuthContext context = null!;
    private ForgotPasswordHandler handler = null!;
    private Faker faker = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new ForgotPasswordHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    [Test]
    public async Task Handle_WhenEmailExists_CreatesForgotPasswordCodeSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ForgotPasswordCommand(email);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var forgotPassword = await this.context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId);
        Assert.That(forgotPassword, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(forgotPassword!.Code, Has.Length.EqualTo(6), "Code should be 6 characters");
            Assert.That(forgotPassword.IsActive, Is.True, "Code should be active");
            Assert.That(forgotPassword.UserId, Is.EqualTo(userId), "UserId should match");
            Assert.That(forgotPassword.ExpirationDate, Is.GreaterThan(DateTime.UtcNow), "Should have future expiration");
        }
    }

    [Test]
    public async Task Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var command = new ForgotPasswordCommand(email);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(
            async () => await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Item not found"));
    }

    [Test]
    public async Task Handle_WhenEmailHasDifferentCase_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var upperCaseEmail = "TEST@EXAMPLE.COM";

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ForgotPasswordCommand(upperCaseEmail);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(
            async () => await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Item not found"));
    }

    [Test]
    public async Task Handle_WhenMultipleUsersExist_CreatesCodeForCorrectUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.AddRange(user1, user2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ForgotPasswordCommand(email1);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var forgotPassword = await this.context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId1);
        Assert.That(forgotPassword, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(forgotPassword!.UserId, Is.EqualTo(userId1), "Should create code for user1");
            Assert.That(forgotPassword.Code, Has.Length.EqualTo(6), "Code should be 6 characters");
        }

        var forgotPasswordForUser2 = await this.context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId2);
        Assert.That(forgotPasswordForUser2, Is.Null, "Should not create code for user2");
    }

    [Test]
    public async Task Handle_WhenCalledMultipleTimesForSameUser_CreatesMultipleCodes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ForgotPasswordCommand(email);

        // Act
        await this.handler.Handle(command, CancellationToken.None);
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var codes = await this.context.ForgottenPasswords.Where(fp => fp.UserId == userId).ToListAsync();
        Assert.That(codes.Count, Is.EqualTo(2), "Should create two codes");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(codes.All(c => c.IsActive), Is.True, "Both codes should be active");
            Assert.That(codes.All(c => c.Code.Length == 6), Is.True, "Both codes should be 6 characters");
        }
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var command = new ForgotPasswordCommand(email);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(
            async () => await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Item not found"));
    }

    [Test]
    public async Task Handle_VerifiesCodeIsUnique()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.AddRange(user1, user2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command1 = new ForgotPasswordCommand(email1);
        var command2 = new ForgotPasswordCommand(email2);

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var codes = await this.context.ForgottenPasswords.ToListAsync();
        var distinctCodes = codes.Select(c => c.Code).Distinct().ToList();
        Assert.That(distinctCodes.Count, Is.EqualTo(2), "Codes should be unique");
    }
}
