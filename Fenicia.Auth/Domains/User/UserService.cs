using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Auth.Domains.UserRole.Responses;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class UserService(DefaultContext db)
{
    public async Task<Pagination<List<UserListItemResponse>>> GetAllAsync(int page, int perPage, CancellationToken ct)
    {
        var request = db.AuthUsers.OrderBy(u => u.Name);
        var totalCount = await request.CountAsync(ct);

        var users = await request.Skip((page - 1) * perPage).Take(perPage).Select(u => new UserListItemResponse(u.Id, u.Name, u.Email)).ToListAsync(ct);

        return new Pagination<List<UserListItemResponse>>(users, totalCount, page, perPage);
    }

    public async Task<GetUserByIdResponse?> GetByIdAsync(Guid userId, CancellationToken ct)
    {
        var request = db.AuthUsers.Where(u => u.Id == userId).Select(u => new GetUserByIdResponse(u.Id, u.Name, u.Email));

        return await request.FirstOrDefaultAsync(ct);
    }

    public async Task<GetByEmailResponse?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await db.AuthUsers.Where(user => user.Email == email)
            .Select(user => new GetByEmailResponse(user.Id,
                user.Email,
                user.Name,
                user.Password))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
    {
        return await db.AuthUsers.AnyEmailAsync(email, ct);
    }

    public async Task<GetUserForRefreshResponse> GetForRefreshAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByIdAsync(userId, ct);

        return new GetUserForRefreshResponse(user.Id, user.Email, user.Name);
    }

    public async Task<List<GetUserCompaniesResponse>> GetCompaniesAsync(Guid userId, CancellationToken ct)
    {
        var query = from ur in db.AuthUserRoles
                    join c in db.AuthCompanies on ur.CompanyId equals c.Id
                    join r in db.AuthRoles on ur.RoleId equals r.Id
                    where ur.UserId == userId
                    select new GetUserCompaniesResponse(c.Id, r.Name, c.Id, c.Name, c.Cnpj);

        return await query.ToListAsync(ct);
    }

    public async Task<CreateUserResponse> CreateAsync(CreateUserCommand command, CancellationToken ct)
    {
        var userExists = await db.AuthUsers.AnyEmailAsync(command.Email, ct);

        if (userExists)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        var hashedPassword = command.Password.Hash();

        var user = new UserModel
        {
            Email = command.Email,
            Password = hashedPassword,
            Name = command.Name
        };

        db.AuthUsers.Add(user);
        await RelateRolesAsync(user.Id, command.Roles, ct);
        await db.SaveChangesAsync(ct);

        return new CreateUserResponse(user.Id, user.Name, user.Email);
    }

    public async Task<CreateNewUserResponse> CreateNewAsync(CreateNewUserCommand command, CancellationToken ct)
    {
        await ValidateAsync(command, ct);

        var (user, company) = await PersistAsync(command, ct);
        var companyResponse = new CreateNewUserCompanyResponse(company.Id, company.Name, company.Cnpj);

        return new CreateNewUserResponse(user.Id, user.Name, user.Email, companyResponse);
    }

    public async Task<UpdateUserResponse> UpdateAsync(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByIdAsync(command.UserId, ct);

        await ValidateFields(user, command, ct);

        var companies = command.CompaniesRoles?.Select(c => c.CompanyId) ?? [];

        await ValidateCompanies(companies, ct);
        await RelateRolesAsync(command, user, ct);
        await db.SaveChangesAsync(ct);

        return new UpdateUserResponse(user.Id, user.Name, user.Email);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByIdAsync(userId, ct);

        user.Deleted = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }

    public async Task<UpdateUserPasswordResponse> UpdatePasswordAsync(UpdateUserPasswordCommand command, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByIdAsync(command.UserId, ct);
        var hashedPassword = command.Password.Hash();

        user.Password = hashedPassword;

        await db.SaveChangesAsync(ct);

        return new UpdateUserPasswordResponse(true, "Password changed successfully");
    }

    public async Task<UpdatePasswordResponse> UpdateHashedPasswordAsync(UpdatePasswordCommand command, CancellationToken ct)
    {
        var user = await db.AuthUsers.UpdatePasswordAsync(command.UserId, command.Password, ct) ?? throw new ItemNotExistsException(ExceptionMessages.UserNotFound);
        db.Entry(user).State = EntityState.Modified;

        await db.SaveChangesAsync(ct);

        return new UpdatePasswordResponse(user.Id, user.Name, user.Email);
    }

    private async Task RelateRolesAsync(Guid userId, List<CreateUserRoleCommand>? command, CancellationToken ct)
    {
        var roles = command ?? [];
        await ValidateCompanies(roles.Select(r => r.CompanyId), ct);
        await ValidateRoles(roles.Select(r => r.RoleId), ct);

        var userRoles = roles.Select(r => new UserRoleModel
        {
            UserId = userId,
            RoleId = r.RoleId,
            CompanyId = r.CompanyId
        });

        db.AuthUserRoles.AddRange(userRoles);
    }

    private async Task ValidateCompanies(IEnumerable<Guid> companies, CancellationToken ct)
    {
        var distinct = companies.Distinct();

        var query = db.AuthCompanies.Where(c => distinct.Contains(c.Id));

        if (distinct.Count() != await query.CountAsync(ct))
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundMessage);
        }
    }

    private async Task ValidateRoles(IEnumerable<Guid> roles, CancellationToken ct)
    {
        var distinct = roles.Distinct();

        var query = db.AuthRoles.Where(r => distinct.Contains(r.Id));

        if (distinct.Count() != await query.CountAsync(ct))
        {
            throw new InvalidRequestException(ExceptionMessages.RoleNotFound);
        }
    }

    private async Task<(UserModel user, CompanyModel company)> PersistAsync(CreateNewUserCommand command, CancellationToken ct)
    {
        var existingUser = await db.AuthUsers.AnyEmailAsync(command.Email, ct);

        if (existingUser)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        var existingCompany = await db.AuthCompanies.AnyCnpjAsync(command.Company.Cnpj, ct);

        if (existingCompany)
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyExists);
        }

        var hashedPassword = command.Password.Hash();
        var user = new UserModel
        {
            Email = command.Email,
            Password = hashedPassword,
            Name = command.Name
        };

        db.AuthUsers.Add(user);

        var company = new CompanyModel
        {
            Name = command.Company.Name,
            Cnpj = command.Company.Cnpj
        };

        db.AuthCompanies.Add(company);

        var adminRole = await db.AuthRoles.GetRoleAsync("Admin", ct) ?? throw new InvalidRequestException(ExceptionMessages.AdminRoleNotFound);
        var userRole = new UserRoleModel
        {
            UserId = user.Id,
            Company = company,
            RoleId = adminRole.Id
        };

        db.AuthUserRoles.Add(userRole);

        await db.SaveChangesAsync(ct);
        return (user, company);
    }

    private async Task ValidateAsync(CreateNewUserCommand request, CancellationToken ct)
    {
        var isExistingUser = await db.AuthUsers.AnyEmailAsync(request.Email, ct);
        var isExistingCompany = await db.AuthCompanies.AnyCnpjAsync(request.Company.Cnpj, ct);

        if (isExistingUser)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        if (isExistingCompany)
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundWithCNPJ);
        }
    }

    private async Task RelateRolesAsync(UpdateUserCommand command, UserModel user, CancellationToken ct)
    {
        var requestedRoles = command.CompaniesRoles ?? [];

        if (requestedRoles.Count == 0)
        {
            var existing = await db.AuthUserRoles.Where(x => x.UserId == user.Id).ToListAsync(ct);

            db.AuthUserRoles.RemoveRange(existing);
            return;
        }

        var requestedRoleIds = requestedRoles.Select(r => r.RoleId).Distinct().ToList();

        var validRoleIds = await db.AuthRoles.Where(r => requestedRoleIds.Contains(r.Id)).Select(r => r.Id).ToListAsync(ct);

        if (validRoleIds.Count != requestedRoleIds.Count)
        {
            var missingRoles = requestedRoleIds.Except(validRoleIds);

            throw new InvalidRequestException($"Role(s) not found: {string.Join(", ", missingRoles)}");
        }

        var requestedSet = requestedRoles.Select(r => (r.CompanyId, r.RoleId)).ToHashSet();

        var existingRoles = await db.AuthUserRoles.Where(x => x.UserId == user.Id).ToListAsync(ct);

        var existingSet = existingRoles.Select(r => (r.CompanyId, r.RoleId)).ToHashSet();

        var toRemove = existingRoles.Where(r => !requestedSet.Contains((r.CompanyId, r.RoleId))).ToList();

        var toInsert = requestedRoles.Where(r => !existingSet.Contains((r.CompanyId, r.RoleId))).Select(r => new UserRoleModel
        {
            UserId = user.Id,
            CompanyId = r.CompanyId,
            RoleId = r.RoleId
        }).ToList();

        if (toRemove.Count > 0)
        {
            db.AuthUserRoles.RemoveRange(toRemove);
        }

        if (toInsert.Count > 0)
        {
            db.AuthUserRoles.AddRange(toInsert);
        }
    }

    private async Task ValidateFields(UserModel user, UpdateUserCommand command, CancellationToken ct)
    {
        user.Name = string.IsNullOrWhiteSpace(command.Name) switch
        {
            false => command.Name,
            _ => user.Name
        };

        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return;
        }

        var emailExists = await db.AuthUsers.AnyAsync(u => u.Email == command.Email && u.Id != command.UserId, ct);

        user.Email = emailExists switch
        {
            true => throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists),
            _ => command.Email
        };
    }

}

