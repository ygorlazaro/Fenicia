using Fenicia.Auth.Domains.Company.DTOs;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyService(DefaultContext db)
{
    public async Task<Pagination<IEnumerable<GetCompaniesByUserResponse>>> GetCompaniesByUserAsync(Guid userId, int page, int perPage, CancellationToken ct)
    {
        if (perPage <= 0)
        {
            throw new InvalidRequestException(ExceptionMessages.UserNotAssociatedWithActiveCompanies);
        }

        var request = from ur in db.AuthUserRoles
                      where ur.UserId == userId && ur.Company.IsActive
                      select ur;
        var total = await request.CountAsync(ct);
        var items = await request
            .OrderBy(ur => ur.Company.Name)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);

        var result = items.Select(ur => ur.MapToGetCompaniesByUserResponse());

        return new Pagination<IEnumerable<GetCompaniesByUserResponse>>(result, total, page, perPage);
    }

    public async Task UpdateAsync(Guid companyId, Guid userId, string name, CancellationToken ct)
    {
        var company = await AnyActiveAsync(companyId, ct) ?? throw new ItemNotExistsException(ExceptionMessages.CompanyNotFoundMessage);
        var isAdmin = await db.AuthUserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == "Admin", ct);

        if (!isAdmin)
        {
            throw new PermissionDeniedException(ExceptionMessages.PermissionDeniedUpdateCompany);
        }

        company.Name = name;

        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> CheckExistsAsync(string cnpj, bool onlyActive, CancellationToken ct)
    {
        return await db.AuthCompanies.AnyAsync(c => c.Cnpj == cnpj && (!onlyActive || c.IsActive), ct);
    }

    public async Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken ct)
    {
        return await db.AuthCompanies.FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, ct);
    }

    public async Task<bool> AnyAsync(Guid companyId, CancellationToken ct)
    {
        return await db.AuthCompanies.AnyAsync(c => c.Id == companyId, ct);
    }
}
