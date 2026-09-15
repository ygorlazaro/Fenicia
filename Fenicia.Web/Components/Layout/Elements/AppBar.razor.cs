using Fenicia.Web.Components.Layout.Models;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Layout.Elements;

public partial class AppBar
{
    [Parameter]

    public bool IsLoggedIn { get; set; }

    [Parameter]

    public string UserName { get; set; } = string.Empty;

    [Parameter]

    public string UserRole { get; set; } = string.Empty;

    [Parameter]

    public string? UserImageUrl { get; set; }

    [Parameter]

    public string SelectedCompanyName { get; set; } = string.Empty;

    [Parameter]

    public List<UserCompanyItem> Companies { get; set; } = [];

    [Parameter]

    public EventCallback OnToggleDrawer { get; set; }

    private static string GetInitials(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return "?";
        }

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return parts.Length == 1 ? parts[0][..1].ToUpper() : (parts[0][..1] + parts[^1])[..1].ToUpper();
    }

    private Task HandleToggleDrawer()
    {
        return OnToggleDrawer.InvokeAsync();
    }
}
