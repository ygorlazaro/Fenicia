using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Common;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Migrations.Services;

namespace Fenicia.Integration.RunCommandTool.Providers;

using System.Net.Http.Headers;

public class NewCompanyProvider
{
    private const string ApiRoute = "http://localhost:5144";

    private readonly Faker _faker;

    private readonly HttpClient _client;

    public NewCompanyProvider()
    {
        _faker = new Faker();
        _client = new HttpClient { BaseAddress = new Uri(NewCompanyProvider.ApiRoute) };
    }

    public async Task RunAsync()
    {
        var newUserRequest = new UserRequest()
        {
            Company = new CompanyRequest
            {
                Cnpj = _faker.Company.Cnpj(includeFormatSymbols: false),
                Name = _faker.Company.CompanyName()
            },
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userResponse = await CreateNewCompanyAsync(newUserRequest);
        var token = await GetToken(newUserRequest);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var newOrder = await OrderNewModules();

        await CreateDatabasesAsync(Guid.Parse("01994745-2544-76d4-bffd-b93d06ca6bb2"));

        Console.WriteLine("Foi");
    }

    private async Task CreateDatabasesAsync(Guid companyId)
    {
        var modules = await GetModulesAsync();
        var migrationService = new MigrationService();

        await migrationService.RunMigrationsAsync(modules.Data, companyId, new CancellationToken());
    }

    private async Task<TokenResponse> GetToken(UserRequest newUserRequest)
    {
        var tokenRequest = new TokenRequest
        {
            Cnpj = newUserRequest.Company.Cnpj,
            Email = newUserRequest.Email,
            Password = newUserRequest.Password
        };

        using StringContent content = new(JsonSerializer.Serialize(tokenRequest), Encoding.UTF8,
                                        "application/json");

        var response = await _client.PostAsync("token", content);

        var newToken = await response.Content.ReadFromJsonAsync<TokenResponse>();

        ArgumentNullException.ThrowIfNull(newToken);

        return newToken;
    }

    private async Task<OrderResponse> OrderNewModules()
    {
        var modules = await GetModulesAsync();

        var orderRequest = new OrderRequest()
        {
            Details = modules.Data.Select(module => new OrderDetailRequest
            {
                ModuleId = module.Id
            })
        };

        using StringContent jsonContent = new(JsonSerializer.Serialize(orderRequest), Encoding.UTF8,
                                        "application/json");
        var response = await jsonContent.ReadFromJsonAsync<OrderResponse>();

        ArgumentNullException.ThrowIfNull(response);

        return response;
    }

    private async Task<Pagination<List<ModuleResponse>>> GetModulesAsync()
    {
        var response = await _client.GetAsync("module");

        var modules = await response.Content.ReadFromJsonAsync<Pagination<List<ModuleResponse>>>();

        ArgumentNullException.ThrowIfNull(modules);

        return modules;
    }

    private async Task<UserResponse> CreateNewCompanyAsync(UserRequest newUserRequest)
    {
        using StringContent jsonContent = new(
                                        JsonSerializer.Serialize(newUserRequest),
                                        Encoding.UTF8,
                                        "application/json");

        using var response = await _client.PostAsync("signup", jsonContent);

        var newUserResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        ArgumentNullException.ThrowIfNull(newUserResponse);

        return newUserResponse;
    }
}

