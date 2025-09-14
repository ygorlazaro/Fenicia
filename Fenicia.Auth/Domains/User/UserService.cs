namespace Fenicia.Auth.Domains.User;

using System.Net;

using Common;
using Common.Database.Requests;

using Fenicia.Common.Database.Models.Auth;
using Common.Database.Responses;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.UserRole;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ISecurityService _securityService;
    private readonly ILoginAttemptService _loginAttemptService;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, ICompanyRepository companyRepository, ISecurityService securityService, ILoginAttemptService loginAttemptService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _companyRepository = companyRepository;
        _securityService = securityService;
        _loginAttemptService = loginAttemptService;
    }

    public async Task<ApiResponse<UserResponse>> GetForLoginAsync(TokenRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting login process for user {Email}", request.Email);

        var attempts = await _loginAttemptService.GetAttemptsAsync(request.Email, cancellationToken);

        if (attempts >= 5)
        {
            _logger.LogWarning("User blocked temporarily - {email}", request.Email);
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.TooManyRequests, "Muitas tentativas. Tente novamente mais tarde.");
        }

        var user = await _userRepository.GetByEmailAndCnpjAsync(request.Email, request.Cnpj, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Invalid login - {email}", request.Email);
            await _loginAttemptService.IncrementAttemptsAsync(request.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, val2: 5)), cancellationToken);
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.InvalidUsernameOrPassword);
        }

        var isValidPassword = _securityService.VerifyPassword(request.Password, user.Password);

        if (isValidPassword.Data)
        {
            await _loginAttemptService.ResetAttemptsAsync(request.Email, cancellationToken);
            var response = UserResponse.Convert(user);
            return new ApiResponse<UserResponse>(response);
        }

        _logger.LogWarning("Invalid password - {email}", request.Email);
        await _loginAttemptService.IncrementAttemptsAsync(request.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, val2: 5)), cancellationToken);

        return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.InvalidUsernameOrPassword);
    }

    public async Task<ApiResponse<UserResponse>> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting user creation process for {Email}", request.Email);
        var isExistingUser = await _userRepository.CheckUserExistsAsync(request.Email, cancellationToken);
        var isExistingCompany = await _companyRepository.CheckCompanyExistsAsync(request.Company.Cnpj, cancellationToken);

        if (isExistingUser)
        {
            _logger.LogInformation("User already exists - {email}", request.Email);

            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.EmailExists);
        }

        if (isExistingCompany)
        {
            _logger.LogInformation("Company already exists - {cnpj}", request.Company.Cnpj);

            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.CompanyExists);
        }

        var hashedPassword = _securityService.HashPassword(request.Password).Data;

        var userRequest = new UserModel
        {
            Email = request.Email,
            Password = hashedPassword!,
            Name = request.Name
        };

        var user = _userRepository.Add(userRequest);
        var company = _companyRepository.Add(new CompanyModel { Name = request.Company.Name, Cnpj = request.Company.Cnpj });
        var adminRole = await _roleRepository.GetAdminRoleAsync(cancellationToken);

        if (adminRole is null)
        {
            _logger.LogCritical("Missing admin role. Please check database.");

            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.InternalServerError, TextConstants.MissingAdminRole);
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

        await _userRepository.SaveAsync(cancellationToken);

        var response = UserResponse.Convert(user);

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verifying user {UserID} existence in company {CompanyID}", userId, companyId);
        var response = await _userRoleRepository.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        return new ApiResponse<bool>(response);
    }

    public async Task<ApiResponse<UserResponse>> GetUserForRefreshAsync(Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving user {UserID} information for token refresh", userId);
        var user = await _userRepository.GetUserForRefreshTokenAsync(userId, cancellationToken);

        if (user is null)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.Unauthorized, TextConstants.PermissionDenied);
        }

        var response = UserResponse.Convert(user);

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<UserResponse>> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving user ID for email {Email}", email);

        var userId = await _userRepository.GetUserIdFromEmailAsync(email, cancellationToken);

        if (userId is null)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
        }

        var response = new UserResponse
        {
            Id = userId.Value
        };

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<UserResponse>> ChangePasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating password change for user {UserID}", userId);
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            _logger.LogInformation("User not found {userID}", userId);

            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
        }

        var hashedPassword = _securityService.HashPassword(password).Data;
        user.Password = hashedPassword!;
        await _userRepository.SaveAsync(cancellationToken);

        var mapped = UserResponse.Convert(user);

        return new ApiResponse<UserResponse>(mapped);
    }
}
