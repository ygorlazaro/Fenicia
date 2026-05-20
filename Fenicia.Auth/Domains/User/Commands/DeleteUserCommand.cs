using MediatR;

namespace Fenicia.Auth.Domains.User.Commands;

public record DeleteUserCommand(Guid UserId) : IRequest;
