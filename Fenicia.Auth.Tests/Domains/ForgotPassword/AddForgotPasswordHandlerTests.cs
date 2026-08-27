using Bogus;

using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.ForgotPassword.DTOs.Commands;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class AddForgotPasswordServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ForgotPasswordService service;

    public AddForgotPasswordServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        var userService = new UserService(db);
        service = new ForgotPasswordService(db, userService);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_CreatesForgotPasswordCodeSuccessfully()
    {

        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        await service.AddAsync(command, CancellationToken.None);

        var forgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId);
        Assert.NotNull(forgotPassword);
        Assert.Equal(6, forgotPassword.Code.Length);
        Assert.True(forgotPassword.IsActive);
        Assert.Equal(userId, forgotPassword.UserId);
        Assert.True(forgotPassword.ExpirationDate > DateTime.UtcNow);
        Assert.Null(forgotPassword.IpAddress);
        Assert.Null(forgotPassword.UserAgent);
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {

        var email = faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await service.AddAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ThrowsItemNotExistsException()
    {

        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(upperCaseEmail);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await service.AddAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_CreatesCodeForCorrectUser()
    {

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = faker.Internet.Email();
        var email2 = faker.Internet.Email();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.AddRange(user1, user2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email1);

        await service.AddAsync(command, CancellationToken.None);

        var forgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId1);
        Assert.NotNull(forgotPassword);
        Assert.Equal(userId1, forgotPassword.UserId);
        Assert.Equal(6, forgotPassword.Code.Length);

        var forgotPasswordForUser2 = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId2);
        Assert.Null(forgotPasswordForUser2);
    }

    [Fact]
    public async Task Handle_WhenCalledMultipleTimesForSameUser_CreatesMultipleCodes()
    {

        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        await service.AddAsync(command, CancellationToken.None);
        await service.AddAsync(command, CancellationToken.None);

        var codes = await db.AuthForgottenPasswords.Where(fp => fp.UserId == userId).ToListAsync();
        Assert.Equal(2, codes.Count);
        Assert.True(codes.All(c => c.IsActive));
        Assert.True(codes.All(c => c.Code.Length == 6));
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {

        var email = faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await service.AddAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task Handle_VerifiesCodeIsUnique()
    {

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = faker.Internet.Email();
        var email2 = faker.Internet.Email();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.AddRange(user1, user2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command1 = new AddForgotPasswordCommand(email1);
        var command2 = new AddForgotPasswordCommand(email2);

        await service.AddAsync(command1, CancellationToken.None);
        await service.AddAsync(command2, CancellationToken.None);

        var codes = await db.AuthForgottenPasswords.ToListAsync();
        var distinctCodes = codes.Select(c => c.Code).Distinct().ToList();
        Assert.Equal(2, distinctCodes.Count);
    }

    [Fact]
    public async Task Handle_WhenIpAddressAndUserAgentProvided_StoresThemCorrectly()
    {

        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0 (Test Browser)";

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email, ipAddress, userAgent);

        await service.AddAsync(command, CancellationToken.None);

        var forgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId);
        Assert.NotNull(forgotPassword);
        Assert.Equal(ipAddress, forgotPassword.IpAddress);
        Assert.Equal(userAgent, forgotPassword.UserAgent);
    }
}
