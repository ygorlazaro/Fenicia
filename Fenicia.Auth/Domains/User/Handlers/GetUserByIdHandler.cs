using Fenicia.Auth.Domains.User.Queries;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Handlers;

public class GetUserByIdHandler(DefaultContext db) : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse?>
{
    public async Task<GetUserByIdResponse?> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var request = db.AuthUsers.Where(u => u.Id == query.UserId).Select(u => new GetUserByIdResponse(u.Id, u.Name, u.Email));

        return await request.FirstOrDefaultAsync(ct);
    }
}
