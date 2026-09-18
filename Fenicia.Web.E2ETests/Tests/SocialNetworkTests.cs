using Fenicia.Web.E2ETests;
using Microsoft.Playwright;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace Fenicia.Web.E2ETests.Tests;

[Collection("BrowserTests")]
public class SocialNetworkTests(BrowserFixture fixture) : BasicTestBase(fixture)
{
    private const string SocialApiBaseUrl = "http://localhost:5026";

    [Fact]
    public async Task Social_Feed_ShouldLoad()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/socialnetwork/feed");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Publicações" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator("mud-progress-circular").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }

    [Fact]
    public async Task Social_Feed_ShouldCreatePost()
    {
        await LoginAndSelectCompanyAsync();
        await NavigateAsync("/socialnetwork/feed");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Publicações" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator("mud-progress-circular").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        var postText = $"E2E Test Post {UniqueId}";

        await Page.GetByPlaceholder("O que está acontecendo?").FillAsync(postText);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Postar" }).ClickAsync();

        await Page.GetByText("Publicação publicada!").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.GetByText(postText).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Social_Feed_ShouldLikePost()
    {
        await LoginAndSelectCompanyAsync();
        var postId = await EnsureSocialFeedExistsAsync();
        await NavigateAsync("/socialnetwork/feed");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Publicações" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator("mud-progress-circular").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        var likeButton = Page.Locator(".post-card").First.Locator("button").Nth(1);
        await likeButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var initialLikes = (await likeButton.TextContentAsync())!.Trim();
        await likeButton.ClickAsync();

        await Page.GetByText(new Regex($"^(?!{Regex.Escape(initialLikes)}$).+$"), new() { Exact = false }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    [Fact]
    public async Task Social_FeedDetail_ShouldLoad()
    {
        await LoginAndSelectCompanyAsync();
        var postId = await EnsureSocialFeedExistsAsync();
        await NavigateAsync($"/socialnetwork/feed/{postId}");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Publicação" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator("mud-progress-circular").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }

    [Fact]
    public async Task Social_Feed_ShouldComment()
    {
        await LoginAndSelectCompanyAsync();
        var postId = await EnsureSocialFeedExistsAsync();
        await NavigateAsync($"/socialnetwork/feed/{postId}");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Publicação" }).WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.Locator("mud-progress-circular").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        var commentText = $"E2E Test Comment {UniqueId}";
        await Page.GetByPlaceholder("Escreva um comentário...").FillAsync(commentText);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Comentar" }).ClickAsync();

        await Page.GetByText(commentText).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    private async Task<Guid> EnsureSocialFeedExistsAsync()
    {
        var token = await Page.EvaluateAsync<string>("() => localStorage.getItem('auth_token')");
        var companyId = await Page.EvaluateAsync<string>("() => localStorage.getItem('selected_company_id')");

        if (string.IsNullOrEmpty(companyId))
        {
            throw new InvalidOperationException("Missing selected_company_id in localStorage.");
        }

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Missing auth_token in localStorage.");
        }

        using var client = new HttpClient();
        client.BaseAddress = new Uri(SocialApiBaseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("CompanyId", companyId);

        using var profileContent = new StringContent("{}");
        profileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var profileResponse = await client.PostAsync("/profile", profileContent);
        profileResponse.EnsureSuccessStatusCode();

        var profileJson = await profileResponse.Content.ReadAsStringAsync();
        using var profileDoc = JsonDocument.Parse(profileJson);
        var profileId = profileDoc.RootElement.GetProperty("id").GetGuid();

        var feedResponse = await client.GetAsync("/feed?page=1&perPage=1");
        if (feedResponse.IsSuccessStatusCode)
        {
            var feedJson = await feedResponse.Content.ReadAsStringAsync();
            using var feedDoc = JsonDocument.Parse(feedJson);
            if (feedDoc.RootElement.EnumerateArray().Any())
            {
                return feedDoc.RootElement[0].GetProperty("id").GetGuid();
            }
        }

        var postId = Guid.NewGuid();
        var postPayload = new
        {
            id = postId,
            date = DateTime.UtcNow,
            text = $"E2E Social Post {UniqueId}",
            profileId = profileId,
            originalFeedId = (Guid?)null
        };

        var createResponse = await client.PostAsJsonAsync("/feed", postPayload);
        createResponse.EnsureSuccessStatusCode();

        for (var attempt = 0; attempt < 20; attempt++)
        {
            var verify = await client.GetAsync($"/feed/{postId}");
            if (verify.IsSuccessStatusCode)
            {
                return postId;
            }

            await Task.Delay(500);
        }

        throw new InvalidOperationException($"Post {postPayload.text} was created but is not visible.");
    }
}
