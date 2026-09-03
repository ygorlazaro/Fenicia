using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.State;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.State;

public class StateServiceTests : IDisposable
{
    private readonly DbContextOptions<DefaultContext> _dbOptions;
    private readonly Faker _faker;
    private readonly Mock<IStateRepository> _mockRepository;
    private readonly StateService _service;

    public StateServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _mockRepository = new Mock<IStateRepository>();
        _service = new StateService(_mockRepository.Object);
        _faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenStatesExist_ReturnsStates()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        var db = new DefaultContext(_dbOptions, new TestCompanyContext());
        db.AuthStates.Add(state);
        await db.SaveChangesAsync(CancellationToken.None);
        _mockRepository.Setup(r => r.Query()).Returns(() => db.AuthStates);

        // Act
        var result = await _service.GetAllAsync(new GetAllStateQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoStatesExist_ReturnsEmptyList()
    {
        // Arrange
        var db = new DefaultContext(_dbOptions, new TestCompanyContext());
        _mockRepository.Setup(r => r.Query()).Returns(() => db.AuthStates);

        // Act
        var result = await _service.GetAllAsync(new GetAllStateQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}