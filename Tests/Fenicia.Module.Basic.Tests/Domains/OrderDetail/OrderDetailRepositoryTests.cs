using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.OrderDetail;

public class OrderDetailRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly OrderDetailRepository _repository;

    public OrderDetailRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new OrderDetailRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrderDetails()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDetailExists_ReturnsOrderDetail()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(orderDetail.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderDetail.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDetailDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenOrderDetailIsValid_InsertsOrderDetail()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };

        // Act
        var result = await _repository.InsertAsync(orderDetail, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderDetailExists_UpdatesOrderDetail()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.UpdateAsync(orderDetail.Id, orderDetail, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderDetail.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderDetailDoesNotExist_ReturnsNull()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };

        // Act
        var result = await _repository.UpdateAsync(orderDetail.Id, orderDetail, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderDetailExists_SoftDeletesOrderDetail()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(orderDetail.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedOrderDetail = await _db.BasicOrderDetails.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == orderDetail.Id);
        deletedOrderDetail.Should().NotBeNull();
        deletedOrderDetail!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingOrderDetails()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(o => o.Id == orderDetail.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenOrderDetailExists_ReturnsTrue()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(o => o.Id == orderDetail.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(o => o.Id == orderDetail.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}
