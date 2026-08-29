using Fenicia.Common.Data;

using FluentAssertions;

namespace Fenicia.Common.Data.Tests.Models;

public class BaseCompanyModelTests
{
    [Fact]
    public void Constructor_ShouldInheritIdFromBaseModel()
    {
        var model = new TestBaseCompanyModel();

        model.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ShouldSetCompanyIdToEmpty()
    {
        var model = new TestBaseCompanyModel();

        model.CompanyId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CompanyId_ShouldBeSettable()
    {
        var model = new TestBaseCompanyModel();
        var companyId = Guid.NewGuid();

        model.CompanyId = companyId;

        model.CompanyId.Should().Be(companyId);
    }

    private sealed class TestBaseCompanyModel : BaseCompanyModel
    {
    }
}
