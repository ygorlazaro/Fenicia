using Fenicia.Web.E2ETests;
using Microsoft.Playwright;
using Xunit;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class BasicInventoryTests(BrowserFixture fixture) : BasicTestBase(fixture)
{
    [Fact]
    public async Task Inventory_ShouldLoadDashboard()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/inventory");

        await Page.GetByText("Custo Total").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByText("Venda Total").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByText("Lucro Potencial").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByText("Quantidade Total").WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Inventory_ShouldLoadCharts()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/inventory");

        await Page.GetByText("Distribuição por Categoria").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByText("Distribuição por Fornecedor").WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Inventory_ShouldLoadLowStockTable()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/inventory");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Produtos com Estoque Baixo" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator(".mud-table-head .mud-table-cell").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator(".mud-table-head .mud-table-cell").Nth(1).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator(".mud-table-head .mud-table-cell").Nth(2).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }
}
