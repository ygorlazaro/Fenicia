using Fenicia.Auth.Domains.Register.Response;
using Fenicia.Auth.Domains.User.Commands;

using MediatR;

namespace Fenicia.Auth.Domains.Register.Command;

public record RegisterCommand(string Email, string Password, string Name, CreateNewUserCompanyCommand Company) : IRequest<RegisterResponse>;
