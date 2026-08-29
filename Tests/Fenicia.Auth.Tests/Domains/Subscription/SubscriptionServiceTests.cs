using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Subscription;

public class SubscriptionServiceTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SubscriptionService _service;

    public SubscriptionServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options, new TestCompanyContext());
        _context.Database.EnsureCreated();

        var userRepository = new UserRepository(_context);
        var userRoleRepository = new UserRoleRepository(_context);
        var roleRepository = new RoleRepository(_context);
        var companyRepository = new CompanyRepository(_context);
        var userRoleService = new UserRoleService(userRoleRepository);
        var roleService = new RoleService(roleRepository);
        var companyService = new CompanyService(companyRepository);
        var userService = new UserService(userRepository, userRoleService, roleService, companyService, new SecurityService());

        _service = new SubscriptionService(new SubscriptionRepository(_context), userService);
    }

    [Fact]
    public async Task GetUserProfileAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var nonExistentUserId = Guid.NewGuid();

        var result = await _service.GetUserProfileAsync(nonExistentUserId, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserProfileAsync_WhenUserExists_ReturnsProfileWithEmptyCollections()
    {
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            Password = new SecurityService().Hash("password123"),
            Created = DateTime.UtcNow
        };

        _context.AuthUsers.Add(user);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetUserProfileAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Test User", result.Name);
        Assert.Equal("test@example.com", result.Email);
        Assert.Empty(result.Companies);
        Assert.Empty(result.Subscriptions);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
