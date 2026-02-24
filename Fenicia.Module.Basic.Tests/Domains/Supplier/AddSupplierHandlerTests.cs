using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Supplier.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

[TestFixture]
public class AddSupplierHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new AddSupplierHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private AddSupplierHandler handler = null!;
    private Faker faker = null!;

    [Test]
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
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Cnpj, Is.EqualTo(command.Cnpj));
        }
    }

    [Test]
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
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var supplier = await this.context.Suppliers
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == command.Id);

        Assert.That(supplier, Is.Not.Null);
        Assert.That(supplier.Person.Name, Is.EqualTo(command.Name));
    }

    [Test]
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
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Cnpj, Is.Null);
    }
}
