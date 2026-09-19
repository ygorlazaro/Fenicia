using Microsoft.Playwright;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class SignupFlowTests(BrowserFixture fixture) : IAsyncLifetime
{
    private readonly BrowserFixture _fixture = fixture;
    private IPage _page = null!;

    public async Task InitializeAsync()
    {
        var context = await _fixture.Browser.NewContextAsync();
        _page = await context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await _page.Context.CloseAsync();
    }

    [Fact]
    public async Task Signup_ThenLogin_ThenSelectCompany_ShouldShowDashboard()
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "http://localhost:5104";
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var email = $"test-{uniqueId}@example.com";
        var password = "Test@1234";
        var fullName = $"Test User {uniqueId}";
        var companyName = $"Test Company {uniqueId}";
        var cnpj = $"{uniqueId}000000";

        await _page.GotoAsync($"{baseUrl}/signup");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await _page.GetByLabel("Nome completo").FillAsync(fullName);
        await _page.GetByLabel("Email").FillAsync(email);
        await _page.GetByLabel("Senha").FillAsync(password);
        await _page.GetByLabel("Nome da empresa").FillAsync(companyName);
        await _page.GetByLabel("CNPJ").FillAsync(cnpj);

        await _page.GetByRole(AriaRole.Button, new() { Name = "Criar minha conta" }).ClickAsync();

        await _page.GetByText("Conta criada com sucesso!").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await _page.GetByRole(AriaRole.Link, new() { Name = "Entrar" }).ClickAsync();
        await _page.GetByLabel("Email").FillAsync(email);
        await _page.GetByLabel("Senha").FillAsync(password);

        await _page.GetByRole(AriaRole.Button, new() { Name = "Entrar" }).ClickAsync();

        await _page.WaitForURLAsync($"{baseUrl}/company**");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await _page.Locator(".company-list-item").First.ClickAsync();

        await _page.WaitForURLAsync($"{baseUrl}/dashboard**");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await _page.GetByText("Receita Total").WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var title = await _page.TitleAsync();
        title.Should().Contain("Painel Financeiro");
    }
}
