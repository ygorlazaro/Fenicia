using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class UserService(
    UserRepository userRepository,
    UserRoleRepository userRoleRepository,
    RoleRepository roleRepository,
    CompanyRepository companyRepository)
{
    public async Task<Pagination<List<UserListItemResponse>>> GetAllAsync(int page, int perPage, CancellationToken ct)
    {
        var request = userRepository.Query().OrderBy(u => u.Name);
        var totalCount = await request.CountAsync(ct);

        var users = await request.Skip((page - 1) * perPage).Take(perPage).Select(u => new UserListItemResponse(u.Id, u.Name, u.Email)).ToListAsync(ct);

        return new Pagination<List<UserListItemResponse>>(users, totalCount, page, perPage);
    }

    public async Task<GetUserByIdResponse?> GetByIdAsync(Guid userId, CancellationToken ct)
    {
        var request = userRepository.Query().Where(u => u.Id == userId).Select(u => new GetUserByIdResponse(u.Id, u.Name, u.Email));

        return await request.FirstOrDefaultAsync(ct);
    }

    public async Task<GetByEmailResponse?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(email, ct);

        if (user is null)
        {
            return null;
        }

        return new GetByEmailResponse(user.Id, user.Email, user.Name, user.Password);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
    {
        return await userRepository.ExistsByEmailAsync(email, ct);
    }

    public async Task<UserModel> FirstByIdAsync(Guid userId, CancellationToken ct)
    {
        return await userRepository.GetByIdAsync(userId, ct) ?? throw new InvalidRequestException(ExceptionMessages.UserNotFound);
    }

    public async Task<UserModel?> FirstByEmailOrDefaultAsync(string email, CancellationToken ct)
    {
        return await userRepository.GetByEmailAsync(email, ct);
    }

    public async Task<UserModel> UpdatePasswordAsync(Guid userId, string plainPassword, CancellationToken ct)
    {
        var user = await FirstByIdAsync(userId, ct);
        user.Password = SecurityService.Hash(plainPassword);
        return user;
    }

    public virtual async Task<GetUserForRefreshResponse> GetForRefreshAsync(Guid userId, CancellationToken ct)
    {
        var user = await FirstByIdAsync(userId, ct);

        return new GetUserForRefreshResponse(user.Id, user.Email, user.Name);
    }

    public async Task<List<GetUserCompaniesResponse>> GetCompaniesAsync(Guid userId, CancellationToken ct)
    {
        return await userRepository.GetCompaniesAsync(userId, ct);
    }

    public async Task<CreateUserResponse> CreateAsync(CreateUserCommand command, CancellationToken ct)
    {
        var userExists = await userRepository.ExistsByEmailAsync(command.Email, ct);

        if (userExists)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        var hashedPassword = SecurityService.Hash(command.Password);

        var user = new UserModel
        {
            Email = command.Email,
            Password = hashedPassword,
            Name = command.Name
        };

        await userRepository.InsertAsync(user, ct);
        await RelateRolesAsync(user.Id, command.Roles, ct);

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
        var user = await FirstByIdAsync(command.UserId, ct);

        await ValidateFields(user, command, ct);

        var companies = command.CompaniesRoles?.Select(c => c.CompanyId) ?? [];

        await ValidateCompanies(companies, ct);
        await RelateRolesAsync(command, user, ct);

        return new UpdateUserResponse(user.Id, user.Name, user.Email);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken ct)
    {
        var user = await FirstByIdAsync(userId, ct);

        user.Deleted = DateTime.UtcNow;
    }

    public async Task<UpdateUserPasswordResponse> UpdatePasswordAsync(UpdateUserPasswordCommand command, CancellationToken ct)
    {
        var user = await FirstByIdAsync(command.UserId, ct);
        var hashedPassword = SecurityService.Hash(command.Password);

        user.Password = hashedPassword;

        return new UpdateUserPasswordResponse(true, "Password changed successfully");
    }

    public async Task<UpdatePasswordResponse> UpdateHashedPasswordAsync(UpdatePasswordCommand command, CancellationToken ct)
    {
        var user = await UpdatePasswordAsync(command.UserId, command.Password, ct) ?? throw new ItemNotExistsException(ExceptionMessages.UserNotFound);
        await userRepository.UpdateAsync(user.Id, user, ct);

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

        await userRoleRepository.InsertRangeAsync(userRoles, ct);
    }

    private async Task ValidateCompanies(IEnumerable<Guid> companies, CancellationToken ct)
    {
        var distinct = companies.Distinct().ToList();

        foreach (var companyId in distinct)
        {
            var exists = await companyRepository.GetByIdAsync(companyId, ct) ?? throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundMessage);
        }
    }

    private async Task ValidateRoles(IEnumerable<Guid> roles, CancellationToken ct)
    {
        var distinct = roles.Distinct().ToList();

        foreach (var roleId in distinct)
        {
            var exists = await roleRepository.GetByIdAsync(roleId, ct) ?? throw new InvalidRequestException(ExceptionMessages.RoleNotFound);
        }
    }

    private async Task<(UserModel User, CompanyModel Company)> PersistAsync(CreateNewUserCommand command, CancellationToken ct)
    {
        var existingUser = await userRepository.ExistsByEmailAsync(command.Email, ct);

        if (existingUser)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        var existingCompany = await companyRepository.GetByCnpjAsync(command.Company.Cnpj, ct);

        if (existingCompany is not null)
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyExists);
        }

        var hashedPassword = SecurityService.Hash(command.Password);
        var user = new UserModel
        {
            Email = command.Email,
            Password = hashedPassword,
            Name = command.Name
        };

        await userRepository.InsertAsync(user, ct);

        var company = new CompanyModel
        {
            Name = command.Company.Name,
            Cnpj = command.Company.Cnpj
        };

        await companyRepository.InsertAsync(company, ct);

        var adminRole = await roleRepository.GetByNameAsync("Admin", ct) ?? throw new InvalidRequestException(ExceptionMessages.AdminRoleNotFound);
        var userRole = new UserRoleModel
        {
            UserId = user.Id,
            CompanyId = company.Id,
            RoleId = adminRole.Id
        };

        await userRoleRepository.InsertAsync(userRole, ct);

        return (user, company);
    }

    private async Task ValidateAsync(CreateNewUserCommand request, CancellationToken ct)
    {
        var isExistingUser = await userRepository.ExistsByEmailAsync(request.Email, ct);
        var isExistingCompany = await companyRepository.GetByCnpjAsync(request.Company.Cnpj, ct);

        if (isExistingUser)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        if (isExistingCompany is not null)
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundWithCNPJ);
        }
    }

    private async Task RelateRolesAsync(UpdateUserCommand command, UserModel user, CancellationToken ct)
    {
        var requestedRoles = command.CompaniesRoles ?? [];

        if (requestedRoles.Count == 0)
        {
            var existing = await userRoleRepository.Query()
                    .Where(x => x.UserId == user.Id)
                    .ToListAsync(ct);

            foreach (var role in existing)
            {
                await userRoleRepository.DeleteAsync(role.Id, ct);
            }

            return;
        }

        var requestedRoleIds = requestedRoles.Select(r => r.RoleId).Distinct().ToList();

        var validRoleIds = await roleRepository.Query()
            .Where(r => requestedRoleIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(ct);

        if (validRoleIds.Count != requestedRoleIds.Count)
        {
            var missingRoles = requestedRoleIds.Except(validRoleIds);

            throw new InvalidRequestException($"Role(s) not found: {string.Join(", ", missingRoles)}");
        }

        var requestedSet = requestedRoles.Select(r => (r.CompanyId, r.RoleId)).ToHashSet();

        var existingRoles = await userRoleRepository.Query()
            .Where(x => x.UserId == user.Id)
            .ToListAsync(ct);

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
            foreach (var role in toRemove)
            {
                await userRoleRepository.DeleteAsync(role.Id, ct);
            }
        }

        if (toInsert.Count > 0)
        {
            await userRoleRepository.InsertRangeAsync(toInsert, ct);
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

        var emailExists = await userRepository.Query()
            .AnyAsync(u => u.Email == command.Email && u.Id != command.UserId, ct);

        user.Email = emailExists switch
        {
            true => throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists),
            _ => command.Email
        };
    }
}
