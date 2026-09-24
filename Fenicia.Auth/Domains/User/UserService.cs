using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Role;
using Fenicia.Common.DTOs.Auth.User;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;
using UserCompanyResponse = Fenicia.Common.DTOs.Auth.UserRole.UserCompanyResponse;

namespace Fenicia.Auth.Domains.User;

public sealed class UserService(
    IUserRepository userRepository,
    IUserRoleService userRoleService,
    IRoleService roleService,
    ICompanyService companyService,
    ISecurityService securityService) : IUserService
{
    public async Task<Pagination<List<UserResponse>>> GetAllAsync(
        UserRequest query,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        var total = await userRepository.CountAsync(cancellationToken);
        var users = await userRepository.GetAllAsync(page, perPage, cancellationToken);

        return new Pagination<List<UserResponse>>(
            [.. users.Select(MapToUserResponse)],
            total,
            page,
            perPage);
    }

    public Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return userRepository.GetByIdAsync(userId, cancellationToken).ContinueWith(
            task => task.Result is null ? null : MapToUserResponse(task.Result), cancellationToken);
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

    public Task<List<UserCompanyResponse>> GetCompaniesAsync(
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

    public async Task<UserResponse> CreateAsync(UserRequest request,
        CancellationToken cancellationToken = default)
    {
        var userExists = await userRepository.ExistsByEmailAsync(request.Email, cancellationToken);

        if (userExists)
        {
            throw new BadRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        var hashedPassword = securityService.Hash(request.Password);

        var user = new UserModel
        {
            Email = request.Email,
            Password = hashedPassword,
            Name = request.Name
        };

        await userRepository.InsertAsync(user, cancellationToken);
        await RelateRolesAsync(user.Id, request.Roles, cancellationToken);

        return MapToUserResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(
        UserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(request.Id, cancellationToken);

        await ValidateFields(user, request, cancellationToken);

        var companies = request.Roles?.Select(c => c.CompanyId) ?? [];

        await ValidateCompanies(companies, cancellationToken);
        await RelateRolesAsync(request, user, cancellationToken);

        return MapToUserResponse(user);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(userId, cancellationToken);

        user.Deleted = DateTime.UtcNow;
        await userRepository.UpdateAsync(user.Id, user, cancellationToken);
    }

    private async Task RelateRolesAsync(
        Guid userId,
        List<RoleRequest>? request,
        CancellationToken cancellationToken = default)
    {
        var roles = request ?? [];
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
                throw new BadRequestException(ExceptionMessages.CompanyNotFoundMessage);
        }
    }

    private async Task ValidateRoles(IEnumerable<Guid> roles, CancellationToken cancellationToken = default)
    {
        var distinct = roles.Distinct().ToList();

        foreach (var roleId in distinct)
        {
            _ = await roleService.GetByIdAsync(roleId, cancellationToken) ??
                throw new BadRequestException(ExceptionMessages.RoleNotFound);
        }
    }

    private async Task RelateRolesAsync(
        UserRequest request,
        BaseModel user,
        CancellationToken cancellationToken = default)
    {
        var requestedRoles = request.Roles ?? [];

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

            throw new BadRequestException($"Role(s) not found: {string.Join(", ", missingRoles)}");
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
        UserRequest request,
        CancellationToken cancellationToken = default)
    {
        user.Name = string.IsNullOrWhiteSpace(request.Name) switch
        {
            false => request.Name,
            _ => user.Name
        };

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return;
        }

        var emailExists = await userRepository.ExistsByEmailAsync(request.Email, cancellationToken);

        user.Email = emailExists switch
        {
            true => throw new BadRequestException(ExceptionMessages.EmailAlreadyExists),
            _ => request.Email
        };
    }

    private async Task<UserModel> FirstByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await userRepository.GetByIdAsync(userId, cancellationToken) ??
               throw new BadRequestException(ExceptionMessages.UserNotFound);
    }

    private static UserResponse MapToUserResponse(UserModel user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            new Fenicia.Common.DTOs.Auth.UserRole.CompanyResponse(Guid.Empty, string.Empty, string.Empty));
    }
}
