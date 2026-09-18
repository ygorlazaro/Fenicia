using Fenicia.Web.E2ETests;
using Microsoft.Playwright;
using Xunit;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class BasicStockMovementTests(BrowserFixture fixture) : BasicTestBase(fixture)
{
    [Fact]
    public async Task StockMovement_ListShouldLoad()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/stockmovement");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Movimentações de Estoque" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task StockMovement_ShouldCreateIn()
    {
        await LoginAndSelectCompanyAsync();
        await EnsureProductExistsAsync();
        await NavigateAsync("/basic/stockmovement");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Movimentações de Estoque" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Nova Movimentação" }).ClickAsync();
        await Page.GetByLabel("Produto").ClickAsync();
        await Page.GetByRole(AriaRole.Option).First.ClickAsync();
        await Page.Locator("[role='dialog']").Locator("div[role='combobox'][aria-label='Tipo']").ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "Entrada" }).ClickAsync();
        await Page.GetByLabel("Quantidade").FillAsync("5");
        await Page.GetByLabel("Preço unitário").FillAsync("50");
        await Page.GetByLabel("Fornecedor").ClickAsync();
        await Page.GetByRole(AriaRole.Option).First.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Registrar" }).ClickAsync();

        await WaitForSuccessAsync("Movimentação registrada.");
    }

    [Fact]
    public async Task StockMovement_ShouldCreateOut()
    {
        await LoginAndSelectCompanyAsync();
        await EnsureProductExistsAsync();
        await NavigateAsync("/basic/stockmovement");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Movimentações de Estoque" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Nova Movimentação" }).ClickAsync();
        await Page.GetByLabel("Produto").ClickAsync();
        await Page.GetByRole(AriaRole.Option).First.ClickAsync();
        await Page.Locator("[role='dialog']").Locator("div[role='combobox'][aria-label='Tipo']").ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "Saída" }).ClickAsync();
        await Page.GetByLabel("Quantidade").FillAsync("2");
        await Page.GetByLabel("Cliente").ClickAsync();
        await Page.GetByRole(AriaRole.Option).First.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Registrar" }).ClickAsync();

        await WaitForSuccessAsync("Movimentação registrada.");
    }
}
