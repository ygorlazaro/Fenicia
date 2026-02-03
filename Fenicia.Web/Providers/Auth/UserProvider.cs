using Fenicia.Common.Database.Responses;
using Fenicia.Web.Abstracts;
using Fenicia.Web.Providers.Auth;

public class UserProvider(IConfiguration configuration, AuthManager authManager) : BaseProvider(configuration, authManager)
{
    public async Task<List<ModuleResponse>> GetModulesAsync()
    {
        return await GetAsync<List<ModuleResponse>>("user/module");
    }
}
