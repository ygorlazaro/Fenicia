namespace Fenicia.Web.Components.Shared;

using Fenicia.Common;

public sealed record CrudRowContext<TItem>(TItem Item, CrudPage<TItem> Page)

    where TItem : Fenicia.Common.ICrudItem;