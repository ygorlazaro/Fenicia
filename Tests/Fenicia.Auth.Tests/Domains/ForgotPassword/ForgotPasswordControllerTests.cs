using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.DTOs;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
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
    private readonly ForgotPasswordController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public ForgotPasswordControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());

        var mockHttpContext = new Mock<HttpContext>();
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        var userRoleService = new UserRoleService(_userRoleRepository);
        var roleService = new RoleService(_roleRepository);
        var companyService = new CompanyService(_companyRepository, userRoleService);
        var moduleRepository = new ModuleRepository(_db);
        var subscriptionRepository = new SubscriptionRepository(_db);
        var subscriptionService = new SubscriptionService(subscriptionRepository, new UserService(new UserRepository(_db), userRoleService, roleService, companyService, new SecurityService()), userRoleService);
        var moduleService = new ModuleService(moduleRepository, userRoleService, subscriptionService);
        var userService = new UserService(_userRepository, userRoleService, roleService, companyService, new SecurityService());
        var forgotPasswordRepository = new ForgotPasswordRepository(_db);
        var forgotPasswordService = new ForgotPasswordService(forgotPasswordRepository, userService, new SecurityService());
        _controller = new ForgotPasswordController(forgotPasswordService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task PostAsync_WhenUserExists_ReturnsCreated()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        var result = await _controller.PostAsync(command, wide, cancellationToken);

        Assert.IsType<CreatedResult>(result);
        Assert.Equal(command.Email, wide.UserId);

        var forgotPasswordRecord = await _db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == user.Id, cancellationToken);
        Assert.NotNull(forgotPasswordRecord);
        Assert.True(forgotPasswordRecord.IsActive);
        Assert.NotNull(forgotPasswordRecord.Code);
        Assert.NotEmpty(forgotPasswordRecord.Code);
    }

    [Fact]
    public async Task PostAsync_WhenUserDoesNotExist_ReturnsBadRequest()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var command = new AddForgotPasswordCommand(_faker.Internet.Email());

        var result = await _controller.PostAsync(command, wide, cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        await _controller.PostAsync(command, wide, cancellationToken);

        Assert.Equal(command.Email, wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenValidCode_ResetsPasswordSuccessfully()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var code = _faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.AuthForgottenPasswords.Add(forgotPassword);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        var result = await _controller.PatchAsync(command, wide, cancellationToken);

        Assert.IsType<NoContentResult>(result);

        Assert.Equal(command.Email, wide.UserId);

        var updatedUser = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
        Assert.NotNull(updatedUser);

        var updatedForgotPassword = await _db.AuthForgottenPasswords.FirstOrDefaultAsync(f => f.Id == forgotPassword.Id, cancellationToken);
        Assert.NotNull(updatedForgotPassword);
        Assert.False(updatedForgotPassword.IsActive);
    }

    [Fact]
    public async Task PatchAsync_WhenInvalidCode_ReturnsBadRequest()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, _faker.Internet.Password(), "INVALID");

        var result = await _controller.PatchAsync(command, wide, cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PatchAsync_WhenUserDoesNotExist_ReturnsBadRequest()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var command = new ResetPasswordCommand(_faker.Internet.Email(), _faker.Internet.Password(), _faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));

        var result = await _controller.PatchAsync(command, wide, cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var code = _faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.AuthForgottenPasswords.Add(forgotPassword);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        await _controller.PatchAsync(command, wide, cancellationToken);

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
