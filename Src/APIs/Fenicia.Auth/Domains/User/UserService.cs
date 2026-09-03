using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public sealed class UserService(
    IUserRepository userRepository,
    IUserRoleService userRoleService,
    IRoleService roleService,
    ICompanyService companyService,
    ISecurityService securityService) : IUserService
{
    public async Task<Pagination<List<UserListItemResponse>>> GetAllAsync(
        GetAllUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = userRepository.Query().OrderBy(u => u.Name);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        var totalCountTask = filteredQuery.CountAsync(cancellationToken);
        var usersTask = filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);

        await Task.WhenAll(totalCountTask, usersTask);

        return new Pagination<List<UserListItemResponse>>(
            [.. usersTask.Result.Select(u => u.MapToUserListItemResponse())],
            totalCountTask.Result,
            query.Page,
            query.PerPage);
    }

    public async Task<GetUserByIdResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        return user?.MapToGetUserByIdResponse();
    }

    public async Task<GetByEmailResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        return user?.MapToGetByEmailResponse();
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return userRepository.ExistsByEmailAsync(email, cancellationToken);
    }

    public async Task<UserModel> FirstByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await userRepository.GetByIdAsync(userId, cancellationToken) ??
               throw new InvalidRequestException(ExceptionMessages.UserNotFound);
    }

    public Task<UserModel?> FirstByEmailOrDefaultAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return userRepository.GetByEmailAsync(email, cancellationToken);
    }

    public async Task<UserModel> UpdatePasswordAsync(
        Guid userId,
        string plainPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(userId, cancellationToken);
        user.Password = securityService.Hash(plainPassword);
        return user;
    }

    public async Task<GetUserForRefreshResponse> GetForRefreshAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(userId, cancellationToken);

        return user.MapToGetUserForRefreshResponse();
    }

    public Task<List<GetUserCompaniesResponse>> GetCompaniesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return userRoleService.GetUserCompaniesAsync(userId, cancellationToken);
    }

    public async Task EnsureCanAccessUserAsync(
        Guid loggedInUserId,
        Guid requestedUserId,
        Guid? companyId,
        CancellationToken cancellationToken = default)
    {
        if (loggedInUserId == requestedUserId)
        {
            return;
        }

        var userRoles = await userRoleService.GetUserRolesByUserIdAsync(loggedInUserId, cancellationToken);

        var isGod = userRoles.Any(r => r.Role.Name.Equals("God", StringComparison.OrdinalIgnoreCase));
        if (isGod)
        {
            return;
        }

        if (!companyId.HasValue)
        {
            var targetUserRoles = await userRoleService.GetUserRolesByUserIdAsync(requestedUserId, cancellationToken);
            var targetCompanyIds = targetUserRoles.Select(r => r.CompanyId).ToHashSet();

            var isAdminInSharedCompany = userRoles.Any(r =>
                r.Role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                targetCompanyIds.Contains(r.CompanyId));
            if (isAdminInSharedCompany)
            {
                return;
            }
        }
        else
        {
            var isAdminInCompany = userRoles.Any(r =>
                r.Role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase) && r.CompanyId == companyId.Value);
            if (isAdminInCompany)
            {
                return;
            }
        }

        throw new UnauthorizedAccessException(ExceptionMessages.Unauthorized);
    }

    public async Task<CreateUserResponse> CreateAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var userExists = await userRepository.ExistsByEmailAsync(command.Email, cancellationToken);

        if (userExists)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        var hashedPassword = securityService.Hash(command.Password);

        var user = new UserModel
        {
            Email = command.Email,
            Password = hashedPassword,
            Name = command.Name
        };

        await userRepository.InsertAsync(user, cancellationToken);
        await RelateRolesAsync(user.Id, command.Roles, cancellationToken);

        return user.MapToCreateUserResponse();
    }

    public async Task<CreateNewUserResponse> CreateNewAsync(
        CreateNewUserCommand command,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(command, cancellationToken);

        var (user, company) = await PersistAsync(command, cancellationToken);
        var companyResponse = new CreateNewUserCompanyResponse(company.Id, company.Name, company.Cnpj);

        return new CreateNewUserResponse(user.Id, user.Name, user.Email, companyResponse);
    }

    public async Task<UpdateUserResponse> UpdateAsync(
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(command.UserId, cancellationToken);

        await ValidateFields(user, command, cancellationToken);

        var companies = command.CompaniesRoles?.Select(c => c.CompanyId) ?? [];

        await ValidateCompanies(companies, cancellationToken);
        await RelateRolesAsync(command, user, cancellationToken);

        return user.MapToUpdateUserResponse();
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(userId, cancellationToken);

        user.Deleted = DateTime.UtcNow;
        await userRepository.UpdateAsync(user.Id, user, cancellationToken);
    }

    public async Task<UpdateUserPasswordResponse> UpdatePasswordAsync(
        UpdateUserPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(command.UserId, cancellationToken);
        var hashedPassword = securityService.Hash(command.Password);

        user.Password = hashedPassword;

        return new UpdateUserPasswordResponse(true, "Password changed successfully");
    }

    public async Task<UpdatePasswordResponse> UpdateHashedPasswordAsync(
        UpdatePasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await UpdatePasswordAsync(command.UserId, command.Password, cancellationToken) ??
                   throw new ItemNotExistsException(ExceptionMessages.UserNotFound);
        await userRepository.UpdateAsync(user.Id, user, cancellationToken);

        return user.MapToUpdatePasswordResponse();
    }

    private async Task RelateRolesAsync(
        Guid userId,
        List<CreateUserRoleCommand>? command,
        CancellationToken cancellationToken = default)
    {
        var roles = command ?? [];
        await ValidateCompanies(roles.Select(r => r.CompanyId), cancellationToken);
        await ValidateRoles(roles.Select(r => r.RoleId), cancellationToken);

        var userRoles = roles.Select(r => new UserRoleModel
        {
            UserId = userId,
            RoleId = r.RoleId,
            CompanyId = r.CompanyId
        });

        await userRoleService.InsertRangeAsync([.. userRoles], cancellationToken);
    }

    private async Task ValidateCompanies(IEnumerable<Guid> companies, CancellationToken cancellationToken = default)
    {
        var distinct = companies.Distinct().ToList();

        foreach (var companyId in distinct)
        {
            _ = await companyService.GetByIdAsync(companyId, cancellationToken) ??
                throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundMessage);
        }
    }

    private async Task ValidateRoles(IEnumerable<Guid> roles, CancellationToken cancellationToken = default)
    {
        var distinct = roles.Distinct().ToList();

        foreach (var roleId in distinct)
        {
            _ = await roleService.GetByIdAsync(roleId, cancellationToken) ??
                throw new InvalidRequestException(ExceptionMessages.RoleNotFound);
        }
    }

    private async Task<(UserModel User, CompanyModel Company)> PersistAsync(
        CreateNewUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await userRepository.ExistsByEmailAsync(command.Email, cancellationToken);

        if (existingUser)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        var existingCompany = await companyService.GetByCnpjAsync(command.Company.Cnpj, cancellationToken);

        if (existingCompany is not null)
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyExists);
        }

        var hashedPassword = securityService.Hash(command.Password);
        var user = new UserModel
        {
            Email = command.Email,
            Password = hashedPassword,
            Name = command.Name
        };

        await userRepository.InsertAsync(user, cancellationToken);

        var company = new CompanyModel
        {
            Name = command.Company.Name,
            Cnpj = command.Company.Cnpj
        };

        await companyService.InsertAsync(company, cancellationToken);

        var adminRole = await roleService.GetRoleAsync("Admin", cancellationToken) ??
                        throw new InvalidRequestException(ExceptionMessages.AdminRoleNotFound);
        var userRole = new UserRoleModel
        {
            UserId = user.Id,
            CompanyId = company.Id,
            RoleId = adminRole.Id
        };

        await userRoleService.InsertAsync(userRole, cancellationToken);

        return (user, company);
    }

    private async Task ValidateAsync(CreateNewUserCommand request, CancellationToken cancellationToken = default)
    {
        var isExistingUser = await userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        var isExistingCompany = await companyService.GetByCnpjAsync(request.Company.Cnpj, cancellationToken);

        if (isExistingUser)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        if (isExistingCompany is not null)
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundWithCNPJ);
        }
    }

    private async Task RelateRolesAsync(
        UpdateUserCommand command,
        BaseModel user,
        CancellationToken cancellationToken = default)
    {
        var requestedRoles = command.CompaniesRoles ?? [];

        if (requestedRoles.Count == 0)
        {
            var existing = await userRoleService.GetUserRolesByUserIdAsync(user.Id, cancellationToken);

            foreach (var role in existing)
            {
                await userRoleService.DeleteAsync(role.Id, cancellationToken);
            }

            return;
        }

        var requestedRoleIds = requestedRoles.Select(r => r.RoleId).Distinct().ToList();

        var validRoleIds = await roleService.GetRolesByIdsAsync(requestedRoleIds, cancellationToken);

        if (validRoleIds.Count != requestedRoleIds.Count)
        {
            var missingRoles = requestedRoleIds.Except(validRoleIds.Select(r => r.Id));

            throw new InvalidRequestException($"Role(s) not found: {string.Join(", ", missingRoles)}");
        }

        var requestedSet = requestedRoles.Select(r => (r.CompanyId, r.RoleId)).ToHashSet();

        var existingRoles = await userRoleService.GetUserRolesByUserIdAsync(user.Id, cancellationToken);

        var existingSet = existingRoles.Select(r => (r.CompanyId, r.RoleId)).ToHashSet();

        var toRemove = existingRoles.Where(r => !requestedSet.Contains((r.CompanyId, r.RoleId))).ToList();

        var toInsert = requestedRoles.Where(r => !existingSet.Contains((r.CompanyId, r.RoleId))).Select(r =>
            new UserRoleModel
            {
                UserId = user.Id,
                CompanyId = r.CompanyId,
                RoleId = r.RoleId
            }).ToList();

        if (toRemove.Count > 0)
        {
            foreach (var role in toRemove)
            {
                await userRoleService.DeleteAsync(role.Id, cancellationToken);
            }
        }

        if (toInsert.Count > 0)
        {
            await userRoleService.InsertRangeAsync(toInsert, cancellationToken);
        }
    }

    private async Task ValidateFields(
        UserModel user,
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        user.Name = string.IsNullOrWhiteSpace(command.Name) switch
                    {
                        false => command.Name,
                        _ => user.Name
                    }

                    ?? string.Empty;

        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return;
        }

        var emailExists = await userRepository.Query()
            .AnyAsync(u => u.Email == command.Email && u.Id != command.UserId, cancellationToken);

        user.Email = emailExists switch
        {
            true => throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists),
            _ => command.Email
        };
    }
}