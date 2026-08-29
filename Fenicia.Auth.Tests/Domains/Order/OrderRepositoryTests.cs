using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Order;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Order;

public class OrderRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new OrderRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task InsertAsync_WhenOrderIsValid_InsertsSuccessfully()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            OrderNumber = "AO-20240101-ABC12345",
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = userId,
            TotalAmount = 100.00m,
            CompanyId = companyId
        };

        var result = await _repository.InsertAsync(order, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal("AO-20240101-ABC12345", result.OrderNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderModel
        {
            Id = orderId,
            OrderNumber = "AO-20240101-TEST1234",
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = Guid.NewGuid(),
            TotalAmount = 100.00m,
            CompanyId = Guid.NewGuid()
        };

        _db.AuthOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(orderId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveOrders()
    {
        var order1 = new OrderModel
        {
            Id = Guid.NewGuid(),
            OrderNumber = "AO-20240101-ACTIVE01",
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = Guid.NewGuid(),
            TotalAmount = 100.00m,
            CompanyId = Guid.NewGuid()
        };

        var order2 = new OrderModel
        {
            Id = Guid.NewGuid(),
            OrderNumber = "AO-20240101-DELETED01",
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = Guid.NewGuid(),
            TotalAmount = 200.00m,
            CompanyId = Guid.NewGuid(),
            Deleted = DateTime.UtcNow
        };

        _db.AuthOrders.AddRange(order1, order2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var results = await _repository.GetAllAsync(1, 10, CancellationToken.None);

        Assert.Single(results);
        Assert.Equal(order1.Id, results.First().Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderExists_UpdatesSuccessfully()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderModel
        {
            Id = orderId,
            OrderNumber = "AO-20240101-ORIGINAL",
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = Guid.NewGuid(),
            TotalAmount = 100.00m,
            CompanyId = Guid.NewGuid()
        };

        _db.AuthOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var updatedOrder = new OrderModel
        {
            OrderNumber = "AO-20240101-UPDATED",
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Cancelled,
            UserId = order.UserId,
            TotalAmount = 200.00m,
            CompanyId = order.CompanyId
        };

        var result = await _repository.UpdateAsync(orderId, updatedOrder, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("AO-20240101-UPDATED", result.OrderNumber);
        Assert.Equal(OrderStatus.Cancelled, result.Status);
        Assert.Equal(200.00m, result.TotalAmount);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        var updatedOrder = new OrderModel
        {
            OrderNumber = "AO-20240101-UPDATED",
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = Guid.NewGuid(),
            TotalAmount = 100.00m,
            CompanyId = Guid.NewGuid()
        };

        var result = await _repository.UpdateAsync(Guid.NewGuid(), updatedOrder, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_SoftDeletesSuccessfully()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderModel
        {
            Id = orderId,
            OrderNumber = "AO-20240101-DELETE01",
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Approved,
            UserId = Guid.NewGuid(),
            TotalAmount = 100.00m,
            CompanyId = Guid.NewGuid()
        };

        _db.AuthOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(orderId, CancellationToken.None);

        Assert.Equal(1, result);

        var deletedOrder = await _repository.GetByIdAsync(orderId, CancellationToken.None);
        Assert.NotNull(deletedOrder);
        Assert.NotNull(deletedOrder.Deleted);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderDoesNotExist_ReturnsZero()
    {
        var result = await _repository.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(0, result);
    }
}
