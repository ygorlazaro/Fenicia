using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.Common;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class AddSupplierHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddSupplierHandler handler;

    public AddSupplierHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        db = new DefaultContext(options, new TestCompanyContext());
        handler = new AddSupplierHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenAddressIsProvided_CreatesSupplierWithAddress()
    {
        var command = new AddSupplierCommand(
            Id: Guid.NewGuid(),
            Name: faker.Company.CompanyName(),
            Email: faker.Internet.Email(),
            Document: faker.Random.Replace("##.###.###/####-##"),
            PhoneNumber: faker.Phone.PhoneNumber(),
            Cnpj: faker.Random.Replace("##.###.###/####-##"),
            Address: new AddressDTO(
                Street: faker.Address.StreetName(),
                Number: faker.Random.Replace("####"),
                Complement: faker.Address.SecondaryAddress(),
                Neighborhood: faker.Address.CityPrefix(),
                ZipCode: faker.Address.ZipCode(),
                StateId: Guid.NewGuid(),
                City: faker.Address.City(),
                Country: "Brasil"
            )
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        var supplier = await db.BasicSuppliers.FirstOrDefaultAsync(s => s.Id == command.Id);
        Assert.NotNull(supplier);
        Assert.Equal(command.Name, supplier.Person.Name);
    }

    [Fact]
    public async Task Handle_WhenAddressIsNull_CreatesSupplierWithoutAddress()
    {
        var command = new AddSupplierCommand(
            Id: Guid.NewGuid(),
            Name: faker.Company.CompanyName(),
            Email: faker.Internet.Email(),
            Document: faker.Random.Replace("##.###.###/####-##"),
            PhoneNumber: faker.Phone.PhoneNumber(),
            Cnpj: faker.Random.Replace("##.###.###/####-##"),
            Address: null
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        var supplier = await db.BasicSuppliers.FirstOrDefaultAsync(s => s.Id == command.Id);
        Assert.NotNull(supplier);
        Assert.Null(supplier.Person.PersonAddresses.FirstOrDefault()?.Address);
    }
}
