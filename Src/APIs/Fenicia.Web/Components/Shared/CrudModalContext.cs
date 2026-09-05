namespace Fenicia.Web.Components.Shared;

using Fenicia.Web.Services;

public sealed record CrudModalContext<TItem>(CrudModalMode Mode, TItem? Item, CrudPage<TItem> Page)
    where TItem : ICrudItem
{
    public Guid Id => Item?.Id ?? Guid.Empty;

    public bool IsAdd => Mode == CrudModalMode.Add;

    public bool IsEdit => Mode == CrudModalMode.Edit;
}