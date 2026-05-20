using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.State.Queries;
using Fenicia.Module.Basic.Domains.State.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State.Handlers;

/// <summary>
///     Handler responsible for retrieving all Brazilian states.
///     Returns a list of states ordered by their UF code.
/// </summary>
public class GetAllStateHandler(DefaultContext db) : IRequestHandler<GetAllStateQuery, List<GetAllStateResponse>>
{
    /// <summary>
    ///     Retrieves all states.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of all states ordered by UF code.</returns>
    public async Task<List<GetAllStateResponse>> Handle(GetAllStateQuery query, CancellationToken ct)
    {
        return await db.AuthStates.OrderBy(s => s.Uf).Select(s => new GetAllStateResponse(s.Id, s.Name, s.Uf)).ToListAsync(ct);
    }
}
