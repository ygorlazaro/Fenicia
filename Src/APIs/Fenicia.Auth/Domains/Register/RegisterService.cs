using Fenicia.Auth.Domains.Register.DTOs;
using Fenicia.Auth.Domains.Register.Interfaces;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.User.Interfaces;

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