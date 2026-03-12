using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Handlers;

public class GetUserByIdHandler(DefaultContext db)
{
    public async Task<GetUserByIdResponse?> Handler(Guid id, CancellationToken ct)
    {
        var request = from u in db.AuthUsers
                      where u.Id == id
                      select new GetUserByIdResponse(u.Id,
                          u.Name,
                          u.Email);

        return await request.FirstOrDefaultAsync(ct);
    }
}