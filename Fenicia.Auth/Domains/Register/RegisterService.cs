using Fenicia.Auth.Domains.Register.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Auth.Domains.Register;

public class RegisterService(IUserService userService) : IRegisterService
{
    public async Task<RegisterResponse> CreateAsync(
        RegisterCommand request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateNewUserCommand(request.Email, request.Password, request.Name, request.Company);
        var user = await userService.CreateNewAsync(command, cancellationToken);

        return user.MapToRegisterResponse();
    }
}