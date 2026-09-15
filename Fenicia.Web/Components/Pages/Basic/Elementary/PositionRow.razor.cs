using Fenicia.Common.DTOs.Basic.Position;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class PositionRow
{
    [Parameter]
    public GetAllPositionResponse Item { get; set; } = default!;

    [Parameter]
    public Fenicia.Web.Components.Shared.CrudPage<GetAllPositionResponse> Page { get; set; } = default!;
}
