using AwesomeAssertions;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Tests.Enums;

public class ModuleTypeEnumTests
{
    [Fact]
    public void ModuleType_ShouldHaveExpectedValues()
    {
        var values = Enum.GetValues<EnumModuleType>();

        values.Should().Contain(EnumModuleType.Auth);
        values.Should().Contain(EnumModuleType.Basic);
        values.Should().Contain(EnumModuleType.Project);
        values.Should().Contain(EnumModuleType.Plus);
    }

    [Fact]
    public void ModuleType_ShouldHaveCorrectCount()
    {
        var values = Enum.GetValues<EnumModuleType>();

        values.Length.Should().Be(12);
    }
}
