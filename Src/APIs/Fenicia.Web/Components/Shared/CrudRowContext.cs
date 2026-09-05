namespace Fenicia.Web.Components.Shared;

using Fenicia.Web.Services;

public sealed record CrudRowContext<TItem>(TItem Item, CrudPage<TItem> Page)

    where TItem : ICrudItem;