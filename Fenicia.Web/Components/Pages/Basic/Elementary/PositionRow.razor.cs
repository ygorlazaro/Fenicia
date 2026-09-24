using Fenicia.Common.DTOs.Basic.Position;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class PositionRow
{
    [Parameter]
    public GetAllPositionResponse Item { get; set; } = null!;

    [Parameter]
    public Fenicia.Web.Components.Shared.CrudPage<GetAllPositionResponse> Page { get; set; } = null!;
}
