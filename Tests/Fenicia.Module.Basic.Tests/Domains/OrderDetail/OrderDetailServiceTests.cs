using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.OrderDetail;

public class OrderDetailServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly OrderDetailService _service;

    public OrderDetailServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new OrderDetailRepository(_db);
        _service = new OrderDetailService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByOrderIdAsync_WhenDetailsExist_ReturnsDetailsWithProductName()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var detail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = orderId, ProductId = product.Id, Price = 100, Quantity = 2, Subtotal = 200 };
        _db.BasicOrderDetails.Add(detail);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByOrderIdAsync(new GetOrderDetailsByOrderIdQuery(orderId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().ProductName.Should().Be(product.Name);
    }

    [Fact]
    public async Task GetByOrderIdAsync_WhenNoDetailsExist_ReturnsEmptyList()
    {
        // Act
        var result = await _service.GetByOrderIdAsync(new GetOrderDetailsByOrderIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
