using Microsoft.Playwright;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class DashboardTests(BrowserFixture fixture) : BasicTestBase(fixture)
{
    [Fact]
    public async Task Dashboard_ShouldLoadKPIs()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/dashboard");

        await Page.GetByText("Receita Total").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByText("Custo Total").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByText("Lucro Bruto").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByText("Estoque").WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var title = await Page.TitleAsync();
        title.Should().Contain("Painel Financeiro");
    }

    [Fact]
    public async Task Dashboard_ShouldLoadCharts()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/dashboard");

        await Page.GetByText("Receita vs Custo").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByText("Pedidos por Status").WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Dashboard_ShouldChangePeriod()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/dashboard");

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Período" }).ClickAsync();
        await Page.GetByText("7 dias").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Atualizar" }).ClickAsync();

        await Page.GetByText("Receita Total").WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }
}
