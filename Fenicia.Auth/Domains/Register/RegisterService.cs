using Fenicia.Auth.Domains.Register.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Auth.Domains.Register;

public class RegisterService(RegisterMapper mapper, IUserService userService) : IRegisterService
{
    public async Task<RegisterResponse> CreateAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UserRequest(request.Email, request.Password, request.Name, request.Company);
        var user = await userService.CreateAsync(command, cancellationToken);

        return mapper.MapToRegisterResponse(user);
    }
}