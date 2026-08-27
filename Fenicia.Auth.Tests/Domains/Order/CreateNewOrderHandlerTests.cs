using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Order.Command;
using Fenicia.Auth.Domains.Order.Handler;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Order;

public class CreateNewOrderHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly CreateNewOrderHandler handler;

    public CreateNewOrderHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new CreateNewOrderHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

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
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        db.AuthModules.AddRange(module1, module2, moduleBasic);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);

        var order = await db.AuthOrders.Include(orderModel => orderModel.Details).FirstOrDefaultAsync(o => o.Id == result.OrderId);
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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
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
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
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
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));

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
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        db.AuthModules.Add(basicModule);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await handler.Handle(command, CancellationToken.None);

        var order = await db.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
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
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        db.AuthModules.AddRange(accountingModule, basicModule);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await handler.Handle(command, CancellationToken.None);

        var order = await db.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
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
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        db.AuthModules.Add(accountingModule);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));

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
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthUserRoles.Add(userRole);
        db.AuthModules.AddRange(module1, basicModule);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        var result = await handler.Handle(command, CancellationToken.None);

        var order = await db.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.NotNull(order);
        Assert.Equal(2, order.Details.Count());
    }
}
