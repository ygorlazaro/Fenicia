using Fenicia.Auth.Domains.Submodule;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;
using Moq;

namespace Fenicia.Auth.Tests.Services;

public class SubmoduleServiceTests
{
    private Mock<ISubmoduleRepository> repoMock = null!;
    private SubmoduleService? sut;

    [SetUp]
    public void Setup()
    {
        repoMock = new Mock<ISubmoduleRepository>();
        sut = new SubmoduleService(repoMock.Object);
    }

    [Test]
    public async Task GetByModuleIdAsync_ReturnsConvertedResponses()
    {
        var moduleId = Guid.NewGuid();
        var items = new List<SubmoduleModel>
        {
            new SubmoduleModel { Name = "S1", Route = "/s1", ModuleId = moduleId, Description = "d1" },
            new SubmoduleModel { Name = "S2", Route = "/s2", ModuleId = moduleId, Description = "d2" }
        };

        repoMock.Setup(r => r.GetByModuleIdAsync(moduleId, It.IsAny<CancellationToken>())).ReturnsAsync(items);

        var result = await sut!.GetByModuleIdAsync(moduleId, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(r => r.Name == "S1" && r.Description == "d1"));
    }
}
