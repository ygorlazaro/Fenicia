namespace Fenicia.Common.API.Providers;

public class TenantProvider
{
    public string? TenantId
    {
        get; private set;
    }

    public void SetTenant(string tenantId)
    {
        TenantId = tenantId;
    }
}
