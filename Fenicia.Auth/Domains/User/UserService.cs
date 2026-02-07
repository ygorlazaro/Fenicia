using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Data.Mappers.Auth;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Migrations.Services;

namespace Fenicia.Auth.Domains.User;

public class UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, ICompanyRepository companyRepository, ISecurityService securityService, ILoginAttemptService loginAttemptService, IMigrationService migrationService)
    : IUserService
{
    public async Task<UserResponse> GetForLoginAsync(TokenRequest request, CancellationToken ct)
    {
        var attempts = await loginAttemptService.GetAttemptsAsync(request.Email, ct);

        if (attempts >= 5)
        {
            throw new PermissionDeniedException(TextConstants.TooManyAttempts);
        }

        var user = await userRepository.GetByEmailAsync(request.Email, ct);

        if (user is null)
        {
            await loginAttemptService.IncrementAttemptsAsync(request.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), ct);

            throw new PermissionDeniedException(TextConstants.InvalidUsernameOrPasswordMessage);
        }

        var isValidPassword = securityService.VerifyPassword(request.Password, user.Password);

        if (isValidPassword)
        {
            await loginAttemptService.ResetAttemptsAsync(request.Email, ct);

            return UserMapper.Map(user);
        }

        await loginAttemptService.IncrementAttemptsAsync(request.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), ct);

        throw new PermissionDeniedException(TextConstants.InvalidUsernameOrPasswordMessage);
    }

    public async Task<UserResponse> CreateNewUserAsync(UserRequest request, CancellationToken ct)
    {
        var isExistingUser = await userRepository.CheckUserExistsAsync(request.Email, ct);
        var isExistingCompany = await companyRepository.CheckCompanyExistsAsync(request.Company.Cnpj, true, ct);

        if (isExistingUser)
        {
            throw new ArgumentException(TextConstants.EmailExistsMessage);
        }

        if (isExistingCompany)
        {
            throw new ArgumentException(TextConstants.CompanyExistsMessage);
        }

        var hashedPassword = securityService.HashPassword(request.Password);
        var userRequest = new UserModel
        {
            Email = request.Email,
            Password = hashedPassword,
            Name = request.Name
        };
        userRepository.Add(userRequest);

        var companyRequest = CompanyMapper.Map(request.Company);

        companyRepository.Add(companyRequest);

        var adminRole = await roleRepository.GetAdminRoleAsync(ct) ?? throw new ArgumentException(TextConstants.MissingAdminRoleMessage);
        var userRole = new UserRoleModel
        {
            UserId = userRequest.Id,
            Company = companyRequest,
            RoleId = adminRole.Id
        };

        userRoleRepository.Add(userRole);

        await userRepository.SaveChangesAsync(ct);

        await migrationService.RunMigrationsAsync(companyRequest.Id, [ModuleType.Basic], ct);

        return UserMapper.Map(userRequest);
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await userRoleRepository.ExistsInCompanyAsync(userId, companyId, ct);
    }

    public async Task<UserResponse> GetUserForRefreshAsync(Guid userId, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(userId, ct) ?? throw new UnauthorizedAccessException(TextConstants.PermissionDeniedMessage);

        return UserMapper.Map(user);
    }

    public async Task<UserResponse> GetUserIdFromEmailAsync(string email, CancellationToken ct)
    {
        var userId = await userRepository.GetUserIdFromEmailAsync(email, ct);

        return userId switch
        {
            null => throw new ArgumentException(TextConstants.ItemNotFoundMessage),
            _ => new UserResponse { Id = userId.Value }
        };
    }

    public async Task<UserResponse> ChangePasswordAsync(Guid userId, string password, CancellationToken t)
    {
        var user = await userRepository.GetByIdAsync(userId, t) ?? throw new ArgumentException(TextConstants.ItemNotFoundMessage);
        var hashedPassword = securityService.HashPassword(password);

        user.Password = hashedPassword;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(t);

        return UserMapper.Map(user);
    }
}