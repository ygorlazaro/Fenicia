using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Order.CreateNewOrder;
using Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Order;

[TestFixture]
public class CreateNewOrderHandlerTests
{
    [SetUp]
    public void SetUp()
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

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private DefaultContext context;
    private CreateNewOrderHandler handler;
    private Mock<CreateCreditsForOrderHandler> createCreditsForOrderHandlerMock;
    private Faker faker;

    [Test]
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);

        var order = await this.context.AuthOrders.Include(orderModel => orderModel.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.That(order, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order!.UserId, Is.EqualTo(userId), "UserId should match");
            Assert.That(order.CompanyId, Is.EqualTo(companyId), "CompanyId should match");
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Approved), "Status should be Approved");
            Assert.That(order.TotalAmount, Is.EqualTo(400.00m), "TotalAmount should be sum of modules");
            Assert.That(order.Details, Has.Count.EqualTo(3), "Should have 2 details");
        }
    }

    [Test]
    public void Handle_WhenUserDoesNotExistInCompany_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<Guid> { Guid.NewGuid() };

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("User does not exists at the company"));
    }

    [Test]
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Modules not found"));
    }

    [Test]
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );

        // Assert
        Assert.That(ex?.Message, Is.EqualTo("Modules not found"));
    }

    [Test]
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.That(order, Is.Not.Null);
        Assert.That(order!.Details, Has.Count.EqualTo(1), "Should only have 1 detail (Basic module)");
    }

    [Test]
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.That(order, Is.Not.Null);
        Assert.That(order!.Details, Has.Count.EqualTo(2), "Should have 2 details (Accounting + Basic)");
    }

    [Test]
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );

        // Assert
        Assert.That(ex?.Message, Is.EqualTo("Modules not found"));
    }

    [Test]
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        this.createCreditsForOrderHandlerMock.Verify(
            x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(),
                SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.AuthOrders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.That(order, Is.Not.Null);
        Assert.That(order!.Details, Has.Count.EqualTo(2), "Should have 2 details (deduplicated module + Basic)");
    }
}
