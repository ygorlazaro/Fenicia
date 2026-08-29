using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.Register.DTOs;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Register;

public class RegisterControllerTests : IDisposable
{
    private readonly Guid _adminRoleId;
    private readonly RegisterController _controller;
    private readonly DefaultContext _db;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public RegisterControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        var userRoleService = new UserRoleService(_userRoleRepository);
        var roleService = new RoleService(_roleRepository);
        var companyService = new CompanyService(_companyRepository, userRoleService);
        var moduleRepository = new ModuleRepository(_db);
        var moduleService = new ModuleService(moduleRepository);
        var userService = new UserService(_userRepository, userRoleService, roleService, companyService, new SecurityService(), moduleService);
        var registerService = new RegisterService(userService);

        var mockHttpContext = new Mock<HttpContext>();
        _controller = new RegisterController(registerService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        _adminRoleId = Guid.NewGuid();
        SeedAdminRoleAsync().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var wide = new Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("existing@example.com", "password123", "Test User", company);

        await _userRepository.InsertAsync(new UserModel { Email = request.Email, Name = "Existing User", Password = "password" }, CancellationToken.None);
        _db.SaveChanges();

        var result = await _controller.CreateNewUserAsync(request, wide, ct);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        var wide = new Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Existing Company");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        await _companyRepository.InsertAsync(new CompanyModel { Cnpj = company.Cnpj, Name = "Existing Company" }, CancellationToken.None);
        _db.SaveChanges();

        var result = await _controller.CreateNewUserAsync(request, wide, ct);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenAdminRoleDoesNotExist_ReturnsBadRequest()
    {
        var wide = new Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        _db.AuthRoles.Remove(_db.AuthRoles.First());
        _db.SaveChanges();

        var result = await _controller.CreateNewUserAsync(request, wide, ct);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenValidRequest_ReturnsCreatedWithUser()
    {
        var wide = new Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        var result = await _controller.CreateNewUserAsync(request, wide, ct);

        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        var response = Assert.IsType<RegisterResponse>(createdResult.Value);

        Assert.Equal(request.Email, response.Email);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(company.Name, response.Company.Name);
        Assert.Equal(request.Email, wide.UserId);

        var createdUser = await _db.AuthUsers.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        Assert.NotNull(createdUser);
        Assert.NotEqual(request.Password, createdUser.Password);

        var createdCompany = await _db.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == company.Cnpj, ct);
        Assert.NotNull(createdCompany);

        var userRole = await _db.AuthUserRoles.FirstOrDefaultAsync(ur => ur.UserId == createdUser.Id, ct);
        Assert.NotNull(userRole);
        Assert.Equal(_adminRoleId, userRole.RoleId);
    }

    [Fact]
    public async Task CreateNewUserAsync_SetsWideEventContextUserId()
    {
        var wide = new Common.API.WideEventContext();
        var ct = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        await _controller.CreateNewUserAsync(request, wide, ct);

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

    private async Task SeedAdminRoleAsync()
    {
        _roleRepository.InsertAsync(new RoleModel { Id = _adminRoleId, Name = "Admin" }, CancellationToken.None).GetAwaiter().GetResult();
        _db.SaveChanges();
    }
}
