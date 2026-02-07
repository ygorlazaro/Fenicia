using Fenicia.Auth.Domains.Submodule;
using Fenicia.Common.Data.Models.Auth;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class SubmoduleServiceTests
{
    private Mock<ISubmoduleRepository> repoMock = null!;
    private SubmoduleService? sut;

    [SetUp]
    public void Setup()
    {
        this.repoMock = new Mock<ISubmoduleRepository>();
        this.sut = new SubmoduleService(this.repoMock.Object);
    }

    [Test]
    public async Task GetByModuleIdAsync_ReturnsConvertedResponses()
    {
        var moduleId = Guid.NewGuid();
        var items = new List<SubmoduleModel>
        {
            new() { Name = "S1", Route = "/s1", ModuleId = moduleId, Description = "d1" },
            new() { Name = "S2", Route = "/s2", ModuleId = moduleId, Description = "d2" }
        };

        this.repoMock.Setup(r => r.GetByModuleIdAsync(moduleId, It.IsAny<CancellationToken>())).ReturnsAsync(items);

        var result = await this.sut!.GetByModuleIdAsync(moduleId, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Any(r => r is { Name: "S1", Description: "d1" }));
    }
}