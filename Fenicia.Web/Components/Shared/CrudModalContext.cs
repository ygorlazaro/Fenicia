namespace Fenicia.Web.Components.Shared;

using Fenicia.Common;

public sealed record CrudModalContext<TItem>(CrudModalMode Mode, TItem? Item, CrudPage<TItem> Page)
    where TItem : Fenicia.Common.ICrudItem
{
    public Guid Id => Item?.Id ?? Guid.Empty;

    public bool IsAdd => Mode == CrudModalMode.Add;

    public bool IsEdit => Mode == CrudModalMode.Edit;
}