using Fenicia.Auth.Domains.Register.Command;
using Fenicia.Auth.Domains.Register.Response;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;

using MediatR;

namespace Fenicia.Auth.Domains.Register.Handler;

public class RegisterHandler(CreateNewUserHandler createNewUserHandler) : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var command = new CreateNewUserCommand(request.Email, request.Password, request.Name, request.Company);
        var user = await createNewUserHandler.Handle(command, cancellationToken);

        return new RegisterResponse(user.Id, user.Name, user.Email, user.Company);
    }
}
