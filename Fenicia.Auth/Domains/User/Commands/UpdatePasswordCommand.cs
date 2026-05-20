using Fenicia.Auth.Domains.User.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.User.Commands;

public record UpdatePasswordCommand(Guid UserId, string Password) : IRequest<UpdatePasswordResponse>;
