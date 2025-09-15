
using Fenicia.Integration.RunCommandTool;

public class MigrationProvider : BaseProvider
{
    public MigrationProvider(string baseUrl) : base(baseUrl)
    {
    }

    public MigrationProvider(string baseUrl, string accessToken) : base(baseUrl, accessToken)
    {
    }

    public async Task<object> CreateDatabasesAsync(string cnpj)
    {
        return await PostAsync<object, string>("migration", cnpj);
    }
}
