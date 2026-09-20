using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Company;
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

    public Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return userRepository.GetByEmailAsync(email, cancellationToken).ContinueWith(
            task => task.Result is null ? null : MapToUserResponse(task.Result), cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return userRepository.ExistsByEmailAsync(email, cancellationToken);
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

    public async Task<UserResponse> GetForRefreshAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(userId, cancellationToken);

        return MapToUserResponse(user);
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
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
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
        UserRequest command,
        CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(command.Id, cancellationToken);

        await ValidateFields(user, command, cancellationToken);

        var companies = command.Roles?.Select(c => c.CompanyId) ?? [];

        await ValidateCompanies(companies, cancellationToken);
        await RelateRolesAsync(command, user, cancellationToken);

        return MapToUserResponse(user);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await FirstByIdAsync(userId, cancellationToken);

        user.Deleted = DateTime.UtcNow;
        await userRepository.UpdateAsync(user.Id, user, cancellationToken);
    }

    public async Task<UserResponse> UpdateHashedPasswordAsync(
        UserPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await UpdatePasswordAsync(request.UserId, request.NewPassword, cancellationToken) ??
                   throw new ItemNotExistsException(ExceptionMessages.UserNotFound);
        await userRepository.UpdateAsync(user.Id, user, cancellationToken);

        return MapToUserResponse(user);
    }

    private async Task AuthorizePasswordChangeAsync(
        Guid loggedInUserId,
        Guid targetUserId,
        UserModel loggedInUser,
        UserModel targetUser,
        UserPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (loggedInUserId == targetUserId)
        {
            if (string.IsNullOrEmpty(request.CurrentPassword))
            {
                throw new InvalidRequestException("Senha atual é obrigatória.");
            }

            if (!securityService.Verify(request.CurrentPassword, loggedInUser.Password))
            {
                throw new InvalidRequestException("Senha atual incorreta.");
            }

            return;
        }

        var loggedInUserRoles = await userRoleService.GetUserRolesByUserIdAsync(loggedInUserId, cancellationToken);
        var isGod = loggedInUserRoles.Any(r => r.Role.Name.Equals("God", StringComparison.OrdinalIgnoreCase));
        var isAdmin = loggedInUserRoles.Any(r => r.Role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));

        if (isGod)
        {
            return;
        }

        if (!isAdmin)
        {
            throw new UnauthorizedAccessException(ExceptionMessages.Unauthorized);
        }

        await AuthorizeAdminPasswordChangeAsync(targetUser, loggedInUserRoles, cancellationToken);
    }

    private async Task AuthorizeAdminPasswordChangeAsync(
        UserModel targetUser,
        IEnumerable<UserRoleModel> loggedInUserRoles,
        CancellationToken cancellationToken = default)
    {
        var targetUserRoles = await userRoleService.GetUserRolesByUserIdAsync(targetUser.Id, cancellationToken);
        var isTargetUser = targetUserRoles.Any(r => r.Role.Name.Equals("User", StringComparison.OrdinalIgnoreCase));

        if (!isTargetUser)
        {
            throw new InvalidRequestException("Admin só pode alterar senha de usuários.");
        }

        var loggedInCompanyIds = loggedInUserRoles.Select(r => r.CompanyId).ToHashSet();
        var targetCompanyIds = targetUserRoles.Select(r => r.CompanyId).ToHashSet();

        if (!loggedInCompanyIds.Overlaps(targetCompanyIds))
        {
            throw new InvalidRequestException("Usuário não pertence à mesma empresa.");
        }
    }

    private async Task RelateRolesAsync(
        Guid userId,
        List<RoleRequest>? command,
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
        UserRequest command,
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

    private async Task ValidateAsync(UserRequest request, CancellationToken cancellationToken = default)
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
        UserRequest command,
        BaseModel user,
        CancellationToken cancellationToken = default)
    {
        var requestedRoles = command.Roles ?? [];

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
        UserRequest command,
        CancellationToken cancellationToken = default)
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

        var emailExists = await userRepository.ExistsByEmailAsync(command.Email, cancellationToken);

        user.Email = emailExists switch
        {
            true => throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists),
            _ => command.Email
        };
    }

    private async Task<UserModel> FirstByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await userRepository.GetByIdAsync(userId, cancellationToken) ??
               throw new InvalidRequestException(ExceptionMessages.UserNotFound);
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
