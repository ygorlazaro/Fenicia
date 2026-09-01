using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.Person.Interfaces;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class EmployeeServiceTests : IDisposable
{
    private readonly Faker _faker;
    private readonly Mock<IEmployeeRepository> _mockRepository;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _mockRepository = new Mock<IEmployeeRepository>();
        var mockPersonService = new Mock<IPersonService>();
        var mockAddressService = new Mock<IAddressService>();
        var mockPersonAddressService = new Mock<IPersonAddressService>();
        var mockOrderService = new Mock<IOrderService>();
        _service = new EmployeeService(_mockRepository.Object, mockPersonService.Object, mockAddressService.Object, mockPersonAddressService.Object, mockOrderService.Object);
        _faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployee()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = "Emp" };
        var position = new PositionModel { Id = Guid.NewGuid(), Name = "Dev" };
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PersonId = person.Id, Person = person, PositionId = position.Id, Position = position };
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _service.GetByIdAsync(new GetEmployeeByIdQuery(employee.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(employee.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeModel?)null);

        // Act
        var result = await _service.GetByIdAsync(new GetEmployeeByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesEmployee()
    {
        // Arrange
        var command = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), "Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<EmployeeModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeModel e, CancellationToken _) => e);

        // Act
        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_DeletesEmployee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(new DeleteEmployeeCommand(employeeId), Guid.NewGuid(), CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCountAsync_ReturnsCount()
    {
        // Arrange
        _mockRepository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(11);

        // Act
        var result = await _service.GetCountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(11);
    }

    [Fact]
    public async Task GetTotalEmployeesAsync_ReturnsTotal()
    {
        // Arrange
        _mockRepository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(15);

        // Act
        var result = await _service.GetTotalEmployeesAsync(CancellationToken.None);

        // Assert
        result.Should().Be(15);
    }
}
