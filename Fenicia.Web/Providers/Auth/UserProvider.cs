using Fenicia.Common;
using Fenicia.Common.Database.Responses;
using Fenicia.Web.Abstracts;
using Fenicia.Web.Providers.Auth;

public class UserProvider : BaseProvider
{
    public UserProvider(IConfiguration configuration, AuthManager authManager) : base(configuration, authManager)
    {

    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetModulesAsync()
    {
        return await GetAsync<List<ModuleResponse>>("user/module");
    }
}
