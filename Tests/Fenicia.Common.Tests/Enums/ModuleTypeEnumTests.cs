using AwesomeAssertions;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Tests.Enums;

public class ModuleTypeEnumTests
{
    [Fact]
    public void ModuleType_ShouldHaveExpectedValues()
    {
        var values = Enum.GetValues<ModuleType>();

        values.Should().Contain(ModuleType.Auth);
        values.Should().Contain(ModuleType.Basic);
        values.Should().Contain(ModuleType.Project);
        values.Should().Contain(ModuleType.Plus);
    }

    [Fact]
    public void ModuleType_ShouldHaveCorrectCount()
    {
        var values = Enum.GetValues<ModuleType>();

        values.Length.Should().Be(12);
    }
}
