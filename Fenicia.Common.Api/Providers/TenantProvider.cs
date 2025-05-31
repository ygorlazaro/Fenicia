namespace Fenicia.Common.Api.Providers;

public class TenantProvider
{
    private string? _tenantId;

    public string? TenantId => _tenantId;

    public void SetTenant(string tenantId)
    {
        _tenantId = tenantId;
    }
}
