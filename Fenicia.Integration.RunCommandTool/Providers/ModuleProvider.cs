
using Fenicia.Common;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Integration.RunCommandTool.Providers;

public class ModuleProvider : BaseProvider
{
    public ModuleProvider(string baseUrl) : base(baseUrl)
    {
    }

    public ModuleProvider(string baseUrl, string accessToken) : base(baseUrl)
    {

    }

    public async Task<List<ModuleResponse>> GetModulesAsync()
    {
        var response = await base.GetAsync<Pagination<List<ModuleResponse>>>("module");

        return response.Data;
    }
}
