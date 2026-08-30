using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class SupplierServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly SupplierService _service;

    public SupplierServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);

        var supplierRepository = new SupplierRepository(_db);
        var productRepository = new ProductRepository(_db);
        var productCategoryRepository = new ProductCategoryRepository(_db);
        var orderDetailRepository = new OrderDetailRepository(_db);
        var stockMovementRepository = new StockMovementRepository(_db);
        var addressRepository = new AddressRepository(_db);
        var personAddressRepository = new PersonAddressRepository(_db);

        var productCategoryService = new ProductCategoryService(productCategoryRepository);
        var orderDetailService = new OrderDetailService(orderDetailRepository);
        var productService = new ProductService(productRepository, productCategoryService, orderDetailService, new StockMovementService());
        var stockMovementService = new StockMovementService(stockMovementRepository, productService);
        var addressService = new AddressService(addressRepository);
        var personAddressService = new PersonAddressService(personAddressRepository);

        _service = new SupplierService(supplierRepository, productService, stockMovementService, addressService, personAddressService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenSuppliersExist_ReturnsPaginationWithSuppliers()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _db.BasicPeople.Add(person);
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = person.Id };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllAsync(new GetAllSupplierQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierExists_ReturnsSupplier()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _db.BasicPeople.Add(person);
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = person.Id };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByIdAsync(new GetSupplierByIdQuery(supplier.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(supplier.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(new GetSupplierByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesSupplier()
    {
        var command = new AddSupplierCommand(Guid.NewGuid(), _faker.Person.FullName, _faker.Person.Email, _faker.Person.Random.AlphaNumeric(11), _faker.Person.Random.AlphaNumeric(11), _faker.Person.Random.AlphaNumeric(11), new AddressDTO(_faker.Address.StreetAddress(), _faker.Address.BuildingNumber(), null, _faker.Address.City(), _faker.Address.ZipCode(), Guid.NewGuid(), _faker.Address.City(), null));

        var result = await _service.AddAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierExists_UpdatesSupplier()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _db.BasicPeople.Add(person);
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = person.Id };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateSupplierCommand(supplier.Id, _faker.Person.FullName, _faker.Person.Email, _faker.Person.Random.AlphaNumeric(11), "12345678900", "12345678900", new AddressDTO(_faker.Address.StreetAddress(), _faker.Address.BuildingNumber(), null, _faker.Address.City(), _faker.Address.ZipCode(), Guid.NewGuid(), _faker.Address.City(), null));

        var result = await _service.UpdateAsync(command with { Id = supplier.Id }, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Cnpj, result.Cnpj);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        var command = new UpdateSupplierCommand(Guid.NewGuid(), _faker.Person.FullName, _faker.Person.Email, _faker.Person.Random.AlphaNumeric(11), "12345678900", "12345678900", new AddressDTO(_faker.Address.StreetAddress(), _faker.Address.BuildingNumber(), null, _faker.Address.City(), _faker.Address.ZipCode(), Guid.NewGuid(), _faker.Address.City(), null));

        var result = await _service.UpdateAsync(command with { Id = Guid.NewGuid() }, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenSupplierExists_SoftDeletesSupplier()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        await _service.DeleteAsync(new DeleteSupplierCommand(supplier.Id), _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        var deletedSupplier = await _db.BasicSuppliers.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == supplier.Id);
        Assert.NotNull(deletedSupplier);
        Assert.NotNull(deletedSupplier.Deleted);
    }
}
