using Microsoft.Playwright;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class BasicProductCategoryTests(BrowserFixture fixture) : BasicTestBase(fixture)
{
    [Fact]
    public async Task ProductCategory_ListShouldLoad()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/productcategory");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Categorias de Produtos" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task ProductCategory_ShouldCreate()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/productcategory");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Categoria" }).ClickAsync();
        await Page.GetByLabel("Nome da Categoria").FillAsync($"Test Cat {UniqueId}");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();

        await WaitForSuccessAsync("Categoria criado(a)");
    }

    [Fact]
    public async Task ProductCategory_ShouldEdit()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/productcategory");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Categorias de Produtos" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasRows = await Page.Locator(".mud-table-body .mud-table-row").CountAsync() > 0;
        if (!hasRows)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Categoria" }).ClickAsync();
            await Page.GetByLabel("Nome da Categoria").FillAsync($"Edit Target {UniqueId}");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();
            await WaitForSuccessAsync("Categoria criado(a)");
            await Page.GetByText($"Edit Target {UniqueId}").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        await Page.Locator(".mud-table-body .mud-table-row").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-icon-button").First.ClickAsync();
        await Page.GetByLabel("Nome da Categoria").FillAsync($"Edited {UniqueId}");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Atualizar" }).ClickAsync();

        await WaitForSuccessAsync("Categoria atualizado(a)");
    }

    [Fact]
    public async Task ProductCategory_ShouldDelete()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/productcategory");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Categorias de Produtos" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasRows = await Page.Locator(".mud-table-body .mud-table-row").CountAsync() > 0;
        if (!hasRows)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Categoria" }).ClickAsync();
            await Page.GetByLabel("Nome da Categoria").FillAsync($"Delete Target {UniqueId}");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();
            await WaitForSuccessAsync("Categoria criado(a)");
            await Page.GetByText($"Delete Target {UniqueId}").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        var firstName = await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-table-cell").Nth(1).TextContentAsync();

        await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-icon-button").Nth(1).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Excluir" }).Last.ClickAsync();

        await WaitForSuccessAsync("Categoria excluído(a)");
        await Page.GetByText(firstName!).WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }
}
