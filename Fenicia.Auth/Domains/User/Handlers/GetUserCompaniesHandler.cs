using Fenicia.Auth.Domains.User.Queries;
using Fenicia.Auth.Domains.UserRole.Responses;
using Fenicia.Common.Data.Contexts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Handlers;

public class GetUserCompaniesHandler(DefaultContext db) : IRequestHandler<GetUserCompaniesQuery, List<GetUserCompaniesResponse>>
{
    public async Task<List<GetUserCompaniesResponse>> Handle(GetUserCompaniesQuery query, CancellationToken ct)
    {
        var request = from ur in db.AuthUserRoles
                      join c in db.AuthCompanies on ur.CompanyId equals c.Id
                      where ur.UserId == query.UserId
                      select new GetUserCompaniesResponse(c.Id,
                          ur.Role.Name,
                          c.Id,
                          c.Name,
                          c.Cnpj);

        return await request.ToListAsync(ct);
    }
}
