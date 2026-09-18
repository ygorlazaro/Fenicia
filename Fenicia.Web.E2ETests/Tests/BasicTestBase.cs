using Microsoft.Playwright;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Fenicia.Web.E2ETests.Tests;

public abstract class BasicTestBase(BrowserFixture fixture) : IAsyncLifetime
{
    protected BrowserFixture Fixture { get; } = fixture;
    protected IPage Page { get; private set; } = null!;
    private string BaseUrl => Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "http://localhost:5104";
    private string? _uniqueId;
    protected string UniqueId => _uniqueId ??= Guid.NewGuid().ToString("N")[..8];
    protected string Document => $"{DateTime.UtcNow.Ticks % 90000000000 + 10000000000}";

    public async Task InitializeAsync()
    {
        var context = await Fixture.Browser.NewContextAsync();
        Page = await context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await Page.Context.CloseAsync();
    }

    protected async Task LoginAndSelectCompanyAsync()
    {
        await Page.Context.ClearCookiesAsync();

        await Page.GotoAsync($"{BaseUrl}/login");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.EvaluateAsync("() => localStorage.clear()");

        await Page.GetByLabel("Email").FillAsync("ygor@ygorlazaro.com");
        await Page.GetByLabel("Senha").FillAsync("ygor");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Entrar" }).ClickAsync();

        await Page.WaitForURLAsync($"{BaseUrl}/company**", new() { Timeout = 10000 });
        await Page.Locator(".company-list-item").First.ClickAsync();

        await Page.WaitForURLAsync($"{BaseUrl}/dashboard**");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.GetByText("Receita Total").WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    protected async Task NavigateAsync(string path)
    {
        await Page.GotoAsync($"{BaseUrl}{path}");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    protected async Task WaitForSuccessAsync(string? contains = null)
    {
        if (!string.IsNullOrEmpty(contains))
        {
            await Page.GetByText(contains, new() { Exact = false }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }
        else
        {
            await Page.GetByText("com sucesso", new() { Exact = false }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }
    }

    protected async Task EnsureProductExistsAsync()
    {
        var token = await Page.EvaluateAsync<string>("() => localStorage.getItem('auth_token')");
        var companyId = await Page.EvaluateAsync<string>("() => localStorage.getItem('selected_company_id')");

        if (string.IsNullOrEmpty(companyId))
        {
            throw new InvalidOperationException("Missing selected_company_id in localStorage.");
        }

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Missing auth_token in localStorage.");
        }

        using var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5083");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("CompanyId", companyId);

        var existingProducts = await client.GetAsync("/datasource/dashboard/product");
        if (existingProducts.IsSuccessStatusCode)
        {
            var json = await existingProducts.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.EnumerateArray().Any(p => p.GetProperty("isActive").GetBoolean()))
            {
                return;
            }
        }

        var categoryId = await EnsureProductCategoryExistsAsync(client);

        var productId = Guid.NewGuid();
        var productPayload = new
        {
            id = productId,
            name = $"E2E Product {UniqueId}",
            sku = $"E2E-{UniqueId}",
            salesPrice = 100,
            quantity = 10,
            categoryId = categoryId,
            isActive = true
        };

        var createResponse = await client.PostAsJsonAsync("/product", productPayload);
        createResponse.EnsureSuccessStatusCode();

        for (var attempt = 0; attempt < 20; attempt++)
        {
            var verify = await client.GetAsync("/datasource/dashboard/product");
            if (verify.IsSuccessStatusCode)
            {
                var verifyJson = await verify.Content.ReadAsStringAsync();
                using var verifyDoc = JsonDocument.Parse(verifyJson);
                if (verifyDoc.RootElement.EnumerateArray().Any(p => p.GetProperty("id").GetGuid() == productId))
                {
                    return;
                }
            }

            await Task.Delay(500);
        }

        throw new InvalidOperationException($"Product {productPayload.name} was created but is not visible in the order form product list.");
    }

    private async Task<Guid> EnsureProductCategoryExistsAsync(HttpClient client)
    {
        var categoriesResponse = await client.GetAsync("/datasource/productcategory");
        categoriesResponse.EnsureSuccessStatusCode();
        var categoriesJson = await categoriesResponse.Content.ReadAsStringAsync();
        using var categoriesDoc = JsonDocument.Parse(categoriesJson);
        var categories = categoriesDoc.RootElement.EnumerateArray().ToList();

        if (categories.Count > 0)
        {
            return categories[0].GetProperty("id").GetGuid();
        }

        var categoryId = Guid.NewGuid();
        var categoryPayload = new
        {
            id = categoryId,
            name = $"E2E Category {UniqueId}"
        };

        var createCategoryResponse = await client.PostAsJsonAsync("/productcategory", categoryPayload);
        createCategoryResponse.EnsureSuccessStatusCode();

        for (var attempt = 0; attempt < 10; attempt++)
        {
            var verify = await client.GetAsync("/datasource/productcategory");
            if (verify.IsSuccessStatusCode)
            {
                var verifyJson = await verify.Content.ReadAsStringAsync();
                using var verifyDoc = JsonDocument.Parse(verifyJson);
                if (verifyDoc.RootElement.EnumerateArray().Any(c => c.GetProperty("id").GetGuid() == categoryId))
                {
                    return categoryId;
                }
            }

            await Task.Delay(500);
        }

        throw new InvalidOperationException($"Category {categoryPayload.name} was created but is not visible in the product category datasource.");
    }
}
