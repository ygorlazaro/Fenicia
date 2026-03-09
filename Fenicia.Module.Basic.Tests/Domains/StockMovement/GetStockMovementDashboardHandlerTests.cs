using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
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
        var query = new GetStockMovementDashboardQuery();
        var ct = CancellationToken.None;

        // Act
        var result = await this.handler.Handle(query, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.History, Is.Empty);
            Assert.That(result.MonthlyInOut, Is.Empty);
            Assert.That(result.TopMovedProducts, Is.Empty);
            Assert.That(result.TurnoverRates, Is.Empty);
        }
    }

    [Test]
    public async Task Handle_WithMovements_ReturnsStockMovementHistory()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var movement = new StockMovementModel
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

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.History[0].ProductName, Is.EqualTo("Test Product"));
            Assert.That(result.History[0].Quantity, Is.EqualTo(10));
            Assert.That(result.History[0].Reason, Is.EqualTo("Test reason"));
        }
    }

    [Test]
    public async Task Handle_WithInAndOutMovements_ReturnsMonthlyInOut()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var now = DateTime.UtcNow;
        var movementIn = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 50,
            Date = now,
            Price = 100.00m,
            Type = StockMovementType.In
        };

        var movementOut = new StockMovementModel
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

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.MonthlyInOut, Is.Not.Empty);

        var monthlyInOut = result.MonthlyInOut[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(monthlyInOut.TotalIn, Is.EqualTo(50));
            Assert.That(monthlyInOut.TotalOut, Is.EqualTo(20));
            Assert.That(monthlyInOut.TotalInValue, Is.EqualTo(100.00m));
            Assert.That(monthlyInOut.TotalOutValue, Is.EqualTo(50.00m));
        }
    }

    [Test]
    public async Task Handle_WithMultipleMovements_ReturnsTopMovedProducts()
    {
        // Arrange
        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 1",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 2",
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 80,
            CategoryId = Guid.NewGuid()
        };

        var category1 = new ProductCategoryModel
        {
            Id = product1.CategoryId,
            Name = "Category 1"
        };

        var category2 = new ProductCategoryModel
        {
            Id = product2.CategoryId,
            Name = "Category 2"
        };

        // More movements for product1
        for (var i = 0; i < 5; i++)
        {
            this.context.BasicStockMovements.Add(new StockMovementModel
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
            this.context.BasicStockMovements.Add(new StockMovementModel
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

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TopMovedProducts, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.TopMovedProducts[0].ProductName, Is.EqualTo("Product 1"));
            Assert.That(result.TopMovedProducts[0].TotalMoved, Is.EqualTo(50)); // 5 * 10
            Assert.That(result.TopMovedProducts[1].ProductName, Is.EqualTo("Product 2"));
            Assert.That(result.TopMovedProducts[1].TotalMoved, Is.EqualTo(10)); // 2 * 5
        }
    }

    [Test]
    public async Task Handle_WithProducts_ReturnsTurnoverRates()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 50, // Current stock
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Add out movements (sales)
        for (var i = 0; i < 5; i++)
        {
            this.context.BasicStockMovements.Add(new StockMovementModel
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

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TurnoverRates, Is.Not.Empty);

        var turnover = result.TurnoverRates[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(turnover.ProductName, Is.EqualTo("Test Product"));
            Assert.That(turnover.CurrentStock, Is.EqualTo(50));
            Assert.That(turnover.TotalSold, Is.EqualTo(100)); // 5 * 20
            Assert.That(turnover.TurnoverRate, Is.EqualTo(2.0)); // 100 / 50
            Assert.That(turnover.TurnoverClassification, Is.EqualTo("High"));
        }
    }

    [Test]
    public async Task Handle_WithLowTurnover_ReturnsLowClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Slow Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100, // High current stock
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Small out movement
        this.context.BasicStockMovements.Add(new StockMovementModel
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

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TurnoverRates, Is.Not.Empty);

        var turnover = result.TurnoverRates[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(turnover.TurnoverRate, Is.LessThan(0.5));
            Assert.That(turnover.TurnoverClassification, Is.EqualTo("Very Low"));
        }
    }

    [Test]
    public async Task Handle_WithDateRangeFilter_OnlyIncludesMovementsInRange()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var now = DateTime.UtcNow;

        // Movement within range (5 days ago)
        var movementInRange = new StockMovementModel
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
        var movementOutOfRange = new StockMovementModel
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

        var query = new GetStockMovementDashboardQuery(); // Last 30 days

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(1));
        Assert.That(result.History[0].Id, Is.EqualTo(movementInRange.Id));
    }

    #region Turnover Classification Tests - GlassifyTurnover

    [Test]
    public async Task Handle_TurnoverRate_Exactly2_ReturnsHighClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "High Turnover Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 50,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Total sold = 100, Current stock = 50, Turnover = 2.0 (exactly)
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 100,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.TurnoverRate, Is.EqualTo(2.0));
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("High"));
    }

    [Test]
    public async Task Handle_TurnoverRate_GreaterThan2_ReturnsHighClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Very High Turnover Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 30,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Total sold = 150, Current stock = 30, Turnover = 5.0
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 150,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.TurnoverRate, Is.EqualTo(5.0));
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("High"));
    }

    [Test]
    public async Task Handle_TurnoverRate_Exactly1_ReturnsMediumClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Medium Turnover Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Total sold = 100, Current stock = 100, Turnover = 1.0 (exactly)
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 100,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.TurnoverRate, Is.EqualTo(1.0));
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("Medium"));
    }

    [Test]
    public async Task Handle_TurnoverRate_Between1And2_ReturnsMediumClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Medium-High Turnover Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Total sold = 150, Current stock = 100, Turnover = 1.5
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 150,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.TurnoverRate, Is.EqualTo(1.5));
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("Medium"));
    }

    [Test]
    public async Task Handle_TurnoverRate_Exactly05_ReturnsLowClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Low Turnover Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Total sold = 50, Current stock = 100, Turnover = 0.5 (exactly)
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 50,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.TurnoverRate, Is.EqualTo(0.5));
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("Low"));
    }

    [Test]
    public async Task Handle_TurnoverRate_Between05And1_ReturnsLowClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Low-Medium Turnover Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Total sold = 75, Current stock = 100, Turnover = 0.75
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 75,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.TurnoverRate, Is.EqualTo(0.75));
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("Low"));
    }

    [Test]
    public async Task Handle_TurnoverRate_LessThan05_ReturnsVeryLowClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Very Low Turnover Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // Total sold = 25, Current stock = 100, Turnover = 0.25
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 25,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.TurnoverRate, Is.EqualTo(0.25));
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("Very Low"));
    }

    [Test]
    public async Task Handle_TurnoverRate_Zero_ReturnsVeryLowClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "No Sales Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        // No out movements, Turnover = 0
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 10,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.In
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.TurnoverRates, Is.Not.Empty);
        var turnover = result.TurnoverRates[0];
        Assert.That(turnover.TurnoverRate, Is.EqualTo(0));
        Assert.That(turnover.TurnoverClassification, Is.EqualTo("Very Low"));
    }

    [Test]
    public async Task Handle_ProductWithZeroStock_IsExcludedFromTurnover()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Zero Stock Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 0, // Zero stock
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.TurnoverRates, Is.Empty);
    }

    #endregion

    #region Customer and Supplier Tests

    [Test]
    public async Task Handle_WithCustomerMovement_ReturnsCustomerNameInHistory()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = "John Doe"
        };

        var customer = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = person.Id,
            Person = person
        };

        var movement = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            CustomerId = customer.Id,
            Customer = customer,
            Quantity = 10,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 15.00m,
            Type = StockMovementType.Out,
            Reason = "Sale"
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        this.context.BasicPeople.Add(person);
        this.context.BasicCustomers.Add(customer);
        this.context.BasicStockMovements.Add(movement);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(1));
        Assert.That(result.History[0].CustomerName, Is.EqualTo("John Doe"));
        Assert.That(result.History[0].SupplierName, Is.Null);
    }

    [Test]
    public async Task Handle_WithSupplierMovement_ReturnsSupplierNameInHistory()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = "ABC Supplier Ltd"
        };

        var supplier = new SupplierModel
        {
            Id = Guid.NewGuid(),
            PersonId = person.Id,
            Person = person
        };

        var movement = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            SupplierId = supplier.Id,
            Supplier = supplier,
            Quantity = 50,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 10.00m,
            Type = StockMovementType.In,
            Reason = "Purchase"
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        this.context.BasicPeople.Add(person);
        this.context.BasicSuppliers.Add(supplier);
        this.context.BasicStockMovements.Add(movement);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(1));
        Assert.That(result.History[0].SupplierName, Is.EqualTo("ABC Supplier Ltd"));
        Assert.That(result.History[0].CustomerName, Is.Null);
    }

    #endregion

    #region TopLimit Tests

    [Test]
    public async Task Handle_WithTopLimitLimit_ReturnsLimitedResults()
    {
        // Arrange
        var products = new List<ProductModel>();
        var categories = new List<ProductCategoryModel>();

        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Category"
        };
        categories.Add(category);

        // Create 5 products
        for (var i = 0; i < 5; i++)
        {
            var product = new ProductModel
            {
                Id = Guid.NewGuid(),
                Name = $"Product {i + 1}",
                CostPrice = 10.00m,
                SalesPrice = 20.00m,
                Quantity = 100,
                CategoryId = category.Id
            };
            products.Add(product);

            // Add out movements with different quantities
            this.context.BasicStockMovements.Add(new StockMovementModel
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Product = product,
                Quantity = (i + 1) * 10, // 10, 20, 30, 40, 50
                Date = DateTime.UtcNow.AddDays(-i),
                Price = 20.00m,
                Type = StockMovementType.Out
            });
        }

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.AddRange(products);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(TopLimit: 3);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TopMovedProducts, Has.Count.EqualTo(3));
        Assert.That(result.TurnoverRates, Has.Count.EqualTo(3));

        // Verify top 3 products by movement
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.TopMovedProducts[0].ProductName, Is.EqualTo("Product 5"));
            Assert.That(result.TopMovedProducts[1].ProductName, Is.EqualTo("Product 4"));
            Assert.That(result.TopMovedProducts[2].ProductName, Is.EqualTo("Product 3"));
        }
    }

    #endregion

    #region Days Filter Tests

    [Test]
    public async Task Handle_WithCustomDaysFilter_OnlyIncludesMovementsInCustomRange()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var now = DateTime.UtcNow;

        // Movement within 7 days (3 days ago)
        var movementWithin7Days = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 10,
            Date = now.AddDays(-3),
            Price = 15.00m,
            Type = StockMovementType.In
        };

        // Movement outside 7 days (10 days ago)
        var movementOutside7Days = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 20,
            Date = now.AddDays(-10),
            Price = 25.00m,
            Type = StockMovementType.In
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        this.context.BasicStockMovements.AddRange(movementWithin7Days, movementOutside7Days);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(Days: 7);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(1));
        Assert.That(result.History[0].Id, Is.EqualTo(movementWithin7Days.Id));
    }

    #endregion

    #region History Ordering Tests

    [Test]
    public async Task Handle_WithMultipleMovements_ReturnsHistoryOrderedByDateDescending()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var now = DateTime.UtcNow;

        var movement1 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 10,
            Date = now.AddDays(-10),
            Price = 15.00m,
            Type = StockMovementType.In
        };

        var movement2 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 20,
            Date = now.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        };

        var movement3 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 15,
            Date = now.AddDays(-1),
            Price = 25.00m,
            Type = StockMovementType.In
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        this.context.BasicStockMovements.AddRange(movement1, movement2, movement3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.History, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.History[0].Id, Is.EqualTo(movement3.Id)); // Most recent
            Assert.That(result.History[1].Id, Is.EqualTo(movement2.Id));
            Assert.That(result.History[2].Id, Is.EqualTo(movement1.Id)); // Oldest
        }
    }

    #endregion

    #region Monthly InOut Grouping Tests

    [Test]
    public async Task Handle_WithMovementsAcrossMultipleMonths_GroupsMonthlyInOutCorrectly()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var january = new DateTime(DateTime.UtcNow.Year, 1, 15);
        var february = new DateTime(DateTime.UtcNow.Year, 2, 15);

        // January movements
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 50,
            Date = january,
            Price = 100.00m,
            Type = StockMovementType.In
        });

        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 20,
            Date = january.AddDays(5),
            Price = 50.00m,
            Type = StockMovementType.Out
        });

        // February movements
        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 60,
            Date = february,
            Price = 120.00m,
            Type = StockMovementType.In
        });

        this.context.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 30,
            Date = february.AddDays(5),
            Price = 75.00m,
            Type = StockMovementType.Out
        });

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(Days: 60);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.MonthlyInOut, Has.Count.EqualTo(2));

        var januaryData = result.MonthlyInOut.First(m => m.Month.StartsWith("01/"));
        var februaryData = result.MonthlyInOut.First(m => m.Month.StartsWith("02/"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(januaryData.TotalIn, Is.EqualTo(50));
            Assert.That(januaryData.TotalOut, Is.EqualTo(20));
            Assert.That(februaryData.TotalIn, Is.EqualTo(60));
            Assert.That(februaryData.TotalOut, Is.EqualTo(30));
        }
    }

    #endregion
}
