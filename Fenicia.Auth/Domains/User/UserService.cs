using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Domains.User;

public class UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, ICompanyRepository companyRepository, ISecurityService securityService, ILoginAttemptService loginAttemptService)
    : IUserService
{
    public async Task<UserResponse> GetForLoginAsync(TokenRequest request, CancellationToken cancellationToken)
    {
        var attempts = await loginAttemptService.GetAttemptsAsync(request.Email, cancellationToken);

        if (attempts >= 5)
        {
            throw new PermissionDeniedException(TextConstants.TooManyAttempts);
        }

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            await loginAttemptService.IncrementAttemptsAsync(request.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, val2: 5)), cancellationToken);

            throw new PermissionDeniedException(TextConstants.InvalidUsernameOrPasswordMessage);
        }

        var isValidPassword = securityService.VerifyPassword(request.Password, user.Password);

        if (isValidPassword)
        {
            await loginAttemptService.ResetAttemptsAsync(request.Email, cancellationToken);

            return UserResponse.Convert(user);
        }

        await loginAttemptService.IncrementAttemptsAsync(request.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, val2: 5)), cancellationToken);

        throw new PermissionDeniedException(TextConstants.InvalidUsernameOrPasswordMessage);
    }

    public async Task<UserResponse> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken)
    {
        var isExistingUser = await userRepository.CheckUserExistsAsync(request.Email, cancellationToken);
        var isExistingCompany = await companyRepository.CheckCompanyExistsAsync(request.Company.Cnpj, onlyActive: true, cancellationToken);

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
        var user = userRepository.Add(userRequest);
        var company = companyRepository.Add(new CompanyModel { Name = request.Company.Name, Cnpj = request.Company.Cnpj });
        var adminRole = await roleRepository.GetAdminRoleAsync(cancellationToken) ?? throw new ArgumentException(TextConstants.MissingAdminRoleMessage);
        var userRole = new UserRoleModel
        {
            UserId = user.Id,
            CompanyId = company.Id,
            RoleId = adminRole.Id
        };

        userRoleRepository.Add(userRole);

        await userRepository.SaveAsync(cancellationToken);

        return UserResponse.Convert(user);
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        return await userRoleRepository.ExistsInCompanyAsync(userId, companyId, cancellationToken);
    }

    public async Task<UserResponse> GetUserForRefreshAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserForRefreshTokenAsync(userId, cancellationToken) ?? throw new UnauthorizedAccessException(TextConstants.PermissionDeniedMessage);

        return UserResponse.Convert(user);
    }

    public async Task<UserResponse> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        var userId = await userRepository.GetUserIdFromEmailAsync(email, cancellationToken);

        return userId switch
        {
            null => throw new ArgumentException(TextConstants.ItemNotFoundMessage),
            _ => new UserResponse { Id = userId.Value }
        };
    }

    public async Task<UserResponse> ChangePasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new ArgumentException(TextConstants.ItemNotFoundMessage);
        var hashedPassword = securityService.HashPassword(password);

        user.Password = hashedPassword;
        userRepository.Update(user);
        await userRepository.SaveAsync(cancellationToken);

        return UserResponse.Convert(user);
    }
}
