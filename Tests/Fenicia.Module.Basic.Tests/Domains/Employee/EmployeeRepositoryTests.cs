using AwesomeAssertions;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class EmployeeRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new EmployeeRepository(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEmployees()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployee()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(employee.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(employee.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenEmployeeIsValid_InsertsEmployee()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        // Act
        var result = await _repository.InsertAsync(employee, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeExists_UpdatesEmployee()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.UpdateAsync(employee.Id, employee, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(employee.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        // Act
        var result = await _repository.UpdateAsync(employee.Id, employee, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_SoftDeletesEmployee()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(employee.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedEmployee =
            await _db.BasicEmployees.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == employee.Id);
        deletedEmployee.Should().NotBeNull();
        deletedEmployee.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingEmployees()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(e => e.Id == employee.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenEmployeeExists_ReturnsTrue()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(e => e.Id == employee.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var employee = new EmployeeModel
            { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(e => e.Id == employee.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}