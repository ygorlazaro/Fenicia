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
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDetailExists_ReturnsOrderDetail()
    {
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(orderDetail.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderDetail.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDetailDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenOrderDetailIsValid_InsertsOrderDetail()
    {
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };

        var result = await _repository.InsertAsync(orderDetail, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderDetailExists_UpdatesOrderDetail()
    {
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(orderDetail.Id, orderDetail, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderDetail.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderDetailDoesNotExist_ReturnsNull()
    {
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };

        var result = await _repository.UpdateAsync(orderDetail.Id, orderDetail, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderDetailExists_SoftDeletesOrderDetail()
    {
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(orderDetail.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedOrderDetail = await _db.BasicOrderDetails.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == orderDetail.Id);
        Assert.NotNull(deletedOrderDetail);
        Assert.NotNull(deletedOrderDetail.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingOrderDetails()
    {
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(o => o.Id == orderDetail.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenOrderDetailExists_ReturnsTrue()
    {
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(o => o.Id == orderDetail.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Price = _faker.Random.Decimal(), Quantity = _faker.Random.Double() };
        _db.BasicOrderDetails.Add(orderDetail);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(o => o.Id == orderDetail.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
