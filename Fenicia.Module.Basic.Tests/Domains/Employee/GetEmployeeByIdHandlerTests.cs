using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Employee.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

[TestFixture]
public class GetEmployeeByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetEmployeeByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetEmployeeByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenEmployeeExists_ReturnsEmployeeResponse()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
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
            Id = employeeId,
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

        var query = new GetEmployeeByIdQuery(employeeId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(employeeId));
            Assert.That(result.PersonId, Is.EqualTo(employee.PersonModel.Id));
            Assert.That(result.PositionId, Is.EqualTo(position.Id));
            Assert.That(result.Name, Is.EqualTo(employee.PersonModel.Name));
            Assert.That(result.Email, Is.EqualTo(employee.PersonModel.Email));
            Assert.That(result.PhoneNumber, Is.EqualTo(employee.PersonModel.PhoneNumber));
            Assert.That(result.Document, Is.EqualTo(employee.PersonModel.Document));
            Assert.That(result.Street, Is.EqualTo(employee.PersonModel.Street));
            Assert.That(result.Number, Is.EqualTo(employee.PersonModel.Number));
            Assert.That(result.Complement, Is.EqualTo(employee.PersonModel.Complement));
            Assert.That(result.Neighborhood, Is.EqualTo(employee.PersonModel.Neighborhood));
            Assert.That(result.ZipCode, Is.EqualTo(employee.PersonModel.ZipCode));
            Assert.That(result.StateId, Is.EqualTo(employee.PersonModel.StateId));
            Assert.That(result.City, Is.EqualTo(employee.PersonModel.City));
        }
    }

    [Test]
    public async Task Handle_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetEmployeeByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetEmployeeByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_VerifiesPersonAndPositionDataIsIncluded()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
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
            Id = employeeId,
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

        var query = new GetEmployeeByIdQuery(employeeId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PositionId, Is.EqualTo(position.Id));
        Assert.That(result.Name, Is.EqualTo(employee.PersonModel.Name));
    }

    [Test]
    public async Task Handle_WithMultipleEmployees_ReturnsOnlyRequestedEmployee()
    {
        // Arrange
        var employee1Id = Guid.NewGuid();
        var employee2Id = Guid.NewGuid();
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
            Id = employee1Id,
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
            Id = employee2Id,
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FirstName,
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

        var query = new GetEmployeeByIdQuery(employee1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(employee1Id));
            Assert.That(result.Name, Is.EqualTo(employee1.PersonModel.Name));
        }
    }

    [Test]
    public async Task Handle_WithNullAddressFields_ReturnsCorrectResponse()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
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
            Id = employeeId,
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = string.Empty,
                Number = string.Empty,
                Complement = null,
                Neighborhood = null,
                ZipCode = string.Empty,
                StateId = state.Id,
                StateModel = state,
                City = null,
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.context.BasicEmployees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeeByIdQuery(employeeId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(employee.PersonModel.Name));
        Assert.That(result.Email, Is.EqualTo(employee.PersonModel.Email));
    }
}
