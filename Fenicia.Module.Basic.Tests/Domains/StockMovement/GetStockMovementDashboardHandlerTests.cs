using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement.GetStockMovementDashboard;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class GetStockMovementDashboardHandlerTests : IDisposable
{
    public GetStockMovementDashboardHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, this.companyContext);
        this.handler = new GetStockMovementDashboardHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly TestCompanyContext companyContext;
    private readonly DefaultContext db;
    private readonly GetStockMovementDashboardHandler handler;

    [Fact]
    public async Task Handle_WithNoMovements_ReturnsEmptyDashboard()
    {
        // Arrange
        var query = new GetStockMovementDashboardQuery();
        var ct = CancellationToken.None;

        // Act
        var result = await this.handler.Handle(query, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.History);
        Assert.Empty(result.MonthlyInOut);
        Assert.Empty(result.TopMovedProducts);
        Assert.Empty(result.TurnoverRates);
    }

    [Fact]
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

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        this.db.BasicStockMovements.Add(movement);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.History);
        Assert.Equal("Test Product", result.History[0].ProductName);
        Assert.Equal(10, result.History[0].Quantity);
        Assert.Equal("Test reason", result.History[0].Reason);
    }

    [Fact]
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

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        this.db.BasicStockMovements.AddRange(movementIn, movementOut);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.MonthlyInOut);

        var monthlyInOut = result.MonthlyInOut[0];
        Assert.Equal(50, monthlyInOut.TotalIn);
        Assert.Equal(20, monthlyInOut.TotalOut);
        Assert.Equal(100.00m, monthlyInOut.TotalInValue);
        Assert.Equal(50.00m, monthlyInOut.TotalOutValue);
    }

    [Fact]
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
            this.db.BasicStockMovements.Add(new StockMovementModel
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
            this.db.BasicStockMovements.Add(new StockMovementModel
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

        this.db.BasicProductCategories.AddRange(category1, category2);
        this.db.BasicProducts.AddRange(product1, product2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TopMovedProducts.Count);
        Assert.Equal("Product 1", result.TopMovedProducts[0].ProductName);
        Assert.Equal(50, result.TopMovedProducts[0].TotalMoved);
        Assert.Equal("Product 2", result.TopMovedProducts[1].ProductName);
        Assert.Equal(10, result.TopMovedProducts[1].TotalMoved);
    }

    [Fact]
    public async Task Handle_WithProducts_ReturnsTurnoverRates()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
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

        for (var i = 0; i < 5; i++)
        {
            this.db.BasicStockMovements.Add(new StockMovementModel
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Product = product,
                Quantity = 20,
                Date = DateTime.UtcNow.AddDays(-i),
                Price = 20.00m,
                Type = StockMovementType.Out
            });
        }

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.TurnoverRates);

        var turnover = result.TurnoverRates[0];
        Assert.Equal("Test Product", turnover.ProductName);
        Assert.Equal(50, turnover.CurrentStock);
        Assert.Equal(100, turnover.TotalSold);
        Assert.Equal(2.0, turnover.TurnoverRate);
        Assert.Equal("High", turnover.TurnoverClassification);
    }

    [Fact]
    public async Task Handle_WithLowTurnover_ReturnsLowClassification()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Slow Product",
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 10,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 15.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.TurnoverRates);

        var turnover = result.TurnoverRates[0];
        Assert.True(turnover.TurnoverRate < 0.5);
        Assert.Equal("Very Low", turnover.TurnoverClassification);
    }

    [Fact]
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

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        this.db.BasicStockMovements.AddRange(movementInRange, movementOutOfRange);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.History);
        Assert.Equal(movementInRange.Id, result.History[0].Id);
    }

    #region Turnover Classification Tests - GlassifyTurnover

    [Fact]
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 100,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result.TurnoverRates);
        var turnover = result.TurnoverRates[0];
        Assert.Equal(2.0, turnover.TurnoverRate);
        Assert.Equal("High", turnover.TurnoverClassification);
    }

    [Fact]
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 150,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result.TurnoverRates);
        var turnover = result.TurnoverRates[0];
        Assert.Equal(5.0, turnover.TurnoverRate);
        Assert.Equal("High", turnover.TurnoverClassification);
    }

    [Fact]
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 100,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result.TurnoverRates);
        var turnover = result.TurnoverRates[0];
        Assert.Equal(1.0, turnover.TurnoverRate);
        Assert.Equal("Medium", turnover.TurnoverClassification);
    }

    [Fact]
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 150,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result.TurnoverRates);
        var turnover = result.TurnoverRates[0];
        Assert.Equal(1.5, turnover.TurnoverRate);
        Assert.Equal("Medium", turnover.TurnoverClassification);
    }

    [Fact]
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 50,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result.TurnoverRates);
        var turnover = result.TurnoverRates[0];
        Assert.Equal(0.5, turnover.TurnoverRate);
        Assert.Equal("Low", turnover.TurnoverClassification);
    }

    [Fact]
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 75,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result.TurnoverRates);
        var turnover = result.TurnoverRates[0];
        Assert.Equal(0.75, turnover.TurnoverRate);
        Assert.Equal("Low", turnover.TurnoverClassification);
    }

    [Fact]
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 25,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result.TurnoverRates);
        var turnover = result.TurnoverRates[0];
        Assert.Equal(0.25, turnover.TurnoverRate);
        Assert.Equal("Very Low", turnover.TurnoverClassification);
    }

    [Fact]
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 10,
            Date = DateTime.UtcNow.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.In
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result.TurnoverRates);
        var turnover = result.TurnoverRates[0];
        Assert.Equal(0, turnover.TurnoverRate);
        Assert.Equal("Very Low", turnover.TurnoverClassification);
    }

    [Fact]
    public async Task Handle_ProductWithZeroStock_IsExcludedFromTurnover()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Zero Stock Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 0,
            CategoryId = Guid.NewGuid()
        };

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.TurnoverRates);
    }

    #endregion

    #region Customer and Supplier Tests

    [Fact]
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

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        this.db.BasicPeople.Add(person);
        this.db.BasicCustomers.Add(customer);
        this.db.BasicStockMovements.Add(movement);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.History);
        Assert.Equal("John Doe", result.History[0].CustomerName);
        Assert.Null(result.History[0].SupplierName);
    }

    [Fact]
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

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        this.db.BasicPeople.Add(person);
        this.db.BasicSuppliers.Add(supplier);
        this.db.BasicStockMovements.Add(movement);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.History);
        Assert.Equal("ABC Supplier Ltd", result.History[0].SupplierName);
        Assert.Null(result.History[0].CustomerName);
    }

    #endregion

    #region TopLimit Tests

    [Fact]
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

            this.db.BasicStockMovements.Add(new StockMovementModel
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Product = product,
                Quantity = (i + 1) * 10,
                Date = DateTime.UtcNow.AddDays(-i),
                Price = 20.00m,
                Type = StockMovementType.Out
            });
        }

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.AddRange(products);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(TopLimit: 3);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TopMovedProducts.Count);
        Assert.Equal(3, result.TurnoverRates.Count);
        Assert.Equal("Product 5", result.TopMovedProducts[0].ProductName);
        Assert.Equal("Product 4", result.TopMovedProducts[1].ProductName);
        Assert.Equal("Product 3", result.TopMovedProducts[2].ProductName);
    }

    #endregion

    #region Days Filter Tests

    [Fact]
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

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        this.db.BasicStockMovements.AddRange(movementWithin7Days, movementOutside7Days);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(Days: 7);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.History);
        Assert.Equal(movementWithin7Days.Id, result.History[0].Id);
    }

    #endregion

    #region History Ordering Tests

    [Fact]
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

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        this.db.BasicStockMovements.AddRange(movement1, movement2, movement3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.History.Count);
        Assert.Equal(movement3.Id, result.History[0].Id);
        Assert.Equal(movement2.Id, result.History[1].Id);
        Assert.Equal(movement1.Id, result.History[2].Id);
    }

    #endregion

    #region Monthly InOut Grouping Tests

    [Fact]
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

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 50,
            Date = january,
            Price = 100.00m,
            Type = StockMovementType.In
        });

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 20,
            Date = january.AddDays(5),
            Price = 50.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 60,
            Date = february,
            Price = 120.00m,
            Type = StockMovementType.In
        });

        this.db.BasicStockMovements.Add(new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = 30,
            Date = february.AddDays(5),
            Price = 75.00m,
            Type = StockMovementType.Out
        });

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetStockMovementDashboardQuery(Days: 60);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.MonthlyInOut.Count);

        var januaryData = result.MonthlyInOut.First(m => m.Month.StartsWith("01/"));
        var februaryData = result.MonthlyInOut.First(m => m.Month.StartsWith("02/"));

        Assert.Equal(50, januaryData.TotalIn);
        Assert.Equal(20, januaryData.TotalOut);
        Assert.Equal(60, februaryData.TotalIn);
        Assert.Equal(30, februaryData.TotalOut);
    }

    #endregion
}
