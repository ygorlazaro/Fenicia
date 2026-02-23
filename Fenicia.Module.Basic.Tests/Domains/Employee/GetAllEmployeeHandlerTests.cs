using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

[TestFixture]
public class GetAllEmployeeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetAllEmployeeHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetAllEmployeeHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllEmployeeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithEmployees_ReturnsAllEmployees()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
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

        var employee2 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
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

        this.context.Employees.AddRange(employee1, employee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Person.Name, Is.EqualTo(employee1.Person.Name));
            Assert.That(result[1].Person.Name, Is.EqualTo(employee2.Person.Name));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = position.Id,
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
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
            this.context.Employees.Add(employee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery(2);

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
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        for (var i = 0; i < 5; i++)
        {
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = position.Id,
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
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
            this.context.Employees.Add(employee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery(10);

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
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = position.Id,
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
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
            this.context.Employees.Add(employee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery();

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
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
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

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Person.Name, Is.Not.Null);
            Assert.That(result[0].Person.Email, Is.Not.Null);
            Assert.That(result[0].Person.Address, Is.Not.Null);
            Assert.That(result[0].PositionId, Is.EqualTo(position.Id));
        }
    }
}
