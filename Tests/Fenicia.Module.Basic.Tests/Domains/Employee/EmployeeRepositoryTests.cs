using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class EmployeeRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new EmployeeRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEmployees()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployee()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(employee.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenEmployeeIsValid_InsertsEmployee()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        var result = await _repository.InsertAsync(employee, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeExists_UpdatesEmployee()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(employee.Id, employee, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };

        var result = await _repository.UpdateAsync(employee.Id, employee, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_SoftDeletesEmployee()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(employee.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedEmployee = await _db.BasicEmployees.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == employee.Id);
        Assert.NotNull(deletedEmployee);
        Assert.NotNull(deletedEmployee.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingEmployees()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(e => e.Id == employee.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenEmployeeExists_ReturnsTrue()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(e => e.Id == employee.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(e => e.Id == employee.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
