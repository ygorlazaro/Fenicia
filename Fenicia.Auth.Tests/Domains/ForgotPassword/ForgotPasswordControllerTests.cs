using Fenicia.Auth.Domains.ForgotPassword.AddForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.ResetPassword;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.ChangePassword;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

[TestFixture]
public class ForgotPasswordControllerTests
{
    private Auth.Domains.ForgotPassword.ForgotPasswordController controller = null!;
    private AuthContext context = null!;
    private ForgotPasswordHandler forgotPasswordHandler = null!;
    private ResetPasswordHandler resetPasswordHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Mock<HashPasswordHandler> mockHashPasswordHandler = null!;
    private ChangePasswordHandler changePasswordHandler = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.mockHashPasswordHandler = new Mock<HashPasswordHandler>();
        this.changePasswordHandler = new ChangePasswordHandler(this.context, this.mockHashPasswordHandler.Object);
        this.forgotPasswordHandler = new ForgotPasswordHandler(this.context);
        this.resetPasswordHandler = new ResetPasswordHandler(this.context, this.changePasswordHandler);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new Auth.Domains.ForgotPassword.ForgotPasswordController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    #region ForgotPassword Tests

    [Test]
    public async Task ForgotPassword_WhenUserExists_CompletesSuccessfully()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ForgotPasswordCommand("test@example.com");

        // Act
        await this.controller.ForgotPassword(
            command,
            this.forgotPasswordHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(command.Email));

        // Verify forgot password record was created
        var forgotPasswordRecord = await this.context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == user.Id, cancellationToken: cancellationToken);
        Assert.That(forgotPasswordRecord, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(forgotPasswordRecord!.IsActive, Is.True);
            Assert.That(forgotPasswordRecord.Code, Is.Not.Null.And.Not.Empty);
        }
    }

    [Test]
    public void ForgotPassword_WhenUserDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var command = new ForgotPasswordCommand("nonexistent@example.com");

        // Act & Assert
        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.controller.ForgotPassword(
                command,
                this.forgotPasswordHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public async Task ForgotPassword_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ForgotPasswordCommand("test@example.com");

        // Act
        await this.controller.ForgotPassword(
            command,
            this.forgotPasswordHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(command.Email));
    }

    #endregion

    #region ResetPassword Tests

    [Test]
    public async Task ResetPassword_WhenValidCode_ResetsPasswordSuccessfully()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            Password = "oldHashedPassword"
        };

        var code = "ABC123";

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        this.context.Users.Add(user);
        this.context.ForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.mockHashPasswordHandler
            .Setup(h => h.Handle("newPassword123"))
            .Returns("newHashedPassword");

        var command = new ResetPasswordCommand("test@example.com", "newPassword123", code);

        // Act
        var result = await this.controller.ResetPassword(
            command,
            this.resetPasswordHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<OkResult>());

        var okResult = result as OkResult;
        Assert.That(okResult, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(wide.UserId, Is.EqualTo(command.Email));
        }

        // Verify password was changed
        var updatedUser = await this.context.Users.FindAsync(user.Id, cancellationToken);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser!.Password, Is.EqualTo("newHashedPassword"));

        // Verify forgot password record was deactivated
        var updatedForgotPassword = await this.context.ForgottenPasswords.FindAsync(forgotPassword.Id, cancellationToken);
        Assert.That(updatedForgotPassword, Is.Not.Null);
        Assert.That(updatedForgotPassword!.IsActive, Is.False);
    }

    [Test]
    public async Task ResetPassword_WhenInvalidCode_ThrowsItemNotExistsException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand("test@example.com", "newPassword123", "INVALID");

        // Act & Assert
        Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.controller.ResetPassword(
                command,
                this.resetPasswordHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public void ResetPassword_WhenUserDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var command = new ResetPasswordCommand("nonexistent@example.com", "newPassword123", "ABC123");

        // Act & Assert
        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.controller.ResetPassword(
                command,
                this.resetPasswordHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public async Task ResetPassword_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        var code = "ABC123";

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        this.context.Users.Add(user);
        this.context.ForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.mockHashPasswordHandler
            .Setup(h => h.Handle("newPassword123"))
            .Returns("newHashedPassword");

        var command = new ResetPasswordCommand("test@example.com", "newPassword123", code);

        // Act
        await this.controller.ResetPassword(
            command,
            this.resetPasswordHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(command.Email));
    }

    #endregion

    #region Attribute Tests

    [Test]
    public void ForgotPasswordController_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.ForgotPassword.ForgotPasswordController);

        // Act
        var allowAnonymousAttribute = controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(allowAnonymousAttribute, Is.Not.Null, "ForgotPasswordController should have AllowAnonymous attribute");
    }

    [Test]
    public void ForgotPasswordController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.ForgotPassword.ForgotPasswordController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ForgotPasswordController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void ForgotPasswordController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.ForgotPassword.ForgotPasswordController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.That(producesAttribute, Is.Not.Null, "ForgotPasswordController should have Produces attribute");
        Assert.That(producesAttribute!.ContentTypes.FirstOrDefault(), Is.EqualTo("application/json"));
    }

    #endregion
}
