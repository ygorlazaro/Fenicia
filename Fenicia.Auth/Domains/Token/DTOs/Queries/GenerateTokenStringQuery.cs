using Fenicia.Auth.Domains.Token.DTOs.Responses;

namespace Fenicia.Auth.Domains.Token.DTOs.Queries;

public sealed record GenerateTokenStringQuery(GenerateTokenResponse User);
