using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public enum TypeFilter
{
    All,
    In,
    Out
}

public partial class StockMovementFilters
{
    [Parameter]
    public string? SearchText { get; set; }

    [Parameter]
    public EventCallback<string?> SearchTextChanged { get; set; }

    [Parameter]
    public DateTime? StartDate { get; set; }

    [Parameter]
    public EventCallback<DateTime?> StartDateChanged { get; set; }

    [Parameter]
    public DateTime? EndDate { get; set; }

    [Parameter]
    public EventCallback<DateTime?> EndDateChanged { get; set; }

    [Parameter]
    public TypeFilter TypeFilter { get; set; } = TypeFilter.All;

    [Parameter]
    public EventCallback<TypeFilter> TypeFilterChanged { get; set; }

    [Parameter]
    public bool Loading { get; set; }

    [Parameter]
    public EventCallback OnFilter { get; set; }
}
