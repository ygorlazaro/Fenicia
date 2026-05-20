using Fenicia.Auth.Domains.Token.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Token.Queries;

public sealed record GenerateTokenStringQuery(GenerateTokenResponse User) : IRequest<string>;
