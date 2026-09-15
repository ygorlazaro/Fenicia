using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Layout.Elements;

public partial class SideDrawer
{
    [Parameter]

    public bool Open { get; set; }

    [Parameter]

    public EventCallback<bool> OpenChanged { get; set; }

    [Parameter]

    public bool IsLoggedIn { get; set; }
}
