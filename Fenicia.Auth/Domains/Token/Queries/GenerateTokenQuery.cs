using Fenicia.Auth.Domains.Token.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Token.Queries;

public record GenerateTokenQuery(string Email, string Password) : IRequest<GenerateTokenResponse>;
