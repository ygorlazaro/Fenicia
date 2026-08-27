using Fenicia.Auth.Domains.Register.DTOs.Commands;
using Fenicia.Auth.Domains.Register.DTOs.Responses;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs.Commands;

namespace Fenicia.Auth.Domains.Register;

public class RegisterService(UserService userService)
{
    public async Task<RegisterResponse> CreateAsync(RegisterCommand request, CancellationToken ct)
    {
        var command = new CreateNewUserCommand(request.Email, request.Password, request.Name, request.Company);
        var user = await userService.CreateNewAsync(command, ct);

        return new RegisterResponse(user.Id, user.Name, user.Email, user.Company);
    }
}
