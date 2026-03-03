using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Employee.GetByPositionId;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

[TestFixture]
public class GetEmployeesByPositionIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetEmployeesByPositionIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetEmployeesByPositionIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithNoEmployeesForPosition_ReturnsEmptyList()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithEmployeesForPosition_ReturnsFilteredList()
    {
        // Arrange
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new BasicPositionModel
        {
            Id = position1Id,
            Name = "Developer"
        };

        var position2 = new BasicPositionModel
        {
            Id = position2Id,
            Name = "Designer"
        };

        this.context.BasicPositions.AddRange(position1, position2);

        var employee1 = new BasicEmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position1Id,
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        var employee2 = new BasicEmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position1Id,
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        var employee3 = new BasicEmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position2Id,
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.BasicEmployees.AddRange(employee1, employee2, employee3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(position1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(e => e.PositionId == position1Id), Is.True);
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new BasicPositionModel
        {
            Id = positionId,
            Name = "Developer"
        };
        this.context.BasicPositions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var employee = new BasicEmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = positionId,
                PersonId = Guid.NewGuid(),
                PersonModel = new BasicPersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Person.FullName} {i}",
                    Email = this.faker.Internet.Email(),
                    Document = this.faker.Random.Replace("###.###.###-##"),
                    Street = this.faker.Address.StreetName(),
                    Number = this.faker.Random.Replace("####"),
                    ZipCode = this.faker.Address.ZipCode(),
                    StateId = Guid.NewGuid(),
                    City = this.faker.Address.City()
                }
            };
            this.context.BasicEmployees.Add(employee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId, 2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new BasicPositionModel
        {
            Id = positionId,
            Name = "Developer"
        };
        this.context.BasicPositions.Add(position);

        for (var i = 0; i < 5; i++)
        {
            var employee = new BasicEmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = positionId,
                PersonId = Guid.NewGuid(),
                PersonModel = new BasicPersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Person.FullName} {i}",
                    Email = this.faker.Internet.Email(),
                    Document = this.faker.Random.Replace("###.###.###-##"),
                    Street = this.faker.Address.StreetName(),
                    Number = this.faker.Random.Replace("####"),
                    ZipCode = this.faker.Address.ZipCode(),
                    StateId = Guid.NewGuid(),
                    City = this.faker.Address.City()
                }
            };
            this.context.BasicEmployees.Add(employee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new BasicPositionModel
        {
            Id = positionId,
            Name = "Developer"
        };
        this.context.BasicPositions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var employee = new BasicEmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = positionId,
                PersonId = Guid.NewGuid(),
                PersonModel = new BasicPersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Person.FullName} {i}",
                    Email = this.faker.Internet.Email(),
                    Document = this.faker.Random.Replace("###.###.###-##"),
                    Street = this.faker.Address.StreetName(),
                    Number = this.faker.Random.Replace("####"),
                    ZipCode = this.faker.Address.ZipCode(),
                    StateId = Guid.NewGuid(),
                    City = this.faker.Address.City()
                }
            };
            this.context.BasicEmployees.Add(employee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task Handle_VerifiesPersonAndPositionDataIsIncluded()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new BasicPositionModel
        {
            Id = positionId,
            Name = "Developer"
        };
        this.context.BasicPositions.Add(position);

        var employee = new BasicEmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = positionId,
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.BasicEmployees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].PersonId, Is.EqualTo(employee.PersonId));
            Assert.That(result[0].PositionId, Is.EqualTo(positionId));
        }
    }
}
