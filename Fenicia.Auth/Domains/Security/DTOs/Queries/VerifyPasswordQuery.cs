namespace Fenicia.Auth.Domains.Security.DTOs.Queries;

public record VerifyPasswordQuery(string Password, string HashedPassword);
