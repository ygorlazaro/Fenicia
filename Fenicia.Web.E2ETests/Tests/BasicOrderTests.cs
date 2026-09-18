using Fenicia.Web.E2ETests;
using Microsoft.Playwright;
using Xunit;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class BasicOrderTests(BrowserFixture fixture) : BasicTestBase(fixture)
{
    [Fact]
    public async Task Order_ListShouldLoad()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/basic/order");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Pedidos", Exact = true }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Order_ShouldCreate()
    {
        await LoginAndSelectCompanyAsync();
        await EnsureProductExistsAsync();

        await NavigateAsync("/basic/order/new");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Carrinho" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await Page.Locator("mud-progress-circular").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        await Page.Locator(".pdv-product-card").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasProducts = await Page.Locator(".pdv-product-card").CountAsync() > 0;
        if (!hasProducts)
        {
            throw new InvalidOperationException("No products available in the order form. EnsureProductExistsAsync may have failed to create an active product for the current company.");
        }

        await Page.Locator(".pdv-product-card").First.ClickAsync();

        await Page.GetByLabel("Cliente").ClickAsync();
        await Page.GetByRole(AriaRole.Option).First.ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Finalizar Pedido" }).ClickAsync();

        await Page.GetByText("Pedido finalizado").WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Order_ShouldViewDetail()
    {
        await LoginAndSelectCompanyAsync();
        await EnsureProductExistsAsync();
        await NavigateAsync("/basic/order");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Pedidos", Exact = true }).WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var hasRows = await Page.Locator(".mud-table-body .mud-table-row").CountAsync() > 0;
        if (!hasRows)
        {
            await NavigateAsync("/basic/order/new");

            await Page.GetByRole(AriaRole.Heading, new() { Name = "Carrinho" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await Page.Locator("mud-progress-circular").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

            var hasProducts = await Page.Locator(".pdv-product-grid").CountAsync() > 0;
            if (!hasProducts)
            {
                throw new InvalidOperationException("No products available in the order form. EnsureProductExistsAsync may have failed to create an active product for the current company.");
            }

            await Page.Locator(".pdv-product-card").First.ClickAsync();

            await Page.GetByLabel("Cliente").ClickAsync();
            await Page.GetByRole(AriaRole.Option).First.ClickAsync();

            await Page.GetByRole(AriaRole.Button, new() { Name = "Finalizar Pedido" }).ClickAsync();
            await Page.GetByText("Pedido finalizado").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            await NavigateAsync("/basic/order");
            await Page.GetByRole(AriaRole.Heading, new() { Name = "Pedidos", Exact = true }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        await Page.Locator(".mud-table-body .mud-table-row").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator(".mud-table-body .mud-table-row").First.Locator(".mud-icon-button").First.ClickAsync();

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Itens do pedido" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }
}
