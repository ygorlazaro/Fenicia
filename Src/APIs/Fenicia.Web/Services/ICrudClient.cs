using MudBlazor;

namespace Fenicia.Web.Services;

public interface ICrudClient
{
    Task<TableData<TItem>> GetPageAsync<TItem>(
        string endpoint,
        TableState state,
        string? search = null,
        Dictionary<string, string>? filters = null,
        CancellationToken ct = default);

    Task<HttpResponseMessage> PostAsync<TPayload>(string endpoint, TPayload payload, CancellationToken ct);

    Task<HttpResponseMessage> PatchAsync<TPayload>(string endpoint, Guid id, TPayload payload, CancellationToken ct);

    Task<HttpResponseMessage> DeleteAsync(string endpoint, Guid id, CancellationToken ct);
}