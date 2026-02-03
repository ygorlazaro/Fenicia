namespace Fenicia.Integration.RunCommandTool.Providers;

public class MigrationProvider : BaseProvider
{
    public MigrationProvider(string uri)
        : base(uri)
    {
    }

    public MigrationProvider(string uri, string accessToken)
        : base(uri, accessToken)
    {
    }

    public async Task<object> CreateDatabasesAsync(string cnpj)
    {
        return await PostAsync<object, string>("migration", cnpj);
    }
}
