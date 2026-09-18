using Fenicia.Web.E2ETests;
using Microsoft.Playwright;
using Xunit;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class BasicPositionTests(BrowserFixture fixture) : BasicTestBase(fixture)
{
    [Fact]
    public async Task Position_ListShouldLoad()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/position");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Cargos" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Position_ShouldCreate()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/position");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Cargo" }).ClickAsync();
        await Page.GetByLabel("Nome do Cargo").FillAsync($"Test Position {UniqueId}");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();

        await WaitForSuccessAsync("Cargo criado(a)");
    }

    [Fact]
    public async Task Position_ShouldEdit()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/position");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Cargos" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasRows = await Page.Locator(".mud-table-body .mud-table-row").CountAsync() > 0;
        if (!hasRows)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Cargo" }).ClickAsync();
            await Page.GetByLabel("Nome do Cargo").FillAsync($"Edit Target {UniqueId}");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();
            await WaitForSuccessAsync("Cargo criado(a)");
            await Page.GetByText($"Edit Target {UniqueId}").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        await Page.Locator(".mud-table-body .mud-table-row").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-icon-button").First.ClickAsync();
        await Page.GetByLabel("Nome do Cargo").FillAsync($"Edited Pos {UniqueId}");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Atualizar" }).ClickAsync();

        await WaitForSuccessAsync("Cargo atualizado(a)");
    }

    [Fact]
    public async Task Position_ShouldDelete()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/position");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Cargos" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasRows = await Page.Locator(".mud-table-body .mud-table-row").CountAsync() > 0;
        if (!hasRows)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Cargo" }).ClickAsync();
            await Page.GetByLabel("Nome do Cargo").FillAsync($"Delete Target {UniqueId}");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();
            await WaitForSuccessAsync("Cargo criado(a)");
            await Page.GetByText($"Delete Target {UniqueId}").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        var firstName = await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-table-cell").Nth(1).TextContentAsync();

        await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-icon-button").Nth(1).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Excluir" }).Last.ClickAsync();

        await WaitForSuccessAsync("Cargo excluído(a)");
        await Page.GetByText(firstName!).WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }
}
