using Fenicia.Auth.Domains.UserRole.Queries;
using Fenicia.Auth.Domains.UserRole.Responses;
using Fenicia.Common.Data.Contexts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole.Handlers;

public class GetUserCompaniesHandler(DefaultContext db) : IRequestHandler<GetUserCompaniesQuery, List<GetUserCompaniesResponse>>
{
    public async Task<List<GetUserCompaniesResponse>> Handle(GetUserCompaniesQuery request, CancellationToken ct)
    {
        var query = from ur in db.AuthUserRoles
                    join c in db.AuthCompanies on ur.CompanyId equals c.Id
                    where ur.UserId == request.UserId
                    select new GetUserCompaniesResponse(c.Id,
                        ur.Role.Name,
                        c.Id,
                        c.Name,
                        c.Cnpj);

        return await query.ToListAsync(ct);
    }

    public async Task<List<GetUserCompaniesResponse>> Handle(Guid userId, CancellationToken ct)
    {
        return await Handle(new GetUserCompaniesQuery(userId), ct);
    }
}
