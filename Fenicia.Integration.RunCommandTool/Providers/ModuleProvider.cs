namespace Fenicia.Integration.RunCommandTool.Providers;

using Common;
using Common.Database.Responses;

public class ModuleProvider : BaseProvider
{
    public ModuleProvider(string baseUrl)
        : base(baseUrl)
    {
    }

    public async Task<List<ModuleResponse>> GetModulesAsync()
    {
        var response = await GetAsync<Pagination<List<ModuleResponse>>>("module");

        return response.Data;
    }
}
