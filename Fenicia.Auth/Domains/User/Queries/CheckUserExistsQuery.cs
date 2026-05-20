using MediatR;

namespace Fenicia.Auth.Domains.User.Queries;

public record CheckUserExistsQuery(string Email) : IRequest<bool>;
