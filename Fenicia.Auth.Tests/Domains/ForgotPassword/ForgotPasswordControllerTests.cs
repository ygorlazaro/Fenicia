using Bogus;

using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Auth.Domains.ForgotPassword.Handlers;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

/// <summary>
///     Unit tests for the ForgotPasswordController.
///     Tests HTTP endpoints behavior including forgot password initiation and password reset.
/// </summary>
public class ForgotPasswordControllerTests : IDisposable
{
    private readonly ForgotPasswordController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;

    public ForgotPasswordControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        var addForgotPasswordHandler = new AddForgotPasswordHandler(db);
        var resetPasswordHandler = new ResetPasswordHandler(db);
        var mockHttpContext = new Mock<HttpContext>();

        controller = new ForgotPasswordController(addForgotPasswordHandler, resetPasswordHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Tests that when a user exists, the forgot password process completes successfully.
    /// </summary>
    [Fact]
    public async Task ForgotPassword_WhenUserExists_CompletesSuccessfully()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
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

        var command = new AddForgotPasswordCommand(email);

        // Act
        await controller.ForgotPassword(command, wide, ct);

        // Assert
        Assert.Equal(command.Email, wide.UserId);

        // Verify forgot password record was created
        var forgotPasswordRecord = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == user.Id, ct);
        Assert.NotNull(forgotPasswordRecord);
        Assert.True(forgotPasswordRecord.IsActive);
        Assert.NotNull(forgotPasswordRecord.Code);
        Assert.NotEmpty(forgotPasswordRecord.Code);
    }

    /// <summary>
    ///     Tests that when no user exists with the given email, BadRequest is returned.
    /// </summary>
    [Fact]
    public async Task ForgotPassword_WhenUserDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var command = new AddForgotPasswordCommand(faker.Internet.Email());

        // Act
        var result = await controller.ForgotPassword(command, wide, ct);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    ///     Tests that the WideEventContext UserId is set to the email for tracking.
    /// </summary>
    [Fact]
    public async Task ForgotPassword_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
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

        var command = new AddForgotPasswordCommand(email);

        // Act
        await controller.ForgotPassword(command, wide, ct);

        // Assert
        Assert.Equal(command.Email, wide.UserId);
    }

    /// <summary>
    ///     Tests that a valid code successfully resets the user's password.
    /// </summary>
    [Fact]
    public async Task ResetPassword_WhenValidCode_ResetsPasswordSuccessfully()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var email = faker.Internet.Email();
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var code = faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        db.AuthUsers.Add(user);
        db.AuthForgottenPasswords.Add(forgotPassword);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act
        var result = await controller.ResetPassword(command, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result);

        var okResult = result as CreatedResult;
        Assert.NotNull(okResult);
        Assert.Equal(201, okResult.StatusCode);
        Assert.Equal(command.Email, wide.UserId);

        // Verify password was changed
        var updatedUser = await db.AuthUsers.FirstOrDefaultAsync(u => u.Id == user.Id, ct);
        Assert.NotNull(updatedUser);

        // Verify forgot password record was deactivated
        var updatedForgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(f => f.Id == forgotPassword.Id, ct);
        Assert.NotNull(updatedForgotPassword);
        Assert.False(updatedForgotPassword.IsActive);
    }

    /// <summary>
    ///     Tests that an invalid code throws InvalidDataException.
    /// </summary>
    [Fact]
    public async Task ResetPassword_WhenInvalidCode_ThrowsInvalidDataException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
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

        var command = new ResetPasswordCommand(email, faker.Internet.Password(), "INVALID");

        // Act
        var result = await controller.ResetPassword(command, wide, ct);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    ///     Tests that when no user exists with the given email, BadRequest is returned.
    /// </summary>
    [Fact]
    public async Task ResetPassword_WhenUserDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var command = new ResetPasswordCommand(faker.Internet.Email(), faker.Internet.Password(), faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));

        // Act
        var result = await controller.ResetPassword(command, wide, ct);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    ///     Tests that the WideEventContext UserId is set to the email when resetting password.
    /// </summary>
    [Fact]
    public async Task ResetPassword_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var email = faker.Internet.Email();
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var code = faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        db.AuthUsers.Add(user);
        db.AuthForgottenPasswords.Add(forgotPassword);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act
        await controller.ResetPassword(command, wide, ct);

        // Assert
        Assert.Equal(command.Email, wide.UserId);
    }

    /// <summary>
    ///     Tests that the ForgotPasswordController has the AllowAnonymousAttribute applied.
    /// </summary>
    [Fact]
    public void ForgotPasswordController_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(ForgotPasswordController);

        // Act
        var allowAnonymousAttribute = controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(allowAnonymousAttribute);
    }

    /// <summary>
    ///     Tests that the ForgotPasswordController has the RouteAttribute with correct template.
    /// </summary>
    [Fact]
    public void ForgotPasswordController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ForgotPasswordController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    /// <summary>
    ///     Tests that the ForgotPasswordController has the ProducesAttribute with correct content type.
    /// </summary>
    [Fact]
    public void ForgotPasswordController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(ForgotPasswordController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }
}
