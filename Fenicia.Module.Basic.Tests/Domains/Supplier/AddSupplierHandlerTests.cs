using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class AddSupplierHandlerTests : IDisposable
{
    public AddSupplierHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new AddSupplierHandler(this.db);
        this.faker = new Faker();
    }

    private readonly DefaultContext db;
    private readonly AddSupplierHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithValidCommand_AddsSupplierAndReturnsResponse()
    {
        // Arrange
        var command = new AddSupplierCommand(
            Guid.NewGuid(),
            this.faker.Company.CompanyName(),
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            "Suite 100",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Random.Replace("##.###.###/####-##"));

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id,
            result.Id);
        Assert.Equal(command.Cnpj,
            result.Cnpj);
    }

    [Fact]
    public async Task Handle_VerifiesSupplierWasSavedToDatabase()
    {
        // Arrange
        var command = new AddSupplierCommand(
            Guid.NewGuid(),
            this.faker.Company.CompanyName(),
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null,
            null);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var supplier = await this.db.BasicSuppliers
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == command.Id);

        Assert.NotNull(supplier);
        Assert.Equal(command.Name,
            supplier.Person.Name);
    }

    [Fact]
    public async Task Handle_WithNullCnpj_HandlesCorrectly()
    {
        // Arrange
        var command = new AddSupplierCommand(
            Guid.NewGuid(),
            this.faker.Company.CompanyName(),
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Cnpj);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }
}
