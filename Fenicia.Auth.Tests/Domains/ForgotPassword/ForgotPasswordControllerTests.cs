using Bogus;

using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class ForgotPasswordControllerTests : IDisposable
{
    private readonly ForgotPasswordController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;

    public ForgotPasswordControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());

        var mockHttpContext = new Mock<HttpContext>();

        var forgotPasswordService = new ForgotPasswordService(db);
        controller = new ForgotPasswordController(forgotPasswordService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task ForgotPassword_WhenUserExists_CompletesSuccessfully()
    {

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

        await controller.ForgotPassword(command, wide, ct);

        Assert.Equal(command.Email, wide.UserId);

        var forgotPasswordRecord = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == user.Id, ct);
        Assert.NotNull(forgotPasswordRecord);
        Assert.True(forgotPasswordRecord.IsActive);
        Assert.NotNull(forgotPasswordRecord.Code);
        Assert.NotEmpty(forgotPasswordRecord.Code);
    }

    [Fact]
    public async Task ForgotPassword_WhenUserDoesNotExist_ReturnsBadRequest()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var command = new AddForgotPasswordCommand(faker.Internet.Email());

        var result = await controller.ForgotPassword(command, wide, ct);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ForgotPassword_SetsWideEventContextUserId()
    {

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

        await controller.ForgotPassword(command, wide, ct);

        Assert.Equal(command.Email, wide.UserId);
    }

    [Fact]
    public async Task ResetPassword_WhenValidCode_ResetsPasswordSuccessfully()
    {

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

        var result = await controller.ResetPassword(command, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result);

        var okResult = result as CreatedResult;
        Assert.NotNull(okResult);
        Assert.Equal(201, okResult.StatusCode);
        Assert.Equal(command.Email, wide.UserId);

        var updatedUser = await db.AuthUsers.FirstOrDefaultAsync(u => u.Id == user.Id, ct);
        Assert.NotNull(updatedUser);

        var updatedForgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(f => f.Id == forgotPassword.Id, ct);
        Assert.NotNull(updatedForgotPassword);
        Assert.False(updatedForgotPassword.IsActive);
    }

    [Fact]
    public async Task ResetPassword_WhenInvalidCode_ThrowsInvalidDataException()
    {

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

        var result = await controller.ResetPassword(command, wide, ct);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ResetPassword_WhenUserDoesNotExist_ReturnsBadRequest()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var command = new ResetPasswordCommand(faker.Internet.Email(), faker.Internet.Password(), faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));

        var result = await controller.ResetPassword(command, wide, ct);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ResetPassword_SetsWideEventContextUserId()
    {

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

        await controller.ResetPassword(command, wide, ct);

        Assert.Equal(command.Email, wide.UserId);
    }

    [Fact]
    public void ForgotPasswordController_HasAllowAnonymousAttribute()
    {

        var controllerType = typeof(ForgotPasswordController);

        var allowAnonymousAttribute = controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        Assert.NotNull(allowAnonymousAttribute);
    }

    [Fact]
    public void ForgotPasswordController_HasRouteAttribute()
    {

        var controllerType = typeof(ForgotPasswordController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ForgotPasswordController_HasProducesAttribute()
    {

        var controllerType = typeof(ForgotPasswordController);

        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }
}
