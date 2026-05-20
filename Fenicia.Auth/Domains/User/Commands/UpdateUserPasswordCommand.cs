using Fenicia.Auth.Domains.User.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.User.Commands;

public record UpdateUserPasswordCommand(Guid UserId, string Password) : IRequest<UpdateUserPasswordResponse>;
