using Fenicia.Web.E2ETests;
using Microsoft.Playwright;
using Xunit;

namespace Fenicia.Web.E2ETests.Tests;

[CollectionDefinition("BrowserTests")]
public class BrowserTestsDefinition : ICollectionFixture<BrowserFixture>
{
}

[Collection("BrowserTests")]
public class HomePageTests(BrowserFixture fixture) : IAsyncLifetime
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
    public async Task HomePage_ShouldLoadSuccessfully()
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "http://localhost:5104";
        await _page.GotoAsync(baseUrl);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var title = await _page.TitleAsync();
        title.Should().NotBeNullOrEmpty();
    }
}
