namespace Fenicia.Integration.RunCommandTool.Providers;

public class MigrationProvider : BaseProvider
{
    public MigrationProvider(Uri uri)
        : base(uri)
    {
    }

    public MigrationProvider(Uri uri, string accessToken)
        : base(uri, accessToken)
    {
    }

    public async Task<object> CreateDatabasesAsync(string cnpj)
    {
        return await this.PostAsync<object, string>("migration", cnpj);
    }
}
