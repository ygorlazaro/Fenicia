namespace Fenicia.Integration.RunCommandTool.Providers;

using Fenicia.Common;
using Fenicia.Common.Database.Responses;

public class ModuleProvider : BaseProvider
{
    public ModuleProvider(string baseUrl)
        : base(baseUrl)
    {
    }

    public ModuleProvider(string baseUrl, string accessToken)
        : base(baseUrl)
    {
    }

    public async Task<List<ModuleResponse>> GetModulesAsync()
    {
        var response = await this.GetAsync<Pagination<List<ModuleResponse>>>("module");

        return response.Data;
    }
}
