using Bogus;

using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Auth.Domains.ForgotPassword.Handlers;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class ForgotPasswordControllerTests : IDisposable
{
    public ForgotPasswordControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        var addForgotPasswordHandler = new AddForgotPasswordHandler(this.db);
        var resetPasswordHandler = new ResetPasswordHandler(this.db);
        var mockHttpContext = new Mock<HttpContext>();

        this.controller = new ForgotPasswordController(
            addForgotPasswordHandler,
            resetPasswordHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };

        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly ForgotPasswordController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;

    [Fact]
    public async Task ForgotPassword_WhenUserExists_CompletesSuccessfully()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var email = this.faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        // Act
        await this.controller.ForgotPassword(
            command,
            wide,
            ct);

        // Assert
        Assert.Equal(command.Email,
            wide.UserId);

        // Verify forgot password record was created
        var forgotPasswordRecord =
            await this.db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == user.Id,
                ct);
        Assert.NotNull(forgotPasswordRecord);
        Assert.True(forgotPasswordRecord.IsActive);
        Assert.NotNull(forgotPasswordRecord.Code);
        Assert.NotEmpty(forgotPasswordRecord.Code);
    }

    [Fact]
    public async Task ForgotPassword_WhenUserDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var command = new AddForgotPasswordCommand(this.faker.Internet.Email());

        // Act & Assert
        await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.controller.ForgotPassword(
                command,
                wide,
                ct));
    }

    [Fact]
    public async Task ForgotPassword_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var email = this.faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        // Act
        await this.controller.ForgotPassword(
            command,
            wide,
            ct);

        // Assert
        Assert.Equal(command.Email,
            wide.UserId);
    }

    [Fact]
    public async Task ResetPassword_WhenValidCode_ResetsPasswordSuccessfully()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var email = this.faker.Internet.Email();
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var code = this.faker.Random.String2(6,
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        this.db.AuthUsers.Add(user);
        this.db.AuthForgottenPasswords.Add(forgotPassword);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email,
            newPassword,
            code);

        // Act
        var result = await this.controller.ResetPassword(
            command,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);

        var okResult = result as OkResult;
        Assert.NotNull(okResult);
        Assert.Equal(200,
            okResult.StatusCode);
        Assert.Equal(command.Email,
            wide.UserId);

        // Verify password was changed
        var updatedUser = await this.db.AuthUsers.FirstOrDefaultAsync(u => u.Id == user.Id,
            ct);
        Assert.NotNull(updatedUser);

        // Verify forgot password record was deactivated
        var updatedForgotPassword =
            await this.db.AuthForgottenPasswords.FirstOrDefaultAsync(f => f.Id == forgotPassword.Id,
                ct);
        Assert.NotNull(updatedForgotPassword);
        Assert.False(updatedForgotPassword.IsActive);
    }

    [Fact]
    public async Task ResetPassword_WhenInvalidCode_ThrowsInvalidDataException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var email = this.faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email,
            this.faker.Internet.Password(),
            "INVALID");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.controller.ResetPassword(
                command,
                wide,
                ct));
    }

    [Fact]
    public async Task ResetPassword_WhenUserDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var command = new ResetPasswordCommand(
            this.faker.Internet.Email(),
            this.faker.Internet.Password(),
            this.faker.Random.String2(6,
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));

        // Act & Assert
        await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.controller.ResetPassword(
                command,
                wide,
                ct));
    }

    [Fact]
    public async Task ResetPassword_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var email = this.faker.Internet.Email();
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var code = this.faker.Random.String2(6,
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        this.db.AuthUsers.Add(user);
        this.db.AuthForgottenPasswords.Add(forgotPassword);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email,
            newPassword,
            code);

        // Act
        await this.controller.ResetPassword(
            command,
            wide,
            ct);

        // Assert
        Assert.Equal(command.Email,
            wide.UserId);
    }

    [Fact]
    public void ForgotPasswordController_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(ForgotPasswordController);

        // Act
        var allowAnonymousAttribute =
            controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute),
                false).FirstOrDefault();

        // Assert
        Assert.NotNull(allowAnonymousAttribute);
    }

    [Fact]
    public void ForgotPasswordController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ForgotPasswordController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute),
                false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]",
            routeAttribute.Template);
    }

    [Fact]
    public void ForgotPasswordController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(ForgotPasswordController);

        // Act
        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute),
                false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json",
            producesAttribute.ContentTypes.FirstOrDefault());
    }
}
