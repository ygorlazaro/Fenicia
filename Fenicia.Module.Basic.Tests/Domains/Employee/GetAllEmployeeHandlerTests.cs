using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Employee.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

[TestFixture]
public class GetAllEmployeeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllEmployeeHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
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
        var position = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.BasicPositions.Add(position);

        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        var employee1 = new BasicEmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
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
                StateId = state.Id,
                StateModel = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        var employee2 = new BasicEmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
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
                StateId = state.Id,
                StateModel = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.context.BasicEmployees.AddRange(employee1, employee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].PersonId, Is.EqualTo(employee1.PersonModel.Id));
            Assert.That(result[0].Name, Is.EqualTo(employee1.PersonModel.Name));
            Assert.That(result[0].Email, Is.EqualTo(employee1.PersonModel.Email));
            Assert.That(result[0].PositionName, Is.EqualTo(position.Name));
            Assert.That(result[0].StateName, Is.EqualTo(state.Name));

            Assert.That(result[1].PersonId, Is.EqualTo(employee2.PersonModel.Id));
            Assert.That(result[1].Name, Is.EqualTo(employee2.PersonModel.Name));
            Assert.That(result[1].Email, Is.EqualTo(employee2.PersonModel.Email));
            Assert.That(result[1].PositionName, Is.EqualTo(position.Name));
            Assert.That(result[1].StateName, Is.EqualTo(state.Name));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var position = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.BasicPositions.Add(position);

        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        for (var i = 0; i < 25; i++)
        {
            var employee = new BasicEmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = position.Id,
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
                    StateId = state.Id,
                    StateModel = state,
                    City = this.faker.Address.City(),
                    PhoneNumber = this.faker.Phone.PhoneNumber()
                }
            };
            this.context.BasicEmployees.Add(employee);
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
        var position = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.BasicPositions.Add(position);

        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        for (var i = 0; i < 5; i++)
        {
            var employee = new BasicEmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = position.Id,
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
                    StateId = state.Id,
                    StateModel = state,
                    City = this.faker.Address.City(),
                    PhoneNumber = this.faker.Phone.PhoneNumber()
                }
            };
            this.context.BasicEmployees.Add(employee);
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
        var position = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.BasicPositions.Add(position);

        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        for (var i = 0; i < 25; i++)
        {
            var employee = new BasicEmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = position.Id,
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
                    StateId = state.Id,
                    StateModel = state,
                    City = this.faker.Address.City(),
                    PhoneNumber = this.faker.Phone.PhoneNumber()
                }
            };
            this.context.BasicEmployees.Add(employee);
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
        var position = new BasicPositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.BasicPositions.Add(position);

        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        var employee = new BasicEmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
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
                StateId = state.Id,
                StateModel = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.context.BasicEmployees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].PersonId, Is.EqualTo(employee.PersonModel.Id));
            Assert.That(result[0].PositionId, Is.EqualTo(position.Id));
            Assert.That(result[0].Name, Is.EqualTo(employee.PersonModel.Name));
            Assert.That(result[0].PositionName, Is.EqualTo(position.Name));
            Assert.That(result[0].StateName, Is.EqualTo(state.Name));
        }
    }
}
