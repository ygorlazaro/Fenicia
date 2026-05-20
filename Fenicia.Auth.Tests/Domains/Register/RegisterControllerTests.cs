using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.Register.Command;
using Fenicia.Auth.Domains.Register.Handler;
using Fenicia.Auth.Domains.Register.Response;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Register;

public class RegisterControllerTests : IDisposable
{
    private readonly Guid adminRoleId;
    private readonly RegisterController controller;
    private readonly DefaultContext db;

    public RegisterControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        var createNewUserHandler = new CreateNewUserHandler(db);
        var registerHandler = new RegisterHandler(createNewUserHandler);
        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .Returns((RegisterCommand command, CancellationToken token) => registerHandler.Handle(command, token));

        var mockHttpContext = new Mock<HttpContext>();
        controller = new RegisterController(mockSender.Object) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        adminRoleId = Guid.NewGuid();
        SeedAdminRole();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SeedAdminRole()
    {
        db.AuthRoles.Add(new RoleModel { Id = adminRoleId, Name = "Admin" });
        db.SaveChanges();
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var wide = new Fenicia.Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("existing@example.com", "password123", "Test User", company);

        db.AuthUsers.Add(new UserModel { Email = request.Email, Name = "Existing User", Password = "password" });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.CreateNewUserAsync(request, wide, ct);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        var wide = new Fenicia.Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Existing Company");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        db.AuthCompanies.Add(new CompanyModel { Cnpj = company.Cnpj, Name = "Existing Company" });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.CreateNewUserAsync(request, wide, ct);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenAdminRoleDoesNotExist_ReturnsBadRequest()
    {
        var wide = new Fenicia.Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        db.AuthRoles.Remove(db.AuthRoles.First());
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.CreateNewUserAsync(request, wide, ct);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenValidRequest_ReturnsCreatedWithUser()
    {
        var wide = new Fenicia.Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        var result = await controller.CreateNewUserAsync(request, wide, ct);

        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        var response = Assert.IsType<RegisterResponse>(createdResult.Value);

        Assert.Equal(request.Email, response.Email);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(company.Name, response.Company.Name);
        Assert.Equal(request.Email, wide.UserId);

        var createdUser = await db.AuthUsers.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        Assert.NotNull(createdUser);
        Assert.NotEqual(request.Password, createdUser.Password);

        var createdCompany = await db.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == company.Cnpj, ct);
        Assert.NotNull(createdCompany);

        var userRole = await db.AuthUserRoles.FirstOrDefaultAsync(ur => ur.UserId == createdUser.Id, ct);
        Assert.NotNull(userRole);
        Assert.Equal(adminRoleId, userRole.RoleId);
    }

    [Fact]
    public async Task CreateNewUserAsync_SetsWideEventContextUserId()
    {
        var wide = new Fenicia.Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        await controller.CreateNewUserAsync(request, wide, ct);

        Assert.Equal(request.Email, wide.UserId);
    }

    [Fact]
    public void RegisterController_HasAllowAnonymousAttribute()
    {
        Assert.NotNull(typeof(RegisterController).GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault());
    }

    [Fact]
    public void RegisterController_HasRouteAttribute()
    {
        var route = typeof(RegisterController).GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;
        Assert.NotNull(route);
        Assert.Equal("[controller]", route.Template);
    }

    [Fact]
    public void RegisterController_HasApiControllerAttribute()
    {
        Assert.NotNull(typeof(RegisterController).GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault());
    }

    [Fact]
    public void RegisterController_HasProducesAttribute()
    {
        var produces = typeof(RegisterController).GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;
        Assert.NotNull(produces);
        Assert.Equal("application/json", produces.ContentTypes.FirstOrDefault());
    }
}
