using System.Net;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.User;

public class UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, ICompanyRepository companyRepository, ISecurityService securityService, ILoginAttemptService loginAttemptService)
    : IUserService
{
    public async Task<ApiResponse<UserResponse>> GetForLoginAsync(TokenRequest request, CancellationToken cancellationToken)
    {
        var attempts = await loginAttemptService.GetAttemptsAsync(request.Email, cancellationToken);

        if (attempts >= 5)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.TooManyRequests, "Muitas tentativas. Tente novamente mais tarde.");
        }

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            await loginAttemptService.IncrementAttemptsAsync(request.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, val2: 5)), cancellationToken);

            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.InvalidUsernameOrPasswordMessage);
        }

        var isValidPassword = securityService.VerifyPassword(request.Password, user.Password);

        if (isValidPassword.Data)
        {
            await loginAttemptService.ResetAttemptsAsync(request.Email, cancellationToken);

            var response = UserResponse.Convert(user);

            return new ApiResponse<UserResponse>(response);
        }

        await loginAttemptService.IncrementAttemptsAsync(request.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, val2: 5)), cancellationToken);

        return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.InvalidUsernameOrPasswordMessage);
    }

    public async Task<ApiResponse<UserResponse>> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken)
    {
        var isExistingUser = await userRepository.CheckUserExistsAsync(request.Email, cancellationToken);
        var isExistingCompany = await companyRepository.CheckCompanyExistsAsync(request.Company.Cnpj, cancellationToken);

        if (isExistingUser)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.EmailExistsMessage);
        }

        if (isExistingCompany)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.CompanyExistsMessage);
        }

        var hashedPassword = securityService.HashPassword(request.Password).Data;
        var userRequest = new UserModel
        {
            Email = request.Email,
            Password = hashedPassword!,
            Name = request.Name
        };
        var user = userRepository.Add(userRequest);
        var company = companyRepository.Add(new CompanyModel { Name = request.Company.Name, Cnpj = request.Company.Cnpj });
        var adminRole = await roleRepository.GetAdminRoleAsync(cancellationToken);

        if (adminRole is null)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.InternalServerError, TextConstants.MissingAdminRoleMessage);
        }

        user.UsersRoles =
        [
            new UserRoleModel
            {
                User = user,
                Company = company,
                Role = adminRole
            }

        ];

        await userRepository.SaveAsync(cancellationToken);

        var response = UserResponse.Convert(user);

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var response = await userRoleRepository.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        return new ApiResponse<bool>(response);
    }

    public async Task<ApiResponse<UserResponse>> GetUserForRefreshAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserForRefreshTokenAsync(userId, cancellationToken);

        if (user is null)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.Unauthorized, TextConstants.PermissionDeniedMessage);
        }

        var response = UserResponse.Convert(user);

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<UserResponse>> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        var userId = await userRepository.GetUserIdFromEmailAsync(email, cancellationToken);

        if (userId is null)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFoundMessage);
        }

        var response = new UserResponse
        {
            Id = userId.Value
        };

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<UserResponse>> ChangePasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFoundMessage);
        }

        var hashedPassword = securityService.HashPassword(password).Data;

        user.Password = hashedPassword!;
        await userRepository.SaveAsync(cancellationToken);

        var mapped = UserResponse.Convert(user);

        return new ApiResponse<UserResponse>(mapped);
    }
}
