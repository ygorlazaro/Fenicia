using Fenicia.Auth.Domains.Register.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Auth.Domains.Register;

public class RegisterService(IUserService userService) : IRegisterService
{
    public async Task<RegisterResponse> CreateAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var registerRequest = new UserRequest(request.Email, request.Password, request.Name, request.Company);
        var user = await userService.CreateAsync(registerRequest, cancellationToken);

        return MapToRegisterResponse(user);
    }

    private static RegisterResponse MapToRegisterResponse(UserResponse user)
    {
        return new RegisterResponse(
            user.Id,
            user.Name,
            user.Email,
            new UserCompanyResponse(user.Company.Id, user.Company.Name, user.Company.Cnpj));
    }
}
