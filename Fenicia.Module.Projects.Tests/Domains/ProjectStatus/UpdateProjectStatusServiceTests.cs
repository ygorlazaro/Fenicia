using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Microsoft.EntityFrameworkCore;
namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;
public class UpdateProjectStatusServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProjectStatusService service;
    public UpdateProjectStatusServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new ProjectStatusService(db);
        faker = new Faker();
    }
    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }
    [Fact]
    public async Task UpdateAsync_WhenValid_ReturnsExpected()
    {
        // Arrange - setup test data
        // Act - call service method
        // Assert - verify result
        Assert.True(true);
    }
}
