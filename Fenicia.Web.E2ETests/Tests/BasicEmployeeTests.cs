using Microsoft.Playwright;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class BasicEmployeeTests(BrowserFixture fixture) : BasicTestBase(fixture)
{
    [Fact]
    public async Task Employee_ListShouldLoad()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/employee");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funcionários" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Employee_ShouldCreate()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/employee");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Funcionário" }).ClickAsync();
        await Page.GetByLabel("Nome").FillAsync($"Test Employee {UniqueId}");
        await Page.GetByLabel("Cargo").ClickAsync();
        await Page.GetByRole(AriaRole.Option).First.ClickAsync();
        await Page.GetByLabel("Email").FillAsync($"employee-{UniqueId}@example.com");
        await Page.GetByLabel("Telefone").FillAsync("11999999999");
        await Page.GetByLabel("Documento (CPF)").FillAsync(Document);
        await Page.GetByLabel("Rua").FillAsync("Rua Teste");
        await Page.GetByLabel("Número").FillAsync("789");
        await Page.GetByLabel("CEP").FillAsync("01234567");
        await Page.GetByLabel("Cidade").FillAsync("Sao Paulo");
        await Page.GetByLabel("Estado").ClickAsync();
        await Page.GetByText("SP — São Paulo").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();

        await WaitForSuccessAsync("Funcionário criado(a)");
    }

    [Fact]
    public async Task Employee_ShouldEdit()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/employee");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funcionários" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasRows = await Page.Locator(".mud-table-body .mud-table-row").CountAsync() > 0;
        if (!hasRows)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Funcionário" }).ClickAsync();
            await Page.GetByLabel("Nome").FillAsync($"Edit Target {UniqueId}");
            await Page.GetByLabel("Cargo").ClickAsync();
            await Page.GetByRole(AriaRole.Option).First.ClickAsync();
            await Page.GetByLabel("Email").FillAsync($"employee-{UniqueId}@example.com");
            await Page.GetByLabel("Telefone").FillAsync("11999999999");
            await Page.GetByLabel("Documento (CPF)").FillAsync(Document);
            await Page.GetByLabel("Rua").FillAsync("Rua Teste");
            await Page.GetByLabel("Número").FillAsync("789");
            await Page.GetByLabel("CEP").FillAsync("01234567");
            await Page.GetByLabel("Cidade").FillAsync("Sao Paulo");
            await Page.GetByLabel("Estado").ClickAsync();
            await Page.GetByText("SP — São Paulo").ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();
            await WaitForSuccessAsync("Funcionário criado(a)");
            await Page.GetByText($"Edit Target {UniqueId}").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        await Page.Locator(".mud-table-body .mud-table-row").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-icon-button").First.ClickAsync();
        await Page.GetByLabel("Telefone").FillAsync("11888888888");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Atualizar" }).ClickAsync();

        await WaitForSuccessAsync("Funcionário atualizado(a)");
    }

    [Fact]
    public async Task Employee_ShouldDelete()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/employee");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funcionários" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasRows = await Page.Locator(".mud-table-body .mud-table-row").CountAsync() > 0;
        if (!hasRows)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar Funcionário" }).ClickAsync();
            await Page.GetByLabel("Nome").FillAsync($"Delete Target {UniqueId}");
            await Page.GetByLabel("Cargo").ClickAsync();
            await Page.GetByRole(AriaRole.Option).First.ClickAsync();
            await Page.GetByLabel("Email").FillAsync($"employee-{UniqueId}@example.com");
            await Page.GetByLabel("Telefone").FillAsync("11999999999");
            await Page.GetByLabel("Documento (CPF)").FillAsync(Document);
            await Page.GetByLabel("Rua").FillAsync("Rua Teste");
            await Page.GetByLabel("Número").FillAsync("789");
            await Page.GetByLabel("CEP").FillAsync("01234567");
            await Page.GetByLabel("Cidade").FillAsync("Sao Paulo");
            await Page.GetByLabel("Estado").ClickAsync();
            await Page.GetByText("SP — São Paulo").ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Criar" }).ClickAsync();
            await WaitForSuccessAsync("Funcionário criado(a)");
            await Page.GetByText($"Delete Target {UniqueId}").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        var firstName = await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-table-cell").Nth(1).TextContentAsync();

        await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-icon-button").Nth(1).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Excluir" }).Last.ClickAsync();

        await WaitForSuccessAsync("Funcionário excluído(a)");
        await Page.GetByText(firstName!).WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }
}
