using Microsoft.Playwright;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class BasicProductTests(BrowserFixture fixture) : BasicTestBase(fixture)
{
    [Fact]
    public async Task Product_ListShouldLoad()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/product");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Produtos" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Product_ShouldCreate()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/product");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Produto" }).ClickAsync();
        await Page.GetByLabel("Nome").FillAsync($"Test Product {UniqueId}");
        await Page.GetByLabel("SKU").FillAsync($"SKU-{UniqueId}");
        await Page.GetByLabel("Preço de Venda").FillAsync("100");
        await Page.GetByLabel("Quantidade").FillAsync("10");
        await Page.GetByLabel("Categoria").ClickAsync();
        await Page.GetByRole(AriaRole.Option).First.ClickAsync();
        await Page.GetByLabel("Fornecedor").ClickAsync();
        await Page.GetByRole(AriaRole.Option).First.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();

        await WaitForSuccessAsync("Produto criado(a)");
    }

    [Fact]
    public async Task Product_ShouldEdit()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/product");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Produtos" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasRows = await Page.Locator(".mud-table-body .mud-table-row").CountAsync() > 0;
        if (!hasRows)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Produto" }).ClickAsync();
            await Page.GetByLabel("Nome").FillAsync($"Edit Target {UniqueId}");
            await Page.GetByLabel("SKU").FillAsync($"SKU-{UniqueId}");
            await Page.GetByLabel("Preço de Venda").FillAsync("100");
            await Page.GetByLabel("Quantidade").FillAsync("10");
            await Page.GetByLabel("Categoria").ClickAsync();
            await Page.GetByRole(AriaRole.Option).First.ClickAsync();
            await Page.GetByLabel("Fornecedor").ClickAsync();
            await Page.GetByRole(AriaRole.Option).First.ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();
            await WaitForSuccessAsync("Produto criado(a)");
            await Page.GetByText($"Edit Target {UniqueId}").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        await Page.Locator(".mud-table-body .mud-table-row").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-icon-button").First.ClickAsync();
        await Page.GetByLabel("Preço de Venda").FillAsync("200");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Atualizar" }).ClickAsync();

        await WaitForSuccessAsync("Produto atualizado(a)");
    }

    [Fact]
    public async Task Product_ShouldDelete()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/product");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Produtos" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasRows = await Page.Locator(".mud-table-body .mud-table-row").CountAsync() > 0;
        if (!hasRows)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Produto" }).ClickAsync();
            await Page.GetByLabel("Nome").FillAsync($"Delete Target {UniqueId}");
            await Page.GetByLabel("SKU").FillAsync($"SKU-{UniqueId}");
            await Page.GetByLabel("Preço de Venda").FillAsync("100");
            await Page.GetByLabel("Quantidade").FillAsync("10");
            await Page.GetByLabel("Categoria").ClickAsync();
            await Page.GetByRole(AriaRole.Option).First.ClickAsync();
            await Page.GetByLabel("Fornecedor").ClickAsync();
            await Page.GetByRole(AriaRole.Option).First.ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();
            await WaitForSuccessAsync("Produto criado(a)");
            await Page.GetByText($"Delete Target {UniqueId}").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        var firstName = await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-table-cell").Nth(1).TextContentAsync();

        await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-icon-button").Nth(1).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Excluir" }).Last.ClickAsync();

        await WaitForSuccessAsync("Produto excluído(a)");
        await Page.GetByText(firstName!).WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }
}
