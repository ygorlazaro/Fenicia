using Fenicia.Auth.Domains.User.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.User.Commands;

public record CreateNewUserCommand(string Email, string Password, string Name, CreateNewUserCompanyCommand Company) : IRequest<CreateNewUserResponse>;
