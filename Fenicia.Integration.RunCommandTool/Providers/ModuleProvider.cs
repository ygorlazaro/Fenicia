using Fenicia.Common;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Integration.RunCommandTool.Providers;

public class ModuleProvider(string uri) : BaseProvider(uri)
{
    public async Task<List<ModuleResponse>> GetModulesAsync()
    {
        var response = await this.GetAsync<Pagination<List<ModuleResponse>>>("module");

        return response.Data;
    }
}
