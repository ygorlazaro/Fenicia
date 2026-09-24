using Fenicia.Auth.Domains.Register.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Auth.Domains.Register;

/// <summary>
/// Service responsible for handling user registration operations. It provides methods to create new user accounts, ensuring that the registration process is handled securely and efficiently. This service interacts with the user management system to create users and map the necessary data for registration responses.
/// </summary>
/// <param name="userService"></param>
public class RegisterService(IUserService userService) : IRegisterService
{
    /// <summary>
    /// Creates a new user account based on the provided registration request. This method handles the necessary validation, user creation, and any additional setup required for a new user. It returns a response indicating the success or failure of the registration process.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the registration response.</returns>
    public async Task<RegisterResponse> CreateAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var registerRequest = new UserRequest(request.Email, request.Password, request.Name, request.Company);
        var user = await userService.CreateAsync(registerRequest, cancellationToken);

        return RegisterMapper.MapToRegisterResponse(user);
    }
}
