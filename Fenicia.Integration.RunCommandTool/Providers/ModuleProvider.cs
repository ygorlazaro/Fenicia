namespace Fenicia.Integration.RunCommandTool.Providers;

using Common;
using Common.Database.Responses;

public class ModuleProvider : BaseProvider
{
    public ModuleProvider(string uri)
        : base(uri)
    {
    }

    public async Task<List<ModuleResponse>> GetModulesAsync()
    {
        var response = await this.GetAsync<Pagination<List<ModuleResponse>>>("module");

        return response.Data;
    }
}
