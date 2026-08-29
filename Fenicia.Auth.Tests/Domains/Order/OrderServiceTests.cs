using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Order.DTOs;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Order;

public class OrderServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly OrderService _service;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly UserRoleService _userRoleService;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRoleRepository = new UserRoleRepository(_db);
        _userRoleService = new UserRoleService(_userRoleRepository);
        var moduleRepository = new ModuleRepository(_db);
        var moduleService = new ModuleService(moduleRepository);
        var orderRepository = new OrderRepository(_db);
        var subscriptionRepository = new SubscriptionRepository(_db);
        var userRepository = new UserRepository(_db);
        var userRoleRepository = new UserRoleRepository(_db);
        var roleRepository = new RoleRepository(_db);
        var companyRepository = new CompanyRepository(_db);
        var userService = new UserService(userRepository, userRoleRepository, roleRepository, companyRepository, new SecurityService());
        var subscriptionService = new SubscriptionService(subscriptionRepository, userService);
        _service = new OrderService(moduleService, orderRepository, subscriptionService, _userRoleService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesOrderSuccessfully()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var module2Id = Guid.NewGuid();
        var modules = new List<Guid> { module1Id, module2Id };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        var module1 = new ModuleModel
        {
            Id = module1Id,
            Name = nameof(ModuleType.CustomerSupport),
            Type = ModuleType.CustomerSupport,
            Price = 100.00m
        };

        var module2 = new ModuleModel
        {
            Id = module2Id,
            Name = nameof(ModuleType.Pos),
            Type = ModuleType.Pos,
            Price = 150.00m
        };

        var moduleBasic = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = nameof(ModuleType.Basic),
            Type = ModuleType.Basic,
            Price = 150.00m
        };

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.AuthModules.AddRange(module1, module2, moduleBasic);
        _db.SaveChanges();

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await _service.CreateAsync(command, CancellationToken.None);

        Assert.NotNull(result);

        var order = await _db.AuthOrders.Include(orderModel => orderModel.Details).FirstOrDefaultAsync(o => o.Id == result.OrderId);
        Assert.NotNull(order);

        Assert.Equal(userId, order.UserId);
        Assert.Equal(companyId, order.CompanyId);
        Assert.Equal(OrderStatus.Approved, order.Status);
        Assert.Equal(400.00m, order.TotalAmount);
        Assert.Equal(3, order.Details.Count());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExistInCompany_ThrowsPermissionDeniedException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<Guid> { Guid.NewGuid() };

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.CreateAsync(command, CancellationToken.None));
        Assert.Equal("User does not exists at the company", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenModulesNotFound_ThrowsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var nonExistentModuleId = Guid.NewGuid();
        var modules = new List<Guid> { nonExistentModuleId };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.SaveChanges();

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.CreateAsync(command, CancellationToken.None));
        Assert.Equal("Modules not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenNoModulesRequested_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<Guid>();

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.SaveChanges();

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.CreateAsync(command, CancellationToken.None));

        Assert.Equal("Modules not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenModuleIsBasicType_DoesNotAddAnotherBasic()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var modules = new List<Guid> { basicModuleId };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        var basicModule = new ModuleModel
        {
            Id = basicModuleId,
            Name = nameof(ModuleType.Basic),
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.AuthModules.Add(basicModule);
        _db.SaveChanges();

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await _service.CreateAsync(command, CancellationToken.None);

        var order = await _db.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.NotNull(order);
        Assert.Single(order.Details);
    }

    [Fact]
    public async Task Handle_WhenModuleIsNotBasic_AddsBasicModuleAutomatically()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var accountingModuleId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var modules = new List<Guid> { accountingModuleId };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        var accountingModule = new ModuleModel
        {
            Id = accountingModuleId,
            Name = nameof(ModuleType.Accounting),
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var basicModule = new ModuleModel
        {
            Id = basicModuleId,
            Name = nameof(ModuleType.Basic),
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.AuthModules.AddRange(accountingModule, basicModule);
        _db.SaveChanges();

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await _service.CreateAsync(command, CancellationToken.None);

        var order = await _db.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.NotNull(order);
        Assert.Equal(2, order.Details.Count());
    }

    [Fact]
    public async Task Handle_WhenBasicModuleNotFound_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var accountingModuleId = Guid.NewGuid();
        var modules = new List<Guid> { accountingModuleId };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        var accountingModule = new ModuleModel
        {
            Id = accountingModuleId,
            Name = nameof(ModuleType.Accounting),
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.AuthModules.Add(accountingModule);
        _db.SaveChanges();

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.CreateAsync(command, CancellationToken.None));

        Assert.Equal("Modules not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenDuplicateModuleIds_RemovesDuplicates()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var modules = new List<Guid> { module1Id, module1Id, module1Id };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        var module1 = new ModuleModel
        {
            Id = module1Id,
            Name = nameof(ModuleType.Accounting),
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = nameof(ModuleType.Basic),
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthUserRoles.Add(userRole);
        _db.AuthModules.AddRange(module1, basicModule);
        _db.SaveChanges();

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await _service.CreateAsync(command, CancellationToken.None);

        var order = await _db.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.NotNull(order);
        Assert.Equal(2, order.Details.Count());
    }
}
