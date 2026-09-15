using Fenicia.Common.DTOs.Basic.Position;
using Fenicia.Common.DTOs.Basic.State;
using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class EmployeeForm
{
    [Parameter]
    public EmployeeFormModel Model { get; set; } = new();

    [Parameter]
    public IReadOnlyList<StateOption>? States { get; set; }

    [Parameter]
    public IReadOnlyList<PositionOption>? Positions { get; set; }
}
