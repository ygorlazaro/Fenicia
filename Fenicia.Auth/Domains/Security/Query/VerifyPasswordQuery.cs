using MediatR;

namespace Fenicia.Auth.Domains.Security.Query;

public record VerifyPasswordQuery(string Password, string HashedPassword) : IRequest<bool>;
