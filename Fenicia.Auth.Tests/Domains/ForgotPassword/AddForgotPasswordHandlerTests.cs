using Bogus;

using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Auth.Domains.ForgotPassword.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class AddForgotPasswordHandlerTests : IDisposable
{
    public AddForgotPasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        this.handler = new AddForgotPasswordHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly AddForgotPasswordHandler handler;
    private readonly Faker faker;

    [Fact]
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

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var forgotPassword = await this.db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId);
        Assert.NotNull(forgotPassword);
        Assert.Equal(6,
            forgotPassword.Code.Length);
        Assert.True(forgotPassword.IsActive);
        Assert.Equal(userId,
            forgotPassword.UserId);
        Assert.True(forgotPassword.ExpirationDate > DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command,
                CancellationToken.None)
        );
        Assert.Equal("User with given email does not exist.",
            ex.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(upperCaseEmail);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command,
                CancellationToken.None)
        );
        Assert.Equal("User with given email does not exist.",
            ex.Message);
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_CreatesCodeForCorrectUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = this.faker.Internet.Email();
        var email2 = this.faker.Internet.Email();

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

        this.db.AuthUsers.AddRange(user1,
            user2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email1);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var forgotPassword = await this.db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId1);
        Assert.NotNull(forgotPassword);
        Assert.Equal(userId1,
            forgotPassword.UserId);
        Assert.Equal(6,
            forgotPassword.Code.Length);

        var forgotPasswordForUser2 =
            await this.db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId2);
        Assert.Null(forgotPasswordForUser2);
    }

    [Fact]
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

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var codes = await this.db.AuthForgottenPasswords.Where(fp => fp.UserId == userId).ToListAsync();
        Assert.Equal(2,
            codes.Count);
        Assert.True(codes.All(c => c.IsActive));
        Assert.True(codes.All(c => c.Code.Length == 6));
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command,
                CancellationToken.None)
        );
        Assert.Equal("User with given email does not exist.",
            ex.Message);
    }

    [Fact]
    public async Task Handle_VerifiesCodeIsUnique()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = this.faker.Internet.Email();
        var email2 = this.faker.Internet.Email();

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

        this.db.AuthUsers.AddRange(user1,
            user2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command1 = new AddForgotPasswordCommand(email1);
        var command2 = new AddForgotPasswordCommand(email2);

        // Act
        await this.handler.Handle(command1,
            CancellationToken.None);
        await this.handler.Handle(command2,
            CancellationToken.None);

        // Assert
        var codes = await this.db.AuthForgottenPasswords.ToListAsync();
        var distinctCodes = codes.Select(c => c.Code).Distinct().ToList();
        Assert.Equal(2,
            distinctCodes.Count);
    }
}
