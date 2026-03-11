using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Order.CreateNewOrder;
using Fenicia.Auth.Domains.Subscription.Commands;
using Fenicia.Auth.Domains.Subscription.Handlers;
using Fenicia.Auth.Domains.Subscription.Queries;
using Fenicia.Auth.Domains.Subscription.Responses;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Order;

public class CreateNewOrderHandlerTests : IDisposable
{
    private readonly DefaultContext context;
    private readonly CreateNewOrderHandler handler;
    private readonly Mock<CreateCreditsForOrderHandler> createCreditsForOrderHandlerMock;
    private readonly Faker faker;

    public CreateNewOrderHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.createCreditsForOrderHandlerMock = new Mock<CreateCreditsForOrderHandler>(this.context);
        this.handler = new CreateNewOrderHandler(
            this.context,
            this.createCreditsForOrderHandlerMock.Object
        );
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesOrderSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var module2Id = Guid.NewGuid();
        var modules = new List<Guid> { module1Id, module2Id };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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

        this.context.AuthUsers.Add(user);
        this.context.AuthCompanies.Add(company);
        this.context.AuthUserRoles.Add(userRole);
        this.context.AuthModules.AddRange(module1, module2, moduleBasic);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        var order = await this.context.AuthOrders.Include(orderModel => orderModel.Details).FirstOrDefaultAsync(o => o.Id == result.OrderId);
        Assert.NotNull(order);
        
        Assert.Equal(userId, order.UserId);
        Assert.Equal(companyId, order.CompanyId);
        Assert.Equal(OrderStatus.Approved, order.Status);
        Assert.Equal(400.00m, order.TotalAmount);
        Assert.Equal(3, order.Details.Count);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExistInCompany_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<Guid> { Guid.NewGuid() };

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(command, CancellationToken.None));
        Assert.Equal("User does not exists at the company", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenModulesNotFound_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var nonExistentModuleId = Guid.NewGuid();
        var modules = new List<Guid> { nonExistentModuleId };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.context.AuthUsers.Add(user);
        this.context.AuthCompanies.Add(company);
        this.context.AuthUserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None));
        Assert.Equal("Modules not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenNoModulesRequested_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<Guid>();

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = Guid.NewGuid()
        };

        this.context.AuthUsers.Add(user);
        this.context.AuthCompanies.Add(company);
        this.context.AuthUserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal("Modules not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenModuleIsBasicType_DoesNotAddAnotherBasic()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var modules = new List<Guid> { basicModuleId };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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

        this.context.AuthUsers.Add(user);
        this.context.AuthCompanies.Add(company);
        this.context.AuthUserRoles.Add(userRole);
        this.context.AuthModules.Add(basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.NotNull(order);
        Assert.Single(order.Details);
    }

    [Fact]
    public async Task Handle_WhenModuleIsNotBasic_AddsBasicModuleAutomatically()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var accountingModuleId = Guid.NewGuid();
        var basicModuleId = Guid.NewGuid();
        var modules = new List<Guid> { accountingModuleId };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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

        this.context.AuthUsers.Add(user);
        this.context.AuthCompanies.Add(company);
        this.context.AuthUserRoles.Add(userRole);
        this.context.AuthModules.AddRange(accountingModule, basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.NotNull(order);
        Assert.Equal(2, order.Details.Count);
    }

    [Fact]
    public async Task Handle_WhenBasicModuleNotFound_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var accountingModuleId = Guid.NewGuid();
        var modules = new List<Guid> { accountingModuleId };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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

        this.context.AuthUsers.Add(user);
        this.context.AuthCompanies.Add(company);
        this.context.AuthUserRoles.Add(userRole);
        this.context.AuthModules.Add(accountingModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal("Modules not found", ex.Message);
    }

    [Fact]
    public async Task Handle_CallsCreateCreditsForOrderHandler()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var modules = new List<Guid> { module1Id };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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

        var moduleBasic = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = nameof(ModuleType.Basic),
            Type = ModuleType.Basic,
            Price = 100.00m
        };

        this.context.AuthUsers.Add(user);
        this.context.AuthCompanies.Add(company);
        this.context.AuthUserRoles.Add(userRole);
        this.context.AuthModules.Add(module1);
        this.context.AuthModules.Add(moduleBasic);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        this.createCreditsForOrderHandlerMock.Verify(
            x => x.Handle(It.IsAny<CreateCreditsForOrderCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenDuplicateModuleIds_RemovesDuplicates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var modules = new List<Guid> { module1Id, module1Id, module1Id };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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

        this.context.AuthUsers.Add(user);
        this.context.AuthCompanies.Add(company);
        this.context.AuthUserRoles.Add(userRole);
        this.context.AuthModules.AddRange(module1, basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.NotNull(order);
        Assert.Equal(2, order.Details.Count);
    }
}
