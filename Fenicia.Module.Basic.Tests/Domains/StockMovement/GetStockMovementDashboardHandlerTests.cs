using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement.GetStockMovementDashboard;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

[TestFixture]
public class GetStockMovementDashboardHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetStockMovementDashboardHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetStockMovementDashboardHandler handler = null!;

    [Test]
    public async Task Handle_WithNoMovements_ReturnsEmptyDashboard()
    {
        // Arrange
        var query = new GetStockMovementDashboardQuery(30, 10);
        var ct = CancellationToken.None;

        // Act
        var result = await this.handler.Handle(query, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.History, Is.Empty);
        Assert.That(result.MonthlyInOut, Is.Empty);
        Assert.That(result.TopMovedProducts, Is.Empty);
        Assert.That(result.TurnoverRates, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMovements_ReturnsStockMovementHistory()
    {
        // Arrange
        var product = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new BasicProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var movement = new BasicStockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 10,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 15.00m,
            Type = StockMovementType.In,
            Reason = "Test reason"
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        this.context.BasicStockMovements.Add(movement);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(30, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(1));
        Assert.That(result.History[0].ProductName, Is.EqualTo("Test Product"));
        Assert.That(result.History[0].Quantity, Is.EqualTo(10));
        Assert.That(result.History[0].Reason, Is.EqualTo("Test reason"));
    }

    [Test]
    public async Task Handle_WithInAndOutMovements_ReturnsMonthlyInOut()
    {
        // Arrange
        var product = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new BasicProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var now = DateTime.UtcNow;
        var movementIn = new BasicStockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 50,
            Date = now,
            Price = 100.00m,
            Type = StockMovementType.In
        };

        var movementOut = new BasicStockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 20,
            Date = now,
            Price = 50.00m,
            Type = StockMovementType.Out
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        this.context.BasicStockMovements.AddRange(movementIn, movementOut);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(30, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.MonthlyInOut, Is.Not.Empty);
        
        var monthlyInOut = result.MonthlyInOut[0];
        Assert.That(monthlyInOut.TotalIn, Is.EqualTo(50));
        Assert.That(monthlyInOut.TotalOut, Is.EqualTo(20));
        Assert.That(monthlyInOut.TotalInValue, Is.EqualTo(100.00m));
        Assert.That(monthlyInOut.TotalOutValue, Is.EqualTo(50.00m));
    }

    [Test]
    public async Task Handle_WithMultipleMovements_ReturnsTopMovedProducts()
    {
        // Arrange
        var product1 = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 1",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var product2 = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 2",
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 80,
            CategoryId = Guid.NewGuid()
        };

        var category1 = new BasicProductCategoryModel
        {
            Id = product1.CategoryId,
            Name = "Category 1"
        };

        var category2 = new BasicProductCategoryModel
        {
            Id = product2.CategoryId,
            Name = "Category 2"
        };

        // More movements for product1
        for (var i = 0; i < 5; i++)
        {
            this.context.BasicStockMovements.Add(new BasicStockMovementModel
            {
                Id = Guid.NewGuid(),
                ProductId = product1.Id,
                Product = product1,
                Quantity = 10,
                Date = DateTime.UtcNow.AddDays(-i),
                Price = 20.00m,
                Type = StockMovementType.Out
            });
        }

        // Fewer movements for product2
        for (var i = 0; i < 2; i++)
        {
            this.context.BasicStockMovements.Add(new BasicStockMovementModel
            {
                Id = Guid.NewGuid(),
                ProductId = product2.Id,
                Product = product2,
                Quantity = 5,
                Date = DateTime.UtcNow.AddDays(-i),
                Price = 15.00m,
                Type = StockMovementType.Out
            });
        }

        this.context.BasicProductCategories.AddRange(category1, category2);
        this.context.BasicProducts.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(30, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TopMovedProducts, Has.Count.EqualTo(2));
        Assert.That(result.TopMovedProducts[0].ProductName, Is.EqualTo("Product 1"));
        Assert.That(result.TopMovedProducts[0].TotalMoved, Is.EqualTo(50)); // 5 * 10
        Assert.That(result.TopMovedProducts[1].ProductName, Is.EqualTo("Product 2"));
        Assert.That(result.TopMovedProducts[1].TotalMoved, Is.EqualTo(10)); // 2 * 5
    }

    [Test]
    public async Task Handle_WithProducts_ReturnsTurnoverRates()
    {
        // Arrange
        var product = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 50, // Current stock
            CategoryId = Guid.NewGuid()
        };

        var category = new BasicProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Add out movements (sales)
        for (var i = 0; i < 5; i++)
        {
            this.context.BasicStockMovements.Add(new BasicStockMovementModel
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Product = product,
                Quantity = 20, // Total sold = 100
                Date = DateTime.UtcNow.AddDays(-i),
                Price = 20.00m,
                Type = StockMovementType.Out
            });
        }

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(30, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.ProductName, Is.EqualTo("Test Product"));
        Assert.That(turnover.CurrentStock, Is.EqualTo(50));
        Assert.That(turnover.TotalSold, Is.EqualTo(100)); // 5 * 20
        Assert.That(turnover.TurnoverRate, Is.EqualTo(2.0)); // 100 / 50
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("High"));
    }

    [Test]
    public async Task Handle_WithLowTurnover_ReturnsLowClassification()
    {
        // Arrange
        var product = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Slow Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100, // High current stock
            CategoryId = Guid.NewGuid()
        };

        var category = new BasicProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Small out movement
        this.context.BasicStockMovements.Add(new BasicStockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 10, // Low sales
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 15.00m,
            Type = StockMovementType.Out
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(30, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.TurnoverRate, Is.LessThan(0.5));
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("Very Low"));
    }

    [Test]
    public async Task Handle_WithDateRangeFilter_OnlyIncludesMovementsInRange()
    {
        // Arrange
        var product = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new BasicProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var now = DateTime.UtcNow;
        
        // Movement within range (5 days ago)
        var movementInRange = new BasicStockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 10,
            Date = now.AddDays(-5),
            Price = 15.00m,
            Type = StockMovementType.In
        };

        // Movement outside range (50 days ago)
        var movementOutOfRange = new BasicStockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 20,
            Date = now.AddDays(-50),
            Price = 25.00m,
            Type = StockMovementType.In
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        this.context.BasicStockMovements.AddRange(movementInRange, movementOutOfRange);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(30, 10); // Last 30 days

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(1));
        Assert.That(result.History[0].Id, Is.EqualTo(movementInRange.Id));
    }
}
