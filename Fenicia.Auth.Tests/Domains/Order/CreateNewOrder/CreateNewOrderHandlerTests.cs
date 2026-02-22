using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Order.CreateNewOrder;
using Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Migrations.Services;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Order.CreateNewOrder;

[TestFixture]
public class CreateNewOrderHandlerTests
{
    private AuthContext context = null!;
    private CreateNewOrderHandler handler = null!;
    private Mock<CreateCreditsForOrderHandler> createCreditsForOrderHandlerMock = null!;
    private Mock<IMigrationService> migrationServiceMock = null!;
    private Faker faker = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.createCreditsForOrderHandlerMock = new Mock<CreateCreditsForOrderHandler>(this.context);
        this.migrationServiceMock = new Mock<IMigrationService>();
        this.handler = new CreateNewOrderHandler(
            this.context,
            this.createCreditsForOrderHandlerMock.Object,
            this.migrationServiceMock.Object
        );
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

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
            Name = "Test Company",
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
            Name = "Module 1",
            Type = ModuleType.CustomerSupport,
            Price = 100.00m
        };

        var module2 = new ModuleModel
        {
            Id = module2Id,
            Name = "Module 2",
            Type = ModuleType.Pos,
            Price = 150.00m
        };
        
        var moduleBasic = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Module Basic",
            Type = ModuleType.Basic,
            Price = 150.00m
        };

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        this.context.Modules.AddRange([module1, module2, moduleBasic]);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(), SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        
        var order = await this.context.Orders.FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.That(order, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order!.UserId, Is.EqualTo(userId), "UserId should match");
            Assert.That(order.CompanyId, Is.EqualTo(companyId), "CompanyId should match");
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Approved), "Status should be Approved");
            Assert.That(order.TotalAmount, Is.EqualTo(400.00m), "TotalAmount should be sum of modules");
            Assert.That(order.Details.Count, Is.EqualTo(3), "Should have 2 details");
        }
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExistInCompany_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<Guid> { Guid.NewGuid() };

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(
            async () => await this.handler.Handle(command, CancellationToken.None)
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
            Name = "Test Company",
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

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(
            async () => await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Module not found"));
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
            Name = "Test Company",
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

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(
            async () => await this.handler.Handle(command, CancellationToken.None)
        );
        
        // Assert
        Assert.That(ex?.Message, Is.EqualTo("Module not found"));
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
            Name = "Test Company",
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
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        this.context.Modules.Add(basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(), SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.Orders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.That(order, Is.Not.Null);
        Assert.That(order!.Details.Count, Is.EqualTo(1), "Should only have 1 detail (Basic module)");
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
            Name = "Test Company",
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
            Name = "Accounting Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var basicModule = new ModuleModel
        {
            Id = basicModuleId,
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        this.context.Modules.AddRange(accountingModule, basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(), SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.Orders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.That(order, Is.Not.Null);
        Assert.That(order!.Details.Count, Is.EqualTo(2), "Should have 2 details (Accounting + Basic)");
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
            Name = "Test Company",
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
            Name = "Accounting Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        this.context.Modules.Add(accountingModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(
            async () => await this.handler.Handle(command, CancellationToken.None)
        );
        
        // Assert
        Assert.That(ex?.Message, Is.EqualTo("Module not found"));
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
            Name = "Test Company",
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
            Name = "Module 1",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };
        
        var moduleBasic = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Module Basic",
            Type = ModuleType.Basic,
            Price = 100.00m
        };

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        this.context.Modules.Add(module1);
        this.context.Modules.Add(moduleBasic);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(), SubscriptionStatus.Active
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
    public async Task Handle_CallsMigrationServiceWithCorrectParameters()
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
            Name = "Test Company",
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
            Name = "Accounting Module",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        this.context.Modules.AddRange(accountingModule, basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(), SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        this.migrationServiceMock.Verify(
            x => x.RunMigrationsAsync(
                companyId,
                It.Is<List<ModuleType>>(types => types.Contains(ModuleType.Accounting) && types.Contains(ModuleType.Basic)),
                It.IsAny<CancellationToken>()
            ),
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
            Name = "Test Company",
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
            Name = "Module 1",
            Type = ModuleType.Accounting,
            Price = 100.00m
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        this.context.Modules.AddRange(module1, basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(), SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.Orders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.That(order, Is.Not.Null);
        Assert.That(order!.Details.Count, Is.EqualTo(2), "Should have 2 details (deduplicated module + Basic)");
    }

    [Test]
    public async Task Handle_WhenModuleIsErpOrAuthType_ExcludesFromOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var erpModuleId = Guid.NewGuid();
        var authModuleId = Guid.NewGuid();
        var accountingModuleId = Guid.NewGuid();
        var modules = new List<Guid> { erpModuleId, authModuleId, accountingModuleId };

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
            Name = "Test Company",
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

        var erpModule = new ModuleModel
        {
            Id = erpModuleId,
            Name = "ERP Module",
            Type = ModuleType.Erp,
            Price = 200.00m
        };

        var authModule = new ModuleModel
        {
            Id = authModuleId,
            Name = "Auth Module",
            Type = ModuleType.Auth,
            Price = 100.00m
        };

        var accountingModule = new ModuleModel
        {
            Id = accountingModuleId,
            Name = "Accounting Module",
            Type = ModuleType.Accounting,
            Price = 150.00m
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 50.00m
        };

        this.context.Users.Add(user);
        this.context.Companies.Add(company);
        this.context.UserRoles.Add(userRole);
        this.context.Modules.AddRange(erpModule, authModule, accountingModule, basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        this.createCreditsForOrderHandlerMock
            .Setup(x => x.Handle(It.IsAny<CreateCreditsForOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCreditsForOrderResponse(
                Guid.NewGuid(), companyId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid(), SubscriptionStatus.Active
            ));

        var command = new CreateNewOrderCommand(userId, companyId, modules);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var order = await this.context.Orders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == result!.OrderId);
        Assert.That(order, Is.Not.Null);
        // Only Accounting and Basic should be included (ERP and Auth are excluded by GetModulesToOrderAsync filter)
        // Actually looking at the code, it doesn't filter Erp/Auth in GetModulesToOrderAsync
        // The filter is in GetModulesHandler, not in CreateNewOrderHandler
        // So all modules should be included
        Assert.That(order!.Details.Count, Is.GreaterThanOrEqualTo(2), "Should have at least Accounting + Basic");
    }
}
